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
            description = "经过或停在此处时可以获得10000月虹币\n\n<i><color=#444444>“欢迎来到梦乐园~”</color></i>";
            Player = PlayerManager.Instance.GetComponent<PlayerManager>();
            Player.PlayerData.OnPassBy += OnPlayerPassBy;
        }

        public override void OnPlayerStop(object playerData, ValueEventArgs args)
            => ((PlayerData)playerData).Balance += 10000;
        

        public override void OnPlayerPassBy(object playerData, object prev, object next)
        {
            var prevPos = (int)prev;
            var nextPos = (int)next;
            if (prevPos <= 40 && nextPos >= 0) ((PlayerData)playerData).Balance += 10000;
        }

    }
}