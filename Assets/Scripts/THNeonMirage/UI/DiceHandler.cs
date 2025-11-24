using Fictology.UnityEditor;
using Photon.Pun;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = System.Random;

namespace THNeonMirage.UI
{
    public class DiceHandler: MonoBehaviourPun, IPointerClickHandler
    { 
        [DisplayOnly] public int DiceValue;
        public int pos;
        public bool canInteract;
        public bool canRenderTooltip;

        public Level level;
        public GameClient client;
        public GameObject inGamePanel;

        private bool shouldRenderTooltip;
        private Random random = new();
        private TMP_Text foreground_text; 
        public PlayerManager player;

        private void Start()
        {
            canInteract = true;
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.black;
            if (canRenderTooltip && shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), ToString());
        }

        public void OnMouseOver() => shouldRenderTooltip = true;    

        public void OnMouseExit() => shouldRenderTooltip = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (player.playerData.roundIndex != level.PlayerRound) return;
            DiceValue = random.Next(1,7);
            pos = player.playerData.position;
            pos += DiceValue;

            player.SetPosIndex(pos);
            inGamePanel.GetComponent<InGamePanelHandler>().SetField(player.playerData.position);
            shouldRenderTooltip = true;
            level.NextTurn();
        }

        
        
        private new string ToString() => string.Concat(DiceValue);
    }
}