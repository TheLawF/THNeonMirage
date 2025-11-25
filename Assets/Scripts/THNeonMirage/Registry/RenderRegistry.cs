using Fictology.Registry;

namespace THNeonMirage.Registry
{
    public class RenderRegistry
    {
        
        public const string SpriteRootKey = "Sprite";
        public static readonly RegistryKey TileSprite = Registries.CreateKey(SpriteRootKey, "Tile");
        public static readonly RegistryKey PlayerSprite = Registries.CreateKey(SpriteRootKey, "Player");
    }
}