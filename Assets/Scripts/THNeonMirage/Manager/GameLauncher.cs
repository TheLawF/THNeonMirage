using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Fictology.UnityEditor;
using MySql.Data.MySqlClient;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Map;
using THNeonMirage.UI;
using THNeonMirage.Util;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace THNeonMirage.Manager
{
    public class GameLauncher : MonoBehaviourPunCallbacks
    {
        private MySqlConnection connection;
        private DatabaseConnector connector;
        private DiceHandler dice;
        
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

        [Header("主机和客户端")]
        public GameObject host;
        public GameObject client;
        public GameObject hostUI;
        public GameObject joinRoomPanel;
        public GameObject roomNameLabel;
        public GameObject gameManager;

        [Header("UI父组件")]
        public GameObject canvas;
        public GameObject hudPanel;
        public GameObject homePanel;
        public GameObject lobbyPanel;
        public GameObject inGamePanel;
        
        [Header("UI预制体")]
        public GameObject diceObject;
        public GameObject balanceLabel;
        public GameObject buttonPrefab;
        public GameObject progressPrefab;
        public GameObject content;

        private Level level;
        private GameHost game_host;
        private GameClient game_client;
        private PlayerManager _playerManager;
        
        private bool isConnecting;
        private GameObject bar_instance;

        private void Awake()
        {
            gameManager = GameObject.Find("GameManager");
        }
        private void Start()
        {
            dice = diceObject.GetComponent<DiceHandler>();
            connector = new DatabaseConnector(serverName, dbName, adminName, adminPwd);
            _user = new User(connector);
            Debug.Log("连接数据库成功");
        }
        
        public void Connect()
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
            PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            
            progressPrefab.SetActive(true);
            var bar = Instantiate(progressPrefab, new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
            bar_instance = bar;
            
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

        public Authorization SaveAll(PlayerData playerData) => _user.Update(playerData);
        public void Save(string username, string columnName, object data) => _user.Save(username, columnName, data);
        
        public void AddAndJoinRoom()
        {
            var randomRoomId = $"{Utils.UniqueShuffle(1000, 9999, 25).Pop()}";
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.CreateRoom(randomRoomId, new RoomOptions { MaxPlayers = 4 });
            PhotonNetwork.JoinRoom(randomRoomId);
        }
        
        public void ExitRoom() => PhotonNetwork.LeaveRoom();
        public void CopyRoomId() => GUIUtility.systemCopyBuffer = roomNameLabel.GetComponent<TMP_Text>().text;
        public void ShowInputRoomIdPanel() => joinRoomPanel.SetActive(true);
        
        protected GameObject Initialize<TArgs>(string prefabName, Vector3 pos, Quaternion rotation, TArgs arg5)
        {
            playerInstance = PhotonNetwork.Instantiate(prefabName, pos, rotation);
            if (arg5 is not PlayerEventArgs args) return playerInstance;
            lobbyPanel.SetActive(false);
            inGamePanel.SetActive(true);
            
            var inGame = inGamePanel.GetComponent<InGamePanelHandler>();
            inGame.player = player;
            
            player = playerInstance.GetComponent<PlayerManager>();
            player.level = level;
            // playerManager.Instance = playerInstance;
            
            player.playerData.balance += 60_000;

            return playerInstance;
        }

        
        public override void OnConnectedToMaster()
        {
            if (!isConnecting) return;
            PhotonNetwork.JoinRandomRoom();
            Destroy(bar_instance);
            
            lobbyPanel.SetActive(true);
            isConnecting = false;

        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnJoinedRoom()
        {
            Debug.Log($"加入到房间：{PhotonNetwork.CurrentRoom}");
            hudPanel.SetActive(true);
            CreatePlayer();

            level.CreateLevel();
            player.playerData.roundIndex = PhotonNetwork.CurrentRoom.Players.Keys.Count;
            player.playerData.roundIndex = PhotonNetwork.LocalPlayer.ActorNumber;
            
            level.Players.Add(PhotonNetwork.LocalPlayer);
            level.PlayerInstances.Add(playerInstance);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "can_interact", "true" } });
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

        private void UpdateButtons()
        {
            foreach (var room in rooms) {
                var newButton = Instantiate(buttonPrefab, content.transform);
                newButton.GetComponentInChildren<TMP_Text>().text = 
                    $"房间名：{room.Name}  玩家：{room.PlayerCount}/{room.MaxPlayers}";
                newButton.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room.Name));
            }
            AdjustContent(20); // 根据按钮高度调整
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

        
        private void AdjustContent(int itemHeight)
        {
            var childCount = content.transform.childCount;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  10 + childCount * itemHeight);
        }
        
        public void CreatePlayer()
        {
            Initialize("playerObject", PlayerManager.GetPlayerPosByIndex(player.playerData.position), Quaternion.identity, new PlayerEventArgs(0));
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // level.PlayerOrder.Add(newPlayer.ActorNumber);
            level.PlayerInstances.Add(playerInstance);
            level.players.Add(playerInstance.GetComponent<PlayerManager>());
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
    }
}