using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fictology.Event;
using Fictology.Registry;
using Fictology.UnityEditor;
using FlyRabbit.EventCenter;
using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    }
}

namespace THNeonMirage.Manager
{
    [Serializable]
    public class PlayerManager : RegistryEntry
    {
        public GameObject diceObj;
        public Level level;
        public GameMain game;
        public PlayerData playerData;

        [DisplayOnly] public GameLauncher database;

        private Vector3 prev_pos;
        private Vector3 next_pos;
        private Vector3 networkPosition;
        private Quaternion networkRotation;

        private void Awake()
        {
            playerData = new PlayerData().SetBalance(60000);
        }

        private void Start()
        {
            level = Registries.Get<Level>(LevelRegistry.Level);
            game = Registries.GetComponent<GameMain>(LevelRegistry.Level);
        }

        public IEnumerator ExecuteAITask()
        {
            AIStartTurn();
            AITossDice();
            AIBuildHouse();
            AIEndTurn();
            
            yield return new WaitForSeconds(1F);
        }
        
        public void SetPosIndex(int currentPos)
        {
            EventCenter.TriggerEvent(EventRegistry.OnPositionChanged, this, playerData.position, currentPos);
            SetPosition(currentPos);
        }

        public void SetBalance(int currentBalance)
        {
            EventCenter.TriggerEvent(EventRegistry.OnBalanceChanged, this, playerData.balance, currentBalance);
            playerData.balance = currentBalance;
        }

        private void OnRoundEnd(MonoBehaviour script, ValueEventArgs args)
        {
            playerData.pauseCount--;
        }
        
        public void GameOver(object playerData, ValueEventArgs balanceArg)
        {
            if (this.playerData.balance <= 0)
            {
                
            }
        }

        public bool CanMove() => playerData.pauseCount <= 0;
        public bool IsMyTurn() => playerData.roundIndex == level.PlayerRound;
        
        public Authorization SaveAll(PlayerData playerData) => database.SaveAll(playerData);
        public void Save(string columnName, object data) => database.Save(playerData.userName, columnName, data);

        public PlayerManager Init(PlayerData playerData)
        {
            this.playerData = playerData;
            SetPosition(playerData.position);
            return this;
        }

        private PlayerManager SetPosition(int position)
        {
            prev_pos = GetPlayerPosByIndex(playerData.position);
            if (playerData.position + position is < 0 and >= -40)
            {
                playerData.position = -position;
            }
            playerData.position = position switch
            {
                <= -40 => -position % 40,
                >= 40 => position % 40,
                _ => position
            };
            next_pos = GetPlayerPosByIndex(playerData.position);
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

        public void AIStartTurn()
        {
        }
        
        public void AITossDice()
        {
            if (playerData.roundIndex != level.PlayerRound) return;
            var random = new Random();
            var diceValue = random.Next(1, 7); 
            SetPosIndex(playerData.position + diceValue);
        }

        public void AIPurchaseField(int fieldIndex)
        {
        }

        public void AIBuildHouse()
        {
            
        }

        public void AIEndTurn()
        {
            level.NextTurn();
        }

        public void EvaluateRisks()
        {
            
        }

        public void EvaluateEarnings()
        {
            
        }
        
        public static Vector3 GetPlayerPosByIndex(int index) => Level.PosInRange.First(pair => 
            Utils.IsInRange(pair.Key, index)).Value.Invoke(index);
    }
}

