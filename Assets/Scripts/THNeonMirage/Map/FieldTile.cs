using System;
using Fictology.UnityEngine;
using Photon.Pun;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = Unity.Mathematics.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class FieldTile : RegistryEntry
    {
        public int id;
        public int level;
        public string description;
        public Color backGroundColor;
        
        public GameClient client;
        public FieldProperty Property;
        public PlayerData Owner;
        
        public PhotonView photonView;
        public SpriteRenderer spriteRenderer;
        [DisplayOnly] public GameObject inGamePanel;
        [DisplayOnly] public GameObject hoverPanel;
        [DisplayOnly] public GameObject hoverText;

        protected PlayerManager Player;
        protected Random Random = new();
        private string tooltipString;
        
        private void Start()
        {
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
        }

        public virtual void Init()
        {
            Start();
            Player = client.playerManager;
        }

        public int CurrentTolls()
        {
            return level switch
            {
                0 => Property.Price.Level0,
                1 => Property.Price.Level1,
                2 => Property.Price.Level2,
                3 => Property.Price.Level3,
                4 => -2000,
                5 => -1,
                _ => throw new IndexOutOfRangeException("等级不能超过3")
            };
        }

        public int GetPurchasePrice() => Property.Price.Purchase;
        public int GetBuildingPrice() => Property.Price.Building;

        // public abstract bool IsStartTile();
        // public abstract bool HasSpecialEffect();
        
        private void OnMouseOver()
        {
            inGamePanel.SetActive(true);
            tooltipString = $"编号：{id}\n名称：{Property.Name}";
            spriteRenderer.color = new Color(backGroundColor.r, backGroundColor.g, backGroundColor.b, 0.85f);
            
            var inGame = inGamePanel.GetComponent<InGamePanelHandler>();
            inGame.SetTexts(Owner, new ValueEventArgs(id));
            if (CurrentTolls() <= 0)
            {
                inGame.purchase.SetActive(false);
                inGame.cancel.SetActive(false);
            }
            else
            {
                inGame.purchase.SetActive(true);
                inGame.cancel.SetActive(true);
            }
            
        }

        private void OnMouseExit()
        {
            spriteRenderer.color = backGroundColor;
        }

        public bool HasOwner() => Owner == null;
        
        public virtual void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            if (!HasOwner())return;
            if (((PlayerData)playerData).UserName == null) return;
            if (Owner.UserName == ((PlayerData)playerData).UserName)return;
            ((PlayerData)playerData).Balance -= CurrentTolls();
            Owner.Balance += CurrentTolls();
            photonView.RPC(nameof(SyncFieldData), RpcTarget.AllBuffered, Owner);
        }

        public virtual void OnPlayerPassBy(object playerData, object prevPosition, object currentPosition)
        {
        
        }

        [PunRPC]
        public void SyncFieldData(PlayerData owner)
        {
            Owner = owner;
        }

        protected bool NextBool() => new System.Random().Next(1, 2) == 1;
        protected int NextInt(int min, int max) => new System.Random().Next(min, max);

        public bool IsTileValid(ValueEventArgs args) => (int)args.Value == id;

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
