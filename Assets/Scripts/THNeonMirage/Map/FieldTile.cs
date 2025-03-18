using System;
using System.Collections.Generic;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using TMPro;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace THNeonMirage.Map
{
    [Serializable]
    public class FieldTile : MonoBehaviour
    {
        public int id;
        public int level;
        public string description;
        public Color backGroundColor;

        public FieldProperty Property;
        public PlayerData Owner;
        private GameObject hoverPanel;
        private GameObject hoverText;

        protected PlayerManager Player;
        protected Random Random = new();
        private SpriteRenderer spriteRenderer;
        private string tooltipString;
        
        private void Start()
        {
            InitPlayer();
            hoverPanel = GameObject.Find("Canvas/HoverPanel");
            hoverText = GameObject.Find("Canvas/HoverPanel/HoverText");
            spriteRenderer = GetComponent<SpriteRenderer>();
            backGroundColor = spriteRenderer.color;
            
            description = $"土地价格：{Property.Price.Purchase}\n\n" +
                          $"空地过路费：{Property.Price.Level0}\n" +
                          $"一幢房屋：{Property.Price.Level1}\n" +
                          $"两幢房屋：{Property.Price.Level2}\n" +
                          $"三幢房屋：{Property.Price.Level3}\n" +
                          $"每幢房屋建造费用：{Property.Price.Building}";
        }

        protected void InitPlayer() => Player = PlayerManager.Instance.GetComponent<PlayerManager>();
        
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
            tooltipString = $"编号：{id}\n名称：{Property.Name}";
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
            hoverPanel.transform.position = Input.mousePosition + new Vector3(40f, 40f);
            hoverText.GetComponent<TMP_Text>().text = tooltipString;
            hoverPanel.SetActive(true);
        }

        private void OnMouseExit()
        {
            spriteRenderer.color = backGroundColor;
            hoverPanel.SetActive(false);
        }

        public bool HasOwner() => Owner == null;
        
        public virtual void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            if (!HasOwner())return;
            if (Owner.UserName == ((PlayerData)playerData).UserName)return;
            ((PlayerData)playerData).Balance -= CurrentTolls();
        }

        public virtual void OnPlayerPassBy(object playerData, object prevPosition, object currentPosition)
        {
        
        }

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
    }
}
