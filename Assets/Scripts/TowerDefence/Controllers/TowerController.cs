using Assets.Scripts.Animation;
using Assets.Scripts.Enemy;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.TowerDefence.Controllers
{
    public class TowerController : MonoBehaviour
    {
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

        [SerializeField] private protected Transform target;
        [SerializeField] private Transform lastInvalidTarget;
        private protected float currentReloadTime;
        private protected float detectionTimer;
        private protected Collider[] enemies = new Collider[16];

        [Header("Settings")]
        [SerializeField] private protected float reloadTime;
        [SerializeField] private protected float maxDistance;
        [SerializeField] private protected float minDistance;
        [SerializeField] private protected int damage;

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
            {
                RotateToTarget();
                if (target.position.y < 0f)
                {
                    target = null;
                    return;
                }
            }

            if (currentReloadTime <= 0f && target != null)
            {
                Shoot();
                currentReloadTime = reloadTime;
            }
        }

        private protected void CheckTarget()
        {
            var list = EnemyManager.Instance.GetEnemies();

            Transform best = null;
            float bestSqr = float.MaxValue;

            foreach (var enemy in list)
            {
                if (enemy == null) continue;
                if (!enemy.gameObject.activeInHierarchy) continue;

                float sqr = (enemy.position - transform.position).sqrMagnitude;

                if (sqr < bestSqr &&
                    sqr < maxDistance * maxDistance &&
                    sqr > minDistance * minDistance)
                {
                    bestSqr = sqr;
                    best = enemy;
                }
            }

            target = best;
        }


        private protected void ValidateTarget()
        {
            if (target == null)
                return;

            float sqrDist = (target.position - transform.position).sqrMagnitude;

            // Враг вышел из зоны -> сразу ищем новую цель
            if (sqrDist > maxDistance)
            {
                TrySwitchTarget();
                return;
            }

            // Враг в мёртвой зоне -> сразу ищем новую цель
            if (sqrDist < minDistance)
            {
                TrySwitchTarget();
                return;
            }

            // Враг уничтожен -> сразу ищем новую цель
            if (!target.gameObject.activeInHierarchy)
            {
                TrySwitchTarget();
                return;
            }
        }

        private void TrySwitchTarget()
        {
            // Запоминаем плохую цель
            lastInvalidTarget = target;

            target = null;
            CheckTarget();

            // Если выбрана та же плохая цель — сбрасываем и ищем ещё раз
            //if (target == lastInvalidTarget)
            //{
            //    target = null;
            //    CheckTarget();
            //}
        }



        private protected virtual void Shoot()
        {
            if (target == null)
                return;

            Vector3 direction = target.position - bulletPos.position;
            float distance = Vector3.Distance(target.position, transform.position);

            if (distance < minDistance)
                return;

            gunAnimation.PlayAnim();
            shotEffect.Play();

            GameObject bullet = BulletObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.SetPositionAndRotation(bulletPos.position, Quaternion.LookRotation(direction));
            
            var bulletController = bullet.GetComponent<BulletContoller>();
            if(bulletController != null)
            {
                bulletController.InitializeWithTimerAndDamage(direction.normalized, distance, damage);
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
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }

        public void SetSettings(float reload, float minDistance, float maxDistance, int damage)
        {
            this.reloadTime = reload;
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.damage = damage;
        }

        public float GetMinDistance()
        {
            return minDistance;
        }

        public float GetMaxDistance()
        {
            return maxDistance;
        }

        public float GetReloadTime()
        {
            return reloadTime;
        }

        public int GetDamage()
        {
            return damage;
        }
    }
}