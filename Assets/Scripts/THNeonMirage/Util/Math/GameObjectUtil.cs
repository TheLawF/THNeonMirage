using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace THNeonMirage.Util.Math
{
    public static class GameObjectUtil
    {
        public static bool HasComponent<TComponent>(GameObject gameObject) where TComponent: Component 
            => gameObject.GetComponent<TComponent>() is null;
        
        public static void FillParentRect(GameObject child)
        {
            var childRect = child.GetComponent<RectTransform>();
            childRect.anchorMin = Vector2.zero;  // 左下角
            childRect.anchorMax = Vector2.one;   // 右上角
        }

        public static List<GameObject> GetAllChildren(GameObject parent) 
            => (from Transform child in parent.transform select child.gameObject).ToList();
        

        /// <summary>
        /// 设置文本几何居中
        /// </summary>
        public static void SetTextGeoMidAndAutoSize(TMP_Text textPro)
        {
            textPro.enableAutoSizing = true;
            textPro.alignment = TextAlignmentOptions.CenterGeoAligned;
        }
        public static void DestroyNoOwnerObject()
        {
            var allViews = Object.FindObjectsOfType<PhotonView>();
            foreach (var view in allViews)
            {
                if (view.Owner is null)
                { 
                    PhotonNetwork.Destroy(view.gameObject);
                }
            }
        }

        public static void DestroyDuplicateObject(int duplicatePlayerActorNumber, List<PhotonView> possibleDuplicateViews)
        {
            var duplicateViews = new List<PhotonView>();
            foreach (var view in possibleDuplicateViews)
            {
                if (view.Owner is not null && view.Owner.ActorNumber == duplicatePlayerActorNumber)
                {
                    duplicateViews.Add(view);
                }
            }

            var keptView = duplicateViews.Min(view => view.ViewID);
            Object.Destroy(PhotonView.Find(keptView));
            // foreach (var view in duplicateViews.Where(view => view.ViewID != keptView))
            // {
            //     Debug.Log(view.ViewID);
            //     Object.Destroy(view.gameObject);
            // }
        }
    }
}