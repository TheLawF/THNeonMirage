using System.Collections;
using System.Diagnostics;
using Fictology.Registry;
using Fictology.UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace THNeonMirage.UI
{
    public class ProgressBarControl : RegistryEntry
    {
        public float progress;
        public float startTime;

        [DisplayOnly] public bool shouldLockProgress = false;
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
            
            progress = Time.time / Timeout;
            var deltaProgress = Time.deltaTime / Timeout;
            rect_transform.sizeDelta += new Vector2(parent_transform.rect.width * 2 * deltaProgress, 0);
        }

        private IEnumerator UpdateProgress()
        {
            if (shouldLockProgress) yield break;
            progress = Time.time / Timeout;
            var deltaProgress = Time.deltaTime / Timeout;
            rect_transform.sizeDelta += new Vector2(parent_transform.rect.width * 2 * deltaProgress, 0);
            yield return new WaitForSeconds(0.1F);
        }
        
        public void LockProgress()
        {
            shouldLockProgress = true;
        }
    }
}
