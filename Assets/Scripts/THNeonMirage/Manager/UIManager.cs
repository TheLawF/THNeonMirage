using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

namespace THNeonMirage.Manager
{
    public class UIManager: MonoBehaviour, IPointerClickHandler
    {
        public Texture2D backgroundTexture;
        public Color backgroundColor;
        public string text;

        public bool shouldRenderTooltip;
        public GameObject hoverText;
        public TMP_FontAsset fontAsset;

        [DisplayOnly] 
        public int DiceValue;

        private Random random = new();
        private TMP_Text foreground_text;
        private GUIStyle _guiStyle = new GUIStyle();
        private void Start()
        {
            DiceValue = 1;
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.black;
            if (shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), ToString());
        }
        
        public void OnMouseExit() => shouldRenderTooltip = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            DiceValue = random.Next(1,7);
            shouldRenderTooltip = true;
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}