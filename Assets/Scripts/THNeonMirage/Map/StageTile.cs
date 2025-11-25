namespace THNeonMirage.Map
{
    public class StageTile: FieldTile
    {
        private void Start()
        {
            Init();
            canPurchased = false;
            description = "在这里停下需要去舞台表演节目，否则失去5000月虹币";
        }
    }
}