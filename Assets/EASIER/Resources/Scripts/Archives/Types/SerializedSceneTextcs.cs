using JetBrains.Annotations;

namespace EASIER.Resources.Scripts.Archives.Types
{
    public class SerializedSceneText
    {
        public string id { get; set; }
        public SerializedUnityTransform transform { get; set; }
        public string content { get; set; }

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