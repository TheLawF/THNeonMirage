using System;
using THNeonMirage.Data;
using UnityEngine;

namespace THNeonMirage.Map
{
    [Serializable]
    public abstract class FieldTile : Component
    {
        public int id;
        public string fieldName;
        public Type fieldType;

        protected FieldTile(int id, string fieldName, Type fieldType)
        {
            this.id = id;
            this.fieldName = fieldName;
            this.fieldType = fieldType;
        }


        public enum Type
        {
            Official,
            Bazaar,
            Custom
        }

        public abstract void OnPlayerStop(Player player);

        public virtual void OnPlayerPassBy(Player player)
        {
        
        }

    }
}
