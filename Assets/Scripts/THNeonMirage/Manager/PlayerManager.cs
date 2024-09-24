using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using THNeonMirage.Map;
using THNeonMirage.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace System.Runtime.CompilerServices
{
    class IsExternalInit
    {
    }
}

namespace THNeonMirage.Data
{
    [Serializable]
    public class PlayerManager : NetworkBehaviour
    {
        public string UserName;
        public string Id;
        public string Password;

        public int Position;
        public DiceType DiceType;
        public Attribute Attribute;
        public Inventory Inventory;
        
        private bool IsAdministrator;

        private void Start()
        {
        }

        private void Update()
        {
            if (IsClient && IsOwner)
            {
                
            }
            else
            {
                
            }
        }

        public PlayerManager SetName(string userName)
        {
            UserName = userName;
            return this;
        }
        
        public PlayerManager SetPosition(int position)
        {
            Position = position;
            transform.position = GetPlayerPosByIndex(Position);
            return this;
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

    [Serializable]
    public class Inventory
    {
        public static readonly int MAX_COUNT = 10;
        public readonly List<ItemStack> Slots = new(10);

        public void AddItemToInventory(int index, ItemStack item)
        {
            if (Slots.Count < index) Slots[index] = item;
            else Slots.Add(item);
        }

        public void TransferItem(int prevSlot, int targetSlot)
        {
            if (Slots[targetSlot] != null) return;
            Slots[targetSlot] = Slots[prevSlot];
            Slots[prevSlot] = null;
        }
    }

    [Serializable]
    public record ItemStack(string Name, int Count);
}

