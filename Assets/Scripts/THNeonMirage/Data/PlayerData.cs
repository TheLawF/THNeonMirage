using System;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using UnityEngine.Rendering;

namespace THNeonMirage.Data
{
    // int Balance, List<int> Items, List<Pair<int,int>> OccupiedFields
    [Serializable]
    public class PlayerData
    {
        private int _position;
        private int _balance;
        public int RoundIndex;
        public string UserName { get; set; }
        public string PlayerUid { get; set; }
        public string Password { get; set; }
        
        public event ValueChangedHandler OnPassBy;

        public int PauseCount;
        public int Position
        {
            get => _position;
            set
            {
                if (Equals(_position, value)) return;
                var prevPos = _position;
                _position = value;
            }
        }

        public int Balance
        {
            get => _balance;
            set
            {
                var oldValue = _balance;
                _balance = value;
            }
        }
        public ObservableList<int> Inventory { get; private set; }
        public ObservableList<int> Fields { get; private set; }

        public PlayerData(string userName, int position) : this()
        {
            UserName = userName;
            Position = position;
        }
        
        public PlayerData()
        {
            Inventory = new ObservableList<int>();
            Fields = new ObservableList<int>();
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

        public PlayerData Uid(string uid)
        {
            PlayerUid = uid;
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

        public PlayerData Copy() => new PlayerData().Name(UserName).Pos(Position).SetBalance(Balance)
            .SetInv(Inventory).SetFields(Fields);
    }
}