using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager.UI;
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
        public int PlayerIndex;
        public string Id;
        public string Password;
        
        public TMP_Text BalanceText { private get; set; }

        public DiceHandler dice;
        public GameObject Instance;
        public GameMap gameMap;
        
        [DisplayOnly] public PlayerData PlayerData;
        [DisplayOnly] public GameLauncher database;

        private PhotonView photonView;
        private Vector3 prev_pos;
        private Vector3 next_pos;
        private Vector3 networkPosition;
        private Quaternion networkRotation;

        private void Awake()
        {
            PlayerData = new PlayerData().SetBalance(60000);
            PlayerData.OnBalanceChanged += GameOver;
            photonView = Instance.GetComponent<PhotonView>();
            photonView.ObservedComponents.Add(this);
            OnInstantiate += Initialize;
        }

        public void SetPosition(object sender, ValueEventArgs currentPos)
        {
            SetPosition((int)currentPos.Value);
        }

        private void OnRoundEnd(MonoBehaviour script, ValueEventArgs args)
        {
            PlayerData.PauseCount--;
        }
        
        public void GameOver(object playerData, ValueEventArgs balanceArg)
        {
            if (PlayerData.Balance <= 0) PhotonNetwork.Destroy(Instance);
        }

        public bool CanMove() => PlayerData.PauseCount <= 0;
        public bool IsMyTurn() => PlayerIndex == gameMap.ActorOrder;
        
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
                stream.SendNext(PlayerData.Balance);
            } else {
                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
                PlayerData.Balance = (int)stream.ReceiveNext();
            }
        }
    }
}

