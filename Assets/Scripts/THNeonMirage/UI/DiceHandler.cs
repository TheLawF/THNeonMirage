using Fictology.UnityEditor;
using Photon.Pun;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.Util;
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
            inGamePanel = Registries.GetObject(UIRegistry.InGamePanel);
            level = Registries.Get<Level>(LevelRegistry.Level);
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
            if (player.IsBot()) return;
            if (!player.IsMyTurn()) return;
            
            DiceValue = random.Next(1,7);
            pos = player.playerData.position;
            pos += DiceValue;

            Debug.Log(pos.ToString());
            
            player.SetPosIndex(pos);
            inGamePanel.GetComponent<InGamePanelHandler>().SetTile(level, player.playerData.position);
            shouldRenderTooltip = true;
            level.NextTurn();
        }

        
        
        private new string ToString() => string.Concat(DiceValue);
    }
}