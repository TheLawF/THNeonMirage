
using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class LuckyTile : FieldTile
    {
        private void Start()
        {
            Init();
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
            description = "可以抽取一次祈福效果";
        }
    }
}