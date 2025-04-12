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
    public class DiceHandler: MonoBehaviour, IPointerClickHandler
    {
        [DisplayOnly] 
        public int DiceValue;
        public int pos;
        public bool canRenderTooltip;
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
            GameMap.Activity++;
            player = client.GetComponent<GameClient>().playerInstance.GetComponent<PlayerManager>();
            
            // 下面这个判断的作用为是否轮到该玩家掷骰子
            Utils.Info($"Game Activity = {GameMap.Activity}, Your Activity = {player.Activity}");
            if (GameMap.Activity == player.Activity) 
                return;
            
            // 判断玩家是否被停止行动
            Utils.Info($"Pause = {player.PlayerData.PauseCount}");
            if (player.PlayerData.PauseCount > 0) 
                return;
            DiceValue = random.Next(1,7);
            pos = player.PlayerData.Position;
            pos += DiceValue;

            player.SetPosition(player.PlayerData, new ValueEventArgs(pos));
            OnPlayerMove(player.PlayerData, new ValueEventArgs(pos));
            inGamePanel.GetComponent<InGamePanelHandler>().SetTexts(player.PlayerData.Position);
            shouldRenderTooltip = true;
            // if (player.PlayerData.PauseCount < 0)
            // {
            //     player.PlayerData.PauseCount++;
            //     goto ExtraMove;
            // }
        }

        public void OnPlayerMove(object sender, ValueEventArgs currentPos)
        {
            var data = (PlayerData)sender;
            data.PauseCount--;
            GameMap.Activity++;
        }

        public void OnRoundSkip(MonoBehaviour script, ValueEventArgs args)
        {
            var manager = (PlayerManager)script;
            manager.PlayerData.PauseCount--;
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}