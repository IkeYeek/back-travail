using JetBrains.Annotations;

namespace EASIER.Resources.Scripts.Archives.Types
{
    //TODO change directory of archives to make them unique and avoid potential collisions
    public class SerializedArchive
    {
        public SerializedScene[] SerializedScenes { get; set; }
    }
}