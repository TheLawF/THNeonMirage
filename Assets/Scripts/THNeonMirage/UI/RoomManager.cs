using System;
using Fictology.Data.Serialization;
using Fictology.Registry;
using Fictology.Util;
using Photon.Pun;
using THNeonMirage.Registry;
using UnityEngine;

namespace THNeonMirage.UI
{
    public class RoomManager: RegistryEntry, IPunObservable
    {
        public int readyPlayers;
        private const int Wait = 0;
        private const int Select = 1;
        private Either<IntData, IntData> m_waitOrSelect = Either<IntData, IntData>.Or(0, 1);

        private GameObject avatar_list;
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
            avatar_list = Registries.GetObject(UIRegistry.AvatarList);
        }

        [PunRPC]
        public void AddNewPlayerToRoomList(int viewId)
        {
            var childCount = avatar_list.transform.childCount;
            var item = PrefabRegistry.Avatar.NetworkInstantiate(Vector3.zero, Quaternion.identity);
            var itemHeight = item.GetComponent<RectTransform>().rect.height;
            avatar_list.GetComponent<RectTransform>().sizeDelta = new Vector2(0,  10 + childCount * itemHeight);
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