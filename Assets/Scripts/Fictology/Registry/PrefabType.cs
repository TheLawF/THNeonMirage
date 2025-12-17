using Fictology.Registry;
using Photon.Pun;
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
            Registries.RegisterPrefabInstance(this, instance);
            return instance;
        }

        public GameObject NetworkInstantiate(Vector3 pos, Quaternion rot)
        {
            var netInstance = PhotonNetwork.Instantiate(PrefabPath, pos, rot);
            Registries.RegisterPrefabInstance(this, netInstance);
            return netInstance;
        }
        
        public GameObject Instantiate(Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation);
            Registries.RegisterPrefabInstance(this, instance);
            return instance;
        }
        
        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform transform)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation, transform);
            Registries.RegisterPrefabInstance(this, instance);
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
            Registries.RegisterPrefabInstance(this, instance);
            return instance;
        }

        public GameObject Instantiate(RegistryEntry prefabEntry, Vector3 position, Quaternion rotation)
        {
            var instance = Object.Instantiate((GameObject)Resources.Load(PrefabPath), position, rotation);
            instance.AddComponent<RegistryEntry>();
            instance.GetComponent<RegistryEntry>().registryKey = prefabEntry.registryKey;
            Registries.RegisterPrefabInstance(this, instance);
            return instance;
        }
    }
}