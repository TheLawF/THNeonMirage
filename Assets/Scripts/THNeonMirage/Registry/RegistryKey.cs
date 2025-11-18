using System;
using System.Collections.Generic;
using UnityEngine;

namespace THNeonMirage.Registry
{
    [Serializable]
    public class RegistryKey
    {
        public string rootName;
        public string registryName;

        public RegistryKey(string rootName, string registryName)
        {
            this.registryName = registryName;
        }

        public string GetRegistryName() => registryName;

        public static RegistryKey Create(string rootName, string registryName) => new (rootName, registryName);
    }
}