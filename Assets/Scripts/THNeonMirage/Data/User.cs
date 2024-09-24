using System;
using UnityEngine;

namespace THNeonMirage.Data
{
    public class User
    {
        private DatabaseConnector connector;
        public string Name { get; private set; }
        public int Pos { get; set; }

        public User(DatabaseConnector connector)
        {
            this.connector = connector;
        }

        public Authorization Register(string username, string password)
        {
            Name = username;
            Pos = 0;
            var date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var queryUserCount = $"SELECT COUNT(*) FROM userinfo WHERE username = '{username}'";
            var queryInsertNewUser = $"INSERT INTO userinfo (username, password, createtime, position(0)) VALUES " +
                                     $"('{username}', '{password}', '{date}', '{Pos}')";

            if (!connector.Connect()) return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError); // 连接错误
            try
            {
                // 执行查询用户名数量的语句，从查询结果中获取数量，如果已存在同名用户则关闭链接
                var dataTable = connector.SelectQuery(queryUserCount);
                var count = int.Parse(dataTable.Rows[0][0].ToString());
                if (count == 0)
                {
                    connector.ExecuteNonQuery(queryInsertNewUser);
                    connector.Disconnect();
                    return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.RegisterSuccess);
                }
                connector.Disconnect();
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.DuplicateUser);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                connector.Disconnect();
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.Failed);
            }
        }

        public Authorization Login(string username, string password)
        {
            // 执行查询指定用户名记录的语句，查询用户输入的用户名是否存在于数据库中
            Name = username;
            var queryUserExists = $"SELECT * FROM userinfo WHERE username = '{username}' LIMIT 1";
            if (!connector.Connect()) return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);
            
            var dataTable = connector.SelectQuery(queryUserExists);
            if (dataTable.Rows.Count == 1)
            {
                // var savedPos = dataTable.Rows[0]["position"];
                var storedPassword = dataTable.Rows[0]["password"].ToString();
                Pos = int.Parse(dataTable.Rows[0]["position"].ToString());
                if (storedPassword == password)
                {
                    connector.Disconnect();
                    return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.LoginSuccess)
                        .SetData(new PlayerData(Name, Pos));
                }
                connector.Disconnect();
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.PasswordError);
            }
            connector.Disconnect();
            return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.UserNonExist);
        }
    }
}

