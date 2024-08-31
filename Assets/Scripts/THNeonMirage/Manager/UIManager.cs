using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class UIManager: MonoBehaviour
    {
        public bool hasHoverInfo;
        public GameObject hoverPanel;
        public Texture2D backgroundTexture;
        public Color backgroundColor;
        public string text;

        private GameObject bgImage;
        private GameObject fgText;
        private void Start()
        {
            bgImage = hoverPanel.transform.GetChild(0).GameObject();
            fgText = hoverPanel.transform.GetChild(1).GameObject();
            bgImage.GetComponent<Image>().color = backgroundColor;
            fgText.GetComponent<TMP_Text>().text = text;
        }

        public void OnMouseOver()
        {
            if (!hasHoverInfo) return;
            hoverPanel.transform.position = Input.mousePosition + new Vector3(40f, 40f);
            hoverPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = text;
            hoverPanel.SetActive(true);
            
        }

        public void OnMouseExit()
        {
            hoverPanel.SetActive(false);
        }
    }
}