using THNeonMirage.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

namespace THNeonMirage.Manager
{
    public class TooltipHandler: MonoBehaviour, IPointerClickHandler
    {
        
        public bool canRenderTooltip;
        public static GameObject playerObj;

        [DisplayOnly] 
        public int DiceValue;
        public GameObject databaseObj;
        
        private bool shouldRenderTooltip;
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
            if (canRenderTooltip && shouldRenderTooltip) GUI.Label(new Rect(10, 10, 200, 20), ToString());
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