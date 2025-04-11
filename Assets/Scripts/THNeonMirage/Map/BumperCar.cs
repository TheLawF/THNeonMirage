using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.Mathematics;

namespace THNeonMirage.Map
{
    public class BumperCar : FieldTile
    {
        private System.Random Random;
        private void Start()
        {
            Init();
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }
        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            var data = (PlayerData)playerData;
            data.PauseCount = NextBool() ? NextInt(-2, 0) : NextInt(1, 3);
        }
    }
}