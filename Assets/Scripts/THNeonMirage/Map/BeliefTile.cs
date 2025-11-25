

namespace THNeonMirage.Map
{
    public class BeliefTile : FieldTile
    {
        private void Start()
        {
            Init();
            canPurchased = false;
            description = "抽取一次信仰效果";
        }
    }
}