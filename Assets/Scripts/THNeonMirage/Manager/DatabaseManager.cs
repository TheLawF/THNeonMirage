using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using THNeonMirage.Data;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace THNeonMirage.Manager
{
    public class DatabaseManager : MonoBehaviour
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

        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;

        public GameObject hud;
        public GameObject homePanel;
        public GameObject mapObject;
        public GameObject inGamePanel;
        
        [DisplayOnly]
        public GameObject playerObj;
        public GameObject playerPrefab;
        public GameObject diceObject;
        public GameObject balanceDisplay;

        private void Start()
        {
            // playerPrefab.GetComponent<PlayerManager>().PlayerData.Position = 0;
            dice = diceObject.GetComponent<DiceHandler>();
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
            }
            usernameInput.text = "";
            passwordInput.text = "";
            
            homePanel.SetActive(false);
            inGamePanel.SetActive(true);
            hud.SetActive(true);

            playerObj = Instantiate(playerPrefab);
            var player = playerObj.GetComponent<PlayerManager>().Init(player_data);
            var map = mapObject.GetComponent<GameMap>();
            
            map.players.Add(player);
            player.PlayerData.Balance = 600000;
            player.database = this;
            player.inGamePanel = inGamePanel;
            player.BalanceText = balanceDisplay.GetComponent<TMP_Text>();
            
            var handler = inGamePanel.GetComponent<ToggleHandler>();
            handler.player = player;

            dice.player = player;
            dice.player.PlayerData.Position = player.PlayerData.Position;
            dice.pos = player.PlayerData.Position;
            dice.playerObject = playerObj;
            
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