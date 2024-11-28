using System;
using THNeonMirage.Data;
using THNeonMirage.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace THNeonMirage.Map
{
    [Serializable]
    public abstract class FieldTile : MonoBehaviour
    {
        public int id;
        public string fieldName;
        public Type fieldType;
        public Color backGroundColor;
        
        private GameObject hoverPanel;
        private GameObject hoverText;

        private SpriteRenderer spriteRenderer;
        private string tooltipString;

        [DisplayOnly]
        public int occupiedCount;
        public int price1;
        public int price2;
        public int price3;
        public int tolls;

        private void Start()
        {
            hoverPanel = GameObject.Find("Canvas/HoverPanel");
            hoverText = GameObject.Find("Canvas/HoverPanel/HoverText");
            spriteRenderer = GetComponent<SpriteRenderer>();
            backGroundColor = spriteRenderer.color;

            price1 = 10000;
            price2 = 12000;
            price3 = 15000;
        }

        private void OnMouseOver()
        {
            tooltipString = $"编号：{id}\n名称：{fieldName}";
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

        public abstract void OnPlayerStop(PlayerManager playerManager);

        public virtual void OnPlayerPassBy(PlayerManager playerManager)
        {
        
        }

        public enum Type
        {
            Official,
            Facility,
            Bazaar,
            Stage,
            Other
        }

    }
}
