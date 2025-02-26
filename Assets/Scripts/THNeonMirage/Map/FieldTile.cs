using System;
using System.Collections.Generic;
using THNeonMirage.Data;
using THNeonMirage.Manager;
using TMPro;
using UnityEngine;

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
        public GameObject textField;
        private GameObject hoverPanel;
        private GameObject hoverText;

        private SpriteRenderer spriteRenderer;
        private string tooltipString;

        private void Start()
        {
            hoverPanel = GameObject.Find("Canvas/HoverPanel");
            hoverText = GameObject.Find("Canvas/HoverPanel/HoverText");
            spriteRenderer = GetComponent<SpriteRenderer>();
            backGroundColor = spriteRenderer.color;

            description = $"空地过路费：{Property.Price.Level0}\n" +
                          $"一幢房屋：{Property.Price.Level1}\n" +
                          $"两幢房屋：{Property.Price.Level2}\n" +
                          $"三幢房屋：{Property.Price.Level3}\n" +
                          $"每幢房屋建造费用：{Property.Price.Building}";
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

        public virtual void OnPlayerStop(PlayerManager playerManager)
        {
            
        }

        public virtual void OnPlayerPassBy(PlayerManager playerManager)
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
