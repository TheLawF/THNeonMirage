using THNeonMirage.Data;

namespace THNeonMirage.Map
{
    public class StartTile : FieldTile
    {
        private void Start()
        {
            id = 0;
            fieldName = "StartField";
            fieldType = Type.Official;
        }

        public override void OnPlayerStop(Player player)
        {
            
        }

        public override void OnPlayerPassBy(Player player)
        {
            
        }
    }
}