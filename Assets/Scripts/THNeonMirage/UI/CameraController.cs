using System.Collections;
using Fictology.Registry;
using UnityEngine;

namespace THNeonMirage.UI
{

    public class CameraController : RegistryEntry
    {
        public const float DevWidth = 9.6F;
        public const float DevHeight = 6.4F;
        
        public Camera camera;
        public bool allowZoom = true;
        public bool allowDrag = true;
        public float ZoomSpeed = 10F;
        public float DragSpeed = 0.8F;

        public bool enableMouseCtrl;

        public GameObject BindingPlayer
        {
            get => m_player;
            set
            {
                if (value is not null) StartCoroutine(UpdatePlayerControl());
                m_player = value;
            }
        }

        private GameObject m_player;
        
        private Vector2 _worldPos, _startPos, _moveDirection;
        private Vector3 _cameraPrevPos;

        private void Start()
        {
            camera = GetComponent<Camera>();
            // var orthoSize = camera.orthographicSize;
            // var aspectRatio = Screen.width * 1F / Screen.height;
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
            UpdateMouseControl();
        }

        private IEnumerator UpdatePlayerControl()
        {
            if (BindingPlayer is null) yield break;
            var pos = BindingPlayer.transform.position;
            SetPos(new Vector3(pos.x, pos.y, -10));
            yield return null;
        }

        public void SetOrthographicSize(float size)
        {
            camera.orthographicSize = size;
        }

        public void SetPos(Vector3 pos)
        {
            transform.position = pos;
        }

        
        private void UpdateMouseControl()
        {
            var delta = Input.GetAxis("Mouse ScrollWheel");
            if (allowZoom) camera.orthographicSize -= delta * ZoomSpeed;
            if (!allowDrag) return;
            _worldPos = GetWorldPos(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(2))
            {
                _startPos = _worldPos;
                _cameraPrevPos = transform.position;
            }

            if (Input.GetMouseButton(2))
            {
                _moveDirection = (_worldPos - _startPos) * DragSpeed;
                var t = transform;
                t.position = new Vector3(_cameraPrevPos.x - _moveDirection.x, _cameraPrevPos.y - _moveDirection.y,
                    t.position.z);
            }
        }

    }
}