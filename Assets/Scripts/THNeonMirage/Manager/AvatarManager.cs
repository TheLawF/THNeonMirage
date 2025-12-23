using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util.Math;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class AvatarManager: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public string avatarName;
        public bool selectable = true;
        
        public Color spriteColor;
        public Outline outline;
        public GameObject localAvatar;
        public RoomManager room;

        private static readonly Color Unselected = new(0.6f, 0.6f, 0.6f, 1f);
        private static readonly Color Hovered = new(0.8f, 0.8f, 0.8f, 1f);
        public static readonly Color Selected = new(0.3f, 0.3f, 0.3f, 1f);

        private Canvas _canvas;
        private GameObject m_select_label;
        private Transform m_transform;
        
        private void OnEnable()
        {
            m_transform = transform;
            outline = gameObject.GetComponent<Outline>();
            spriteColor = gameObject.GetComponent<RawImage>().color;
            _canvas = Registries.GetComponent<Canvas>(UIRegistry.Canvas);
            room = Registries.Get<RoomManager>(UIRegistry.RoomWindow);
        }
        
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            
            var results = new List<RaycastResult>();
            var eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            EventSystem.current.RaycastAll(eventData, results);
            
            // 如果你只需要最顶层的UI元素，可以这样做：
            if (results.Count <= 0) return;
            var topmostUIElement = results[0].gameObject;
            Debug.Log("Topmost UI Element: " + topmostUIElement.name);

        }

        public void SendPlayerJoinEvent()
        {
            var view = gameObject.GetPhotonView();
            view.RPC(nameof(ReceivePlayerJoinEvent), RpcTarget.All, view.ViewID);
        }
        
        [PunRPC]
        public void ReceivePlayerJoinEvent(int viewId)
        {
            var newJoinedPlayerAvatar = PhotonView.Find(viewId).gameObject;
            var rawImage = newJoinedPlayerAvatar.GetComponent<RawImage>();
            room.remotes.ForEach(obj =>
            {
                if (room.HasRemotePlayerUnder(obj)) return;
                newJoinedPlayerAvatar.transform.parent = obj.transform;
                rawImage.texture = Resources.Load<Texture2D>("Textures/reimu");
                rawImage.uvRect = new Rect(1, 0.23f, 1, 0.7f);
            });
        }
        
        public void SendAvatarUpdate(string texturePath)
        {
            var view = gameObject.GetPhotonView();
            view.RPC(nameof(ReceiveAvatarUpdate), RpcTarget.Others, view.ViewID, texturePath);
        }
        
        [PunRPC]
        private void ReceiveAvatarUpdate(int viewId, string texturePath)
        {
            var obj = PhotonView.Find(viewId).gameObject;
            var img = obj.GetComponent<RawImage>();
            img.texture = Resources.Load<Texture2D>(texturePath);
        }

        public void SendLockSelectionAndReady() =>
            gameObject.GetPhotonView().RPC(nameof(ReceiveLockSelectionAndReady), RpcTarget.Others);

        [PunRPC]
        private void ReceiveLockSelectionAndReady()
        {
            room.readyPlayers++;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("左键单击了");
            if (!selectable) return;
            m_select_label = PrefabRegistry.BackgroundLabel.Instantiate(m_transform.position, Quaternion.identity, m_transform);
            m_select_label.GetComponent<TMP_Text>().text = $"Player{PhotonNetwork.LocalPlayer.ActorNumber}";
            spriteColor = Selected;
            localAvatar.GetPhotonView().RPC(nameof(ReceiveAvatarUpdate), RpcTarget.Others, 
                localAvatar.GetPhotonView().ViewID, avatarName);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("鼠标悬浮");
            if (!selectable)
            {
                outline.enabled = false;
                spriteColor = Selected;
                return;
            }
            outline.enabled = true;
            spriteColor = Hovered;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("鼠标退出");
            if (!selectable)
            {
                outline.enabled = false;
                spriteColor = Selected;
                return;
            }
            outline.enabled = false;
            spriteColor = Unselected;
        }
    }
}