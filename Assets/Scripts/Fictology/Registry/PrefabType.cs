using Fictology.Registry;
using THNeonMirage.Registry;
using UnityEngine;

namespace Fictology.Registry
{
    public class PrefabType
    {
        public const string RootKey = "Prefab";
        public string PrefabPath;
        public Component[] PrefabComponents;

        public static PrefabType Of(string prefabPath)
        {
            return new PrefabType(prefabPath);
        }

        private PrefabType(string prefabPath)
        {
            Registries.CreateKey(RootKey, prefabPath);
            PrefabPath = prefabPath;
        }

        public GameObject Instantiate()
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath));
            return instance;
        }
        
        public GameObject Instantiate(Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation);
            return instance;
        }
        
        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform transform)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation);
            return instance;
        }

        public GameObject Instantiate(RegistryEntry prefabEntry)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath));
            instance.AddComponent<RegistryEntry>();
            instance.GetComponent<RegistryEntry>().registryKey = prefabEntry.registryKey;
            Registries.RegisterPrefabInstance(this, instance);
            return instance;
        }
        public GameObject Instantiate(RegistryEntry prefabEntry, Transform parent)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), parent);
            instance.AddComponent<RegistryEntry>();
            instance.GetComponent<RegistryEntry>().registryKey = prefabEntry.registryKey;
            return instance;
        }

        public GameObject Instantiate(RegistryEntry prefabEntry, Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation);
            instance.AddComponent<RegistryEntry>();
            instance.GetComponent<RegistryEntry>().registryKey = prefabEntry.registryKey;
            return instance;
        }
    }
}