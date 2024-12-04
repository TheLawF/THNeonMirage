using THNeonMirage.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

namespace THNeonMirage.Manager
{
    public class DiceHandler: MonoBehaviour, IPointerClickHandler
    {
        [DisplayOnly] 
        public int DiceValue;
        public int pos;
        public bool canRenderTooltip;
        private bool shouldRenderTooltip;

        public GameObject playerObject;
        public PlayerManager player;
        
        private Random random = new();
        private TMP_Text foreground_text;
        
        private void Start()
        {
            DiceValue = 1;
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.black;
            if (canRenderTooltip && shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), ToString());
        }
        
        public void OnMouseExit() => shouldRenderTooltip = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            DiceValue = random.Next(1,7);
            pos += DiceValue;
            player.SetPosition(pos);
            player.SaveAll(player.PlayerData);
            // Debug.Log($"Index = {player.PlayerData.Position}, Pos = {pos}, Dice = {DiceValue}");
            shouldRenderTooltip = true;
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}