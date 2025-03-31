using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.Mathematics;

namespace THNeonMirage.Map
{
    public class PirateShip : FieldTile
    {

        private void Start()
        {
            Init();
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }
        
        // 随机向前或者向后移动0-5格
        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            var data = (PlayerData)playerData;
            var bonus = Random.NextInt(-5, 5);
            var result = Random.NextBool() ? bonus : -bonus;
            data.Position += result;
        }
    }
}