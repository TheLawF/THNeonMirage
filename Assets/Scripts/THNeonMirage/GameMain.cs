using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExitGames.Client.Photon.StructWrapping;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using KeyValuePair = System.Collections.Generic.KeyValuePair;

namespace THNeonMirage
{
    public class GameMain : MonoBehaviour
    {
        
        /// <summary>
        /// GameStart Clicked -> Disable Home Panel -> Enable Background -> Create Map -> Game Loop
        /// </summary>
        private void Start()
        {
            RegisterWhenSceneStart();
        }

        private void Update()
        {
            WaitForOtherPlayer();
        }

        private void RegisterWhenSceneStart()
        {
            UIRegistry.RegisterTypes();
            
            var validEntries = GetAllSceneObjects()
                .Select(obj => new { Object = obj, Entry = obj.GetComponent<RegistryEntry>() })
                .Where(objAndEntry => objAndEntry.Entry != null && Registries.RegistryTypes.Contains(objAndEntry.Entry.GetType()))
                .ToDictionary(objAndEntry => objAndEntry.Entry, objAndEntry => objAndEntry.Object);
            
            Registries.Entry2ObjMap.AddRange(validEntries);
            foreach (var keyValuePair in Registries.Entry2ObjMap)
            {
                UIRegistry.RegisterPanels(keyValuePair.Key, keyValuePair.Value);
                UIRegistry.RegisterButtons(keyValuePair.Key, keyValuePair.Value);
            }
        }
        
        public void OnGameStartClicked()
        {
        }

        public void WaitForOtherPlayer()
        {
            
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