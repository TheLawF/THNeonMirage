using System;
using System.Linq;
using Fictology.Data.Serialization;
using Fictology.Registry;
using Fictology.Util;
using Photon.Pun;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THNeonMirage.UI
{
    public class RoomManager: RegistryEntry, IPunObservable
    {
        private const int Wait = 0;
        private const int Select = 1;
        private GameObject local_instance;

        public int readyPlayers;
        public int selectedPlayers;
        private int select_index;
        private bool localPlayerisReady;

        private GameObject local_avatar;
        private GameObject remote1_avatar;
        private GameObject remote2_avatar;
        private GameObject remote3_avatar;
        private Either<IntData> m_waitOrSelect = Either<IntData>.Or(0, 1);
        
        private GameObject m_ready;
        private GameObject player_list;
        private GameObject avatar_list;
        
        private GameObject _up;
        private GameObject _down;
        private GameObject _lock;

        private GameObject local;
        private GameObject remote1;
        private GameObject remote2;
        private GameObject remote3;

        private int localId;
        private int remote1Id;
        private int remote2Id;
        private int remote3Id;

        private void OnEnable()
        {
            local = Registries.GetObject(UIRegistry.LocalAvatar);
            remote1 = Registries.GetObject(UIRegistry.Remote1);
            remote2 = Registries.GetObject(UIRegistry.Remote2);
            remote3 = Registries.GetObject(UIRegistry.Remote3);
            
            m_ready = Registries.GetObject(UIRegistry.ReadyButton);
            player_list = Registries.GetObject(UIRegistry.PlayerList);
            avatar_list = Registries.GetObject(UIRegistry.AvatarList);
            
            _up = Registries.GetObject(UIRegistry.UpButton);
            _down = Registries.GetObject(UIRegistry.DownButton);
            _lock = Registries.GetObject(UIRegistry.LockSelection);
            
            m_ready.GetComponent<Button>().onClick.AddListener(SendReadyToRemote);
            _lock.GetComponent<Button>().onClick.AddListener(OnLockSelection);
        }
        
        [PunRPC]
        public void AddNewPlayerToRoomList(int viewId)
        {
            var childCount = player_list.transform.childCount;
            local_instance = PrefabRegistry.JoinedPlayer.NetworkInstantiate(Vector3.zero, Quaternion.identity, player_list.transform);
            var itemHeight = local_instance.GetComponent<RectTransform>().rect.height;
            
            player_list.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  10 + childCount * itemHeight);
            local_instance.GetComponentInChildren<TextMeshPro>().text = Utils.NextRandomString(10, UnicodeTable.Characters);
        }

        private void SendReadyToRemote()
        {
            // TODO: Call OnPhotonSerializeView
            localPlayerisReady = true;
            readyPlayers++;
            if (readyPlayers != PhotonNetwork.PlayerList.Length) return;
            
            m_waitOrSelect.SwitchToAnother();
            player_list.SetActive(false);
            avatar_list.SetActive(true);
            
            local_instance.GetPhotonView().RPC(nameof(ReceiveReadyFromRemote), RpcTarget.Others, local_instance.GetPhotonView().ViewID);
        }

        private void ReceiveReadyFromRemote(int viewId)
        {
            var views = player_list.GetComponentsInChildren<PhotonView>();
            var obj = views.First(view => view.ViewID == viewId).gameObject;
            readyPlayers++;
        }

        private void InitAvatar()
        {
            local_avatar = PrefabRegistry.RawImageSprite.NetworkInstantiate(local.transform.position, Quaternion.identity, local.transform);
            remote1_avatar = PrefabRegistry.RawImageSprite.NetworkInstantiate(remote1.transform.position,
                Quaternion.identity, remote1.transform);
            remote2_avatar = PrefabRegistry.RawImageSprite.NetworkInstantiate(remote2.transform.position,
                Quaternion.identity, remote2.transform);
            remote3_avatar = PrefabRegistry.RawImageSprite.NetworkInstantiate(remote3.transform.position,
                Quaternion.identity, remote3.transform);
        }

        private void OnClickAvatarSprite()
        {
            
        }

        private void ReceiveAvatarUpdate()
        {
            
        }

        private void OnLockSelection()
        {
            // TODO: Call OnPhotonSerializeView
            selectedPlayers++;
            if (selectedPlayers != PhotonNetwork.PlayerList.Length) return;
            
            var room = Registries.GetObject(UIRegistry.RoomWindow);
            var level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            room.SetActive(false);
            level.CreateLevel();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(readyPlayers);
                stream.SendNext(selectedPlayers);
            }
            else
            {
                
            }
        }
    }
}