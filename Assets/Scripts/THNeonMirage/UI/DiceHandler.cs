using Photon.Pun;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

namespace THNeonMirage.UI
{
    public class DiceHandler: MonoBehaviourPun, IPointerClickHandler
    { 
        [DisplayOnly] public int DiceValue;
        public int pos;
        public bool canInteract;
        public bool canRenderTooltip;

        public GameMap gameMap;
        public GameClient client;
        public GameObject inGamePanel;

        private bool shouldRenderTooltip;
        private Random random = new();
        private TMP_Text foreground_text; 
        private PlayerManager player;

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
            player = client.GetComponent<GameClient>().playerInstance.GetComponent<PlayerManager>();
            if (PhotonNetwork.LocalPlayer.ActorNumber != gameMap.PlayerOrder[gameMap.ActorOrder - 1]) return;
            DiceValue = random.Next(1,7);
            pos = player.PlayerData.Position;
            pos += DiceValue;

            player.SetPosition(player.PlayerData, new ValueEventArgs(pos));
            inGamePanel.GetComponent<InGamePanelHandler>().SetField(player.PlayerData.Position);
            shouldRenderTooltip = true;
            gameMap.NextTurn();
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}