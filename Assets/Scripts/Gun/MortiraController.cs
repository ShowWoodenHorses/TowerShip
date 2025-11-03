using System.Collections;
using Assets.Scripts.Bullet;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Gun
{
    public class MortiraController : GunController
    {
        [SerializeField] private float offsetAngle;
        private void Update()
        {
            currentTimeReloading -= Time.deltaTime;
            HandleRotate();

            if (Input.GetMouseButtonDown(0) && canShoot)
            {
                if (currentTimeReloading <= 0f)
                {
                    Shoot();
                }
            }
        }

        public override void Shoot()
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
            shotEffect.Play();

            GameObject bullet = BulletObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.SetLocalPositionAndRotation(shootPosition.position, Quaternion.LookRotation(direction));

            var bulletController = bullet.GetComponent<MortiraCoreBullet>();
            if (bulletController != null)
            {
                bulletController.InitializeCore(shootPosition.position, mousePosition);

                gunAnimation.ResetAnim();
            }


            currentTimeReloading = reloading;
        }

        public override void HandleRotate()
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
            float angleX = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

            // Ограничиваем угол (по вкусу)
            angleX = Mathf.Clamp(angleX, minAngel, maxAngel);

            // Применяем только наклон по X
            gunTransform.localRotation = Quaternion.Euler(angleX + offsetAngle, 0f, 0f);

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