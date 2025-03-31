using THNeonMirage.Data;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using Unity.Mathematics;

namespace THNeonMirage.Map
{
    public class BumperCar : FieldTile
    {
        private void Start()
        {
            Init();
            Player.PlayerData.OnPositionChanged += OnPlayerStop;
        }
        public override void OnPlayerStop(object playerData, ValueEventArgs currentPos)
        {
            var data = (PlayerData)playerData;
            data.PauseCount = Random.NextBool() ? Random.NextInt(-2, 0) : Random.NextInt(1, 3);
        }
    }
}