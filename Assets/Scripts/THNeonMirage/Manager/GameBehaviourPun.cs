using System;
using Photon.Pun;
using THNeonMirage.Map;
using UnityEngine;

namespace THNeonMirage.Manager
{
    public class GameBehaviourPun : MonoBehaviourPun
    {
        public GameObject mapObject;
        protected GameMap GameMap;
        
        public event Func<string, Vector3, Quaternion, GameObject> OnInstantiate;

        public GameObject Initialize(string prefabName, Vector3 pos, Quaternion rotation)
        {
            OnInstantiate?.Invoke(prefabName, pos, rotation);
            return PhotonNetwork.Instantiate(prefabName, pos, rotation);
        }
    }
}