using System;
using THNeonMirage.Data;
using UnityEngine;

namespace THNeonMirage.Map
{
    [Serializable]
    public abstract class FieldTile : MonoBehaviour
    {
        public int id;
        public string fieldName;
        public Type fieldType;
        public Color backGroundColor;

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            backGroundColor = spriteRenderer.color;
        }

        private void OnMouseOver()
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }

        private void OnMouseExit()
        {
            spriteRenderer.color = backGroundColor;
        }

        public abstract void OnPlayerStop(Player player);

        public virtual void OnPlayerPassBy(Player player)
        {
        
        }

        public enum Type
        {
            Official,
            Bazaar,
            Custom
        }
    }
}
