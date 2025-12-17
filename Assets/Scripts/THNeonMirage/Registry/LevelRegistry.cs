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
        public static readonly RegistryKey ClientLevel = Registries.CreateKey(LevelRootKey, "ClientLevel");
        public static readonly RegistryKey ServerLevel = Registries.CreateKey(LevelRootKey, nameof(ServerLevel));
        public static readonly RegistryKey Tile = Registries.CreateKey(TileRootKey, "Tile");

        public static void RegisterTypes()
        {
            Registries.RegistryTypes.Add(typeof(Level));
            Registries.RegistryTypes.Add(typeof(PrefabType));
            Registries.RegistryTypes.Add(typeof(PlayerManager));
        }
        

    }
}