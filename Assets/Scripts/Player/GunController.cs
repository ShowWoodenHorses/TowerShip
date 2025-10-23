using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class GunController : MonoBehaviour
    {
        [Header("Shooting")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform shootPosition;
        [SerializeField] private float reloading;

        [Header("Settings")]
        [SerializeField] private float minDistance;
        [SerializeField] private ShipAimLine shipAimLine;

        private float currentTimeReloading;

        private void Start()
        {
            currentTimeReloading = reloading;
            shipAimLine.Initialize();
        }

        private void Update()
        {
            currentTimeReloading -= Time.deltaTime;
            HandleRotate();

            if (Input.GetMouseButtonDown(0))
            {
                if(currentTimeReloading <= 0f)
                {
                    Shoot();
                }
            }
        }

        private void Shoot()
        {
            Vector3 mousePosition = GetMousePosition();
            Vector3 direction = mousePosition - shootPosition.position;
            float distance = Vector3.Distance(mousePosition, transform.position);

            if (distance < minDistance)
            {
                return;
            }

            GameObject bullet = BulletObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.SetLocalPositionAndRotation(shootPosition.position, Quaternion.LookRotation(direction));

            var bulletController = bullet.GetComponent<BulletContoller>();
            if (bulletController != null)
            {
                bulletController.InitializeWithTimer(direction.normalized, distance);
            }

            currentTimeReloading = reloading;
        }

        private Vector3 GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if(groundPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            return ray.origin + ray.direction * 100f;
        }

        private void HandleRotate()
        {
            Vector3 mousePosition = GetMousePosition();
            Vector3 direction = mousePosition - transform.position;

            direction.Normalize();

            float distance = Vector3.Distance(mousePosition, transform.position);
            if(distance > minDistance)
            {
                transform.rotation = Quaternion.LookRotation(direction);
                UpdateLaserAndTrajectoryForSelected(mousePosition);
            }
        }

        void UpdateLaserAndTrajectoryForSelected(Vector3 mouseWorld)
        {
            Vector3 startPos = shootPosition.position;
            Vector3 endPos = mouseWorld;
            shipAimLine.DrawLine(startPos, endPos, true);
        }
    }
}