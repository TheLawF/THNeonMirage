using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExitGames.Client.Photon.StructWrapping;
using Fictology.Registry;
using FlyRabbit.EventCenter;
using FlyRabbit.EventCenter.Core;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using KeyValuePair = System.Collections.Generic.KeyValuePair;

namespace THNeonMirage
{
    public class GameMain : MonoBehaviour
    {
        public Button startButton;
        public Button aboutButton;
        public Level level;
        public List<PlayerManager> players;

        /// <summary>
        /// GameStart Clicked -> Disable Home Panel -> Enable Background -> Create Map -> Game Loop
        /// </summary>
        private void Start()
        {
            RegisterWhenSceneStart();
            InitAllFields();
            RegisterUIListeners();
            CreateEventListeningChain();
        }


        private void RegisterWhenSceneStart()
        {
            UIRegistry.RegisterTypes();
            LevelRegistry.RegisterTypes();
            
            var validEntries = GetAllSceneObjects()
                .Select(obj => new { Object = obj, Entry = obj.GetComponent<RegistryEntry>() })
                .Where(objAndEntry => objAndEntry.Entry != null && Registries.RegistryTypes.Contains(objAndEntry.Entry.GetType()))
                .ToDictionary(objAndEntry => objAndEntry.Entry, objAndEntry => objAndEntry.Object);

            Registries.RegisterAll(validEntries);
        }

        private void InitAllFields()
        {
            startButton = Registries.GetComponent<Button>(UIRegistry.StartButton);
            aboutButton = Registries.GetComponent<Button>(UIRegistry.AboutButton);
            level = Registries.GetComponent<Level>(LevelRegistry.Level);
        }

        private void RegisterUIListeners()
        {
            startButton.onClick.AddListener(OnGameStartClicked);
        }
        
        public void CreateEventListeningChain()
        {
            EventCenter.AddListener<int, int>(EventRegistry.OnBalanceChanged, SetBalanceDisplay);
        }
        
        public void OnGameStartClicked()
        {
            Registries.GetObject(UIRegistry.HomePage).SetActive(false);
            Registries.Tiles.Values.ToList().ForEach(go => go.SetActive(true));
            level.CreateLevel();
        }

        public void CreatePlayer(bool isBot)
        {
            LevelRegistry.Player.Instantiate();
        }

        public void SetBalanceDisplay(int before, int after)
        {
            
        }

        public void TriggerTileEvent(int oldPos, int newPos)
        {
            EventCenter.TriggerEvent(EventRegistry.OnPositionChanged, oldPos, newPos);
        }

        public static List<GameObject> GetAllSceneObjects(bool includeInactive = true)
        {
            List<GameObject> allObjects = new List<GameObject>();
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
    
            foreach (var root in rootObjects)
            {
                // 如果includeInactive为false，且根物体被禁用，则跳过
                if (!includeInactive && !root.activeInHierarchy) continue;
        
                allObjects.Add(root);
                GetChildrenRecursive(root.transform, allObjects, includeInactive);
            }
    
            return allObjects;
        }

        private static void GetChildrenRecursive(Transform parent, List<GameObject> objectList, bool includeInactive)
        {
            foreach (Transform child in parent)
            {
                if (includeInactive || child.gameObject.activeInHierarchy)
                {
                    objectList.Add(child.gameObject);
                    GetChildrenRecursive(child, objectList, includeInactive);
                }
            }
        }
    }
}