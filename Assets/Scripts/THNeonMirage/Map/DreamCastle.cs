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
            Player.PlayerData.OnPassBy += OnPlayerPassBy;
        }


        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
            => ((PlayerData)playerData).Balance += 10000;
        

        public override void OnPlayerPassBy(object playerData, object prevPosition, object currentPosition)
        {
            var prevPos = (int)prevPosition;
            var nextPos = (int)currentPosition;
            if (prevPos <= 40 && nextPos >= 0) ((PlayerData)playerData).Balance += 10000;
        }

    }
}