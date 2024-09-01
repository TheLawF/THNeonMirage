using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace THNeonMirage.Manager
{
    public class UIManager: MonoBehaviour
    {
        public Texture2D backgroundTexture;
        public Color backgroundColor;
        public string text;

        
        public bool shouldRenderTooltip;
        public GameObject hoverText;
        private TMP_Text foregroundText;
        private void Start()
        {
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.white;
            if (shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), text);
        }
        
        public void OnMouseOver() => shouldRenderTooltip = true;
        public void OnMouseExit() => shouldRenderTooltip = false;
    }
}