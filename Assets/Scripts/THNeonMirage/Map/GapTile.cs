using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Util;

namespace THNeonMirage.Map
{
    public class GapTile: FieldTile
    {
        private void Start()
        {
            Init();
            description = "被紫妈抓进隙间，停三回合";
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }

        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            base.OnPlayerStop(playerData, currentPos);
            if (!IsTileValid(currentPos))return;
            ((PlayerData)playerData).PauseCount = 3;
        }
    }
}