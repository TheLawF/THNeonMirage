using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = System.Random;

namespace THNeonMirage.Manager.UI
{
    public class DiceHandler: MonoBehaviour, IPointerClickHandler
    {
        [DisplayOnly] 
        public int DiceValue;
        public int pos;
        public bool canRenderTooltip;
        public GameObject playerInstance;
        
        private PlayerManager player;
        private Random random = new();
        private TMP_Text foreground_text; 
        private bool shouldRenderTooltip;

        private void OnGUI()
        {
            GUI.contentColor = Color.black;
            if (canRenderTooltip && shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), ToString());
        }

        public void OnMouseOver() => shouldRenderTooltip = true;    

        public void OnMouseExit() => shouldRenderTooltip = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            player = PlayerManager.Instance.GetComponent<PlayerManager>();
            pos = player.PlayerData.Position;
            
            DiceValue = random.Next(1,7);
            pos += DiceValue;
            player.SetPosition(pos);
            player.SaveAll(player.PlayerData);
            shouldRenderTooltip = true;
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}