using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Fictology.UnityEditor;
using FlyRabbit.EventCenter;
using MySql.Data.MySqlClient;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = Unity.Mathematics.Random;

namespace THNeonMirage
{
    public class GameHost : MonoBehaviourPunCallbacks, IPunObservable
    {
        private MySqlConnection connection;
        private DatabaseConnector connector;
        
        private User _user;
        private string serverName = "localhost";
        private string dbName = "unitygameusers";	//数据库名
        private string adminName = "root";		//登录数据库的用户名
        private string adminPwd = "123456";		//登录数据库的密码
        private string port = "3306";			//MySQL服务的端口号
        
        [Header("用户信息")]
        public GameObject playerInstance;
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        
        [Header("连接配置")]
        public string gameVersion = "1.0";
        public byte maxPlayersPerRoom = 4;
        public ObservableList<RoomInfo> rooms = new ();

        [DisplayOnly] public PlayerManager player;
        private PlayerManager local_player;

        [Header("主机和客户端")]
        public GameObject joinRoomPanel;

        private GameObject room;
        private RoomManager roomManager;

        [Header("UI父组件")]
        public GameObject canvas;
        public GameObject hudPanel;
        public GameObject lobbyPanel;
        public GameObject inGamePanel;
        
        [Header("UI预制体")]
        public GameObject buttonPrefab;
        public GameObject progressPrefab;
        public GameObject content;

        private Level level;
        private PlayerManager _playerManager;
        public TMP_Text lobbyText;
        
        public bool isConnecting;
        private GameObject bar_instance;
        private ProgressBarControl progress;
        
        public GameState currentState = GameState.WaitingForPlayers;
        public int currentTurnPlayerId = -1;
        public int currentRound = 0;
        public float turnTimeLimit = 30f; // 每回合时间限制
    
        // 玩家顺序列表
        public List<int> playerOrder = new ();
        public int currentPlayerIndex = 0;
    
        // 事件委托
        public Action<int> OnTurnChanged;
        public Action<int> OnRoundStarted;
        public Action OnGameStarted;

        // 房间属性键
        private const string ROOM_CURRENT_TURN = "CurrentTurn";
        private const string ROOM_CURRENT_ROUND = "CurrentRound";
        private const string ROOM_GAME_STATE = "GameState";
        private const string ROOM_PLAYER_ORDER = "PlayerOrder";

        private void Start()
        {
            connector = new DatabaseConnector(serverName, dbName, adminName, adminPwd);
            _user = new User(connector);
            Debug.Log("连接数据库成功");
        }
        
        
        // 注册
        public void Register()
        {
            var username = usernameInput.text;
            var pwd = EncryptPassword(passwordInput.text);
            if (username == "" || pwd == "") Debug.LogWarning("账号或密码不能为空");
            else
            {
                var status = _user.Register(username, pwd).Status;
                switch (status)
                {
                    case Authorization.ConnectionStatus.RegisterSuccess:
                        Debug.Log("注册成功");
                        break;
                    case Authorization.ConnectionStatus.DuplicateUser:
                        Debug.LogWarning("用户名已存在，请选择不同的用户名！");
                        break;
                    case Authorization.ConnectionStatus.ConnectionError:
                        Debug.LogError("连接错误");
                        break;
                    case Authorization.ConnectionStatus.SaveSuccess:
                        break;
                    default:
                    case Authorization.ConnectionStatus.Failed:
                    case Authorization.ConnectionStatus.LoginSuccess:
                    case Authorization.ConnectionStatus.UserNonExist:
                    case Authorization.ConnectionStatus.PasswordError:
                        Debug.LogWarning("注册失败");
                        break;
                }
            }
            usernameInput.text = "";
            passwordInput.text = "";
        }

