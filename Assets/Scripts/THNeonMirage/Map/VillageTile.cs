using THNeonMirage.Data;

namespace THNeonMirage.Map
{
    public class VillageTile : FieldTile
    {
        public override void OnPlayerStop(Player player)
        {
            
        }

        public VillageTile(int id, string fieldName, Type fieldType) : base(id, fieldName, fieldType)
        {
        }
    }
}