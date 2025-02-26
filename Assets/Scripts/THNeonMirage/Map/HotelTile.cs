using THNeonMirage.Data;
using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class HotelTile: FieldTile
    {
        private void Start() => description = "红魔";
        public override void OnPlayerStop(PlayerManager playerManager)
        {
            
        }

    }
}