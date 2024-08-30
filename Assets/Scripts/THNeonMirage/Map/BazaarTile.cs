using THNeonMirage.Data;

namespace THNeonMirage.Map
{
    public class BazaarTile : FieldTile
    {
        public override void OnPlayerStop(Player player)
        {
            
        }

        public BazaarTile(int id, string fieldName, Type fieldType) : base(id, fieldName, fieldType)
        {
        }
    }
}