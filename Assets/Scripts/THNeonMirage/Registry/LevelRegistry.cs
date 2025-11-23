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
        public const string PlayerRootKey = "Player";
        public static readonly RegistryKey Level = Registries.CreateKey(LevelRootKey, "MainLevel");
        public static readonly RegistryKey Tile = Registries.CreateKey(TileRootKey, "Tile");
        public static readonly PrefabType Player = Registries.CreateType("Prefabs/PlayerObject");
        public static readonly PrefabType TilePrefab = Registries.CreateType("Prefabs/TilePrefab");

        public static void RegisterTypes()
        {
            Registries.RegistryTypes.Add(typeof(Level));
            Registries.RegistryTypes.Add(typeof(PrefabType));
            Registries.RegistryTypes.Add(typeof(PlayerManager));
        }
        

    }
}