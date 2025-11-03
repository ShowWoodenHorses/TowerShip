using Assets.Scripts.Animation;
using UnityEngine;

namespace Assets.Scripts.TowerDefence.Controllers
{
    public class TowerController : MonoBehaviour
    {
        [SerializeField] private protected float reloadTime;
        [SerializeField] private protected float range;
        [SerializeField] private protected GameObject bulletPrefab;
        [SerializeField] private protected Transform bulletPos;
        [SerializeField] private protected LayerMask layerMask;

        [Header("Behaviour")]
        [SerializeField] private protected float detectionInterval = 0.2f;
        [SerializeField] private protected float rotationSpeed = 10f;

        [Header("Animation")]
        [SerializeField] private protected GunAnimation gunAnimation;

        [Header("Effect")]
        [SerializeField] private protected ParticleSystem shotEffect;

        private protected Transform target;
        private protected float currentReloadTime;
        private protected float detectionTimer;
        private protected Collider[] enemies = new Collider[16];

        private protected void Start()
        {
            target = null;

            if (gunAnimation == null)
                gunAnimation = GetComponent<GunAnimation>();

            gunAnimation.InitializeAnim();
            currentReloadTime = 0f;
            shotEffect.Stop();
        }

        private protected void Update()
        {
            currentReloadTime -= Time.deltaTime;
            detectionTimer -= Time.deltaTime;

            if (detectionTimer <= 0f)
            {
                detectionTimer = detectionInterval;
                if (target == null)
                    CheckTarget();
                else
                    ValidateTarget();
            }

            if (target != null)
                RotateToTarget();

            if (currentReloadTime <= 0f && target != null)
            {
                Shoot();
                currentReloadTime = reloadTime;
            }
        }

        private protected void CheckTarget()
        {
            int found = Physics.OverlapSphereNonAlloc(transform.position, range, enemies, layerMask);

            Transform nearest = null;
            float nearestSqr = float.MaxValue;

            for (int i = 0; i < found; i++)
            {
                var c = enemies[i];
                if (c == null) continue;

                // Защита: объект может быть уничтожен раньше чем мы отработаем
                Transform candidate = c.transform;
                if (!candidate.gameObject.activeInHierarchy) continue;

                float sqr = (candidate.position - transform.position).sqrMagnitude;
                if (sqr < nearestSqr)
                {
                    nearestSqr = sqr;
                    nearest = candidate;
                }
            }

            target = nearest;
        }

        private protected void ValidateTarget()
        {
            if (target == null) return;

            // Если цель дальше, чем range (любой вариант - для простоты проверим расстояние до позиции)
            float sqrDist = (target.position - transform.position).sqrMagnitude;
            if (sqrDist > range * range)
            {
                target = null;
                return;
            }

            // Если цель уничтожена/неактивна
            if (!target.gameObject.activeInHierarchy)
            {
                target = null;
                return;
            }
        }

        private protected virtual void Shoot()
        {
            if (target == null)
                return;

            Vector3 direction = target.position - bulletPos.position;
            float distance = Vector3.Distance(target.position, transform.position);

            gunAnimation.PlayAnim();
            shotEffect.Play();

            GameObject bullet = BulletObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.SetPositionAndRotation(bulletPos.position, Quaternion.LookRotation(direction));
            
            var bulletController = bullet.GetComponent<BulletContoller>();
            if(bulletController != null)
            {
                bulletController.InitializeWithTimer(direction.normalized, distance);
                gunAnimation.ResetAnim();
            }
        }

        private protected virtual void RotateToTarget()
        {
            Vector3 dir = (target.position - transform.position);
            dir.y = 0f; // если нужно игнорировать высоту (опционально)
            if (dir.sqrMagnitude <= 0.001f) return;

            Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
            // Плавный поворот:
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Mathf.Clamp01(rotationSpeed * Time.deltaTime));
        }

        private protected void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}