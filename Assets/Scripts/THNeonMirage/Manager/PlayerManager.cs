using System;
using System.Collections;
using System.Linq;
using Fictology.Registry;
using Fictology.UnityEditor;
using FlyRabbit.EventCenter;
using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
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
        public Level level;
        public GameObject indexLabel;
        public PlayerData playerData;
        public GameObject canvas;

        [DisplayOnly] public GameHost database;

        private Transform m_transform;
        private Vector3 prev_pos;
        private Vector3 next_pos;
        private SpriteRenderer sprite;
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private Camera _camera;

        private void Awake()
        {
            playerData = new PlayerData().SetBalance(60000);
        }

        private void Start()
        {
            _camera = Camera.main;
            level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            sprite = GetComponent<SpriteRenderer>();

            canvas = Registries.GetObject(UIRegistry.Canvas);
            m_transform = GetComponent<Transform>();
            indexLabel = UIRegistry.IndexLabel.Instantiate(GetPointOnScreen(), Quaternion.identity, canvas.transform);
            indexLabel.GetComponent<TextMeshProUGUI>().text = playerData.roundIndex.ToString();
        }

        public IEnumerator ExecuteAITask()
        {
            yield return new WaitForSeconds(1F);
            if (playerData.pauseCount > 0) yield break;
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

        public bool IsMyTurn()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                return playerData.roundIndex == PhotonNetwork.LocalPlayer.ActorNumber;
            }
            return playerData.roundIndex == level.PlayerRound;
        }
        public bool IsBot() => playerData.isBot;
        public Authorization SaveAll(PlayerData playerData) => database.SaveAll(playerData);
        public void Save(string columnName, object data) => database.Save(playerData.userName, columnName, data);

        public PlayerManager Init(PlayerData playerData)
        {
            this.playerData = playerData;
            SetPosition(playerData.position);
            return this;
        }

        public PlayerManager SetPosition(int position)
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
                m_transform.position = Vector3.Lerp(prevPos, nextPos, i);
                indexLabel.transform.position = GetPointOnScreen();
                
                yield return new WaitForSeconds(0.02f);
            }
        }

        public Vector3 GetPointOnScreen()
        {
            return _camera.WorldToScreenPoint(m_transform.position + Vector3.up + Vector3.right * 2);
        }

        public void AIStartTurn()
        {
            
        }
        
        public void AITossDice()
        {
            var random = new Random();
            var diceValue = random.Next(1, 7); 
            var level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            
            SetPosIndex(playerData.position + diceValue);
            level.GetTile<FieldTile>(playerData.position).OnPlayerStopAt(this, playerData.position, playerData.position);
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

