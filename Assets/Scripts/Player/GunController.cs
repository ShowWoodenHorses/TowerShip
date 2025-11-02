using System.Collections;
using Assets.Scripts.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Player
{
    public class GunController : MonoBehaviour
    {
        [Header("Shooting")]
        [SerializeField] private protected GameObject bulletPrefab;
        [SerializeField] private protected Transform shootPosition;
        [SerializeField] private protected Transform gunTransform;
        [SerializeField] private protected float reloading;

        [Header("Angel")]
        [SerializeField] private protected float minAngel = -45f;
        [SerializeField] private protected float maxAngel = 45f;

        [Header("Settings")]
        [SerializeField] private protected float minDistance;
        [SerializeField] private protected float distanceFireRadius;
        [SerializeField] private protected ShipAimLine shipAimLine;
        [SerializeField] private protected Transform aimLinePos;
        [SerializeField] private protected CircleNoFire circleNoFire;

        [Header("Animation")]
        [SerializeField] private protected GunAnimation gunAnimation;

        private protected float currentTimeReloading;

        public virtual void Initialize(ShipAimLine shipAimLine, CircleNoFire circleNoFire)
        {
            currentTimeReloading = reloading;
            gunAnimation.InitializeAnim();

            if (aimLinePos == null)
            {
                aimLinePos = shootPosition;
            }

            this.shipAimLine = shipAimLine;
            this.circleNoFire = circleNoFire;

            circleNoFire.Initialize(distanceFireRadius);
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

        public virtual void Shoot()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3 mousePosition = GetMousePosition();
            Vector3 direction = mousePosition - shootPosition.position;
            float distance = Vector3.Distance(mousePosition, transform.position);

            if (distance < minDistance)
            {
                return;
            }

            gunAnimation.PlayAnim();

            GameObject bullet = BulletObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.SetLocalPositionAndRotation(shootPosition.position, Quaternion.LookRotation(direction));

            var bulletController = bullet.GetComponent<BulletContoller>();
            if (bulletController != null)
            {
                bulletController.InitializeWithTimer(direction.normalized, distance);

                gunAnimation.ResetAnim();
            }

            currentTimeReloading = reloading;
        }

        private protected Vector3 GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if(groundPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            return ray.origin + ray.direction * 100f;
        }

        public virtual void HandleRotate()
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
            angleX = Mathf.Clamp(angleX, minAngel, maxAngel);

            // Применяем только наклон по X
            gunTransform.localRotation = Quaternion.Euler(angleX, 0f, 0f);

            // --- 3. Обновляем прицел ---
            UpdateLaserAndTrajectoryForSelected(mousePosition);
        }

        private void UpdateLaserAndTrajectoryForSelected(Vector3 mouseWorld)
        {
            Vector3 startPos = aimLinePos.position;
            Vector3 endPos = mouseWorld;
            if (Vector3.Distance(startPos, endPos) > minDistance)
            {
                shipAimLine.DrawLine(startPos, endPos, true);
                circleNoFire.HideCircleNoFire();
            }
            else
            {
                shipAimLine.Hide();
                circleNoFire.ShowCircleNoFire();
            }
        }
    }
}