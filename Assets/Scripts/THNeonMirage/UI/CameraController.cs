using UnityEngine;

namespace THNeonMirage.UI
{

    public class CameraController : MonoBehaviour
    {
        public const float DevWidth = 9.6F;
        public const float DevHeight = 6.4F;
        
        public Camera camera;
        public bool allowZoom = true;
        public bool allowDrag = true;
        public float ZoomSpeed = 10F;
        public float DragSpeed = 0.8F;
        private Vector2 _worldPos, _startPos, _moveDirection;
        private Vector3 _cameraPrevPos;

        private void Start()
        {
            camera = GetComponent<Camera>();
            // var orthoSize = camera.orthographicSize;
            // var aspectRatio = Screen.width * 1F / Screen.height;
// 
            // var cameraWidth = orthoSize * 2 * aspectRatio;
            // if (cameraWidth < )
            // {
            //     
            // }
        }

        Vector2 GetWorldPos(Vector3 mousePos)
        {
            var factor = Screen.height / camera.orthographicSize / 2;
            var x = (mousePos.x - Screen.width / 2) / factor;
            var y = (mousePos.y - Screen.height / 2) / factor;
            return new Vector2(x, y);
        }

        private void Update()
        {
            if (!allowZoom || !allowDrag) return;
            var delta = Input.GetAxis("Mouse ScrollWheel");
            camera.orthographicSize -= delta * ZoomSpeed;
            _worldPos = GetWorldPos(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(1))
            {
                _startPos = _worldPos;
                _cameraPrevPos = transform.position;
            }

            if (Input.GetMouseButton(1))
            {
                _moveDirection = (_worldPos - _startPos) * DragSpeed;
                var t = transform;
                t.position = new Vector3(_cameraPrevPos.x - _moveDirection.x, _cameraPrevPos.y - _moveDirection.y,
                    t.position.z);
            }
        }

    }
}