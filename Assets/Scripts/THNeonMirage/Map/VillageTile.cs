using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class VillageTile : FieldTile
    {
        private void Start()
        {
            Init();
            canPurchase = false;
            description = "玩家停在此处时可以领取任务获得金币";
        }

        public override void OnPlayerStopAt(PlayerManager player, int prevPos, int currentPos)
        {
            
        }

    }
}