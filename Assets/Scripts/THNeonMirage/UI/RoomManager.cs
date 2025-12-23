using System;
using System.Collections.Generic;
using System.Linq;
using Fictology.Data.Serialization;
using Fictology.Registry;
using Fictology.Util;
using Photon.Pun;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THNeonMirage.UI
{
    public class RoomManager: RegistryEntry
    {
        public int readyPlayers;
        public int selectedPlayers;
        private int select_index;
        private bool localPlayerisReady;
        
        private GameObject local_avatar;
        private GameObject avatar_list;
        private GameObject _lock;

        private GameObject local;
        private GameObject remote1;
        private GameObject remote2;
        private GameObject remote3;
        
        public List<GameObject> remotes = new();
        public List<AvatarManager> avatars = new();

        private void OnEnable()
        {
            local = Registries.GetObject(UIRegistry.LocalAvatar);
            remote1 = Registries.GetObject(UIRegistry.Remote1);
            remote2 = Registries.GetObject(UIRegistry.Remote2);
            remote3 = Registries.GetObject(UIRegistry.Remote3);
            
            remotes.Add(remote1);
            remotes.Add(remote2);
            remotes.Add(remote3);
            
            avatar_list = Registries.GetObject(UIRegistry.AvatarList);
            avatars.AddRange(avatar_list.GetComponentsInChildren<AvatarManager>());
            
            _lock = Registries.GetObject(UIRegistry.LockSelection);
            _lock.GetComponent<Button>().onClick.AddListener(LockSelectionAndSendReady);
        }
        
        // 本地玩家加入时更改本地的层级布局
        public void CreateAvatarWhenJoinIn()
        {
            if (HasRemotePlayerUnder(local))
            {
                remotes.ForEach(obj =>
                {
                    if (HasRemotePlayerUnder(obj)) return;
                    var prevJoinedPlayer = local.GetComponentInChildren<Transform>();
                    prevJoinedPlayer.parent = obj.transform;
                });
            }

            local_avatar = PrefabRegistry.RawImageSprite.NetworkInstantiate(Vector3.zero, Quaternion.identity, local.transform);
            local_avatar.GetComponent<AvatarManager>().SendPlayerJoinEvent();
            avatars.ForEach(manager => manager.localAvatar = local_avatar);
            
            var rect = local_avatar.GetComponent<RectTransform>();
            rect.position = local.GetComponent<RectTransform>().rect.position;
        }

        private void LockSelectionAndSendReady()
        {
            localPlayerisReady = true;
            readyPlayers++;
            if (readyPlayers != PhotonNetwork.PlayerList.Length) return;
            
            avatar_list.SetActive(true);
            avatars.ForEach(manager => manager.selectable = false);
            local_avatar.GetComponent<AvatarManager>().SendLockSelectionAndReady();
        }

        [PunRPC]
        private void ReceiveReadyFromRemote()
        {
            readyPlayers++;
        }

        public bool HasRemotePlayerUnder(GameObject emptyParent) => emptyParent.transform.childCount > 0;
        
    }
}