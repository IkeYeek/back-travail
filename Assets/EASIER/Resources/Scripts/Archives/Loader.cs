using EASIER.Resources.Scripts.Archives.Types;
using Newtonsoft.Json;
using UnityEngine;

namespace EASIER.Resources.Scripts.Archives
{
    public class Loader
    {
        public static SerializedArchive ReadJSONContent(string json)
        {
            return new SerializedArchive { SerializedScenes = JsonConvert.DeserializeObject<SerializedScene[]>(json)};
        }
    }

}