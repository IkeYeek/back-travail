using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EASIER.Resources.Scripts.Archives;
using EASIER.Resources.Scripts.Archives.Types;
using EASIER.Resources.Scripts.Exceptions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

namespace EASIER.Resources.Scripts.World
{
    public class Manager : MonoBehaviour
    {
        [SerializeField] private TextAsset _jsonFile;

        private SerializedArchive _archive;
        private int _currentScene = 0;
        private Scene? _scene = null;
        private List<SerializedSceneObject> _sceneObjects;
        private List<GameObject> _sceneGameObjects;
        private List<SerializedSceneText> _sceneTexts;
        private List<GameObject> _sceneTextGameObjects;
        private Material _simpleImageMaterial;
        private GameObject _simpleTextPrefab;

        private void Start()
        {
            _archive = Loader.ReadJSONContent(_jsonFile.text);
            if (_archive.SerializedScenes.Length == 0)
            {
                throw new NoSceneInArchive();
            }
            _simpleImageMaterial = UnityEngine.Resources.Load<Material>("Materials/SimpleImageMaterial");
            if (ReferenceEquals(_simpleImageMaterial, null)) throw new EASIERCore("Couldn't find SimpleImageMaterial");
            _simpleTextPrefab = UnityEngine.Resources.Load<GameObject>("Prefabs/SimpleTextPrefab");
            if (ReferenceEquals(_simpleTextPrefab, null)) throw new EASIERCore("Couldn't find SimpleTextPrefab");
            LoadCurrentScene();
        }

        public void LoadCurrentScene()
        {
            if (_currentScene >= 0 && _currentScene < _archive.SerializedScenes.Length)
            {
                var currentSceneFromArchive = _archive.SerializedScenes[_currentScene];
                if (_scene.HasValue)
                {
                    SceneManager.UnloadSceneAsync(_scene.Value);
                }

                var rootScene = SceneManager.GetActiveScene();
                _scene = SceneManager.CreateScene($"{currentSceneFromArchive.name} (#{currentSceneFromArchive.id})");
                SceneManager.SetActiveScene(_scene.Value);
                //TODO try to optimize by lazy-loading objects and then cache them in the DontDestroyOnLoad
                _sceneObjects = currentSceneFromArchive.objects.ToList();
                _sceneGameObjects = _sceneObjects.ConvertAll(_createGOFromSceneObject);

                _sceneTexts = currentSceneFromArchive.texts.ToList();
                _sceneTextGameObjects = _sceneTexts.ConvertAll(_createSceneTextGO);

                SceneManager.SetActiveScene(rootScene);
            }
            else
            {
                throw new IndexOutOfRangeException(
                    $"Cannot load scene of index {_currentScene}. Scene index must be between 0 and {_archive.SerializedScenes.Length} - 1");
            }
        }

        private GameObject _createSceneTextGO(SerializedSceneText input)
        {
            var go = Instantiate(_simpleTextPrefab);
            go.transform.position = new Vector3(input.transform.position.X, input.transform.position.Y,
                input.transform.position.Z);
            var simpleTextComponent = go.GetComponent<SimpleText>();
            simpleTextComponent.Text = input.content;
            return go;
        }

        public void GoToNextScene()
        {
            _currentScene += 1;
            _currentScene %= _archive.SerializedScenes.Length;
            LoadCurrentScene();
        }

        public void GoToPrevScene()
        {
            _currentScene -= 1;
            if (_currentScene < 0)
                _currentScene = 0;
            _currentScene %= _archive.SerializedScenes.Length;
            LoadCurrentScene();
        }

        public void GoToNthScene(int n)
        {
            if (n >= 0 && n < _archive.SerializedScenes.Length)
            {
                _currentScene = n;
                LoadCurrentScene();
            }
            else
            {
                throw new IndexOutOfRangeException(
                    $"Cannot load scene of index {n}. Scene index must be between 0 and {_archive.SerializedScenes.Length} - 1");
            }
        }
        
