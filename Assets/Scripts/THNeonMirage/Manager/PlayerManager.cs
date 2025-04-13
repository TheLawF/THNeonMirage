using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    }
}

namespace THNeonMirage.Manager
{
    [Serializable]
    public class PlayerManager : GameBehaviourPun, IPunObservable
    {
        public int Round;
        public string Id;
        public string Password;
        
        public DiceType DiceType;
        public TMP_Text BalanceText { private get; set; }
        
        public GameObject Instance;
        public GameMap gameMap;
        
        [DisplayOnly] public PlayerData PlayerData;
        [DisplayOnly] public GameLauncher database;

        private Vector3 prev_pos;
        private Vector3 next_pos;
        private Vector3 networkPosition;
        private Quaternion networkRotation;

        private void Awake()
        {
            PlayerData = new PlayerData().SetBalance(60000);
            PlayerData.OnBalanceChanged += GameOver;
            OnInstantiate += Initialize;
        }

        public void SetPosition(object sender, ValueEventArgs currentPos)
        {
            SetPosition((int)currentPos.Value);
        }

        private void OnRoundEnd(MonoBehaviour script, ValueEventArgs args)
        {
            
        }
        
        public void GameOver(object playerData, ValueEventArgs balanceArg)
        {
            if (PlayerData.Balance <= 0) PhotonNetwork.Destroy(Instance);
        }

        public bool CanMove() => PlayerData.PauseCount <= 0;
        public bool IsMyTurn() => Round == gameMap.TurnIndex;

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
            prev_pos = GetPlayerPosByIndex(PlayerData.Position);
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
            next_pos = GetPlayerPosByIndex(PlayerData.Position);
            StartCoroutine(Move(prev_pos, next_pos));
            return this;
        }

        public IEnumerator Move(Vector3 prevPos, Vector3 nextPos)
        {
            for (var i = 0f; i < 1; i += 0.02f)
            {
                transform.position = Vector3.Lerp(prevPos, nextPos, i);
                yield return new WaitForSeconds(0.02f);
            }
            
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

