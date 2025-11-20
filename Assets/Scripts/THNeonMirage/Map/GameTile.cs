using THNeonMirage.Registry;
using UnityEngine;

namespace THNeonMirage.Map
{
    public class GameTile<T> where T : FieldTile
    {
        public T tileComponent;
        public GameObject tilePrefab;

        public GameTile(T tileComponent, GameObject tilePrefab)
        {
            this.tileComponent = tileComponent;
            this.tilePrefab = tilePrefab;
        }
    }
}