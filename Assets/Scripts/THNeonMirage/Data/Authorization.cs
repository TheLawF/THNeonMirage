using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace THNeonMirage.Data
{
    public class Authorization
    {
        public Role UserRole { get; set; }
        public ConnectionStatus Status { get; }
        public PlayerData PlayerData { get; private set; }

        public Authorization(Role userRole, ConnectionStatus status)
        {
            UserRole = userRole;
            Status = status;
        }

        public Authorization SetData([NotNull] PlayerData playerData)
        {
            PlayerData = playerData;
            return this;
        }

        public enum Role
        {
            Forbidden,
            User,
            Administrator,
            Proprietor
        }

        public static void ShowConnectionMsg(ConnectionStatus status)
        {
            switch (status)
            {
                case ConnectionStatus.LoginSuccess:
                    Debug.Log("登录成功");
                    break;
                case ConnectionStatus.UserNonExist:
                    Debug.LogWarning("用户名已存在，请选择不同的用户名！");
                    break;
                case ConnectionStatus.PasswordError:
                    Debug.LogWarning("用户名或者密码错误");
                    break;
                case ConnectionStatus.ConnectionError:
                    Debug.LogWarning("连接错误");
                    break;
                case ConnectionStatus.SaveSuccess:
                    break;
                case ConnectionStatus.RegisterSuccess:
                    Debug.Log("登录成功");
                    break;
                case ConnectionStatus.DuplicateUser:
                    Debug.LogWarning("重复的用户名");
                    break;
                default:
                case ConnectionStatus.Failed:
                    Debug.LogWarning("登录失败");
                    break;
            }
        }
        
        public enum ConnectionStatus
        {
            RegisterSuccess,
            LoginSuccess,
            SaveSuccess,
            DuplicateUser,
            UserNonExist,
            PasswordError,
            ConnectionError,
            Failed
        }
    }
}