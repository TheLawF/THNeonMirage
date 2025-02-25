using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using THNeonMirage.Event;
using THNeonMirage.Util;
using UnityEngine;
using UnityEngine.Rendering;
using Utils = UnityEngine.Diagnostics.Utils;

namespace THNeonMirage.Data
{
    // int Balance, List<int> Items, List<Pair<int,int>> OccupiedFields
    [Serializable]
    public class PlayerData
    {
        private int _position;
        private int _balance;
        
        public EventHandler<ValueEventArgs> OnPositionChanged;
        public event ValueChangedHandler PositionChanged;
        public event ValueChangedHandler BalanceChanged;
        public string UserName { get; set; }
        
        public int Position
        {
            get => _position;
            set
            {
                if (Equals(_position, value)) return;
                var oldValue = _position;
                _position = value;
                PositionChanged?.Invoke(oldValue, _position);
                OnPositionChanged?.Invoke(this, new ValueEventArgs(_position));
            }
        }

        public int Balance
        {
            get => _balance;
            set
            {
                if (Equals(_balance, value)) return;
                var oldValue = _balance;
                _balance = value;
                BalanceChanged?.Invoke(oldValue, _balance);
            }
        }
        public ObservableList<int> Inventory { get; private set; }
        public ObservableList<Pair<int, int>> Fields { get; private set; }

        public PlayerData(string userName, int position) : this()
        {
            UserName = userName;
            Position = position;
        }
        
        public PlayerData()
        {
            Inventory = new ObservableList<int>();
            Fields = new ObservableList<Pair<int, int>>();
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

        private PlayerData SetInv(ObservableList<int> inv)
        {
            Inventory = inv;
            return this;
        }
        
        private PlayerData SetFields(ObservableList<Pair<int, int>> fields)
        {
            Fields = fields;
            return this;
        }

        public PlayerData AddField(int fieldId, int bidId)
        {
            Fields.Add(new Pair<int, int>(fieldId, bidId));
            return this;
        }

        public ObservableList<int> CopyInv() => new (Inventory);
        public ObservableList<Pair<int, int>> CopyFields() => new (Fields);

        // public override string ToString()
        //     => $@"Player: {{ UserName: {UserName}, Pos: {Position}, Balance: {Balance},
        //              Inventory: {Util.Utils.ListToString(Inventory)}, 
        //              Fields: {Util.Utils.ListToString(Fields)}}}";

        public PlayerData Copy() => new PlayerData().Name(UserName).Pos(Position).SetBalance(Balance)
            .SetInv(Inventory).SetFields(Fields);
    }
}