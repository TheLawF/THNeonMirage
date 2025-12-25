using System.Collections.Generic;
using System.Linq;
using Fictology.Registry;
using Photon.Pun;
using THNeonMirage.Manager;
using THNeonMirage.Registry;
using THNeonMirage.Util.Math;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace THNeonMirage.UI
{
    public class RoomManager: RegistryEntry
    {
        public int readyPlayers;
        public int selectedPlayers;
        private int select_index;
        private bool localPlayerisReady;
        
        private GameObject avatar_list;
        private GameObject _lock;

        public GameObject local;
        private GameObject remote1;
        private GameObject remote2;
        private GameObject remote3;

        public GameObject localAvatar;
        public List<GameObject> remotes = new();
        public List<GameObject> remoteAvatars;
        public List<AvatarManager> avatars = new();
        public List<PhotonView> players = new();

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
            var parent = DoesParentHasChild(local) ? remotes.First(o => !DoesParentHasChild(o)).transform : local.transform;
            if (localAvatar is not null)
            {
                var remoteAvatar = PrefabRegistry.RawImageSprite.NetworkInstantiate(parent.position, Quaternion.identity, parent);
                remoteAvatar.GetComponent<AvatarManager>().SendPlayerJoinEvent();
                remoteAvatar.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                remoteAvatars.Add(remoteAvatar);
                return;
            }
            localAvatar = PrefabRegistry.RawImageSprite.NetworkInstantiate(parent.position, Quaternion.identity, parent);
            localAvatar.GetComponent<AvatarManager>().SendPlayerJoinEvent();
            localAvatar.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

            GameObjectUtil.FillParentRect(localAvatar);
            avatars.ForEach(manager => manager.localAvatar = localAvatar);
        }

        private void LockSelectionAndSendReady()
        {
            localPlayerisReady = true;
            readyPlayers++;
            if (readyPlayers != PhotonNetwork.PlayerList.Length) return;
            
            avatar_list.SetActive(true);
            avatars.ForEach(manager => manager.selectable = false);
            localAvatar.GetComponent<AvatarManager>().SendLockSelectionAndReady();
        }

        [PunRPC]
        private void ReceiveReadyFromRemote()
        {
            readyPlayers++;
        }

        public bool DoesParentHasChild(GameObject emptyParent) => emptyParent.transform.childCount > 0;
        public bool ChildIsMine(GameObject emptyParent) => emptyParent.GetComponentInChildren<PhotonView>().IsMine;
    }
}