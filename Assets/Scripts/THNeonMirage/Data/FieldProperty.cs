using JetBrains.Annotations;
using THNeonMirage.Map;

namespace THNeonMirage.Data
{
    public record FieldProperty(string Name, FieldTile.Type FieldType, Price Price)
    {

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