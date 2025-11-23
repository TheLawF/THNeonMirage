using System;
using System.Xml;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

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
        
        public UniqueId UniqueId;
        public ObservableList<int> Inventory { get; private set; }
        public ObservableList<int> Fields { get; private set; }

        public PlayerData(string userName, int position) : this()
        {
            this.userName = userName;
            this.position = position;
            UniqueId = new UniqueId();
        }
        
        public PlayerData()
        {
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