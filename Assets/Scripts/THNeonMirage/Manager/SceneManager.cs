using System;
using UnityEngine;

namespace THNeonMirage.Manager
{
    public class SceneManager: MonoBehaviour
    {
        public Camera gameCamera;
        public Camera uiCamera;

        public bool allowZoom = true;
        public bool allowDrag = true;
        public float zoomSpeed = 10F;
        public float dragSpeed = 0.8F;
        
        private Vector2 worldPos, startPos, moveDirection;
        private Vector3 cameraPrevPos;
        
        private void Update()
        {
            if (!allowZoom || !allowDrag) return;
            var delta = gameCamera.orthographicSize <= 0 ? 0 : Input.GetAxis("Mouse ScrollWheel");
            gameCamera.orthographicSize -= delta * zoomSpeed;
            worldPos = GetWorldPos(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(1))
            {
                startPos = worldPos;
                cameraPrevPos = transform.position;
            }

            if (!Input.GetMouseButton(1)) return;
            moveDirection = (worldPos - startPos) * dragSpeed;
            var t = transform;
            t.position = new Vector3(cameraPrevPos.x - moveDirection.x, cameraPrevPos.y - moveDirection.y,
                t.position.z);
        }
        
        private Vector2 GetWorldPos(Vector3 mousePos)
        {
            var factor = Screen.height / gameCamera.orthographicSize / 2;
            var x = (mousePos.x - Screen.width / 2) / factor;
            var y = (mousePos.y - Screen.height / 2) / factor;
            return new Vector2(x, y);
        }

        public void SwitchCamera(bool enableMain, bool enableUI)
        {
            if (enableMain)
            {
                gameCamera.enabled = true;
                uiCamera.enabled = false;
                return;
            }

            if (!enableUI) return;
            gameCamera.enabled = false;
            uiCamera.enabled = true;
        }
    }
}