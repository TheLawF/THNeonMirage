using Fictology.Registry;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using UnityEngine;

namespace THNeonMirage.Registry
{
    public class LevelRegistry
    {
        public const string LevelRootKey = "Level";
        public const string TileRootKey = "Tile";
        public static readonly RegistryKey Level = RegistryKey.Create(LevelRootKey, "MainLevel");
        public static readonly PrefabType<PlayerManager, MonoBehaviour> Player = PrefabType<PlayerManager, MonoBehaviour>.Of("Prefabs/PlayerObject");
        public static readonly PrefabType<FieldTile, MonoBehaviour> Tile = PrefabType<FieldTile, MonoBehaviour>.Of("Prefabs/TilePrefab");

        public static void RegisterTypes()
        {
            Registries.RegistryTypes.Add(typeof(Level));
            Registries.RegistryTypes.Add(typeof(FieldTile));
        }
        
        public static void RegisterLevel(RegistryKey key, Level level)
        {
            Registries.Register(key, level);
        }
    }
}