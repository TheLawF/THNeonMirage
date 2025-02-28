using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using THNeonMirage.Util;

namespace THNeonMirage.Data
{
    public class DataSerializer
    {
        public readonly List<string> RandomInvitationCodes = new();

        public DataSerializer()
        {
            Utils.ForAddToList(200, RandomInvitationCodes, _ => GenerateNextInvitationCode());
        }

        public string GenerateNextInvitationCode()
        {
            var random = new Random(165494150);
            return random.Next(1000,9999).ToString();
        }
        public User CreateUserFromCsv(string csvPath, DatabaseConnector connector)
        {
            return new User(connector);
        }

        public bool CheckInvitationCode(string csvPath)
        {
            var lines = File.ReadAllLines(csvPath);
            var csvData = lines.Select(line => line.Split(',')).ToList();
            return false;
        }
    }
}