using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Fictology.Registry;
using Fictology.UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace THNeonMirage.UI
{
    public class ProgressBarControl : RegistryEntry
    {
        private float m_progress;
        private float m_lockedAt;
        private float m_jumpedAt = -1;

        public GameObject progressObj;
        private RectTransform rect_transform;
        private RectTransform parent_transform;

        private Stopwatch stop_watch;
        public static readonly float Timeout = 20;
        private void Start()
        {
            rect_transform = GetComponent<RectTransform>();
            rect_transform.sizeDelta = new Vector2(0, 20);
            parent_transform = transform.parent.GetComponent<RectTransform>();
            
            stop_watch = new Stopwatch();
            stop_watch.Start();
        }

        private void Update()
        {
            if (m_progress > 1) Destroy(progressObj);
            m_progress = Time.time / Timeout;
            if (ShouldLockProgress()) return;
            rect_transform.sizeDelta = new Vector2(parent_transform.rect.width * m_progress * 2, 20);
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

        public bool ShouldLockProgress() => m_lockedAt <= m_progress;
        public void LockProgress(float lockedAt) => m_lockedAt = lockedAt;
        

        public void ContinueProgress()
        {
            m_lockedAt = 2;
        }

        public void JumpToProgress(float jumpedAt) => m_jumpedAt = jumpedAt;
    }
}
