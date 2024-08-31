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
