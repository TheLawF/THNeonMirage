using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using Random = System.Random;
using Utils = THNeonMirage.Util.Utils;

namespace THNeonMirage.Manager.UI
{
    public class DiceHandler: MonoBehaviourPun, IPointerClickHandler
    {
        [DisplayOnly] 
        public int DiceValue;
        public int pos;
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
            // PlayerManager.Instance.GetComponent<PlayerManager>().PlayerData.OnPositionChanged += OnPlayerMove;
            // GameMap.OnRoundEnd += OnRoundSkip;
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
            Utils.Info($"Player Round = {player.Round}");
            // 下面这个判断的作用为是否轮到该玩家掷骰子，不管玩家是否是玩家回合或者被暂停回合或者
            if (!player.IsMyTurn() || !player.CanMove()) return;
            
            DiceValue = random.Next(1,7);
            pos = player.PlayerData.Position;
            pos += DiceValue;

            player.SetPosition(player.PlayerData, new ValueEventArgs(pos));
            inGamePanel.GetComponent<InGamePanelHandler>().SetField(player.PlayerData.Position);
            shouldRenderTooltip = true;
            gameMap.NextTurn(1);

            // if (player.PlayerData.PauseCount < 0)
            // {
            //     player.PlayerData.PauseCount++;
            //     goto ExtraMove;
            // }
        }

        public void OnRoundSkip(MonoBehaviour script, ValueEventArgs args)
        {
            var manager = (PlayerManager)script;
            manager.PlayerData.PauseCount--;
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}