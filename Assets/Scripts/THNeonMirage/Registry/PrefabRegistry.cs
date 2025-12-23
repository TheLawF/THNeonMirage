using Fictology.Registry;

namespace THNeonMirage.Registry
{
    public class PrefabRegistry
    {
        public static readonly PrefabType RawImageSprite = Registries.CreateType("Prefabs/RawImagePrefab");
        public static readonly PrefabType IndexLabel = Registries.CreateType("Prefabs/IndexLabel");

        public static readonly PrefabType BackgroundLabel = Registries.CreateType("Prefabs/TextBackground");
        public static readonly PrefabType JoinedPlayer = Registries.CreateType("Prefabs/Table");
        public static readonly PrefabType TilePrefab = Registries.CreateType("Prefabs/TilePrefab");
        public static readonly PrefabType Player = Registries.CreateType("Prefabs/PlayerObject");
    }
}