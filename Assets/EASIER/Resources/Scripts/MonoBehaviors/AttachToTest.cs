using System;
using EASIER.Resources.Scripts.Archives;
using UnityEngine;

namespace EASIER.Resources.Scripts.MonoBehaviors
{
    public class AttachToTest : MonoBehaviour
    {
        private void Start()
        {
            var ar = Loader.ReadJSONContent(
                "[{\"id\":\"lgnumrr6\",\"name\":\"SampleScene\",\"objects\":[{\"id\":\"lgnuwmrr\",\"file\":{\"name\":\"190236110_212741663829447_7481100419650878565_n.jpg\",\"type\":\"Image\",\"id\":\"lgnuwl5j\"}}]},{\"id\":\"lgnuwfzv\",\"name\":\"Scene 2\",\"objects\":[]}]");
            Debug.Log(ar);
        }
    }
}