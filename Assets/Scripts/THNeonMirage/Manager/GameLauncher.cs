using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Manager.UI;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace THNeonMirage.Manager
{
    public class GameLauncher : MonoBehaviour
    {
        [NotNull]
        private PlayerData player_data;
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
        public GameObject playerPrefab;
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;

        [Header("主机和客户端")]
        public GameObject gameManager;
        public GameObject host;
        public GameObject client;
        public GameObject hostUI;
        public GameObject roomNameLabel;

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

        private GameMap game_map;
        private GameHost game_host;
        private GameClient game_client;
        private PlayerManager _playerManager;

        private void Start()
        {
            dice = diceObject.GetComponent<DiceHandler>();
            connector = new DatabaseConnector(serverName, dbName, adminName, adminPwd);
            _user = new User(connector);
            Debug.Log("连接数据库成功");
        }


        private void Awake()
        {
            gameManager = GameObject.Find("GameManager");
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
                player_data = authorization.PlayerData;
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

                StartClient();
                // if (Administrators.Exists(s => s.Equals(player_data.UserName))) StartHost();
                // else StartClient();
            }
        }
        
        private void StartHost()
        {
            var hostInst = Instantiate(host);
            game_host = hostInst.GetComponent<GameHost>();
            InitClient();
            hostUI.SetActive(true);
        }
        
        private void StartClient()
        {
            var clientInst = Instantiate(client);
            game_client = clientInst.GetComponent<GameClient>();
            dice.client = game_client;
            dice.inGamePanel = inGamePanel;
            InitClient();
        }

        private void InitClient()
        {
            game_client = game_client.GetComponent<GameClient>();
            game_map = gameManager.GetComponent<GameMap>();
            game_map.client = game_client.GetComponent<GameClient>();
            game_client.gameMap = game_map;
            game_client.data = player_data;

            game_client.canvas = canvas;
            game_client.hudPanel = hudPanel;
            game_client.lobbyPanel = lobbyPanel;
            game_client.inGamePanel = inGamePanel;

            game_client.buttonPrefab = buttonPrefab;
            game_client.progressPrefab = progressPrefab;
            game_client.balanceLabel = balanceLabel;
            game_client.content = content;
            game_client.Connect();

            homePanel.SetActive(false);
            diceObject.SetActive(true);
            dice.pos = _playerManager.PlayerData.Position;
        }

        public Authorization SaveAll(PlayerData playerData) => _user.Update(playerData);
        public void Save(string username, string columnName, object data) => _user.Save(username, columnName, data);
        
        public void AddAndJoinRoom()
        {
            var randomRoomId = $"{Utils.UniqueShuffle(1000, 9999, 25).Pop()}";
            PhotonNetwork.CreateRoom(randomRoomId, new RoomOptions { MaxPlayers = 4 });
        }

        public void ExitRoom() => PhotonNetwork.LeaveRoom();
        public void CopyRoomId() => GUIUtility.systemCopyBuffer = roomNameLabel.GetComponent<TMP_Text>().text;
        
        
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