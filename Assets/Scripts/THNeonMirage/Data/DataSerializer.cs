using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using THNeonMirage.Util;

namespace THNeonMirage.Data
{
    /// <summary>
    /// 游客登录将遵循如下流程
    /// <code>
    ///flowchart TB
    ///  游客下单-->
    ///  生成csv信息表格，包含微信身份信息-->
    ///  微信身份信息录入数据库-->
    ///  游客入场扫码-->
    ///  游客使用购买了门票的微信身份信息登录
    /// </code>
    /// </summary>
    public class DataSerializer
    {
        public readonly List<string> RandomInvitationCodes = new();
        public List<List<string>> CsvData = new();
        private string csv_path;

        public DataSerializer(string csvPath)
        {
            csv_path = csvPath;
            Utils.ForAddToList(200, RandomInvitationCodes, _ => GenerateNextInvitationCode());
            AddInvitationCodeToCsv();
        }

        private string GenerateNextInvitationCode()
        {
            var random = new Random(DateTime.Now.Millisecond);
            return random.Next(1000,9999).ToString();
        }
        
        private void AddInvitationCodeToCsv()
        {
            var lineString = File.ReadAllLines(csv_path);
            CsvData = lineString.Select(line => line.Split(',').ToList()).ToList();
            foreach (var line in from line in CsvData from each in line select line)
            {
                line.Add(RandomInvitationCodes[CsvData.IndexOf(line)]);
            }
        }
        
        public User CreateUserFromCsv(DatabaseConnector connector)
        {
            return new User(connector);
        }

        public bool CheckInvitationCode()
        {
            var lines = File.ReadAllLines(csv_path);
            var csvData = lines.Select(line => line.Split(',')).ToList();
            return false;
        }

        public bool CheckQrCode(string qrCode)
        {
            return false;
        }
    }
}