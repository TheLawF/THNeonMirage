using System;
using THNeonMirage.Data;
using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class DreamCastle : FieldTile
    {
        private void Start() => description = "经过或停在此处时可以获得10000月虹币\n\n<i><color=#444444>“欢迎来到梦乐园~”</color></i>";

        public override void OnPlayerStop(PlayerManager playerManager) => playerManager.PlayerData.Balance += 10000;
        public override void OnPlayerPassBy(PlayerManager playerManager) => OnPlayerStop(playerManager);

    }
}