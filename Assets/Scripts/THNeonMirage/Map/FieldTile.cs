using System;
using Fictology.Event;
using Fictology.Registry;
using Fictology.UnityEditor;
using FlyRabbit.EventCenter;
using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class FieldTile : RegistryEntry
    {
        public int index;
        public int level;
        public string description;
        public Color backGroundColor;
        
        public ObsoleteGameClient client;
        public FieldProperty Property;
        public PlayerManager Owner;
        public bool canPurchase;
        
        public SpriteRenderer spriteRenderer;
        [DisplayOnly] public GameObject inGamePanel;
        [DisplayOnly] public GameObject hoverPanel;
        [DisplayOnly] public GameObject hoverText;

        protected Random Random = new();
        [FormerlySerializedAs("onlineOwner")] public PhotonView remoteOwner;
        private string tooltipString;
        
        private void Start()
        {
            canPurchase = true;
            hoverPanel = GameObject.Find("Canvas/HoverPanel");
            hoverText = GameObject.Find("Canvas/HoverPanel/HoverText");
            backGroundColor = new Color(1f,1f, 1f, 0.6f);
            spriteRenderer.color = backGroundColor;

            description = $"土地价格：{Property.Price.Purchase}\n\n" +
                          $"空地过路费：{Property.Price.Level0}\n" +
                          $"一幢房屋：{Property.Price.Level1}\n" +
                          $"两幢房屋：{Property.Price.Level2}\n" +
                          $"三幢房屋：{Property.Price.Level3}\n" +
                          $"每幢房屋建造费用：{Property.Price.Building}";
            Init();
        }

        public virtual void Init()
        {
            remoteOwner = GetComponent<PhotonView>();
            Random = new Random((uint)DateTime.Now.Millisecond);
            inGamePanel = Registries.GetObject(UIRegistry.InGamePanel);

            EventCenter.AddListener<PlayerManager, int, int>(EventRegistry.OnPositionChanged, OnPlayerPassBy);
            EventCenter.AddListener<PlayerManager, int, int>(EventRegistry.OnPositionChanged, OnPlayerStopAt);
        }

        public int CurrentTolls()
        {
            var tolls = level switch
            {
                0 => Property.Price.Level0,
                1 => Property.Price.Level1,
                2 => Property.Price.Level2,
                3 => Property.Price.Level3,
                4 => -2000,
                5 => -1,
                _ => throw new IndexOutOfRangeException("等级不能超过3")
            };
            return HasOwner() ? tolls : index == 0 ? 0 : tolls;
        }

        public int GetPurchasePrice() => Property.Price.Purchase;
        public int GetBuildingPrice() => Property.Price.Building;

        // public abstract bool IsStartTile();
        // public abstract bool HasSpecialEffect();

        private void OnMouseUpAsButton()
        {
            inGamePanel.SetActive(true);
            tooltipString = $"编号：{index}\n名称：{Property.Name}";
            // spriteRenderer.color = new Color(backGroundColor.r, backGroundColor.g, backGroundColor.b, 0.85f);
            
            var inGame = inGamePanel.GetComponent<InGamePanelHandler>();
            var gameLevel = Registries.Get<Level>(LevelRegistry.ClientLevel);
            inGame.TrySetTexts(gameLevel, index);
            inGame.UpdateUI(gameLevel, index);
        }

        private void OnMouseExit()
        {
        }

        public bool HasOwner() => Owner is not null;
        public bool HasRemoteOwner() => remoteOwner is not null && remoteOwner.GetComponent<PlayerManager>() is not null;

        public virtual void OnPlayerStopAt(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos)) return;
            if (HasRemoteOwner())
            {
                OnLocalPlayerStopAt(player);
                return;
            }
            if (!HasOwner())return;
            
            player.SetBalance(player.playerData.balance - CurrentTolls());
            Owner.SetBalance(Owner.playerData.balance + CurrentTolls());
            OnLocalPlayerStopAt(player);
        }

        /// <summary>
        /// <list type="number">
        /// <item>
        /// 本地玩家在开始设置新位置时调用该方法，检测本地玩家自己客户端上即将抵达的土地是否已被某个在线玩家占有，如果占有则扣除当前过路费
        /// </item>
        /// <item>
        /// 这里需要发送两次，因为假如A被B收取过路费，则需要通知房间内所有玩家B,C,D扣除他们各自客户端上的A玩家的相应余额。同时还要通知A,B,C,D增加他们各自客户端上B玩家的相应余额
        /// </item>
        /// <item>
        /// 我将 A(扣除余额) -> B,C,D 的网络通信集成在 player.SetBalance() 方法中了，这个方法只需额外调用 A(增加B的余额) -> B,C,D 的逻辑同步即可
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="player">本地玩家</param>
        public virtual void OnLocalPlayerStopAt(PlayerManager player)
        {
            if (!PhotonNetwork.IsConnected) return; // 仅限在线模式调用
            if (OwnerIsMe(player))
            {
                OnStoppedAtMyProperty();
                return;
            }
            
            var onlinePlayer = remoteOwner.GetComponent<PlayerManager>();
            
            player.SetBalance(player.playerData.balance - CurrentTolls());
            player.SendPlayerDataUpdate(remoteOwner.ViewID, onlinePlayer.playerData.AddBalance(CurrentTolls()));
        }

        public virtual void OnStoppedAtMyProperty()
        {
            
        }

        [PunRPC]
        public void SendBalanceUpdateToOnlineOwner()
        {
            
        }

        /// <summary>
        /// 检测是否是自己的土地
        /// <list type="number">
        /// <item>
        /// 在离线模式下，直接检测player和owner的引用是否一致
        /// </item>
        /// <item>
        /// 如果检测到field存储的远端玩家的viewId等于传入的本地玩家的viewId，则表明土地是自己的
        /// </item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public bool OwnerIsMe(PlayerManager player)
        {
            if (player == Owner) return true;
            return remoteOwner.ViewID == player.gameObject.GetPhotonView().ViewID;
        }
        
        public virtual void OnPlayerPassBy(PlayerManager player, int prevPosition, int currentPosition)
        {
        }

        public virtual void OnPlayerPassByRPC(PhotonView player, int prevPosition, int currentPosition)
        {
            
        }

        protected bool NextBool() => new System.Random().Next(1, 2) == 1;
        protected int NextInt(int min, int max) => new System.Random().Next(min, max);

        public bool IsTileValid(ValueEventArgs args) => (int)args.Value == index;
        public bool IsTileValid(int index) => index == this.index;

        // [PunRPC]
        // public void SendEventToMaster()
        // {
        //     var type = GetType();
        //     var onPlayerStop = GetType().GetMethod(nameof(OnLocalPlayerStopAt));
        //     var onPlayerPass = GetType().GetMethod(nameof(OnPlayerPassByRPC));
        //     
        //     if (onPlayerStop == null)return;
        //     onlineOwner.RPC(nameof(RegisterEventOnMaster), RpcTarget.All, EventRegistry.OnPositionChangedRPC, 
        //         Delegate.CreateDelegate(type, onPlayerStop));
        //     
        //     if (onPlayerPass == null)return;
        //     onlineOwner.RPC(nameof(RegisterEventOnMaster), RpcTarget.All, EventRegistry.OnPositionChangedRPC, 
        //         Delegate.CreateDelegate(type, onPlayerPass));
        // }

        [PunRPC]
        public void RegisterEventOnMaster(EventKey key, Delegate method)
        {
            if (!PhotonNetwork.IsMasterClient)return;
            EventCenter.AddListenerByKey(key, method);
        }

        public PhotonView GetOnlineOwner() => remoteOwner;

        public void SetOwnerOnLocal(int onlineOwnerViewId)
        {
            canPurchase = false;
            remoteOwner = PhotonView.Find(onlineOwnerViewId);
            spriteRenderer.color = remoteOwner.GetComponent<SpriteRenderer>().color;
        }

        public void RemoveOnlineOwner() => remoteOwner = null;

        public enum Type
        {
            DreamWorld,
            MagicForest,
            YoukaiMountain,
            BambooForest,
            AncientHell,
            Village,
            Nether,
            Fairies,
            Higan,
            Other
        }

        // public abstract string GetRegistryName();
    }
}
