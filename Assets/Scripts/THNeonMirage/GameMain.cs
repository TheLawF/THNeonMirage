using System.Collections.Generic;
using System.Linq;
using Fictology.Registry;
using FlyRabbit.EventCenter;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util.Math;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace THNeonMirage
{
    public class GameMain : MonoBehaviour
    {
        public Button startButton;
        public Button aboutButton;
        public TMP_Text balanceLabel;
        
        public Level level;
        public List<GameObject> players;
        
        public GameObject diceObj;
        public GameObject inGamePanelObj;
        public DiceHandler dice;
        public InGamePanelHandler inGamePanel;

        /// <summary>
        /// GameStart Clicked -> Disable Home Panel -> Enable Background -> Create Map -> Game Loop
        /// </summary>
        private void Start()
        {
            RegisterWhenSceneStart();
            InitAllFields();
            RegisterUIListeners();
        }


        private void RegisterWhenSceneStart()
        {
            UIRegistry.RegisterTypes();
            LevelRegistry.RegisterTypes();
            
            var validEntries = GetAllSceneObjects()
                .Select(obj => new { Object = obj, Entry = obj.GetComponent<RegistryEntry>() })
                .Where(objAndEntry => objAndEntry.Entry != null && Registries.RegistryTypes.Contains(objAndEntry.Entry.GetType()))
                .ToDictionary(objAndEntry => objAndEntry.Entry, objAndEntry => objAndEntry.Object);

            Registries.RegisterAll(validEntries);
        }

        private void InitAllFields()
        {
            startButton = Registries.GetComponent<Button>(UIRegistry.StartButton);
            aboutButton = Registries.GetComponent<Button>(UIRegistry.AboutButton);
            balanceLabel = Registries.GetComponent<TMP_Text>(UIRegistry.BalanceText);
            level = Registries.GetComponent<Level>(LevelRegistry.Level);
            
            inGamePanelObj = Registries.GetObject(UIRegistry.InGamePanel);
        }

        private void RegisterUIListeners()
        {
            startButton.onClick.AddListener(OnGameStartClicked);
        }
        
        public void OnGameStartClicked()
        {
            Registries.GetObject(UIRegistry.HomePage).SetActive(false);
            Registries.Tiles.Values.ToList().ForEach(go => go.SetActive(true));
            
            level.CreateLevel();
            inGamePanelObj.SetActive(true);

            var client = GetComponent<GameClient>();
            
            CreatePlayer(false);
            CreatePlayer(true);
            CreatePlayer(true);
            CreatePlayer(true);
            
            CreateEventListeningChain();
            level.players.AddRange(players.Select(obj => obj.GetComponent<PlayerManager>()));
        }

        public void CreatePlayer(bool isBot)
        {
            var playerObject = LevelRegistry.Player.Instantiate(PlayerManager.GetPlayerPosByIndex(0), Quaternion.identity);
            var player = playerObject.GetComponent<PlayerManager>();
            players.Add(playerObject);
            
            player.playerData.isBot = isBot;
            player.playerData.roundIndex = players.IndexOf(playerObject);

            if (isBot) return;
            inGamePanelObj = Registries.GetObject(UIRegistry.InGamePanel);
            inGamePanel = Registries.GetComponent<InGamePanelHandler>(UIRegistry.InGamePanel);
            inGamePanel.player = player;
            
            diceObj = Registries.GetObject(UIRegistry.DiceButton);
            diceObj.SetActive(true);
            dice = diceObj.GetComponent<DiceHandler>();
            dice.player = player;
        }
        
        
        public void CreateEventListeningChain()
        {
            EventCenter.AddListener<PlayerManager, int, int>(EventRegistry.OnBalanceChanged, SetBalanceText);
            EventCenter.AddListener<PlayerManager, int, int>(EventRegistry.OnBalanceChanged, CheckBalance);
        }


        private void OnMouseUpAsButton()
        {
            if (inGamePanelObj == null) return;
            if(!RayHelper.CheckMouseClickHit(Camera.main, out var hit))
            {
                inGamePanelObj.SetActive(false);
            }
        }

        private void SetBalanceText(PlayerManager player, int prevBalance, int currentBalance)
        {
            if (player.playerData.isBot) return;
            // if(!PhotonView.IsMine) return;
            balanceLabel.SetText(currentBalance.ToString());
        }

        private void CheckBalance(PlayerManager player, int prevBalance, int currentBalance)
        {
            var anyPropertyLeft = player.playerData.Fields.Count > 0;
            var affordableTolls = currentBalance >= 0;
            if (affordableTolls) return;
            if (!anyPropertyLeft) GameOver(player);
            
        }

        public void GameOver(PlayerManager player)
        {
            
        }

        public static List<GameObject> GetAllSceneObjects(bool includeInactive = true)
        {
            var allObjects = new List<GameObject>();
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    
            foreach (var root in rootObjects)
            {
                // 如果includeInactive为false，且根物体被禁用，则跳过
                if (!includeInactive && !root.activeInHierarchy) continue;
        
                allObjects.Add(root);
                GetChildrenRecursive(root.transform, allObjects, includeInactive);
            }
    
            return allObjects;
        }

        private static void GetChildrenRecursive(Transform parent, List<GameObject> objectList, bool includeInactive)
        {
            foreach (Transform child in parent)
            {
                if (includeInactive || child.gameObject.activeInHierarchy)
                {
                    objectList.Add(child.gameObject);
                    GetChildrenRecursive(child, objectList, includeInactive);
                }
            }
        }
    }
}