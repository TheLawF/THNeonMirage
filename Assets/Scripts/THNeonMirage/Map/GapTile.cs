using THNeonMirage.Data;
using THNeonMirage.Event;

namespace THNeonMirage.Map
{
    public class GapTile: FieldTile
    {
        private void Start()
        {
            InitPlayer();
            description = "被紫妈抓进隙间，停三回合";
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }

        public override void OnPlayerStop(object playerData, ValueEventArgs playerManager)
        {
            ((PlayerData)playerData).PauseCount += 3;
        }
    }
}