using THNeonMirage.Data;
using THNeonMirage.Event;
using Unity.VisualScripting;

namespace THNeonMirage.Map
{
    public class RollerCoaster: FieldTile
    {
        private void Start()
        {
            
        }

        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            var data = (PlayerData)playerData;
            var bonus = Random.NextInt(300, 10000);
            var result = Random.NextBool() ? bonus : -bonus;
            data.Balance += result;
        }
    }
}