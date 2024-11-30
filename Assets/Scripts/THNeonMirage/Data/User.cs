using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using THNeonMirage.Manager;
using THNeonMirage.Util;
using Unity.Collections.LowLevel.Unsafe;
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
        public const string fields = "{\"occupied_fields\": {[\"field_id\": 1, \"bid\": 1]}}";

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
                $"INSERT INTO userinfo (username, password, createtime, position, {nameof(inventory)}) " +
                $"VALUES ('{username}', '{password}', '{date}', '{Pos}', '{inventory}')";

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
            var queryUser = $"SELECT * FROM userinfo WHERE username = '{username}' LIMIT 1";
            if (!connector.Connect())
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);

            var dataTable = connector.SelectQuery(queryUser);
            var npd = FromJsonQuery(queryUser);
            if (dataTable.Rows.Count == 1)
            {
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

        // public Authorization Update(PlayerData playerData)
        // {
        //     var savePosQuery =
        //         $"UPDATE userinfo SET position = {playerData.Position} WHERE username = '{playerData.UserName}'";
        //     if (!connector.Connect())
        //         return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);
        //     connector.ExecuteNonQuery(savePosQuery);
        //     return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.SaveSuccess);
        // }

        public Authorization Save(string name, string columnName, object data)
        {
            var savePosQuery =
                $"UPDATE userinfo SET '{columnName}' = {data} WHERE username = '{name}'";
            if (!connector.Connect())
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);
            connector.ExecuteNonQuery(savePosQuery);
            return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.SaveSuccess);
        }
        
        public Authorization SaveAll(PlayerData playerData)
        {
            var savePosQuery = $@"UPDATE userinfo 
                SET position = {playerData.Position},
                    balance = {playerData.Balance},
                    fields = {FieldsToJson(playerData.Fields)},
                    inventory = {InvToJson(playerData.Inventory)} 
                WHERE username = {playerData.UserName}";
            if (!connector.Connect())
                return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.ConnectionError);
            connector.ExecuteNonQuery(savePosQuery);
            return new Authorization(Authorization.Role.User, Authorization.ConnectionStatus.SaveSuccess);
        }

        public NeoPlayerData FromJsonQuery(string query)
        {
            var pairList = new List<Pair<int, int>>();

            var name = connector.GetAsString(query, "username");
            int.TryParse(connector.GetAsString(query, "position"), out var position);
            int.TryParse(connector.GetAsString(query, "balance"), out var balance);

            var jsonInv = connector.GetAsString(query, nameof(inventory));
            var jsonFields = connector.GetAsString(query, nameof(fields));

            var inv = Utils.CastJsonAsList<int>(jsonInv, "inv");
            var fieldsToken = Utils.CastJsonAsList<JObject>(jsonFields, "fields");

            fieldsToken?.ForEach(o =>
            {
                var id = Utils.CastJsonAsInt(o, "id");
                var bid = Utils.CastJsonAsInt(o, "bid");
                var pair = Pair<int, int>.Of(id, bid);

                pairList.Add(pair);
            });

            // Debug.Log($"Player Data: {{name: {name}, pos: {position} balance: {balance}," +
            //           $" inventory: {ListString(inv)}, fields: {ListString(pairList)}}}");
            return new NeoPlayerData(name, position, balance, inv, pairList);
        }

        public string InvToJson(List<int> inv)
        {
            return JsonConvert.SerializeObject(inv);
        }
        
        public string FieldsToJson(List<Pair<int, int>> inv)
        {
            return JsonConvert.SerializeObject(inv);
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

