using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class GunController : MonoBehaviour
    {
        [Header("Shooting")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform shootPosition;
        [SerializeField] private Transform gunTransform;
        [SerializeField] private float reloading;

        [Header("Settings")]
        [SerializeField] private float minDistance;
        [SerializeField] private ShipAimLine shipAimLine;

        private float currentTimeReloading;

        public void Initialize(ShipAimLine shipAimLine)
        {
            currentTimeReloading = reloading;
            this.shipAimLine = shipAimLine;
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

            // --- 1. Вращаем основание по горизонтали ---
            Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
            if (flatDirection.sqrMagnitude > 0.01f)
            {
                Quaternion baseRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, baseRotation, Time.deltaTime * 10f);
            }

            // --- 2. Вращаем пушку (только наклон по вертикали) ---
            // Определяем направление к цели в мировом пространстве
            Vector3 gunDir = mousePosition - gunTransform.position;

            // Преобразуем его в локальные координаты пушки
            Vector3 localDir = gunTransform.parent.InverseTransformDirection(gunDir.normalized);

            // Получаем угол наклона вверх/вниз (по оси X)
            float angleX = -Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

            // Ограничиваем угол (по вкусу)
            angleX = Mathf.Clamp(angleX, -45f, 45f);

            // Применяем только наклон по X
            gunTransform.localRotation = Quaternion.Euler(angleX, 0f, 0f);

            // --- 3. Обновляем прицел ---
            UpdateLaserAndTrajectoryForSelected(mousePosition);
        }

        void UpdateLaserAndTrajectoryForSelected(Vector3 mouseWorld)
        {
            Vector3 startPos = shootPosition.position;
            Vector3 endPos = mouseWorld;
            shipAimLine.DrawLine(startPos, endPos, true);
        }
    }
}