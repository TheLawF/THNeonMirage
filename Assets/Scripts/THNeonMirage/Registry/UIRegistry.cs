using System;
using System.Collections.Generic;
using THNeonMirage.UI;
using THNeonMirage.Registry;
using UnityEngine;

namespace THNeonMirage.Registry
{
    public static class UIRegistry
    {
        public const string PanelRootKey = "Panel";
        public const string ButtonRootKey = "Button";
        
        public static readonly RegistryKey HomePage = Registries.Register(PanelRootKey, "HomePage");
        public static readonly RegistryKey InGamePanel = Registries.Register(PanelRootKey, "InGame");
        public static readonly RegistryKey HUD = Registries.Register(PanelRootKey, "HUD");
        
        public static readonly RegistryKey StartButton = Registries.Register(ButtonRootKey, "StartGame");
        public static readonly RegistryKey AboutButton = Registries.Register(ButtonRootKey, "About");
        public static void RegisterTypes()
        {
            Registries.RegistryTypes.Add(typeof(GamePanel));
            Registries.RegistryTypes.Add(typeof(GameButton));
        }

        public static void RegisterPanels(RegistryEntry registryEntry, GameObject gameObject)
        {
            if (registryEntry is GamePanel panel) Registries.Panels.Add(panel, gameObject);
            if (Registries.RootKeys.ContainsKey(registryEntry.registryKey.rootName))
                Registries.RegistryKeys.Add(registryEntry.registryKey, registryEntry);
            
        }
        public static void RegisterButtons(RegistryEntry registryEntry, GameObject gameObject)
        {
            if (registryEntry is GameButton button) Registries.Buttons.Add(button, gameObject);
            if (Registries.RootKeys.ContainsKey(registryEntry.registryKey.rootName))
                Registries.RegistryKeys.Add(registryEntry.registryKey, registryEntry);
        }

    }
}