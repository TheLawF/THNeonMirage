using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class DreamCastle : FieldTile
    {
        private void Start()
        {
            Init();
            canPurchase = false;
            description = "经过或停在此处时可以获得10000月虹币\n\n<i><color=#A1A1A1>“欢迎来到梦乐园~”</color></i>";
        }

        public override void OnPlayerStopAt(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos))return;
            player.SetBalance(player.playerData.balance + 10000);
        }
        
        public override void OnPlayerPassBy(PlayerManager player, int prevPosition, int currentPosition)
        {
            if (currentPosition > 40) player.SetBalance(player.playerData.balance + 10000);
        }

    }
}