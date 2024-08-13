using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace System.Runtime.CompilerServices
{
    class IsExternalInit
    {
    }
}

namespace THNeonMirage.Data
{
    [Serializable]
    public class Player : NetworkBehaviour
    {
        public string Name;
        public string EmailAddr;
        public string Id;
        public string Password;
        public bool IsAdministrator;

        public int Position;
        public DiceType DiceType;
        public Attribute Attribute;
        public Inventory Inventory;

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

