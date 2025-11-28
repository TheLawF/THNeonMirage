using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.Mathematics;
using Unity.VisualScripting;

namespace THNeonMirage.Map
{
    public class RollerCoaster: FieldTile
    {
        private void Start()
        {
            Init();
            canPurchased = false;
        }

        public override void OnPlayerStopAt(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos))return;
            var bonus = Random.NextInt(300, 10000);
            var result = Random.NextBool() ? bonus : -bonus;
            player.SetBalance(player.playerData.balance + result);
        }
    }
}