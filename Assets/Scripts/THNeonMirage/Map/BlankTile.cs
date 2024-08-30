using THNeonMirage.Data;

namespace THNeonMirage.Map
{
    public class BlankTile: FieldTile
    {
        public override void OnPlayerStop(Player player)
        {
            
        }

        public BlankTile(int id, string fieldName, Type fieldType) : base(id, fieldName, fieldType)
        {
        }
    }
}