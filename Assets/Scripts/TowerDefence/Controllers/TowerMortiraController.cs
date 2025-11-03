using Assets.Scripts.Bullet;
using UnityEngine;

namespace Assets.Scripts.TowerDefence.Controllers
{
    public class TowerMortiraController : TowerController
    {
        [SerializeField] private Transform gunTransform;
        [SerializeField] private Vector3 offset;

        [Header("Angel")]
        [SerializeField] private float minAngel = -45f;
        [SerializeField] private float maxAngel = 45f;
        private protected override void Shoot()
        {
            if (target == null)
                return;

            Vector3 direction = target.position - bulletPos.position;

            gunAnimation.PlayAnim();
            shotEffect.Play();

            GameObject bullet = BulletObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.SetLocalPositionAndRotation(bulletPos.position, Quaternion.LookRotation(direction));

            var bulletController = bullet.GetComponent<MortiraCoreBullet>();
            if (bulletController != null)
            {
                bulletController.InitializeCore(bulletPos.position, target.position + offset);

                gunAnimation.ResetAnim();
            }
        }

        private protected override void RotateToTarget()
        {
            Vector3 direction = target.position - transform.position;

            // --- 1. Вращаем основание по горизонтали ---
            Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
            if (flatDirection.sqrMagnitude > 0.01f)
            {
                Quaternion baseRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, baseRotation, Time.deltaTime * 10f);
            }

            // --- 2. Вращаем пушку (только наклон по вертикали) ---
            // Определяем направление к цели в мировом пространстве
            Vector3 gunDir = target.position - gunTransform.position;

            // Преобразуем его в локальные координаты пушки
            Vector3 localDir = gunTransform.parent.InverseTransformDirection(gunDir.normalized);

            // Получаем угол наклона вверх/вниз (по оси X)
            float angleX = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;

            // Ограничиваем угол (по вкусу)
            angleX = Mathf.Clamp(angleX, minAngel, maxAngel);

            // Применяем только наклон по X
            gunTransform.localRotation = Quaternion.Euler(angleX, 0f, 0f);
        }
    }
}