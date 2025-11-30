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
            canPurchase = false;
            description = "被紫妈抓进隙间，停三回合";
        }

        public override void OnPlayerStopAt(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos))return;
            player.playerData.pauseCount = 3;
        }
    }
}