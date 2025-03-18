using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.Mathematics;

namespace THNeonMirage.Map
{
    public class PirateShip : FieldTile
    {
        private Random random;

        private void Start()
        {
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }
        
        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            var data = (PlayerData)playerData;
            var bonus = random.NextInt(300, 10000);
            var result = random.NextBool() ? bonus : -bonus;
            data.Balance += result;
        }
    }
}