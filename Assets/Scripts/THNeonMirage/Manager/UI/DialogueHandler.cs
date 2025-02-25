using THNeonMirage.Data;
using THNeonMirage.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace THNeonMirage.Manager.UI
{
    public class DialogueHandler : MonoBehaviour
    {
        public GameObject title;
        public GameObject description;
        public GameObject playerInstance;
        public DialogueType type;

        private TMP_Text text;
        private PlayerManager player;
        
        private void Start()
        {
            text = description.GetComponent<TMP_Text>();
            player = DatabaseManager.PlayerInstance.GetComponent<PlayerManager>();
            player.PlayerData.OnPositionChanged += OnPlayerPositionChanged;
        }

        private static void OnPlayerPositionChanged(object sender, ValueEventArgs args)
        {
            
        }

    }

    public enum DialogueType
    {
        Info,
        Warn,
        Error,
        Fatal
    }
}
