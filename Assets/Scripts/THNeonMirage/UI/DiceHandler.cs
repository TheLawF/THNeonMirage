using System.Collections;
using Fictology.UnityEditor;
using Photon.Pun;
using THNeonMirage.Event;
using THNeonMirage.Manager;
using THNeonMirage.Map;
using THNeonMirage.Registry;
using THNeonMirage.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

namespace THNeonMirage.UI
{
    public class DiceHandler: MonoBehaviourPun, IPointerClickHandler, IPunObservable
    { 
        [DisplayOnly] public int DiceValue;
        public int pos;
        public bool canInteract;
        public bool canRenderTooltip;

        public Level level;
        
        public PlayerManager player;
        public GameObject inGamePanel;

        private bool shouldRenderTooltip;
        private RawImage m_image;
        private Random random = new();
        private TMP_Text foreground_text; 

        private void Start()
        {
            canInteract = true;
            inGamePanel = Registries.GetObject(UIRegistry.InGamePanel);
            level = Registries.Get<Level>(LevelRegistry.ClientLevel);
            m_image = gameObject.GetComponent<RawImage>();
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.black;
            if (canRenderTooltip && shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), ToString());
        }

        public void OnMouseOver() => shouldRenderTooltip = true;    

        public void OnMouseExit() => shouldRenderTooltip = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            var server = Registries.GetComponent<GameHost>(LevelRegistry.ServerLevel);
            Toss(server);
        }

        public void Toss(GameHost server)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (!server.IsMyTurn()) return;
                if (!player.CanMove())
                {
                    server.NextTurn();
                    return;
                }
                StartCoroutine(PlayDiceAnimation(server));
                return;
            }
            if (player.IsBot()) return;
            if (!player.IsMyTurn()) return;
            if (!player.CanMove())
            {
                level.NextTurn();
                return;
            }
            StartCoroutine(PlayDiceAnimation(server));
        }
        
        private IEnumerator PlayDiceAnimation(GameHost server)
        {
            var value = 0;
            for (var i = 0; i < 12; i++)
            {
                value = random.Next(1, 7);
                m_image.uvRect = new Rect(0F, 1F - value / 6F, 1F, 1F / 6F);
                yield return new WaitForSeconds(0.2F);
            }
            DiceValue = value;
            ApplyTossResult(server);
        }
        
        private void ApplyTossResult(GameHost server)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                pos = player.playerData.position;
                pos += DiceValue;

                player.SetPosIndex(pos);
                inGamePanel.GetComponent<InGamePanelHandler>().SetTile(level, player.playerData.position);
                shouldRenderTooltip = true;
                server.NextTurn();
                return;
            }
            pos = player.playerData.position;
            pos += DiceValue;

            player.SetPosIndex(pos);
            inGamePanel.GetComponent<InGamePanelHandler>().SetTile(level, player.playerData.position);
            shouldRenderTooltip = true;
            level.NextTurn();
        }
        
        private new string ToString() => string.Concat(DiceValue);
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }
    }
}