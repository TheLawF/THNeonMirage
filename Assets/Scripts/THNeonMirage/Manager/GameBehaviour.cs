using System;
using THNeonMirage.Event;
using THNeonMirage.Map;
using Unity.VisualScripting;
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