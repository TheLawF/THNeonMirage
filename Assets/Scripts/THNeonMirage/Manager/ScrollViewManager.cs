using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class ScrollViewManager: MonoBehaviour
    {
        public GameObject content;
        public GameObject buttonPrefab;

        private void Start()
        {
            // buttonPrefab.GetComponent<TMP_Text>().text = "#1";
            for (var i = 0; i < 20; i++)
            {
                var newButton = Instantiate(buttonPrefab, content.transform);
                // var roomName = $"{i}";
                // newButton.GetComponent<Button>().onClick.AddListener(() => NetworkManager.JoinRoom(roomName));
                // newButton.GetComponent<TMP_Text>().text = $"#{i + 1}";
                AdjustContent(10);
            }
        }
        
        private void AdjustContent(int itemHeight)
        {
            var childCount = content.transform.childCount;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, childCount * itemHeight);
        }
    }
}