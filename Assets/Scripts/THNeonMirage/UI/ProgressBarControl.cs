using System.Diagnostics;
using Fictology.Registry;
using UnityEngine;

namespace THNeonMirage.UI
{
    public class ProgressBarControl : RegistryEntry
    {
        public float progress;
        private float m_lockedAt;
        private float m_jumpedAt = -1;

        public GameObject progressObj;
        private RectTransform rect_transform;
        private RectTransform parent_transform;

        public static readonly float Timeout = 20;
        public readonly Stopwatch Watch = new ();
        
        private void Start()
        {
            rect_transform = GetComponent<RectTransform>();
            rect_transform.sizeDelta = new Vector2(0, 20);
            parent_transform = transform.parent.GetComponent<RectTransform>();
            
            Watch.Start();
        }

        private void Update()
        {
            if (progress > 1) Destroy(progressObj);
            if (ShouldLockProgress()) return;
            progress = Watch.ElapsedMilliseconds / Timeout / 1000;
            rect_transform.sizeDelta = new Vector2(parent_transform.rect.width * progress * 2, 20);
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

        public bool ShouldLockProgress() => m_lockedAt <= progress;
        public void LockProgress(float lockedAt) => m_lockedAt = lockedAt;
        

        public void ContinueProgress()
        {
            m_lockedAt = 2;
        }

        public void JumpToProgress(float jumpedAt) => m_jumpedAt = jumpedAt;
    }
}
