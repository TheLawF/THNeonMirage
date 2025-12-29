using System;
using THNeonMirage.Map;
using UnityEngine;

namespace THNeonMirage.Manager
{
    public class GameBehaviour: MonoBehaviour
    {
        public GameObject mapObject;
        protected Level Level;
        public event Func<GameObject, Vector3, Quaternion, GameObject> OnInstantiate;

        public GameObject Initialize(GameObject obj, Vector3 pos, Quaternion rotation)
        {
            OnInstantiate?.Invoke(obj, pos, rotation);
            return Instantiate(obj, pos, rotation);
        }
    }
}