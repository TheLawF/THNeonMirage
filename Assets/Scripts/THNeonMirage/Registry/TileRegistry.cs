using THNeonMirage.Map;
using UnityEngine;

namespace THNeonMirage.Registry
{
    public class TileRegistry
    {
        // ECS 架构：
        // Entity -> Hierarchy面板的游戏物体，每一个Entity都是"唯一的"实例
        // Component -> 游戏物体挂载的组件和数据，不同的Entity可能挂载不同的组件，挂载相同组件的Entity可被视为“一类”物体
        // System -> 游戏物体之间的互动
        // 所以，TileRegistry解决的是 E 和 C 的挂载与互动，是单例模式
        // ECS -> ECTS 架构，加入了类型组件，所有游戏物体必须添加类型组件
        public static void RegisterTiles(RegistryEntry entry, GameObject gameObject)
        {
            if (entry is FieldTile tile) Registries.Tiles.Add(tile, gameObject);
        }
    }
}