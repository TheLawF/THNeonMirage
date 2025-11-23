using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Util;
using Unity.Mathematics;

namespace THNeonMirage.Map
{
    public class BumperCar : FieldTile
    {
        private System.Random Random;
        private void Start()
        {
            Init();
            // Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }
        public override void OnPlayerStop(PlayerManager player, int currentPos)
        {
            base.OnPlayerStop(player, currentPos);
            if (!IsTileValid(currentPos))return;
            player.playerData.pauseCount = IsTileValid(currentPos) && NextBool() ? NextInt(-2, 0) : NextInt(1, 3);
        }
    }
}