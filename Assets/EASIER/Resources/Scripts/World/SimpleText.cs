using System.Collections;
using System.Collections.Generic;
using EASIER.Resources.Scripts.Exceptions;
using TMPro;
using UnityEngine;

namespace EASIER
{
    public class SimpleText : MonoBehaviour
    {
        public string Text;

        [SerializeField] private TextMeshProUGUI _textMeshPro;
        // Start is called before the first frame update
        void Start()
        {
            _textMeshPro.text = Text;
        }
    }
}
