using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using THNeonMirage.Map;
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


        public void SendPlayerJoinEvent()
        {
            var view = gameObject.GetPhotonView();
            var image = gameObject.GetComponent<RawImage>();
            image.texture = Resources.Load<Texture2D>("Textures/reimu");
            image.uvRect = new Rect(0f, 0.23f, 1f, 0.7f);
            image.color = Color.white;
            view.RPC(nameof(ReceivePlayerJoinEvent), RpcTarget.Others, view.ViewID);
        }

        public void SendCreateExistingPlayer(Player newPlayer)
        {
            var view = gameObject.GetPhotonView();
            var viewIds = FindObjectsOfType<PhotonView>()
                .Where(v => v.ViewID >= 1000)
                .Select(v => v.ViewID).Distinct();
            
            var remoteIds = FindObjectsOfType<PhotonView>()
                .Where(v => v.ViewID != view.ViewID && v.ViewID >= 1000)
                .Select(v => v.ViewID);
            
            foreach (var remoteId in remoteIds)
            {
                var remoteObj = PhotonView.Find(remoteId).gameObject;
                remoteObj.transform.position = room.GetVacantParent().position;
                remoteObj.transform.localScale = new Vector3(1.8F, 1.8F, 1.0F);
            }
            view.RPC(nameof(ReceiveCreateExistingPlayer), newPlayer, viewIds.ToArray());
        }

        [PunRPC]
        public void ReceiveCreateExistingPlayer(int[] existingViewId)
        {
            foreach (var id in existingViewId)
            {
                var instance = PhotonView.Find(id).gameObject;
                var parent = room.GetVacantParent();

                instance.transform.parent = parent;
                instance.transform.position = parent.position;
                instance.transform.localScale = new Vector3(1.8f, 1.8f, 1f);
                SendPlayerJoinEvent();
            }
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

        public void SendLockSelectionAndReady()
        {
            gameObject.GetPhotonView().RPC(nameof(ReceiveLockSelectionAndReady), RpcTarget.Others);
        }

        [PunRPC]
        private void ReceiveLockSelectionAndReady()
        {
            room.readyPlayers++;
            Debug.Log($"Ready Players = {room.readyPlayers}, Require {room.maxPlayerInRoom} Ready");
            var level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            var host = Registries.GetComponent<GameHost>(LevelRegistry.ServerLevel);
            if (room.readyPlayers == room.maxPlayerInRoom - 1)
            {
                room.gameObject.SetActive(false);
                level.CreateLevel();
                host.CreateOnlinePlayer(true);
                gameObject.GetPhotonView().RPC(nameof(NotifyLevelCreate), RpcTarget.Others);
            }
        }

        /// <summary>
        /// 使用 Notify xxx 的命名表示没有参数的远端方法调用，且该方法内部没有调用其它远端方法
        /// </summary>
        [PunRPC]
        private void NotifyLevelCreate()
        {
            room.gameObject.SetActive(false);
            var level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            var host = Registries.GetComponent<GameHost>(LevelRegistry.ServerLevel);
            level.CreateLevel();
            host.CreateOnlinePlayer(true);
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

        public void SetOwner(Player remotePlayer) => gameObject.GetPhotonView().TransferOwnership(remotePlayer);
    }
}