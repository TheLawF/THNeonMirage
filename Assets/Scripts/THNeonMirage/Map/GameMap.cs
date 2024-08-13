using System;
using System.Collections.Generic;
using System.Linq;
using THNeonMirage.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace THNeonMirage.Map
{
    [Serializable]
    public class GameMap : MonoBehaviour
    {
        public GameObject playerPrefab;
        public List<MapField> fields;
        public List<Player> players;
        public List<GameObject> playerObj;

        private void Start()
        {
            
        }

        private void Update()
        {
        }

        public int GetPlayerCountOn(int fieldId)
            => players.Count(player => player.Position == fieldId);

        public static event Action<GameObject, GameObject> OnUserLogin;
        public static void CreateAvatar(GameObject playerPrefab, GameObject mapObj)
        {
            mapObj.GetComponent<GameMap>().playerObj.Add(Instantiate(playerPrefab));
            OnUserLogin?.Invoke(playerPrefab, mapObj);
        }
    }
}
