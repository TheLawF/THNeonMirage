using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace THNeonMirage.Util.Math
{
    public static class GameObjectUtil
    {
        public static void FillParentRect(GameObject child)
        {
            var childRect = child.GetComponent<RectTransform>();
            childRect.anchorMin = Vector2.zero;  // 左下角
            childRect.anchorMax = Vector2.one;   // 右上角
        
        }

        /// <summary>
        /// 设置文本几何居中
        /// </summary>
        public static void SetTextGeoMidAndAutoSize(TMP_Text textPro)
        {
            textPro.enableAutoSizing = true;
            textPro.alignment = TextAlignmentOptions.CenterGeoAligned;
        }
    }
}