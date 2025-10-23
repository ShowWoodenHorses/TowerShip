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

        void Start()
        {
            if (cam == null)
                cam = Camera.main;

            targetFov = cam.fieldOfView;
        }

        void Update()
        {
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            HandleMouseZoom();
            //HandleMouseDrag();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchZoom();
        HandleTouchDrag();
#endif

            // Плавный переход зума
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * zoomLerpSpeed);
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
    }
}