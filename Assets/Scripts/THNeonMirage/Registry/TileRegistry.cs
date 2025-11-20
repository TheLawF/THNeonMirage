using THNeonMirage.Map;
using UnityEngine;

namespace THNeonMirage.Registry
{
    
    public class TileRegistry
    {
        public static readonly string TileLoaderRootKey = "TileLoader";
        public static readonly string TileRootKey = "Tile";
        public static readonly RegistryKey TileLoader = RegistryKey.Create(TileLoaderRootKey, "Loader");
        public static void RegisterTiles(RegistryEntry entry, GameObject gameObject)
        {
            if (entry is FieldTile tile) Registries.Tiles.Add(tile, gameObject);
        }
    }
}