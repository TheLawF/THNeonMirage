using System;
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

        public int readyPlayers;
        public int selectedPlayers;
        private int select_index;
        private bool localPlayerisReady;
        private Either<IntData, IntData> m_waitOrSelect = Either<IntData, IntData>.Or(0, 1);
        
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
            var current = (IntData)m_waitOrSelect.Current;
            m_ready = Registries.GetObject(UIRegistry.ReadyButton);
            player_list = Registries.GetObject(UIRegistry.PlayerList);
            avatar_list = Registries.GetObject(UIRegistry.AvatarList);
            
            _up = Registries.GetObject(UIRegistry.UpButton);
            _down = Registries.GetObject(UIRegistry.DownButton);
            _lock = Registries.GetObject(UIRegistry.LockSelection);
            
            m_ready.GetComponent<Button>().onClick.AddListener(SetReadyAndSendToRemote);
            _lock.GetComponent<Button>().onClick.AddListener(OnLockSelection);
        }


        [PunRPC]
        public void AddNewPlayerToRoomList(int viewId)
        {
            var childCount = player_list.transform.childCount;
            var instance = PrefabRegistry.Avatar.NetworkInstantiate(Vector3.zero, Quaternion.identity);
            var itemHeight = instance.GetComponent<RectTransform>().rect.height;
            
            player_list.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  10 + childCount * itemHeight);
            instance.GetComponentInChildren<TextMeshPro>().text = Utils.NextRandomString(10, UnicodeTable.Characters);
        }

        private void SetReadyAndSendToRemote() 
        {
            // TODO: Call OnPhotonSerializeView
            localPlayerisReady = true;
            readyPlayers++;
            if (readyPlayers != PhotonNetwork.PlayerList.Length) return;
            
            m_waitOrSelect.SwitchToAnother();
            player_list.SetActive(false);
            avatar_list.SetActive(true);
        }

        private void OnClickAvatarSprite()
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
                
            }
            else
            {
                
            }
        }
    }
}