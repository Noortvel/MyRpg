using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRpg
{
    public class DLogger : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text text;
        private static UnityEngine.UI.Text _stext;
        private static RectTransform textRect;

        private void Awake()
        {
            _stext = text;
            textRect = text.GetComponent<RectTransform>();
        }

        public static void WriteLineToScreen(string str)
        {
            _stext.text += str;
            _stext.text += '\n';
        }

    }
}