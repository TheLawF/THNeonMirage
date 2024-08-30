using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace THNeonMirage.Data
{
    public class DatabaseConnector
    {
        private string connectionString; // 存储 MySQL 连接字符串
        private MySqlConnection connection; // 存储 MySQL 连接实例

        
        public DatabaseConnector(string server, string database, string uid, string password)
        {
            connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";
        }
        
        public bool Connect()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }
        
        public bool Disconnect()
        {
            if (connection == null)
            {
                return false;
            }
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Debug.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 向指定的 MySQL 数据库发送 SQL 语句并返回结果
        /// </summary>
        /// <param name="query">接受一个字符串作为参数表示查询</param>
        /// <returns></returns>
        public DataTable SelectQuery(string query)
        {
            var list = new List<string>();
            var dataTable = new DataTable();
            var command = new MySqlCommand(query, connection);
            var reader = command.ExecuteReader();
            dataTable.Load(reader);
            reader.Close();
            return dataTable;
        }

        /// <summary>
        /// 执行非查询语句（如 Insert, Update, Delete）
        /// </summary>
        /// <param name="query">非查询语句</param>
        public void ExecuteNonQuery(string query)
        {
            var command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 使用Linq查询代替SQL语句查询
        /// </summary>
        public TOut LinqSelect<TIn, TOut>(string query, Predicate<TIn> predicate, Func<Predicate<TIn>, TOut> predictFunc)
            where TOut : class where TIn : new()
        {
            var enumerable = SelectQuery(query).AsEnumerable().Select(_ => new TIn());
            if (enumerable.Any(predicate.Invoke)) return predictFunc.Target as TOut;
            return null;
        }
    }
}