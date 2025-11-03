using System.Collections;
using UnityEngine;

namespace Assets.Scripts.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        public Camera cam;

        [Header("Zoom Settings")]
        public float zoomSpeedMouse = 10f;
        public float zoomSpeedTouch = 0.1f;
        public float minFov = 20f;
        public float maxFov = 80f;
        public float zoomLerpSpeed = 10f;

        [Header("Drag Settings")]
        public float dragSpeedMouse = 0.3f;
        public float dragSpeedTouch = 0.01f;
        public bool invertDrag = true; // чтобы можно было поменять направление

        private float targetFov;
        private Vector3 lastPanPosition;
        private int panFingerId; // для тачей

        [Header("Movement Bounds")]
        public bool useBounds = true;
        public Vector2 minBounds = new Vector2(-50f, -50f);
        public Vector2 maxBounds = new Vector2(50f, 50f);

        private float positionnY;
        private Vector3 startPosition;
        private bool canMove = false;

        void Start()
        {
            if (cam == null)
                cam = Camera.main;

            targetFov = cam.fieldOfView;

            positionnY = transform.position.y;
            startPosition = transform.position;
        }

        void Update()
        {
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            HandleMouseZoom();
            if(canMove)
                HandleMouseDrag();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchZoom();
        HandleTouchDrag();
#endif

            // Плавный переход зума
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * zoomLerpSpeed);
        }

        public void SetActiveMoving()
        {
            canMove = true;
        }

        public void SetDisableMoving()
        {
            canMove = false;
            SetStartPosition();
        }
        
        void SetStartPosition()
        {
            transform.position = startPosition;
        }
        // --- ПК ЗУМ ---
        void HandleMouseZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetFov -= scroll * zoomSpeedMouse;
                targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
            }
        }

        // --- ПК ДРАГ ---
        void HandleMouseDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastPanPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 delta = Input.mousePosition - lastPanPosition;
                lastPanPosition = Input.mousePosition;

                float direction = invertDrag ? -1f : 1f;
                cam.transform.Translate(
                    delta.x * dragSpeedMouse * direction,
                    delta.y * dragSpeedMouse * direction,
                    0
                );

                ClampCameraPosition();
            }
        }

        // --- МОБИЛЬНЫЙ ЗУМ ---
        void HandleTouchZoom()
        {
            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrev = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrev = touchOne.position - touchOne.deltaPosition;

                float prevMagnitude = (touchZeroPrev - touchOnePrev).magnitude;
                float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                targetFov -= difference * zoomSpeedTouch;
                targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
            }
        }

        // --- МОБИЛЬНЫЙ ДРАГ ---
        void HandleTouchDrag()
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    panFingerId = touch.fingerId;
                    lastPanPosition = touch.position;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.deltaPosition;
                    float direction = invertDrag ? -1f : 1f;

                    cam.transform.Translate(
                        delta.x * dragSpeedTouch * direction,
                        delta.y * dragSpeedTouch * direction,
                        0
                    );
                }
            }
        }

        void ClampCameraPosition()
        {
            if (!useBounds) return;

            Vector3 pos = cam.transform.position;

            float zoomFactor = (cam.fieldOfView - minFov) / (maxFov - minFov);
            float extraRange = Mathf.Lerp(0f, 10f, zoomFactor);

            float minX = minBounds.x - extraRange;
            float maxX = maxBounds.x + extraRange;
            float minZ = minBounds.y - extraRange;
            float maxZ = maxBounds.y + extraRange;

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

            pos.y = positionnY;

            cam.transform.position = pos;
        }
    }
}