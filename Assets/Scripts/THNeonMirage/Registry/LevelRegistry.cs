using Fictology.Registry;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using UnityEngine;

namespace THNeonMirage.Registry
{
    public class LevelRegistry
    {
        public const string LevelRootKey = "Level";
        public static readonly string TileRootKey = "Tile";
        public static readonly RegistryKey Level = RegistryKey.Create(LevelRootKey, "MainLevel");
        public static readonly PrefabType<FieldTile, MonoBehaviour> Tile = PrefabType<FieldTile, MonoBehaviour>.Of("Prefabs/TilePrefab");

        public static void RegisterLevel(RegistryKey key, LevelManager level)
        {
            Registries.Register(key, level);
        }
    }
}