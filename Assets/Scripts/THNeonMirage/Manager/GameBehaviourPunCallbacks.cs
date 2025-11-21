using System;
using Photon.Pun;
using THNeonMirage.Event;
using THNeonMirage.Map;
using UnityEngine;
using UnityEngine.Serialization;

namespace THNeonMirage.Manager
{
    public class GameBehaviourPunCallbacks : MonoBehaviourPunCallbacks
    {
        [FormerlySerializedAs("levelManager")] [FormerlySerializedAs("gameMap")] public Level level;
        public event Func<string, Vector3, Quaternion, IGameEventArgs, GameObject> OnInstantiate;

        protected virtual GameObject Initialize<TArgs>(string prefabName, Vector3 pos, Quaternion rotation, TArgs arg5)
        where TArgs : IGameEventArgs
        {
            var instance = PhotonNetwork.Instantiate(prefabName, pos, rotation);
            OnInstantiate?.Invoke(prefabName, pos, rotation, arg5);
            return instance;
        }
    }
}