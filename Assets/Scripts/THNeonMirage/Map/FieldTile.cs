using System;
using Fictology.Registry;
using Fictology.UnityEditor;
using FlyRabbit.EventCenter;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using UnityEngine;
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
        public bool canPurchased;
        
        public SpriteRenderer spriteRenderer;
        [DisplayOnly] public GameObject inGamePanel;
        [DisplayOnly] public GameObject hoverPanel;
        [DisplayOnly] public GameObject hoverText;

        protected Random Random = new();
        private string tooltipString;
        
        private void Start()
        {
            canPurchased = true;
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
            
            Random = new Random((uint)DateTime.Now.Millisecond);
            inGamePanel = Registries.GetObject(UIRegistry.InGamePanel);

            EventCenter.AddListener<PlayerManager, int, int>(EventRegistry.OnPositionChanged, OnPlayerPassBy);
            EventCenter.AddListener<PlayerManager, int, int>(EventRegistry.OnPositionChanged, OnPlayerStop);
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
        
        public virtual void OnPlayerStop(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos)) return;
            if (!HasOwner())return;
            if (player.playerData.userName == null) return;
            if (Owner.playerData.userName == player.playerData.userName)return;
            
            player.SetBalance(player.playerData.balance - CurrentTolls());
            Owner.SetBalance(Owner.playerData.balance + CurrentTolls());
        }

        public virtual void OnPlayerPassBy(PlayerManager player, int prevPosition, int currentPosition)
        {
        }


        protected bool NextBool() => new System.Random().Next(1, 2) == 1;
        protected int NextInt(int min, int max) => new System.Random().Next(min, max);

        public bool IsTileValid(ValueEventArgs args) => (int)args.Value == index;
        public bool IsTileValid(int index) => index == this.index;

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
