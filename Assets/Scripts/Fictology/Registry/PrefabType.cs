using Fictology.Registry;
using THNeonMirage.Registry;
using UnityEngine;

namespace Fictology.Registry
{
    public class PrefabType<TEntry, TComponent> where TEntry : RegistryEntry where TComponent : Component
    {
        public string PrefabPath;
        public TEntry PrefabEntry;
        public TComponent[] PrefabComponents;

        public static PrefabType<TEntry, TComponent> Of(string prefabPath, TEntry prefabEntry)
        {
            return new PrefabType<TEntry, TComponent>(prefabPath, prefabEntry);
        }
        
        public static PrefabType<TEntry, TComponent> Of(string prefabPath)
        {
            return new PrefabType<TEntry, TComponent>(prefabPath);
        }

        public PrefabType(string prefabPath)
        {
            PrefabPath = prefabPath;
        }

        public PrefabType(string prefabPath, TEntry prefabEntry)
        {
            PrefabPath = prefabPath;
            PrefabEntry = prefabEntry;
        }

        public GameObject Instantiate()
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath));
            if (PrefabEntry == null) return instance;
            instance.AddComponent<TEntry>();
            instance.GetComponent<TEntry>().registryKey = PrefabEntry.registryKey;
            return instance;
        }
        public GameObject Instantiate(Transform parent)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), parent);
            if (PrefabEntry == null) return instance;
            instance.AddComponent<TEntry>();
            instance.GetComponent<TEntry>().registryKey = PrefabEntry.registryKey;
            return instance;
        }

        public GameObject Instantiate(Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation);
            if (PrefabEntry == null) return instance;
            instance.AddComponent<TEntry>();
            instance.GetComponent<TEntry>().registryKey = PrefabEntry.registryKey;
            return instance;
        }
    }
}