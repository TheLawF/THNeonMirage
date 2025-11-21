using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.Mathematics;

namespace THNeonMirage.Map
{
    public class PirateShip : FieldTile
    {

        private System.Random random = new ();
        private void Start()
        {
            Init();
            Random = new Random();
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }
        
        // 随机向前或者向后移动0-5格
        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            if ((int)currentPos.Value != id) return;
            var data = (PlayerData)playerData;
            var bonus = random.Next(5);
            var result = random.Next(1) == 0 ? bonus : -bonus;
            Player.SetPosIndex(playerData, new ValueEventArgs(result));
        }
    }
}