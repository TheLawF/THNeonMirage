
using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class LuckyTile : FieldTile
    {
        private void Start()
        {
            Init();
            canPurchased = false;
            description = "可以抽取一次祈福效果";
        }

        public override void OnPlayerStop(PlayerManager player, int prevPos, int currentPos)
        {
            
        }
        
    }
}