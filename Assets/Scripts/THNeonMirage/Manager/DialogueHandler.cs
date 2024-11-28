using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace THNeonMirage.Manager
{
    public class DialogueHandler : MonoBehaviour
    {
        public Image img;
        public string title;
        public string description;
        public DialogueType type;
        
        private void Start()
        {
            img = GetComponent<Image>();
        }

    }

    public enum DialogueType
    {
        Info,
        Warn,
        Error,
        Fatal
    }
}
