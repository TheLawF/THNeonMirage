using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Fictology.Data.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using THNeonMirage.Util;
using UnityEngine;

namespace THNeonMirage.Data
{
    // public record PlayerData(string UserName, int Position, int Coin);
    public class User
    {
        private DatabaseConnector connector;
        public string Name { get; private set; }
        public int Pos { get; set; }
        public const string inventory = "{\"inventory\":[]}";
        public const string fields = "{\"occupied\":[{}]}";

        public User(DatabaseConnector connector)
        {
            this.connector = connector;
        }

        public Authorization Register(string username, string password)
        {
            Pos = 0;
            Name = username;

            var date = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var queryUserCount = $"SELECT COUNT(*) FROM userinfo WHERE username = '{username}'";
            var newUserQuery =
                $"INSERT INTO userinfo (username, password, createtime, position, {nameof(inventory)}, {nameof(fields)}) " +
                $"VALUES ('{username}', '{password}', '{date}', '{Pos}', '{inventory}', '{fields}')";

            // var newUserQuery = 
            //     $"INSERT INTO userinfo (username, password, createtime, position(0), inventory(\"{{}}\"), fields(\"{{}}\")))" +
            //     $"VALUES ('{Name}', '{password}', '{date}', '{Pos}', '{PlayerInvData}', '{PlayerFieldData}')";

            if (!connector.Connect())
                return new Authorization(Authorization.Role.User,
                    Authorization.ConnectionStatus.ConnectionError); // 连接错误
            try
            {
                // 执行查询用户名数量的语句，从查询结果中获取数量，如果已存在同名用户则关闭链接
                // var dataTable = connector.SelectQuery(queryUserCount);
                var dataTable = connector.QueryJson(queryUserCount, nameof(inventory), inventory);
                var count = int.Parse(dataTable.Rows[0][0].ToString());
                if (count == 0)
                {
                    connector.ExecuteNonQuery(newUserQuery);
                    connector.Disconnect();
                    return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.RegisterSuccess);
                }
                if (count > 0) {
                    connector.Disconnect();
                    return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.DuplicateUser);
                }

                connector.Disconnect();
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.Failed);
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
            var queryUser = $"SELECT * FROM userinfo WHERE username = '{username}' LIMIT 1";
            if (!connector.Connect())
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);

            var dataTable = connector.SelectQuery(queryUser);
            if (dataTable.Rows.Count == 1)
            {
                var storedPassword = dataTable.Rows[0]["password"].ToString();
                Pos = int.Parse(dataTable.Rows[0]["position"].ToString());
                // var inv = dataTable.Rows[0]["inventory"].ToString();
                // var fields = dataTable.Rows[0]["fields"].ToString();
                
                if (storedPassword == password)
                {
                    connector.Disconnect();
                    return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.LoginSuccess)
                        .SetData(new PlayerData().Name(Name).Pos(Pos));
                }

                connector.Disconnect();
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.PasswordError);
            }

            connector.Disconnect();
            return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.UserNonExist);
        }

        public Authorization Update(PlayerData playerData)
        {
            const string savePosQuery = "UPDATE userinfo SET position = @pos WHERE username = @name";
            if (!connector.Connect())
            {
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);
            }
            connector.ExecuteByParams(savePosQuery, new Dictionary<string, object>
            {
                {"@pos", playerData.position},
                {"@name", playerData.userName}
            });
            return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.SaveSuccess);
        }

        public Authorization Save(string name, string columnName, object data)
        {
            var savePosQuery =
                $"UPDATE userinfo SET {columnName} = {data} WHERE username = '{name}'";
            if (!connector.Connect())
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);
            connector.ExecuteNonQuery(savePosQuery);
            return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.SaveSuccess);
        }
        
        public Authorization SaveAll(PlayerData playerData)
        {
            var savePosQuery = $@"UPDATE userinfo 
                SET position = {playerData.position},
                    balance = {playerData.balance} 
                WHERE username = '{playerData.userName}'";
            if (!connector.Connect())
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);
            connector.ExecuteNonQuery(savePosQuery);
            return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.SaveSuccess);
        }

        private static string InvToJson(ICollection inv) => Utils.ListToJsonString("inv", inv);

        private static string FieldsToJson(List<Pair<IntData, IntData>> list)
        {
            var sb = new StringBuilder();
            sb.Append("{\"fields\":[");
            foreach (var each in list)
            {
                sb.Append(each.ToJsonString("id", "bid"));
                sb.Append(list.GetEnumerator().MoveNext() ? "," : "");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            return sb.ToString();
        }
        
        public static string ListString(ICollection list)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            // var enumerable = list as object[] ?? list.Cast<object>().ToArray();
            foreach (var each in list)
            {
                sb.Append(each);
                sb.Append(list.GetEnumerator().MoveNext() ? "," : "");
            }
            sb.Append("]");
            return sb.ToString();
        }

    }
}

