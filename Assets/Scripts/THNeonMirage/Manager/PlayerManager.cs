using System;
using System.Collections.Generic;
using System.Linq;
using THNeonMirage.Data;
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

namespace THNeonMirage.Manager
{
    [Serializable]
    public class PlayerManager : NetworkBehaviour
    {
        public int Position;
        public string UserName;
        public int Balance;

        public string Id;
        public string Password;

        public DiceType DiceType;
        public Attribute Attribute;
        public Inventory Inventory;
        
        private bool IsAdministrator;

        private void Start()
        {
        }

        private void Update()
        {
            transform.position = GetPlayerPosByIndex(Position);
            if (IsClient && IsOwner)
            {
                
            }
            else
            {
                
            }
        }

        public PlayerManager Init(PlayerData playerData)
        {
            UserName = playerData.UserName;
            SetPosition(playerData.Position);
            return this;
        }

        public PlayerManager SetName(string userName)
        {
            UserName = userName;
            return this;
        }
        
        public PlayerManager SetPosition(int position)
        {
            if (Position + position is < 0 and >= -40)
            {
                Position = -position;
            }
            Position = position switch
            {
                <= -40 => -position % 40,
                >= 40 => position % 40,
                _ => position
            };
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

