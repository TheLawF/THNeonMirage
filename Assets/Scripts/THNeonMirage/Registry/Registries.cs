using System;
using System.Collections.Generic;
using System.Linq;
using FlyRabbit.EventCenter;
using THNeonMirage.Map;
using THNeonMirage.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace THNeonMirage.Registry
{
    /// <summary>
    /// 使用单例模式管理所有游戏物体，别问我为什么，问就是写MC模组写的<br></br>
    /// 对于唯一的游戏物体，例如已经添加在Hierarchy面板上的游戏物体，采取直接注册的方式
    /// 对于预制体和使用脚本创建和实例化的游戏物体，采取
    /// </summary>
    public static class Registries
    {
        public static readonly List<Type> RegistryTypes = new();
        public static readonly Dictionary<string, List<RegistryKey>> RootKeys = new();
        private static readonly Dictionary<string, GameObject> Key2ObjectMap = new();
        private static readonly Dictionary<string, RegistryEntry> Key2EntryMap = new();
        public static readonly Dictionary<RegistryEntry, GameObject> Entry2ObjMap = new();

        public static readonly Dictionary<GamePanel, GameObject> Panels = new();
        public static readonly Dictionary<GameButton, GameObject> Buttons = new();
        
        public static readonly Dictionary<FieldTile, GameObject> Tiles = new();

        public static RegistryKey CreateKey(string rootName, string registryName)
        {
            var key = RegistryKey.Create(rootName, registryName);
            RootKeys.GetValueOrDefault(rootName, new List<RegistryKey>()).Add(key);
            return key;
        }

        public static void Register<TEntry>(RegistryKey key, TEntry entry) where TEntry : RegistryEntry
        {
            if (!RootKeys.ContainsKey(key.rootName)) return;
            Key2EntryMap.TryAdd(key.ToString(), entry);
        }

        public static void RegisterObject(RegistryKey key, GameObject gameObject)
        {
            if (!RootKeys.ContainsKey(key.rootName)) return;
            Key2ObjectMap.TryAdd(key.ToString(), gameObject);
        }

        public static void RegisterAll(Dictionary<RegistryEntry, GameObject> dictionary)
        {
            Entry2ObjMap.AddRange(dictionary);
            Key2ObjectMap.AddRange(dictionary
                .Select(pair => new {Key = pair.Key.registryKey.ToString(), Obj = pair.Value})
                .ToDictionary(pair => pair.Key, pair => pair.Obj));
        }

        public static TEntry Get<TEntry>(RegistryKey registryKey) where TEntry : RegistryEntry
        {
            return (TEntry)Key2EntryMap[registryKey.ToString()];
        }

        public static GameObject GetObject(RegistryKey registryKey)
        {
            return Key2ObjectMap[registryKey.ToString()];
        }

        public static TComponent GetComponent<TComponent>(RegistryKey registryKey) where TComponent : Component
        {
            return Key2ObjectMap[registryKey.ToString()].GetComponent<TComponent>();
        }
    }
}