using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    }
}

namespace THNeonMirage.Manager
{
    [Serializable]
    public class PlayerManager : MonoBehaviourPun, IPunObservable
    {
        public string Id;
        public int Activity;
        public string Password;
        
        public DiceType DiceType;
        public TMP_Text BalanceText { private get; set; }
        
        public GameObject playerPrefab;
        public GameObject Instance;
        public UnityEvent<int> onPlayerStop;
        
        [DisplayOnly] public PlayerData PlayerData;
        [DisplayOnly] public GameLauncher database;

        private bool IsAdministrator;
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        
        private void Start()
        {
            PlayerData = new PlayerData().SetBalance(60000);
            // GameMap.OnRoundEnd += OnRoundEnd;
        }

        private void Update()
        {

        }
        
        public void SetPosition(object sender, ValueEventArgs currentPos)
        {
            SetPosition((int)currentPos.Value);
        }

        public void OnRoundStart()
        {
            
        }
        
        private void OnRoundEnd(MonoBehaviour script, ValueEventArgs args)
        {
            
        }
        
        public Authorization SaveAll(PlayerData playerData) => database.SaveAll(playerData);
        public void Save(string columnName, object data) => database.Save(PlayerData.UserName, columnName, data);

        public PlayerManager Init(PlayerData playerData)
        {
            PlayerData = playerData;
            SetPosition(playerData.Position);
            return this;
        }

        private PlayerManager SetPosition(int position)
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
        public static Vector3 GetPlayerPosByIndex(int index) => GameMap.PosInRange.First(pair => 
            Utils.IsInRange(pair.Key, index)).Value.Invoke(index);

        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
            } else {
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
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

