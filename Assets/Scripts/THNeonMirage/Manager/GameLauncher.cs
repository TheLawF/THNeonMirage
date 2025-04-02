using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using THNeonMirage.Data;
using THNeonMirage.Manager.UI;
using THNeonMirage.Map;
using TMPro;
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

        [Header("服务端客户端启动器")]
        public GameObject gameManager;
        public GameObject launcher;
        public GameObject host;
        public GameObject client;

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

        private static readonly List<string> Administrators = new()
        {
            "user", "admin"
        };

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

                if (Administrators.Exists(s => s.Equals(player_data.UserName))) StartHost();
                else StartClient();
            }
        }
        
        private void StartHost()
        {
            var hostInst = Instantiate(host);
            DontDestroyOnLoad(hostInst);
            game_host = hostInst.GetComponent<GameHost>();
            InitClient(game_host);
        }
        
        private void StartClient()
        {
            var clientInst = Instantiate(client);
            DontDestroyOnLoad(clientInst);
            game_client = clientInst.GetComponent<GameClient>();
            InitClient(game_client);
            
        }

        private void InitClient(GameClient gameClient)
        {
            gameClient = gameClient.GetComponent<GameClient>();
            game_map = gameManager.GetComponent<GameMap>();
            game_map.client = client.GetComponent<GameClient>();
            game_client.gameMap = game_map;
            game_client.data = player_data;

            gameClient.canvas = canvas;
            gameClient.hudPanel = hudPanel;
            gameClient.lobbyPanel = lobbyPanel;
            gameClient.inGamePanel = inGamePanel;

            gameClient.buttonPrefab = buttonPrefab;
            gameClient.progressPrefab = progressPrefab;
            gameClient.balanceLabel = balanceLabel;
            gameClient.content = content;
            gameClient.Connect();

            homePanel.SetActive(false);
            lobbyPanel.SetActive(true);
            diceObject.SetActive(true);
            dice.pos = _playerManager.PlayerData.Position;
        }

        public Authorization SaveAll(PlayerData playerData) => _user.Update(playerData);
        public void Save(string username, string columnName, object data) => _user.Save(username, columnName, data);

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