using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class ToggleHandler : MonoBehaviour
    {
        public GameObject inGamePanel;
        public PlayerManager player;

        public List<GameObject> toggleList;
        private Toggle toggle_1;
        private Toggle toggle_2;
        private Toggle toggle_3;
        
        private TMP_Text _tmpText1;
        private TMP_Text _tmpText2;
        private TMP_Text _tmpText3;

        public int Price { get; private set; }
        public int Price1 { private get; set; }
        public int Price2 { private get; set; }
        public int Price3 { private get; set; }

        private void Start()
        {
            
            // _tmpText1 = toggleList[0].GetComponentInChildren<TMP_Text>();
            // _tmpText2 = toggleList[1].GetComponentInChildren<TMP_Text>();
            // _tmpText3 = toggleList[2].GetComponentInChildren<TMP_Text>();
            _tmpText1 = GameObject.Find("Canvas/InGamePanel/FirstBid/Text (TMP)").GetComponent<TMP_Text>();
            _tmpText2 = GameObject.Find("Canvas/InGamePanel/SecondBid/Text (TMP)").GetComponent<TMP_Text>();
            _tmpText3 = GameObject.Find("Canvas/InGamePanel/ThirdBid/Text (TMP)").GetComponent<TMP_Text>();
            
            toggle_1 = toggleList[0].GetComponent<Toggle>();
            toggle_2 = toggleList[1].GetComponent<Toggle>();
            toggle_3 = toggleList[2].GetComponent<Toggle>();

            _tmpText1.SetText($"第一收购价：{Price1}");
            _tmpText2.SetText($"第二收购价：{Price2}");
            _tmpText3.SetText($"第三收购价：{Price3}");
        }

        private void Update()
        {
            _tmpText1.SetText($"第一收购价：{Price1}");
            _tmpText2.SetText($"第二收购价：{Price2}");
            _tmpText3.SetText($"第三收购价：{Price3}");
        }

        // public void OnPrice1Changed()
        // {
        //     toggle_2.SetIsOnWithoutNotify(false);
        //     toggle_3.SetIsOnWithoutNotify(false);
        //     Price = Price1;
        // }
        // 
        // public void OnPrice2Changed()
        // {
        //     toggle_1.SetIsOnWithoutNotify(false);
        //     toggle_3.SetIsOnWithoutNotify(false);
        //     Price = Price2;
        // }
        // 
        // public void OnPrice3Changed()
        // {
        //     toggle_1.SetIsOnWithoutNotify(false);
        //     toggle_2.SetIsOnWithoutNotify(false);
        //     Price = Price3;
        // }

        public void SetText(int index, string txt)
        {
            switch (index)
            {
                case 0: _tmpText1.SetText(txt);
                    break;
                case 1: _tmpText2.SetText(txt);
                    break;
                case 2: _tmpText3.SetText(txt);
                    break;
            }
        }

        public void OnPurchase()
        {
            player.Balance -= Price;
        }

        public void ClosePanel()
        {
            inGamePanel.SetActive(false);
        }

        public static void DisplayPanel()
        {
            
        }
    }
}