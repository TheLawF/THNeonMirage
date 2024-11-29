using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class ToggleHandler : MonoBehaviour
    {
        public PlayerManager player;

        public List<GameObject> toggleList;

        public GameObject object1;
        public GameObject object2;
        public GameObject object3;
        
        private TMP_Text text1;
        private TMP_Text text2;
        private TMP_Text text3;
        
        private Toggle t1;
        private Toggle t2;
        private Toggle t3;

        public int Price { get; private set; }
        private int price1;
        private int price2;
        private int price3;

        private void Start()
        {
            text1 = object1.GetComponentInChildren<TMP_Text>();
            text2 = object2.GetComponentInChildren<TMP_Text>();
            text3 = object3.GetComponentInChildren<TMP_Text>();

            t1 = object1.GetComponent<Toggle>();
            t2 = object2.GetComponent<Toggle>();
            t3 = object3.GetComponent<Toggle>();
        }

        private void Update()
        {
            text1.SetText($"第一收购价：{price1}");
            text2.SetText($"第二收购价：{price2}");
            text3.SetText($"第三收购价：{price3}");
        }

        public void SetText(int index, string txt)
        {
            switch (index)
            {
                case 0: text1.SetText(txt);
                    break;
                case 1: text2.SetText(txt);
                    break;
                case 2: text3.SetText(txt);
                    break;
            }
        }

        public void OnPurchase()
        {
            if (player.Balance < price1) return;
            player.Balance -= t1.isOn ? price1 : t2.isOn ? price2 : t3.isOn ? price3 : 0;
        }


        public static void DisplayPanel()
        {
            
        }

        public void SetPrice(int price1, int price2, int price3)
        {
            this.price1 = price1;
            this.price2 = price2;
            this.price3 = price3;
        }
    }
}