        // 登录
        public void Login()
        {
            var username = usernameInput.text;
            var pwd = EncryptPassword(passwordInput.text);
            if (username == "" || pwd == "") Debug.LogWarning("账号或密码不能为空");
            else
            {
                var authorization = _user.Login(username, pwd);
                player.playerData = authorization.PlayerData;
                switch (authorization.Status)
                {
                    case Authorization.ConnectionStatus.LoginSuccess:
                        Debug.Log("登录成功");
                        break;
                    case Authorization.ConnectionStatus.UserNonExist:
                        Debug.LogWarning("用户名已存在，请选择不同的用户名！");
                        break;
                    case Authorization.ConnectionStatus.PasswordError:
                        Debug.LogWarning("用户名或者密码错误");
                        break;
                    case Authorization.ConnectionStatus.ConnectionError:
                        Debug.LogWarning("连接错误");
                        break;
                    case Authorization.ConnectionStatus.SaveSuccess:
                        break;
                    default:
                    case Authorization.ConnectionStatus.Failed:
                    case Authorization.ConnectionStatus.RegisterSuccess:
                    case Authorization.ConnectionStatus.DuplicateUser:
                        Debug.LogWarning("登录失败");
                        break;
                }

                Connect();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (cause != DisconnectCause.DisconnectByClientLogic) StartCoroutine(TryReconnect());
        }

        public void Connect()
        {
            level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            lobbyText = Registries.GetComponent<TMP_Text>(UIRegistry.LobbyText);
            lobbyText.text = "加载中...";
            
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            
            progressPrefab.SetActive(true);
            bar_instance = Instantiate(progressPrefab, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
            
            roomManager = Registries.Get<RoomManager>(UIRegistry.RoomWindow);
            Debug.Log(bar_instance.name);
            
            progress = bar_instance.GetComponent<ProgressBarControl>();
            StartCoroutine(CheckConnectionLockProgress());
        }

        private IEnumerator CheckConnectionLockProgress()
        {
            switch (PhotonNetwork.NetworkClientState)
            {
                case ClientState.ConnectingToNameServer:
                    progress.LockProgress(lockedAt: 0.9F);
                    break;
                case ClientState.ConnectedToNameServer:
                    progress.ContinueProgress();
                    break;
                case ClientState.ConnectingToMasterServer:
                    progress.LockProgress(lockedAt: 0.99F);
                    break;
                case ClientState.ConnectedToMasterServer:
                    progress.ContinueProgress();
                    break;
            }

            yield return null;
        }
        
        public override void OnConnectedToMaster()
        {
            if (!isConnecting) return;
            PhotonNetwork.JoinLobby();
            
            progress.ContinueProgress();
            progress.JumpToProgress(1F);
            var addRoom = Registries.GetObject(UIRegistry.AddRoomButton);
            var joinRoom = Registries.GetObject(UIRegistry.JoinRoomButton);
            lobbyText.text = "游戏大厅";
            
            addRoom.SetActive(true);
            joinRoom.SetActive(true);
            // joinRoom.GetComponent<Button>().onClick.AddListener(ShowInputRoomIdPanel);
            
            lobbyPanel.SetActive(true);
            isConnecting = false;
        }

        public Authorization SaveAll(PlayerData playerData) => _user.Update(playerData);
        public void Save(string username, string columnName, object data) => _user.Save(username, columnName, data);
        
        public void AddAndJoinRoom()
        {
            var randomRoomId = $"{Utils.Shuffle(1000, 9999, 25).Pop()}";
            PhotonNetwork.CreateRoom(randomRoomId, new RoomOptions { MaxPlayers = 4 });
            PhotonNetwork.JoinRoom(randomRoomId);
        }
        
        public void ExitRoom() => PhotonNetwork.LeaveRoom();
        public void CopyRoomId()
        {
            var roomIdText = Registries.GetComponent<TextMeshProUGUI>(UIRegistry.RoomIdText);
            var pattern = @"[\u200B-\u200D\uFEFF]";
            var s = Regex.Replace(roomIdText.text, pattern, "");
            GUIUtility.systemCopyBuffer = s.Replace("房间号：", "");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnJoinedRoom()
        {
            Debug.Log($"加入到房间：{PhotonNetwork.CurrentRoom}");
            hudPanel.SetActive(true);
            
            room = Registries.GetObject(UIRegistry.RoomWindow);
            room.SetActive(true);

            var roomIdText = Registries.GetComponent<TextMeshProUGUI>(UIRegistry.RoomIdText);
            roomIdText.text += PhotonNetwork.CurrentRoom.Name;
            //
            // level.CreateLevel();
            // CreateOnlinePlayer(false);
            // player.SendSpriteUpdateToOthers(null, playerInstance.GetComponent<SpriteRenderer>().color);
            // player.playerData.roundIndex = PhotonNetwork.LocalPlayer.ActorNumber;
            //
            // level.players.Add(player);
            // level.PlayerInstances.Add(playerInstance);
            //
            // InitializeGame();
        }
        
        public void CreateOnlinePlayer(bool isBot)
        {
            playerInstance = PhotonNetwork.Instantiate(PrefabRegistry.Player.PrefabPath, PlayerManager.GetPlayerPosByIndex(0), Quaternion.identity);
            player = playerInstance.GetComponent<PlayerManager>();
            level.PlayerInstances.Add(playerInstance);
            
            player.playerData.isBot = isBot;
            player.SendSpriteUpdateToOthers(null, playerInstance.GetComponent<SpriteRenderer>().color);
            player.playerData.roundIndex = PhotonNetwork.IsConnectedAndReady
                ? PhotonNetwork.LocalPlayer.ActorNumber
                : level.PlayerInstances.IndexOf(playerInstance);
            
            var exitRoom = Registries.GetComponent<Button>(UIRegistry.ExitButton);
            exitRoom.onClick.AddListener(() => GameMain.GameOver(player));
            
            if (playerInstance.GetPhotonView().IsMine)
            {
                var camera = Registries.Get<CameraController>(UIRegistry.MainCamera);
                camera.BindingPlayer = playerInstance;
            }

            var random = new Random((uint)DateTime.Now.Millisecond);
            var sprite = player.GetComponent<SpriteRenderer>();
            sprite.color = new Color(random.NextFloat(0, 1), random.NextFloat(0, 1), random.NextFloat(0, 1));
            if (!isBot)
            {
                EventCenter.TriggerEvent(EventRegistry.OnBalanceChanged, player, 
                    player.playerData.balance, player.playerData.balance);
                
                var inGamePanelHandler = Registries.GetComponent<InGamePanelHandler>(UIRegistry.InGamePanel);
                var diceButton = Registries.GetObject(UIRegistry.DiceButton);
                var diceHandler = diceButton.GetComponent<DiceHandler>();
                
                inGamePanelHandler.playerObject = playerInstance;
                inGamePanelHandler.player = player;
                diceHandler.player = player;
                diceButton.SetActive(true);
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                Registries.Instance.RegisterNetworkInstances(playerInstance.GetPhotonView(), playerInstance);
            }
        }
        

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // 添加或更新开放房间
            foreach (var room in roomList.Where(r => r.IsOpen && !r.RemovedFromList)) {
                if (!rooms.Contains(room))
                    rooms.Add(room);
                else {
                    var index = rooms.IndexOf(room);
                    rooms[index] = room;
                }
            }

            // 更新UI
            UpdateButtons();
        }
        
        public void JoinRoom(string roomName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRoom(roomName);
                inGamePanel.SetActive(true);
            }
            else Debug.LogWarning("未连接到 Photon，无法加入房间！");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            roomManager.SendPlayerJoinEvent();
            // playerInstance.GetPhotonView().RPC(nameof(roomManager.AddNewPlayerToRoomList), RpcTarget.All, playerInstance.GetPhotonView().ViewID);
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            player.SendSpriteUpdateToOthers(null, playerInstance.GetComponent<SpriteRenderer>().color);
            // 主客户端更新玩家列表
            UpdatePlayerOrder();
            SyncGameState();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"玩家 {otherPlayer.ActorNumber} 离开了房间");
            
            if (PhotonNetwork.IsMasterClient)
            {
                HandlePlayerLeft(otherPlayer.ActorNumber);
            }
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                GameMain.GameOver(player);
            }
        }
        
