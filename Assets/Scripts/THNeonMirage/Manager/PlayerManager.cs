using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    }
}

namespace THNeonMirage.Manager
{
    [Serializable]
    public class PlayerManager : NetworkBehaviour
    {
        // public int Position;
        // public string UserName;
        // public int Balance;

        public PlayerData PlayerData;

        public string Id;
        public string Password;

        public DiceType DiceType;
        public Attribute Attribute;

        private bool IsAdministrator;
        public GameObject inGamePanel;
        private ToggleHandler toggle_handler;
        [DisplayOnly]
        public DatabaseManager database;
        public TMP_Text BalanceText { private get; set; }

        private void Start()
        {
            toggle_handler = inGamePanel.GetComponent<ToggleHandler>();
            PlayerData = new PlayerData();
        }

        private void Update()
        {
            // transform.position = GetPlayerPosByIndex(PlayerData.Position);
            BalanceText.SetText($"月虹币余额：{PlayerData.Balance}");
            // toggle_handler.SetPrice(
            //     GameMap.Fields[PlayerData.Position].FirstBid,
            //     GameMap.Fields[PlayerData.Position].SecondBid,
            //     GameMap.Fields[PlayerData.Position].ThirdBid);

            if (IsClient && IsOwner)
            {
                
            }
        }

        public Authorization SaveAll(PlayerData playerData) => database.SaveAll(playerData);
        public void Save(string columnName, object data) => database.Save(PlayerData.UserName, columnName, data);

        public PlayerManager Init(PlayerData playerData)
        {
            PlayerData = playerData;
            SetPosition(playerData.Position);
            return this;
        }

        public PlayerManager SetPosition(GameObject player, int position)
        {
            if (PlayerData.Position + position is < 0 and >= -40)
            {
                PlayerData.Position = -position;
            }
            PlayerData.Position = position switch
            {
                <= -40 => -position % 40,
                >= 40 => position % 40,
                _ => position
            };
            player.transform.position = GetPlayerPosByIndex(PlayerData.Position);
            return this;
        }
        
        public PlayerManager SetPosition(int position)
        {
            if (PlayerData.Position + position is < 0 and >= -40)
            {
                PlayerData.Position = -position;
            }
            PlayerData.Position = position switch
            {
                <= -40 => -position % 40,
                >= 40 => position % 40,
                _ => position
            };
            transform.position = GetPlayerPosByIndex(PlayerData.Position);
            return this;
        }

        public void Move(int steps)
        {
            PlayerData.Position += steps;
            transform.position =
                GetPlayerPosByIndex(PlayerData.Position >= 40 ? PlayerData.Position % 40 : PlayerData.Position);
        }
        public Vector3 GetPlayerPosByIndex(int index) => GameMap.PosInRange.First(pair => 
            Utils.IsInRange(pair.Key, index)).Value.Invoke(index);
    }

    [Serializable]
    public enum DiceType
    {
        Default,
        Cirno,
        Flandre,
        Koishi
    }

    [Serializable]
    public record Attribute(int Health, int AttackDamage);

    // [Serializable]
    // public class Inventory
    // {
    //     public static readonly int MAX_COUNT = 10;
    //     public readonly List<ItemStack> Slots = new(10);
// 
    //     public void AddItemToInventory(int index, ItemStack item)
    //     {
    //         if (Slots.Count < index) Slots[index] = item;
    //         else Slots.Add(item);
    //     }
// 
    //     public void TransferItem(int prevSlot, int targetSlot)
    //     {
    //         if (Slots[targetSlot] != null) return;
    //         Slots[targetSlot] = Slots[prevSlot];
    //         Slots[prevSlot] = null;
    //     }
    // }

    public class Inventory
    {
        public List<int> Inv;

        public Inventory(List<int> inventory)
        {
            this.Inv = inventory;
        }
    }
    
    public class OccupiedFields
    {
        
            
    }

    [Serializable]
    public record ItemStack(string Name, int Count);
}

