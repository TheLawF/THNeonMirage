using System;
using THNeonMirage.Map;

namespace THNeonMirage.Data
{
    [Serializable]
    public record FieldProperty(string Name, FieldTile.Type FieldType, Price Price)
    {
        public SerializableProperty AsSerializable() => new(Name, (int)FieldType, Price.Purchase, Price.Building,
            Price.Level0, Price.Level1, Price.Level2, Price.Level3);

        public static FieldProperty FromSerializable(SerializableProperty property) => new (
            property.Name, (FieldTile.Type)property.FieldType, new Price(property.PurchasePrice, property.BuildPrice, 
                property.TollLevel0, property.TollLevel1, property.TollLevel2, property.TollLevel3));
    }

    [Serializable]
    public struct SerializableProperty
    {
        public SerializableProperty(string name, int fieldType, int purchasePrice, int buildPrice, 
            int tollLevel0, int tollLevel1, int tollLevel2, int tollLevel3)
        {
            Name = name;
            FieldType = fieldType;
            PurchasePrice = purchasePrice;
            BuildPrice = buildPrice;
            TollLevel0 = tollLevel0;
            TollLevel1 = tollLevel1;
            TollLevel2 = tollLevel2;
            TollLevel3 = tollLevel3;
        }

        public readonly string Name;
        public readonly int FieldType;
        public readonly int PurchasePrice;
        public readonly int BuildPrice;
        public readonly int TollLevel0;
        public readonly int TollLevel1;
        public readonly int TollLevel2;
        public readonly int TollLevel3;
    }
    
    public struct Price
    {
        public readonly int Purchase, Building, Level0, Level1, Level2, Level3;

        public Price(int purchase, int building, int level0, int level1, int level2, int level3)
        {
            Purchase = purchase;
            Building = building;
            Level0 = level0;
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
        }
    }
    
}