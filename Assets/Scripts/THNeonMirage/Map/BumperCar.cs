using THNeonMirage.Manager;

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
        public override void OnPlayerStopAt(PlayerManager player, int prevPos, int currentPos)
        {
            base.OnPlayerStopAt(player, prevPos, currentPos);
            if (!IsTileValid(currentPos))return;
            player.playerData.pauseCount = IsTileValid(currentPos) && NextBool() ? NextInt(-2, 0) : NextInt(1, 3);
        }
    }
}