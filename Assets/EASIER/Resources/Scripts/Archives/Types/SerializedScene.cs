using System;
using JetBrains.Annotations;

namespace EASIER.Resources.Scripts.Archives.Types
{
    public class SerializedScene
    {
        public string id { get; set; }
        public string name { get; set; }
        public SerializedSceneObject[] objects { get; set; }
    }
}