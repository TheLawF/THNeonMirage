using System.Diagnostics;
using UnityEngine;

namespace THNeonMirage.UI
{
    public class ProgressBarControl : MonoBehaviour
    {
        public float progress;
        public float startTime;

        [DisplayOnly] public bool isReady;
        public GameObject progressObj;
        private RectTransform rect_transform;
        private RectTransform parent_transform;

        private Stopwatch stop_watch;
        public static readonly float Timeout = 20;
        public static readonly float[] Ratio = { 0.2f, 0.5f, 0.8f, 0.95f };
        private void Start()
        {
            rect_transform = GetComponent<RectTransform>();
            rect_transform.sizeDelta = new Vector2(0, 20);
            parent_transform = transform.parent.GetComponent<RectTransform>();
            
            stop_watch = new Stopwatch();
            stop_watch.Start();
        }
        
        // private IEnumerator CheckUpdateProgress()
        // {
        //     UpdateProgress();
        //     while (clientState != ClientState.ConnectedToNameServer)
        //     {
        //         yield return null;
        //         if (Time.time > Timeout) yield break;
        //     }
        // }

        private void Update()
        {
            if (isReady)
            {
                rect_transform.sizeDelta =
                    new Vector2(transform.parent.GetComponent<RectTransform>().rect.width * 4, 0);
                Destroy(this);
                return;
            }
            progress = Time.time / Timeout;
            var deltaProgress = Time.deltaTime / Timeout;
            rect_transform.sizeDelta += new Vector2(transform.parent.GetComponent<RectTransform>().rect.width * 2 * deltaProgress, 0);
        }
    }
}
