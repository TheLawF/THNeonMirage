using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Registry;
using THNeonMirage.UI;
using THNeonMirage.Util.Math;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class AvatarManager: MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public string avatarName;
        public bool selectable = true;
        public bool isSelected;
        
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

        public static void CreateAvatar(RoomManager room) => room.CreateAvatarWhenJoinIn();

        public void SendPlayerJoinEvent()
        {
            var view = gameObject.GetPhotonView();
            var image = gameObject.GetComponent<RawImage>();
            image.texture = Resources.Load<Texture2D>("Textures/reimu");
            image.uvRect = new Rect(0f, 0.23f, 1f, 0.7f);
            image.color = Color.white;
            view.RPC(nameof(ReceivePlayerJoinEvent), RpcTarget.Others, view.ViewID);
        }
        
        [PunRPC]
        public void ReceivePlayerJoinEvent(int viewId)
        {
            var newJoinedPlayerAvatar = PhotonView.Find(viewId).gameObject;
            var rawImage = newJoinedPlayerAvatar.GetComponent<RawImage>();
            room.remotes.ForEach(obj =>
            {
                if (room.DoesParentHasChild(obj)) return;
                newJoinedPlayerAvatar.transform.parent = obj.transform;
                rawImage.texture = Resources.Load<Texture2D>("Textures/reimu");
                rawImage.uvRect = new Rect(0f, 0.23f, 1f, 0.7f);
            });
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
            if (!selectable) return;
            room.avatars.ForEach(avatar =>
            {
                if (avatar.avatarName != avatarName) return;
                GameObjectUtil.GetAllChildren(avatar.gameObject).ForEach(Destroy);
            });
            
            m_select_label = PrefabRegistry.BackgroundLabel.Instantiate(m_transform.position, Quaternion.identity, m_transform);
            m_select_label.GetComponent<RectTransform>().localScale = new Vector3(0.2f, 0.2f, 1f);
            
            var label = PrefabRegistry.Label.Instantiate(m_select_label.transform.position, Quaternion.identity, m_select_label.transform);
            var textPro = label.GetComponent<TMP_Text>();
            textPro.text = $"Player{PhotonNetwork.LocalPlayer.ActorNumber}";
            
            GameObjectUtil.FillParentRect(label);
            GameObjectUtil.SetTextGeoMidAndAutoSize(textPro);
            
            spriteColor = Selected;
            isSelected = true;
            
            localAvatar.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(avatarName);
            localAvatar.GetPhotonView().RPC(nameof(ReceiveAvatarUpdate), RpcTarget.Others, 
                localAvatar.GetPhotonView().ViewID, avatarName);
        }
        
        [PunRPC]
        private void ReceiveAvatarUpdate(int viewId, string texturePath)
        {
            var obj = PhotonView.Find(viewId).gameObject;
            var img = obj.GetComponent<RawImage>();
            img.texture = Resources.Load<Texture2D>(texturePath);
            img.uvRect = new Rect(0f, 0.23f, 1f, 0.7f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (outline == null) return;
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
            if (outline == null) return;
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