        private GameObject _createGOFromSceneObject(SerializedSceneObject obj)
        {
            var go = obj.file.type switch
            {
                "Image" => _createImageGO(obj),
                "Video" => _createVideoGO(obj),
                "Vidéo" => _createVideoGO(obj),  // TODO simplify this quircky sh*t
                "Modèle 3D" => _createModelGO(obj),
                "Model3D" => _createModelGO(obj),
                "Audio" => _createAudioGO(obj),
                "Model" => _createModelGO(obj),
                _ => throw new UnknownSceneObjectType(obj.file.type)
            };
            applyParametersToGO(go, obj);
            return go;
        }

        private GameObject _createImageGO(SerializedSceneObject imgObj)
        {
            var imResource =
                UnityEngine.Resources.Load<Texture2D>("images/" + Path.GetFileNameWithoutExtension($"{imgObj.file.id}_{imgObj.file.name}"));
            if (ReferenceEquals(imResource, null)) throw new ResourceNotFound(imgObj.id, "Image");
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = $"Object #{imgObj.id} - File: {imgObj.file.name} #{imgObj.file.id}";
            var goTransform = go.transform;
            goTransform.localScale = new Vector3(2, 1, 1);
            goTransform.position = _unserializeVector3(imgObj.transform.position);

            var goRenderer = go.GetComponent<Renderer>();
            goRenderer.material = _simpleImageMaterial;
            var goMaterial = goRenderer.material;
            goMaterial.mainTexture = imResource;
            return go;
        }
        
        private GameObject _createVideoGO(SerializedSceneObject vidObj)
        {
            var vidResource = UnityEngine.Resources.Load<VideoClip>("videos/" +
                                                                    Path.GetFileNameWithoutExtension(
                                                                        $"{vidObj.file.id}_{vidObj.file.name}"));
            if (ReferenceEquals(vidResource, null)) throw new ResourceNotFound(vidObj.id, "Video");
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = $"Object #{vidObj.id} - File: {vidObj.file.name} #{vidObj.file.id}";
            var goTransform = go.transform;
            goTransform.localScale = new Vector3(2, 1, 1);
            goTransform.position = _unserializeVector3(vidObj.transform.position);

            var goRenderer = go.GetComponent<Renderer>();
            goRenderer.material = _simpleImageMaterial;
            var goMaterial = goRenderer.material;

            var screenTexture = new RenderTexture((int)vidResource.width, (int)vidResource.height, 3);
            
            var goVideoPlayer = go.AddComponent<VideoPlayer>();
            goVideoPlayer.clip = vidResource;
            goVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
            goVideoPlayer.targetTexture = screenTexture;

            goMaterial.mainTexture = screenTexture;
            
            return go;
        }
        
        private GameObject _createAudioGO(SerializedSceneObject audioObj)
        {            
            throw new NotImplementedException();
        }

        private GameObject _createModelGO(SerializedSceneObject modelObj)
        {
            var prefabResource = UnityEngine.Resources.Load<GameObject>("models/" +
                                                                        Path.GetFileNameWithoutExtension(
                                                                            $"{modelObj.file.id}_{modelObj.file.name}"));
            if (ReferenceEquals(prefabResource, null)) throw new ResourceNotFound(modelObj.id, "Modele");
            var go = Instantiate(prefabResource);
            go.name = $"Object #{modelObj.id} - File: {modelObj.file.name} #{modelObj.file.id}";
            var goTransform = go.transform;
            goTransform.position = _unserializeVector3(modelObj.transform.position);
            goTransform.localScale = _unserializeVector3(modelObj.transform.size);
            return go;
        }

        private void applyParametersToGO(GameObject go, SerializedSceneObject src)
        {
            if (src.stuckToPlane)
            {
                var mask = new Vector3(1, 0, 1);
                go.transform.position = Vector3.Scale(go.transform.position, mask);
            }

            if (src.grabbable)
            {
                go.AddComponent<XRGrabInteractable>();
                if (src.placeholder)
                {
                    //TODO placeholder
                }
                if (src.showWhenHover)
                {
                    //TODO showwhenhover
                }
            }
        }

        private static Vector3 _unserializeVector3(SerializedSceneObject.SerializedVector3 v3) => new Vector3(v3.X, v3.Y, v3.Z);
    }
}
