

namespace THNeonMirage.Map
{
    public class BeliefTile : FieldTile
    {
        private void Start()
        {
            Init();
            canPurchase = false;
            description = "抽取一次信仰效果";
        }
    }
}