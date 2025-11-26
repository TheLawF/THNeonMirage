using System;
using System.Collections.Generic;
using Fictology.Registry;
using THNeonMirage.UI;
using THNeonMirage.Registry;
using TMPro;
using UnityEngine;

namespace THNeonMirage.Registry
{
    public static class UIRegistry
    {
        public const string LabelRootKey = "Label";
        public const string PanelRootKey = "Panel";
        public const string ButtonRootKey = "Button";
        public const string CanvasKey = "Canvas";

        public static readonly RegistryKey Canvas = Registries.CreateKey(CanvasKey, "Main");
        public static readonly RegistryKey TileName = Registries.CreateKey(LabelRootKey, "TileName");
        public static readonly RegistryKey BalanceText = Registries.CreateKey(LabelRootKey, "BalanceText");
        public static readonly RegistryKey CountdownText = Registries.CreateKey(LabelRootKey, "CountdownText");

        public static readonly RegistryKey DescriptionText = Registries.CreateKey(LabelRootKey, "DescriptionText");
        public static readonly RegistryKey PurchaseText = Registries.CreateKey(LabelRootKey, nameof(PurchaseText));
        public static readonly RegistryKey TollText = Registries.CreateKey(LabelRootKey, "TollText");
        public static readonly RegistryKey RoomIdText = Registries.CreateKey(LabelRootKey, nameof(RoomIdText));

        public static readonly RegistryKey HostLauncher = Registries.CreateKey(PanelRootKey, "Launcher");
        public static readonly RegistryKey LobbyPanel = Registries.CreateKey(PanelRootKey, "Lobby");
        public static readonly RegistryKey CreateRoomDialogue = Registries.CreateKey(PanelRootKey, "CreateRoom");
        public static readonly RegistryKey InputRoomIdDialogue = Registries.CreateKey(PanelRootKey, "InputRoomId");
        
        public static readonly RegistryKey RoomPage = Registries.CreateKey(PanelRootKey, nameof(RoomPage));
        public static readonly RegistryKey HomePage = Registries.CreateKey(PanelRootKey, nameof(HomePage));
        public static readonly RegistryKey InGamePanel = Registries.CreateKey(PanelRootKey, "InGame");
        public static readonly RegistryKey HUD = Registries.CreateKey(PanelRootKey, "HUD");

        public static readonly RegistryKey RegisterButton = Registries.CreateKey(ButtonRootKey, "Register");
        public static readonly RegistryKey LoginButton = Registries.CreateKey(ButtonRootKey, "Login");
        public static readonly RegistryKey AddRoomButton = Registries.CreateKey(ButtonRootKey, "AddRoom");
        public static readonly RegistryKey JoinRoomButton = Registries.CreateKey(ButtonRootKey, "JoinRoom");

        public static readonly RegistryKey StartButton = Registries.CreateKey(ButtonRootKey, "StartGame");
        public static readonly RegistryKey AboutButton = Registries.CreateKey(ButtonRootKey, "About");
        public static readonly RegistryKey DiceButton = Registries.CreateKey(ButtonRootKey, "Dice");
        
        public static readonly RegistryKey PurchaseButton = Registries.CreateKey(ButtonRootKey, "Purchase");
        public static readonly RegistryKey BuildingButton = Registries.CreateKey(ButtonRootKey, "Building");
        public static readonly RegistryKey CancelButton = Registries.CreateKey(ButtonRootKey, "Cancel");

        // public static readonly RegistryKey Room
        public static readonly PrefabType IndexLabel = Registries.CreateType("Prefabs/IndexLabel");

        public static void RegisterTypes()
        {
            Registries.RegistryTypes.Add(typeof(GameCanvas));
            Registries.RegistryTypes.Add(typeof(GamePanel));
            Registries.RegistryTypes.Add(typeof(GameButton));
            
            Registries.RegistryTypes.Add(typeof(TextInput));
            Registries.RegistryTypes.Add(typeof(TextLabel));
            Registries.RegistryTypes.Add(typeof(ProgressBarControl));
        }

    }
}