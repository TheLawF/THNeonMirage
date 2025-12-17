using System;
using System.Runtime.Serialization;
using System.Text;
using Fictology.Registry;
using Photon.Pun;
using UnityEngine;
using Color = System.Drawing.Color;

namespace THNeonMirage.UI
{
    public class AvatarManager: RegistryEntry
    {
        public PhotonView view;
        public Color color;
        public Sprite sprite;

        private void Start()
        {
            
        }
    }
}