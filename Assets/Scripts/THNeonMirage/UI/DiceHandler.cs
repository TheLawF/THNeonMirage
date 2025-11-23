using Fictology.UnityEngine;
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

        [FormerlySerializedAs("levelManager")] [FormerlySerializedAs("gameMap")] public Level level;
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
            if (PhotonNetwork.LocalPlayer.ActorNumber != level.PlayerOrder[level.PlayerRound - 1]) return;
            DiceValue = random.Next(1,7);
            pos = player.playerData.Position;
            pos += DiceValue;

            player.SetPosIndex(player.playerData, pos);
            inGamePanel.GetComponent<InGamePanelHandler>().SetField(player.playerData.Position);
            shouldRenderTooltip = true;
            level.NextTurn();
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}