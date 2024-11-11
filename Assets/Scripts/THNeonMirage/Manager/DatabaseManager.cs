using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using THNeonMirage.Data;
using THNeonMirage.Map;
using TMPro;
using UnityEngine;

namespace THNeonMirage.Manager
{
    public class DatabaseManager : MonoBehaviour
    {
        private GameObject playerObj;
        
        private MySqlConnection connection;
        private DatabaseConnector Connector;
        private PlayerData player_data;
        private User User;

        private string serverName = "localhost";
        private string dbName = "UnityGameUsers";	//数据库名
        private string adminName = "root";		//登录数据库的用户名
        private string adminPwd = "123456";		//登录数据库的密码
        private string port = "3306";			//MySQL服务的端口号

        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;

        public GameObject mapObject;
        public GameObject playerPrefab;
        public GameObject homePanel;
        public GameObject hud;

        private void Start()
        {
            playerPrefab.GetComponent<PlayerManager>().Position = 0;
            Connector = new DatabaseConnector(serverName, dbName, adminName, adminPwd);
            User = new User(Connector);
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
                var status = User.Register(username, pwd).Status;
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
                var authorization = User.Login(username, pwd);
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
            hud.SetActive(true);
            
            if (player_data == null) return;
            playerObj = Instantiate(playerPrefab);
            var player = playerObj.GetComponent<PlayerManager>().Init(player_data);
            playerObj.transform.position = player.GetPlayerPosByIndex(player.Position);
            mapObject.GetComponent<GameMap>().players.Add(player);
            UIManager.playerObj = playerObj;
            
            // Debug.Log($"Position(Start): {player.Position}");
            // gameManager.GetComponent<SceneManager>().SwitchCamera(true, false);
        }

        public void UpdateUserData(PlayerData playerData)
        {
            User.SaveData(playerData);
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