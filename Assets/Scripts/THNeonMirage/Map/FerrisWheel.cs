using System;
using System.Diagnostics;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.VisualScripting;

namespace THNeonMirage.Map
{
    public class FerrisWheel : FieldTile
    {
        private Stopwatch stopWatch;
        public const long Total = 1000 * 60 * 30;

        private void Start()
        {
            Init();
            stopWatch = new Stopwatch();
        }
        public override void OnPlayerStop(PlayerManager player, int prevPos, int currentPos)
        {
            if (!IsTileValid(currentPos))return;
            stopWatch.Start();
        }
    }
}