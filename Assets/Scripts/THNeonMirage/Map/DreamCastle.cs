using System;
using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;

namespace THNeonMirage.Map
{
    public class DreamCastle : FieldTile
    {
        private void Start()
        {
            Init();
            description = "经过或停在此处时可以获得10000月虹币\n\n<i><color=#444444>“欢迎来到梦乐园~”</color></i>";
        }

        public override void OnPlayerStop(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos))return;
            player.playerData.balance += 10000;
        }


        public override void OnPlayerPassBy(PlayerManager player, int prevPosition, int currentPosition)
        {
            var prevPos = prevPosition;
            var nextPos = currentPosition;
            if (prevPos <= 40 && nextPos >= 0) player.playerData.balance += 10000;
        }

    }
}