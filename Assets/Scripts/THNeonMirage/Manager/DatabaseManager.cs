using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using THNeonMirage.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace THNeonMirage.Manager
{
    public class DatabaseManager : MonoBehaviour
    {
        private MySqlConnection connection;
        private DatabaseConnector Connector;
        private UserData User;

        private string serverName = "localhost";
        private string dbName = "UnityGameUsers";	//数据库名
        private string adminName = "root";		//登录数据库的用户名
        private string adminPwd = "123456";		//登录数据库的密码
        private string port = "3306";			//MySQL服务的端口号

        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;

        public GameObject playerPrefab;
        public GameObject canvas;

        private void Start()
        {
            Connector = new DatabaseConnector(serverName, dbName, adminName, adminPwd);
            User = new UserData(Connector);
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
                var status = User.Register(username, pwd);
                switch (status)
                {
                    case Authorization.Status.RegisterSuccess:
                        Debug.Log("注册成功");
                        break;
                    case Authorization.Status.DuplicateUser:
                        Debug.LogWarning("用户名已存在，请选择不同的用户名！");
                        break;
                    case Authorization.Status.ConnectionError:
                        Debug.LogError("连接错误");
                        break;
                    default:
                    case Authorization.Status.Failed:
                    case Authorization.Status.LoginSuccess:
                    case Authorization.Status.UserNonExist:
                    case Authorization.Status.PasswordError:
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
                var status = User.Login(username, pwd);
                switch (status)
                {
                    case Authorization.Status.LoginSuccess:
                        Debug.Log("登录成功");
                        break;
                    case Authorization.Status.UserNonExist:
                        Debug.LogWarning("用户名已存在，请选择不同的用户名！");
                        break;
                    case Authorization.Status.PasswordError:
                        Debug.LogWarning("用户名或者密码错误");
                        break;
                    case Authorization.Status.ConnectionError:
                        Debug.LogWarning("连接错误");
                        break;
                    default:
                    case Authorization.Status.Failed:
                    case Authorization.Status.RegisterSuccess:
                    case Authorization.Status.DuplicateUser:
                        Debug.LogWarning("登录失败");
                        break;
                }
            }
            usernameInput.text = "";
            passwordInput.text = "";
            
            canvas.SetActive(false);
            var mapObj = GameObject.Find("MapObject");
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