using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Fictology.Registry;
using Photon.Pun;
using THNeonMirage.Map;
using THNeonMirage.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace THNeonMirage.Registry
{
    /// <summary>
    /// 使用单例模式管理所有游戏物体，别问我为什么，问就是写MC模组写的<br></br>
    /// 对于唯一的游戏物体，例如已经添加在Hierarchy面板上的游戏物体，采取直接注册的方式
    /// 对于预制体和使用脚本创建和实例化的游戏物体，采取
    /// </summary>
    public class Registries
    {
        public static readonly List<Type> RegistryTypes = new();
        public static readonly ConcurrentDictionary<int, List<GameObject>> NetworkInstances = new();
        
        public static readonly Dictionary<string, List<RegistryKey>> RootKeys = new();
        private static readonly Dictionary<string, GameObject> Key2ObjectMap = new();
        private static readonly Dictionary<string, RegistryEntry> Key2EntryMap = new();
        public static readonly Dictionary<PrefabType, List<GameObject>> Prefab2InstancesMap = new();

        public static readonly Dictionary<GamePanel, GameObject> Panels = new();
        public static readonly Dictionary<GameButton, GameObject> Buttons = new();
        
        public static readonly Dictionary<FieldTile, GameObject> Tiles = new();

        private static Registries m_instance;
        public static Registries Instance => m_instance ??= new Registries();

        public void RegisterNetworkInstances(PhotonView view, GameObject gameObject)
        {
            var containsId = NetworkInstances.ContainsKey(view.ViewID);
            if (containsId) NetworkInstances[view.ViewID].Add(gameObject);
            else NetworkInstances.TryAdd(view.ViewID, new List<GameObject>());
        }

        public static RegistryKey CreateKey(string rootName, string registryName)
        {
            var key = RegistryKey.Create(rootName, registryName);
            RootKeys.GetValueOrDefault(rootName, new List<RegistryKey>()).Add(key);
            return key;
        }

        public static PrefabType CreateType(string prefabPath)
        {
            var prefabType = PrefabType.Of(prefabPath);
            Prefab2InstancesMap.TryAdd(prefabType, new List<GameObject>());
            return prefabType;
        }

        public static void Register<TEntry>(RegistryKey key, TEntry entry) where TEntry : RegistryEntry
        {
            if (!RootKeys.ContainsKey(key.rootName)) return;
            Key2EntryMap.TryAdd(key.ToString(), entry);
        }

        public static void RegisterPrefabInstance(PrefabType prefabType, GameObject gameObject)
        {
            Prefab2InstancesMap[prefabType].Add(gameObject);
        }
        
        public static List<GameObject> GetPrefabInstances(PrefabType prefabType)
        {
            return Prefab2InstancesMap[prefabType];
        }

        public static void RegisterAll(Dictionary<RegistryEntry, GameObject> dictionary)
        {
            Key2ObjectMap.AddRange(dictionary
                .Select(pair => new {Key = pair.Key.registryKey.ToString(), Obj = pair.Value})
                .ToDictionary(keyAndObj => keyAndObj.Key, keyAndObj => keyAndObj.Obj));
            Key2EntryMap.AddRange(dictionary
                .Select(pair => new {Key = pair.Key.registryKey.ToString(), Entry = pair.Key})
                .ToDictionary(keyAndEntry => keyAndEntry.Key, keyAndEntry => keyAndEntry.Entry));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registryKey">RegistryEntry 组件注册名</param>
        /// <typeparam name="TEntry">RegistryEntry 组件类型</typeparam>
        /// <returns>挂载的 RegistryEntry 组件实例</returns>
        public static TEntry Get<TEntry>(RegistryKey registryKey) where TEntry : RegistryEntry
        {
            return (TEntry)Key2EntryMap[registryKey.ToString()];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registryKey">注册名</param>
        /// <returns>挂载了 RegistryEntry 组件实例的游戏物体</returns>
        public static GameObject GetObject(RegistryKey registryKey)
        {
            return Key2ObjectMap[registryKey.ToString()];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registryKey"></param>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns>挂载了 RegistryEntry 组件实例的游戏物体上的其它脚本组件</returns>
        public static TComponent GetComponent<TComponent>(RegistryKey registryKey) where TComponent : Component
        {
            return Key2ObjectMap[registryKey.ToString()].GetComponent<TComponent>();
        }

        public static bool IsSameRegistryType(RegistryEntry left, RegistryEntry right)
        {
            return left.registryKey == right.registryKey;
        }
        public static bool IsSameRegistryType(GameObject left, GameObject right)
        {
            return left.GetComponent<RegistryEntry>() != null && 
                   right.GetComponent<RegistryEntry>() != null && 
                   left.GetComponent<RegistryEntry>().registryKey == right.GetComponent<RegistryEntry>().registryKey;
        }
    }
}