using System;
using Fictology.Registry;
using Photon.Pun;
using UnityEngine;
using Color = System.Drawing.Color;

namespace THNeonMirage.UI
{
    public class TextureManager: RegistryEntry
    {
        public PhotonView view;
        public Color color;
        public Sprite sprite;

        private void Start()
        {
            
        }
    }
}