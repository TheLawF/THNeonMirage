using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace THNeonMirage.Manager
{
    public class ToggleHandler : MonoBehaviour
    {
        public GameObject playerObj;
        private PlayerManager m_player;

        public List<GameObject> toggleList;
        private Toggle toggle_1;
        private Toggle toggle_2;
        private Toggle toggle_3;

        public int Price { get; private set; }

        public int Price1 { private get; set; }
        public int Price2 { private get; set; }
        public int Price3 { private get; set; }

        private void Start()
        {
            m_player = playerObj.GetComponent<PlayerManager>();
            toggle_1 = toggleList[0].GetComponent<Toggle>();
            toggle_2 = toggleList[1].GetComponent<Toggle>();
            toggle_3 = toggleList[2].GetComponent<Toggle>();
        }

        public void OnPrice1Changed()
        {
            toggle_2.isOn = false;
            toggle_3.isOn = false;
            Price = Price1;
        }
        
        public void OnPrice2Changed()
        {
            toggle_1.isOn = false;
            toggle_3.isOn = false;
            Price = Price2;
        }
        
        public void OnPrice3Changed()
        {
            toggle_1.isOn = false;
            toggle_2.isOn = false;
            Price = Price3;
        }

        public void OnPurchase()
        {
            m_player.Balance -= Price;
        }
    }
}