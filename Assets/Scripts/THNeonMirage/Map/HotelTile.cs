using THNeonMirage.Data;

namespace THNeonMirage.Map
{
    public class HotelTile: FieldTile
    {
        public override void OnPlayerStop(Player player)
        {
            
        }

        public HotelTile(int id, string fieldName, Type fieldType) : base(id, fieldName, fieldType)
        {
        }
    }
}