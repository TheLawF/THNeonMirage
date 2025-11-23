using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.VisualScripting;

namespace THNeonMirage.Map
{
    public class RollerCoaster: FieldTile
    {
        private void Start()
        {
            Init();
        }

        public override void OnPlayerStop(PlayerManager player, int currentPos)
        {
            var bonus = Random.NextInt(300, 10000);
            var result = Random.NextBool() ? bonus : -bonus;
            player.playerData.balance += result;
        }
    }
}