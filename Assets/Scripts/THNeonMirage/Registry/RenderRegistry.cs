using System.Collections.Generic;
using Fictology.Registry;
using UnityEngine;

namespace THNeonMirage.Registry
{
    public class RenderRegistry
    {
        
        public const string SpriteRootKey = "Sprite";
        public static readonly RegistryKey TileSprite = Registries.CreateKey(SpriteRootKey, "Tile");
        public static readonly RegistryKey PlayerSprite = Registries.CreateKey(SpriteRootKey, "Player");

        public static readonly List<Sprite> Avatars = new()
        {
            Resources.Load<Sprite>("Textures/flandre"),
            Resources.Load<Sprite>("Textures/cirno"),
            Resources.Load<Sprite>("Textures/reimu"),
            Resources.Load<Sprite>("Textures/marisa"),
            Resources.Load<Sprite>("Textures/sakuya"),
            Resources.Load<Sprite>("Textures/youmu")
        };
    }
}