using Fictology.Registry;
using THNeonMirage.Registry;
using UnityEngine;

namespace Fictology.Registry
{
    public class PrefabType<TEntry, TComponent> where TEntry : RegistryEntry where TComponent : Component
    {
        public string PrefabPath;
        public TComponent[] PrefabComponents;

        public static PrefabType<TEntry, TComponent> Of(string prefabPath)
        {
            return new PrefabType<TEntry, TComponent>(prefabPath);
        }

        private PrefabType(string prefabPath)
        {
            PrefabPath = prefabPath;
        }

        public GameObject Instantiate(RegistryEntry prefabEntry)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath));
            instance.AddComponent<TEntry>();
            instance.GetComponent<TEntry>().registryKey = prefabEntry.registryKey;
            return instance;
        }
        public GameObject Instantiate(RegistryEntry prefabEntry, Transform parent)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), parent);
            instance.AddComponent<TEntry>();
            instance.GetComponent<TEntry>().registryKey = prefabEntry.registryKey;
            return instance;
        }

        public GameObject Instantiate(RegistryEntry prefabEntry, Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation);
            instance.AddComponent<TEntry>();
            instance.GetComponent<TEntry>().registryKey = prefabEntry.registryKey;
            return instance;
        }
    }
}