using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExitGames.Client.Photon.StructWrapping;
using Fictology.Registry;
using FlyRabbit.EventCenter;
using FlyRabbit.EventCenter.Core;
using Photon.Pun;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util.Math;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using KeyValuePair = System.Collections.Generic.KeyValuePair;

namespace THNeonMirage
{
    public class GameMain : MonoBehaviour
    {
        public Button startButton;
        public Button aboutButton;
        
        public Level level;
        public List<GameObject> players;
        public GameObject inGamePanelObj;
        public InGamePanelHandler inGamePanel;
        
        public GameObject diceObj;
        public DiceHandler dice;

        /// <summary>
        /// GameStart Clicked -> Disable Home Panel -> Enable Background -> Create Map -> Game Loop
        /// </summary>
        private void Start()
        {
            RegisterWhenSceneStart();
            InitAllFields();
            RegisterUIListeners();
            CreateEventListeningChain();
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
            level = Registries.GetComponent<Level>(LevelRegistry.Level);
        }

        private void RegisterUIListeners()
        {
            startButton.onClick.AddListener(OnGameStartClicked);
        }
        
        public void CreateEventListeningChain()
        {
        }
        
        public void OnGameStartClicked()
        {
            Registries.GetObject(UIRegistry.HomePage).SetActive(false);
            Registries.Tiles.Values.ToList().ForEach(go => go.SetActive(true));
            level.CreateLevel();

            var client = GetComponent<GameClient>();
            
            CreatePlayer(false);
            CreatePlayer(true);
            CreatePlayer(true);
            CreatePlayer(true);
            
            level.players.AddRange(players.Where(obj => obj.GetComponent<PlayerManager>()));
        }

        public void CreatePlayer(bool isBot)
        {
            var playerObject = LevelRegistry.Player.Instantiate(PlayerManager.GetPlayerPosByIndex(0), Quaternion.identity);
            var player = playerObject.GetComponent<PlayerManager>();
            players.Add(playerObject);
            
            player.playerData.isBot = isBot;
            player.playerData.roundIndex = players.IndexOf(playerObject);

            if (!isBot) return;
            inGamePanelObj = Registries.GetObject(UIRegistry.InGamePanel);
            inGamePanel = Registries.GetComponent<InGamePanelHandler>(UIRegistry.InGamePanel);
            inGamePanel.player = player;
            
            diceObj = Registries.GetObject(UIRegistry.DiceButton);
            diceObj.SetActive(true);
            dice = diceObj.GetComponent<DiceHandler>();
            dice.player = player;

        }

        private void OnMouseUpAsButton()
        {
            if (inGamePanelObj == null) return;
            if(RayHelper.CheckMouseClickHit(Camera.main, out var hit))
            {
                inGamePanelObj.SetActive(false);
            }
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