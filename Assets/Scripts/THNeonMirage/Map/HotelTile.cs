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
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }

        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            Player.PlayerData.Balance -= 10000;
        }

    }
}