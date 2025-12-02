using System;
using System.IO;
using System.Xml;
using ExitGames.Client.Photon;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using InvalidDataException = System.IO.InvalidDataException;

namespace THNeonMirage.Data
{
    // int Balance, List<int> Items, List<Pair<int,int>> OccupiedFields
    [Serializable]
    public class PlayerData
    {
        public int roundIndex;
        public string userName;
        public bool isBot;

        public int pauseCount;
        public int position;
        public int balance;

        public static int SerializeVersion = 1;
        
        public UniqueId UniqueId;
        public ObservableList<int> Inventory { get; private set; }
        public ObservableList<int> Fields { get; private set; }

        public static byte[] Serialize(object obj)
        {
            var data = (PlayerData)obj;
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            
            writer.Write((byte)SerializeVersion);
            writer.Write(data.isBot);
            
            writer.Write(data.roundIndex);
            writer.Write(data.pauseCount);
            writer.Write(data.position);
            writer.Write(data.balance);

            return stream.ToArray();
        }

        public static object Deserialize(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);
            if (reader.ReadByte() != SerializeVersion)
            {
                throw new InvalidDataException($"不支持的序列化版本，仅支持：{SerializeVersion}，但是发现了：{(int)reader.ReadByte()}");
            }

            var isbot = reader.ReadBoolean();
            var round = reader.ReadInt32();
            var pause = reader.ReadInt32();
            var posIndex = reader.ReadInt32();
            var balanceCount = reader.ReadInt32();

            return new PlayerData
            {
                isBot = isbot,
                roundIndex = round,
                pauseCount = pause,
                balance = balanceCount,
                position = posIndex,
            };
        }

        public PlayerData(string userName, int position) : this()
        {
            this.userName = userName;
            this.position = position;
            UniqueId = new UniqueId();
        }
        
        public PlayerData()
        {
            userName = "";
            Inventory = new ObservableList<int>();
            Fields = new ObservableList<int>();
            UniqueId = new UniqueId();
        }
        
        public PlayerData Name(string name)
        {
            userName = name;
            return this;
        }

        public PlayerData Pos(int pos)
        {
            position = pos;
            return this;
        }

        public PlayerData Uid(UniqueId uid)
        {
            UniqueId = uid;
            return this;
        }

        public PlayerData SetBalance(int balance)
        {
            this.balance = balance;
            return this;
        }

        public PlayerData SetRoundIndex(int index)
        {
            roundIndex = index;
            return this;
        }
        
        public PlayerData SetPauseCount(int pause)
        {
            pauseCount = pause;
            return this;
        }

        public PlayerData AddBalance(int additionalBalanceCount)
        {
            return SetBalance(balance + additionalBalanceCount);
        }

        public PlayerData AddInv(int itemId)
        {
            Inventory.Add(itemId);
            return this;
        }
        
        public PlayerData RemoveInv(int itemId)
        {
            Inventory.Remove(itemId);
            return this;
        }

        private PlayerData SetInv(ObservableList<int> inv)
        {
            Inventory = inv;
            return this;
        }
        
        private PlayerData SetFields(ObservableList<int> fields)
        {
            Fields = fields;
            return this;
        }

        public PlayerData AddField(int fieldId)
        {
            Fields.Add(fieldId);
            return this;
        }

        public ObservableList<int> CopyInv() => new (Inventory);
        public ObservableList<int> CopyFields() => new (Fields);

        // public override string ToString()
        //     => $@"Player: {{ UserName: {UserName}, Pos: {Position}, Balance: {Balance},
        //              Inventory: {Util.Utils.ListToString(Inventory)}, 
        //              Fields: {Util.Utils.ListToString(Fields)}}}";

        public PlayerData Copy() => new PlayerData().Name(userName).Pos(position).SetBalance(balance)
            .SetInv(Inventory).SetFields(Fields);
    }
}