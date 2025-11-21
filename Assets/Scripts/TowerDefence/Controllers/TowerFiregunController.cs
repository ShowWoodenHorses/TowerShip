using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.TowerDefence.Controllers
{
    public class TowerFiregunController : TowerController
    {
        //[SerializeField] private AudioSource flameSound;

        [Header("Flamethrower Settings")]
        [SerializeField] private GameObject effectsObject;
        [SerializeField] private ParticleSystem flameEffect;
        [SerializeField] private ParticleSystem smokeEffect;

        [Header("Speed particle")]
        [SerializeField] private float coefDistanceSpeed = 0.8f;
        [SerializeField] private float minSpeed = 1f;
        [SerializeField] private float maxSpeed = 10f;

        [Header("life particle")]
        [SerializeField] private float coefDistanceLife = 0.2f;
        [SerializeField] private float minLife = 0.1f;
        [SerializeField] private float maxLife = 1f;

        private bool isFiring;
        private float damageTimer;

        private new void Start()
        {
            isFiring = false;

            effectsObject.SetActive(true);
            flameEffect.Stop();
            smokeEffect.Stop();
        }

        private new void Update()
        {
            detectionTimer -= Time.deltaTime;

            if (detectionTimer <= 0f)
            {
                detectionTimer = detectionInterval;
                if (target == null)
                {
                    CheckTarget();
                    StopFire();
                }
                else
                {
                    ValidateTarget();
                }
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

            if (target != null)
            {
                Shoot();
            }
        }

        private void StartFire()
        {
            if (isFiring) return;
            isFiring = true;

            flameEffect.transform.position = bulletPos.position;
            flameEffect.transform.rotation = bulletPos.rotation;

            flameEffect.Play();
            smokeEffect.Play();
            //flameSound.Play();
        }

        private void UpdateFlameParameters()
        {
            if (target == null) return;

            float distance = Vector3.Distance(target.position, bulletPos.position);

            // нормализуем дистанцию в диапазон 0…1
            float t = Mathf.Clamp01(distance / maxDistance);

            var main = flameEffect.main;

            main.startSpeed = Mathf.Lerp(minSpeed, maxSpeed, t);
            main.startLifetime = Mathf.Lerp(minLife, maxLife, t);
        }

        private void StopFire()
        {
            if (!isFiring) return;
            isFiring = false;

            flameEffect.Stop();
            smokeEffect.Stop();
            //flameSound.Stop();
        }

        private void ApplyFireDamage()
        {
            // Наносим урон раз в 0.5 сек (можно настроить)
            damageTimer -= Time.deltaTime;
            if (damageTimer > 0f) return;
            damageTimer = 0.5f;

            var health = target.gameObject.GetComponent<IDamagable>();
            if (health != null)
                health.TakeDamage(damage);
        }

        private protected override void Shoot()
        {
            if (target == null)
                return;

            StartFire();          // запускаем один раз
            UpdateFlameParameters();
            ApplyFireDamage();
        }
    }
}