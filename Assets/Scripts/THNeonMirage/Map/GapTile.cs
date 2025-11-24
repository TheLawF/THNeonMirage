using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Util;

namespace THNeonMirage.Map
{
    public class GapTile: FieldTile
    {
        private void Start()
        {
            Init();
            description = "被紫妈抓进隙间，停三回合";
        }

        public override void OnPlayerStop(PlayerManager player, int prevPos, int currentPos)
        {
            // base.OnPlayerStop(playerData, currentPos);
            if (!IsTileValid(currentPos))return;
            player.playerData.pauseCount = 3;
        }
    }
}