        private void InitializeGame()
        {
            if (PhotonNetwork.IsMasterClient) InitializeRoomProperties();
            else SyncFromRoomProperties();
            lobbyPanel.SetActive(false);
        }
        
        private void InitializeRoomProperties()
        {
            // 初始化玩家顺序
            UpdatePlayerOrder();
            
            // 设置初始房间属性
            var initialProps = new Hashtable();
            initialProps[ROOM_GAME_STATE] = (int)GameState.WaitingForPlayers;
            initialProps[ROOM_CURRENT_TURN] = 0;
            initialProps[ROOM_CURRENT_ROUND] = 1;
            initialProps[ROOM_PLAYER_ORDER] = playerOrder.ToArray();
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(initialProps);
            
            StartGame();
            // 检查是否所有玩家已准备好，开始游戏
            // if (PhotonNetwork.CurrentRoom.PlayerCount >= 2) // 假设至少需要2名玩家
            // {
            //     StartGame();
            // }
        }
        
        private void UpdatePlayerOrder()
        {
            playerOrder.Clear();
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                playerOrder.Add(player.ActorNumber);
            }
            playerOrder.Sort(); // 简单的按ActorNumber排序，可根据需要自定义
        }
        
        [PunRPC]
        public void StartGame()
        {
            currentRound = 1;
            currentPlayerIndex = 0;
            currentTurnPlayerId = playerOrder[currentPlayerIndex];
            
            // 更新房间属性
            Hashtable props = new Hashtable();
            props[ROOM_GAME_STATE] = (int)GameState.PlayerTurn;
            props[ROOM_CURRENT_TURN] = currentTurnPlayerId;
            props[ROOM_CURRENT_ROUND] = currentRound;
            props[ROOM_PLAYER_ORDER] = playerOrder.ToArray();
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            
            currentState = GameState.PlayerTurn;
            OnGameStarted?.Invoke();
            StartTurn();
            
            Debug.Log($"游戏开始！第{currentRound}轮，玩家{currentTurnPlayerId}的回合");
        }
        
        private void StartTurn()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == currentTurnPlayerId)
            {
                // 当前玩家回合的逻辑
                Debug.Log("这是你的回合！");
                StartCoroutine(TurnTimer());
            }
            
            OnTurnChanged?.Invoke(currentTurnPlayerId);
        }
        
        private IEnumerator TurnTimer()
        {
            float timeLeft = turnTimeLimit;
            while (timeLeft > 0 && currentTurnPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                timeLeft -= Time.deltaTime;
                // UIManager.Instance.UpdateTurnTimer(timeLeft); // 更新UI
                
                if (timeLeft <= 0)
                {
                    // 时间到，自动结束回合
                    NextTurn();
                    player.AITossDice();
                }
                yield return null;
            }
        }
        
        // 玩家结束回合
        public void NextTurn()
        {
            if (player.playerData.pauseCount > 0)
                player.playerData.pauseCount--;

            if (player.playerData.pauseCount < 0)
                player.playerData.pauseCount = 0;
            if (PhotonNetwork.LocalPlayer.ActorNumber == currentTurnPlayerId)
            {
                var camera = Registries.Get<CameraController>(UIRegistry.MainCamera);
                var nextBindingPlayer = level.PlayerInstances.First(
                    obj => obj.GetComponent<PlayerManager>().playerData.roundIndex == currentTurnPlayerId);
                camera.BindingPlayer = nextBindingPlayer;
                photonView.RPC(nameof(RPC_EndTurn), RpcTarget.MasterClient);
            }
        }

        [PunRPC]
        private void RPC_EndTurn()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            // 切换到下一个玩家
            currentPlayerIndex++;
            
            // 检查是否一轮结束
            if (currentPlayerIndex >= playerOrder.Count)
            {
                currentPlayerIndex = 0;
                currentRound++;
                OnRoundStarted?.Invoke(currentRound);
            }
            
            currentTurnPlayerId = playerOrder[currentPlayerIndex];
            
            // 更新房间属性
            var props = new Hashtable();
            props[ROOM_CURRENT_TURN] = currentTurnPlayerId;
            props[ROOM_CURRENT_ROUND] = currentRound;
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            
            // 通知所有客户端开始新回合
            photonView.RPC(nameof(RPC_NextTurn), RpcTarget.All, currentTurnPlayerId, currentRound);
        }
        
        [PunRPC]
        private void RPC_NextTurn(int playerId, int round)
        {
            currentTurnPlayerId = playerId;
            currentRound = round;
            StartTurn();
            
            Debug.Log($"第{currentRound}轮，玩家{currentTurnPlayerId}的回合开始");
        }
        
        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            // 同步房间属性变化
            if (propertiesThatChanged.TryGetValue(ROOM_GAME_STATE, out var state))
            {
                currentState = (GameState)(int)state;
            }
            
            if (propertiesThatChanged.TryGetValue(ROOM_CURRENT_TURN, out var playerIndex))
            {
                int newTurnPlayer = (int)playerIndex;
                if (newTurnPlayer != currentTurnPlayerId)
                {
                    currentTurnPlayerId = newTurnPlayer;
                    OnTurnChanged?.Invoke(currentTurnPlayerId);
                }
            }
            
            if (propertiesThatChanged.TryGetValue(ROOM_CURRENT_ROUND, out var round))
            {
                currentRound = (int)round;
            }
            
            if (propertiesThatChanged.TryGetValue(ROOM_PLAYER_ORDER, out var order))
            {
                playerOrder = new List<int>((int[])order);
            }
        }
        
        private void SyncFromRoomProperties()
        {
            // 从房间属性同步当前状态
            var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            
            if (roomProps.TryGetValue(ROOM_GAME_STATE, out var state))
            {
                currentState = (GameState)(int)state;
            }
            
            if (roomProps.TryGetValue(ROOM_CURRENT_TURN, out var playerIndex))
            {
                currentTurnPlayerId = (int)playerIndex;
            }
            
            if (roomProps.TryGetValue(ROOM_CURRENT_ROUND, out var round))
            {
                currentRound = (int)round;
            }
            
            if (roomProps.TryGetValue(ROOM_PLAYER_ORDER, out var order))
            {
                playerOrder = new List<int>((int[])order);
            }
        }
        
        private void HandlePlayerLeft(int leftPlayerId)
        {
            // 移除离开的玩家
            playerOrder.Remove(leftPlayerId);
            
            if (playerOrder.Count == 0)
            {
                // 所有玩家都离开了
                currentState = GameState.GameOver;
                return;
            }
            
            // 调整当前玩家索引
            if (currentTurnPlayerId == leftPlayerId)
            {
                // 如果离开的玩家是当前回合玩家，切换到下一个玩家
                RPC_EndTurn();
            }
            else
            {
                // 更新玩家顺序
                currentPlayerIndex = playerOrder.IndexOf(currentTurnPlayerId);
                SyncGameState();
            }
        }
        
        private void SyncGameState()
        {
            var props = new Hashtable();
            props[ROOM_PLAYER_ORDER] = playerOrder.ToArray();
            props[ROOM_CURRENT_TURN] = currentTurnPlayerId;
            props[ROOM_CURRENT_ROUND] = currentRound;

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        
        // 工具方法
        public bool IsMyTurn()
        {
            return PhotonNetwork.LocalPlayer.ActorNumber == currentTurnPlayerId;
        }
        
        public int GetPlayerCount()
        {
            return playerOrder.Count;
        }

        public int GetCurrentRound()
        {
            return currentRound;
        }
            
        private void AdjustContent(int itemHeight)
        {
            var childCount = content.transform.childCount;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  10 + childCount * itemHeight);
        }
        
        private void UpdateButtons()
        {
            foreach (var room in rooms) {
                var btnInstance = Instantiate(buttonPrefab, content.transform);
                var text = btnInstance.GetComponentInChildren<TMP_Text>();
                var rect = btnInstance.GetComponent<RectTransform>().rect;
                
                rect.width = text.preferredWidth + 4;
                rect.height = text.preferredHeight + 4;
                text.color = Color.white;
                
                btnInstance.GetComponentInChildren<TMP_Text>().text = $"房间名：{room.Name}  玩家：{room.PlayerCount}/{room.MaxPlayers}";
                btnInstance.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.Name));
            }
            AdjustContent(20); // 根据按钮高度调整
        }

        private IEnumerator TryReconnect()
        {
            if (PhotonNetwork.Reconnect())
            {
                var waitTime = 0f;
                var timeout = 10f; // 10秒超时

                while (waitTime < timeout)
                {
                    waitTime += Time.deltaTime;
                    if (PhotonNetwork.IsConnected) yield break;
                    yield return null;
                }
            }
        }

        public static string EncryptPassword(string password)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }
    }
    public enum GameState
    {
        WaitingForPlayers,
        PlayerTurn,
        RoundEnd,
        GameOver
    }
}