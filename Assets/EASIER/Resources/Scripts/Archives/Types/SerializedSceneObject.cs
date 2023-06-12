using JetBrains.Annotations;

namespace EASIER.Resources.Scripts.Archives.Types
{
    public class SerializedSceneObject
    {
        public string id { get; set; }
        public SerializedAsset file { get; set; }
        public SerializedUnityTransform transform { get; set; }
        
        public bool stuckToPlane;
        public bool grabbable;
        public bool placeholder;
        public bool showWhenHover;

        public class SerializedVector3
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }

        public class SerializedUnityTransform
        {
            public SerializedVector3 position { get; set; }
            public SerializedVector3 size { get; set; }
        }
    }
}