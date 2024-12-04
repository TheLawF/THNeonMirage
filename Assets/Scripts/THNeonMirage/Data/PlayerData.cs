using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using THNeonMirage.Util;
using UnityEngine;
using Utils = UnityEngine.Diagnostics.Utils;

namespace THNeonMirage.Data
{
    // int Balance, List<int> Items, List<Pair<int,int>> OccupiedFields
    [Serializable]
    public class PlayerData
    {
        public string UserName { get; set; }
        public int Position { get; set; }
        public int Balance { get; set; }
        public List<int> Inventory { get; private set; }
        public List<Pair<int, int>> Fields { get; private set; }

        public PlayerData(string userName, int position) : this()
        {
            UserName = userName;
            Position = position;
        }
        
        public PlayerData()
        {
            Inventory = new List<int>();
            Fields = new List<Pair<int, int>>();
        }
        
        public PlayerData Name(string name)
        {
            UserName = name;
            return this;
        }

        public PlayerData Pos(int pos)
        {
            Position = pos;
            return this;
        }

        public PlayerData SetBalance(int balance)
        {
            Balance = balance;
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

        private PlayerData SetInv(List<int> inv)
        {
            Inventory = inv;
            return this;
        }
        
        private PlayerData SetFields(List<Pair<int, int>> fields)
        {
            Fields = fields;
            return this;
        }

        public PlayerData AddField(int fieldId, int bidId)
        {
            Fields.Add(new Pair<int, int>(fieldId, bidId));
            return this;
        }

        public List<int> CopyInv() => new (Inventory);
        public List<Pair<int, int>> CopyFields() => new (Fields);

        public override string ToString()
            => $@"Player: {{ UserName: {UserName}, Pos: {Position}, Balance: {Balance},
                     Inventory: {Util.Utils.ListToString(Inventory)}, 
                     Fields: {Util.Utils.ListToString(Fields)}}}";

        public PlayerData Copy() => new PlayerData().Name(UserName).Pos(Position).SetBalance(Balance)
            .SetInv(Inventory).SetFields(Fields);
    }
}