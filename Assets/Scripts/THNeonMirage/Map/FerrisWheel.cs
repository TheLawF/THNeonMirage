using System;
using System.Diagnostics;
using THNeonMirage.Manager;
using Unity.VisualScripting;

namespace THNeonMirage.Map
{
    public class FerrisWheel : FieldTile
    {
        private Stopwatch stopWatch;
        public bool finished;
        public const long Total = 1000 * 60 * 30;

        private void Start()
        {
            stopWatch = new Stopwatch();
        }
        public override void OnPlayerStop(PlayerManager playerManager)
        {
            stopWatch.Start();
        }

        private void Update()
        {
            finished = stopWatch.ElapsedMilliseconds >= Total;
        }
    }
}