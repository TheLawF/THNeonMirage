using THNeonMirage.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

namespace THNeonMirage.Manager
{
    public class UIManager: MonoBehaviour, IPointerClickHandler
    {
        public Texture2D backgroundTexture;
        public Color backgroundColor;
        public string text;

        public bool shouldRenderTooltip;
        public static GameObject playerObj;
        public TMP_FontAsset fontAsset;

        [DisplayOnly] 
        public int DiceValue;
        public GameObject databaseObj;
        
        private DatabaseManager dbManager;
        private PlayerManager player;
        private Random random = new();
        private TMP_Text foreground_text;
        private void Start()
        {
            DiceValue = 1;
            dbManager = databaseObj.GetComponent<DatabaseManager>();
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.black;
            if (shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), ToString());
        }
        
        public void OnMouseExit() => shouldRenderTooltip = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            player = playerObj.GetComponent<PlayerManager>();
            DiceValue = random.Next(1,7);
            player.SetPosition(player.Position + DiceValue);
            dbManager.UpdateUserData(new PlayerData(player.UserName, player.Position));
            shouldRenderTooltip = true;
        }

        private new string ToString() => string.Concat(DiceValue);
    }
}