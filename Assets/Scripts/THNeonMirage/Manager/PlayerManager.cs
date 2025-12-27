using System;
using System.Collections;
using System.Linq;
using Fictology.Data.Serialization;
using Fictology.Registry;
using Fictology.UnityEditor;
using FlyRabbit.EventCenter;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        private PhotonView m_view;
        private Transform m_transform;
        private Vector3 prev_pos;
        private Vector3 next_pos;
        private Camera _camera;

        private void Awake()
        {
            playerData = new PlayerData().SetBalance(60000);
        }

        private void Start()
        {
            _camera = Camera.main;
            level = Registries.Get<Level>(LevelRegistry.ClientLevel);

            canvas = Registries.GetObject(UIRegistry.Canvas);
            m_view = gameObject.GetPhotonView();
            m_transform = GetComponent<Transform>();
            indexLabel = PrefabRegistry.Label.Instantiate(GetPointOnScreen(), Quaternion.identity, canvas.transform);
            indexLabel.GetComponent<TextMeshProUGUI>().text = playerData.roundIndex.ToString();
        }

        public void BindUIElements(InGamePanelHandler inGamePanel, DiceHandler diceHandler)
        {
            inGamePanel.playerObject = gameObject;
            inGamePanel.player = this;
            diceHandler.player = this;
            var dice = diceHandler.gameObject;
            var img = dice.GetComponent<RawImage>();
            dice.SetActive(true);
            img.texture = Resources.Load<Texture2D>("Textures/dice");
            img.uvRect = new Rect(0, 0.16667F, 1, 1);
        }

        public static void CreateOnlinePlayer(GameHost host, bool isBot)
        {
            host.playerInstance = PhotonNetwork.Instantiate(PrefabRegistry.Player.PrefabPath, GetPlayerPosByIndex(0), Quaternion.identity);
            host.player = host.playerInstance.GetComponent<PlayerManager>();
            host.level.PlayerInstances.Add(host.playerInstance);
            
            host.player.playerData.isBot = isBot;
            host.player.SendSpriteUpdateToOthers(null, host.playerInstance.GetComponent<SpriteRenderer>().color);
            host.player.playerData.roundIndex = PhotonNetwork.IsConnectedAndReady
                ? PhotonNetwork.LocalPlayer.ActorNumber
                : host.level.PlayerInstances.IndexOf(host.playerInstance);
            
            var exitRoom = Registries.GetComponent<Button>(UIRegistry.ExitButton);
            exitRoom.onClick.AddListener(() => GameMain.GameOver(host.player));
            
            if (host.playerInstance.GetPhotonView().IsMine)
            {
                var camera = Registries.Get<CameraController>(UIRegistry.MainCamera);
                camera.BindingPlayer = host.playerInstance;
            }

            var random = new Unity.Mathematics.Random((uint)DateTime.Now.Millisecond);
            var sprite = host.player.GetComponent<SpriteRenderer>();
            sprite.color = new Color(random.NextFloat(0, 1), random.NextFloat(0, 1), random.NextFloat(0, 1));
            if (!isBot)
            {
                EventCenter.TriggerEvent(EventRegistry.OnBalanceChanged, host.player, 
                    host.player.playerData.balance, host.player.playerData.balance);
                
                var inGamePanelHandler = Registries.GetComponent<InGamePanelHandler>(UIRegistry.InGamePanel);
                var diceButton = Registries.GetObject(UIRegistry.DiceButton);
                var diceHandler = diceButton.GetComponent<DiceHandler>();
                
                inGamePanelHandler.playerObject = host.playerInstance;
                inGamePanelHandler.player = host.player;
                diceHandler.player = host.player;
                diceButton.SetActive(true);
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                Registries.Instance.RegisterNetworkInstances(host.playerInstance.GetPhotonView(), host.playerInstance);
            }
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
            var view = gameObject.GetPhotonView();
            EventCenter.TriggerEvent(EventRegistry.OnPositionChanged, this, playerData.position, currentPos);
            SetPosition(currentPos);
        }

        public void SetBalance(int currentBalance)
        {
            EventCenter.TriggerEvent(EventRegistry.OnBalanceChanged, this, playerData.balance, currentBalance);
            playerData.balance = currentBalance;
            SendPlayerDataUpdate(m_view.ViewID, playerData);
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

        /// <summary>
        /// Fictology - 网络发包命名规则：<br></br>
        /// 发送端命名为：NotifyXXXXXUpdate(); 可以在末尾添加ToEveryone表示发送给所有人<br></br>
        /// 接收端命名为：ReceiveXXXXUpdate();<br></br>
        /// </summary>
        /// <param name="posIndex">发送者购买的土地的索引</param>
        /// <param name="fieldLevel">土地的等级，用以判断过路费费用</param>
        /// <param name="property">土地的其它数据，比如售价、不同等级土地的所有过路费</param>
        public void SendFieldPropertyUpdate(int posIndex, int fieldLevel, SerializableProperty property)
        {
            var view = gameObject.GetPhotonView();
            view.RPC(nameof(ReceiveOnlineOwnerUpdate), RpcTarget.Others, view.ViewID, posIndex);
            view.RPC(nameof(ReceiveFieldLevelUpdate), RpcTarget.Others, fieldLevel, posIndex);
        }

        /// <summary>
        /// Fictology - 网络发包命名规则：<br></br>
        /// 发送端命名为：NotifyXXXXXUppdate(); 或者 SendXXXXXUpdate();<br></br>
        /// 接收端命名为：ReceiveXXXXUppdate();<br></br>
        /// </summary>
        /// <param name="senderViewId">发送者</param>
        /// <param name="posIndex">发送者购买的土地的索引</param>
        [PunRPC]
        public void ReceiveOnlineOwnerUpdate(int senderViewId, int posIndex)
        {
            var tile = level.GetTile<FieldTile>(posIndex);
            tile.SetOwnerOnLocal(senderViewId);
        }

        [PunRPC]
        public void ReceiveFieldLevelUpdate(int fieldLevel, int posIndex)
        {
            var tile = level.GetTile<FieldTile>(posIndex);
            tile.level = fieldLevel;
        }

        public void SendPlayerDataUpdate(Player targetPlayer, PlayerData data)
        {
            var view = gameObject.GetPhotonView();
            view.RPC(nameof(ReceivePlayerDataUpdate), targetPlayer, data.Serialize());
        }
        
        public void SendPlayerDataUpdate(int actorViewId, PlayerData data)
        {
            m_view.RPC(nameof(ReceivePlayerDataUpdate), RpcTarget.Others, actorViewId, data.Serialize());
        }

        [PunRPC]
        public void ReceivePlayerDataUpdate(int actorViewId, CompoundData compound)
        { 
            var data = PhotonView.Find(actorViewId).GetComponent<PlayerManager>().playerData; 
            data.Deserialize(compound); 
            EventCenter.TriggerEvent(EventRegistry.OnBalanceChangedRPC, m_view.ViewID, data.balance);
        }

        public void SendSpriteUpdateToOthers(string skinPath, Color color)
        {
            var sprite = gameObject.GetComponent<SpriteRenderer>();
            var view = gameObject.GetPhotonView();
            if (skinPath != null)
            {
                sprite.sprite = (Sprite) Resources.Load(skinPath);
            }

            sprite.color = color;
            view.RPC(nameof(ReceiveSpriteUpdate), RpcTarget.Others, skinPath, view.ViewID, color.r, color.g, color.b);
        }

        [PunRPC]
        public void ReceiveSpriteUpdate(string skinPath, int senderId, float r, float g, float b)
        {
            if (gameObject.GetPhotonView().ViewID != senderId) return;
            var sprite = gameObject.GetComponent<SpriteRenderer>();
            if (skinPath != null)
            {
                sprite.sprite = (Sprite) Resources.Load(skinPath);
            }

            sprite.color = new Color(r, g, b);
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

        public static Vector3 GetPlayerPosByIndex(int index)
        {
            var level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            var pos = level.fields[index].GetComponent<Transform>().position;
            return new Vector3(pos.x, pos.y, -1);
        }
    }
}

