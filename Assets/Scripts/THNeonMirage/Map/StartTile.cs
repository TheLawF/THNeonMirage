using THNeonMirage.Data;

namespace THNeonMirage.Map
{
    public class StartTile : FieldTile
    {

        public override void OnPlayerStop(Player player)
        {
            
        }

        public override void OnPlayerPassBy(Player player)
        {
            
        }

        public StartTile(int id, string fieldName, Type fieldType) : base(id, fieldName, fieldType)
        {
        }
    }
}