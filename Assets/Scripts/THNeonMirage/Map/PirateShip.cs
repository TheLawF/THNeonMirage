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
            canPurchased = false;
            Random = new Random();
        }
        
        // 随机向前或者向后移动0-5格
        public override void OnPlayerStopAt(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos)) return;
            var bonus = random.Next(5);
            var result = random.Next(1) == 0 ? bonus : -bonus;
            player.SetPosition(currentPos);
        }
    }
}