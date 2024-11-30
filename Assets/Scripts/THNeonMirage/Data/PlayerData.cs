using System.Collections.Generic;
using JetBrains.Annotations;
using THNeonMirage.Util;
using Utils = UnityEngine.Diagnostics.Utils;

namespace THNeonMirage.Data
{
    // int Balance, List<int> Items, List<Pair<int,int>> OccupiedFields
    public class PlayerData
    {
        public string UserName { get; set; }
        public int Position { get; set; }
        public int Balance { get; set; }
        public List<int> Inventory { get; }
        public List<Pair<int, int>> Fields { get; }

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

        public PlayerData AddField(int fieldId, int bidId)
        {
            Fields.Add(new Pair<int, int>(fieldId, bidId));
            return this;
        }

        public List<int> CopyInv() => new (Inventory);
        public List<Pair<int, int>> CopyFields() => new (Fields);

        public override string ToString()
            => $@"Player: {{ UserName: {UserName}, Pos: {Position}, Balance: {Balance},
                     Inventory: {Util.Utils.PrintList(Inventory)}, 
                     Fields: {Util.Utils.PrintList(Fields)}}}";
        
    }
}