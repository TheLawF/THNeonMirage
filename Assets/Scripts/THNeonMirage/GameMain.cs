using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Fictology.Data.Serialization;
using Fictology.Registry;
using FlyRabbit.EventCenter;
using THNeonMirage.Data;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util.Math;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

namespace THNeonMirage
{
    /// <summary>
    /// PUN2 学习笔记：<br></br>
    /// <list type="number">
    /// <item>
    /// photonView.RPC: 在方法中使用 <code> photonView.RPC("methodName", RpcTarget.ALL, parameters) </code>表示将数据从本地端发送到远程端进行网络通信
    /// </item>
    /// <item>
    /// PunRPC：在带有该属性的方法如果存在参数且不存在photonView.RPC调用，表明该方法是一个在本地端接收远程端数据修改本地端数据的接收方法
    /// </item>
    /// <item>
    /// PhotonNetwork.IsMasterClient：判断当前本地端是否为房主端，是Pun在云端自动管理的一个属性，可用于管理房间信息，例如GamHose.cs内注册表的增删改，在增删改等操作后可以使用PunRPC同步到远程端的GameHost.cs的注册表，之后远程端就能实时查看注册表内容了
    /// </item>
    /// <item>
    /// PhotonNetwork.LocalPlayer.ActorNumber 初始为 1，当有玩家退出房间后，新加入的玩家不会被分配退出的玩家的ActorNumber，而会分配一个新值
    /// </item>
    /// </list>
    /// </summary>
    public class GameMain : MonoBehaviour
    {
        public Button startButton;
        public Button aboutButton;
        public TMP_Text balanceLabel;
        public Random random;
        
        public Level level;
        public List<GameObject> players;

        public GameObject lobby;
        public GameObject diceObj;
        public GameObject inGamePanelObj;
        public DiceHandler dice;
        public InGamePanelHandler inGamePanel;
        public GameHost host;

        private static int m_type_code = 23;

        private void Awake()
        {
            PhotonPeer.RegisterType(typeof(PlayerData), NextId(), PlayerData.Serialize, PlayerData.Deserialize);
            PhotonPeer.RegisterType(typeof(ISynchronizable), NextId(), ISynchronizable.Serialize, ISynchronizable.Deserialize);
        }

        private static byte NextId()
        {
            return (byte)(m_type_code + 1);
        }

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
            level = Registries.GetComponent<Level>(LevelRegistry.ClientLevel);

            lobby = Registries.GetObject(UIRegistry.LobbyPanel);
            inGamePanelObj = Registries.GetObject(UIRegistry.InGamePanel);
            inGamePanel = inGamePanelObj.GetComponent<InGamePanelHandler>();
            random = new Random((uint)DateTime.Now.Millisecond);

            host = Registries.GetComponent<GameHost>(LevelRegistry.ServerLevel);
        }

        private void RegisterUIListeners()
        {
            startButton.onClick.AddListener(OnGameStartClicked);
        }
        
        public void OnGameStartClicked()
        {
            Registries.GetObject(UIRegistry.HomePage).SetActive(false);
            Registries.Tiles.Values.ToList().ForEach(go => go.SetActive(true));

            lobby.SetActive(true);
            host.Connect();
            // level.CreateLevel();
            // inGamePanelObj.SetActive(true);
            CreateEventListeningChain();
            level.players.AddRange(players.Select(obj => obj.GetComponent<PlayerManager>()));
        }

        public void CreatePlayer(bool isBot)
        {
            var playerObject = LevelRegistry.Player.Instantiate(PlayerManager.GetPlayerPosByIndex(0), Quaternion.identity);
            var player = playerObject.GetComponent<PlayerManager>();
            var sprite = player.GetComponent<SpriteRenderer>();
            players.Add(playerObject);
            
            player.playerData.isBot = isBot;
            player.playerData.roundIndex = players.IndexOf(playerObject);
            sprite.color = new Color(random.NextFloat(0, 1), random.NextFloat(0, 1), random.NextFloat(0, 1));

            if (isBot) return;
            EventCenter.TriggerEvent(EventRegistry.OnBalanceChanged, player, player.playerData.balance, player.playerData.balance);
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
            balanceLabel.SetText("月虹币余额：" + currentBalance);
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