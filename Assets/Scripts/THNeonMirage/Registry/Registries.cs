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
    public static class Registries
    {
        public static readonly List<Type> RegistryTypes = new();
        public static readonly Dictionary<string, List<RegistryKey>> RootKeys = new();
        public static readonly Dictionary<RegistryKey, RegistryEntry> RegistryKeys = new();
        public static readonly Dictionary<RegistryEntry, GameObject> Entry2ObjMap = new();

        public static readonly Dictionary<GamePanel, GameObject> Panels = new();
        public static readonly Dictionary<GameButton, GameObject> Buttons = new();
        
        public static readonly Dictionary<FieldTile, GameObject> Tiles = new();

        public static RegistryKey Register(string rootName, string registryName)
        {
            var key = RegistryKey.Create(rootName, registryName);
            RootKeys.GetValueOrDefault(rootName, new List<RegistryKey>()).Add(key);
            return key;
        }

        public static RegistryEntry GetRegistry(RegistryKey registryKey)
        {
            return RegistryKeys[registryKey];
        }
        
        public static TEntry Get<TEntry>(RegistryKey registryKey) where TEntry : RegistryEntry
        {
            return (TEntry)RegistryKeys[registryKey];
        }

        public static TComponent GetComponent<TEntry, TComponent>(RegistryKey registryKey) where TEntry : RegistryEntry where TComponent : Component
        {
            return ((TEntry)RegistryKeys[registryKey]).GameObject().GetComponent<TComponent>();
        }
    }
}