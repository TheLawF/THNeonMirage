using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class HotelTile: FieldTile
    {
        private void Start()
        {
            Init();
            description = "蕾米莉亚开的酒店，在这里停下需要支付10000月虹币的酒店费用哦";
        }

        public override void OnPlayerStop(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos))return;
            player.SetBalance(player.playerData.balance - 10000);
        }

    }
}