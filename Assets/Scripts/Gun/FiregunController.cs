using System.Collections;
using Assets.Scripts.Interface;
using Assets.Scripts.Player;
using Assets.Scripts.Save;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Gun
{
    public class FiregunController : GunController
    {
        [Header("Flamethrower Settings")]
        [SerializeField] private ParticleSystem flameEffect;
        [SerializeField] private ParticleSystem smokeEffect;
        //[SerializeField] private AudioSource flameSound;
        [SerializeField] private int damage = 5;
        [SerializeField] private float fireRange = 5f;
        [SerializeField] private LayerMask damageLayer;
        [SerializeField] private float spreadAngle = 30f;

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

        public override void Initialize(ShipAimLine shipAimLine, CircleNoFire circleNoFire, SaveLifecycle saveLifecycle, string gunId)
        {
            base.Initialize(shipAimLine, circleNoFire, saveLifecycle, gunId);
            isFiring = false;

            flameEffect.Stop();
            smokeEffect.Stop();
        }

        private void Update()
        {
            HandleRotate();

            if (Input.GetMouseButtonDown(0) && canShoot)
                StartFire();

            if (Input.GetMouseButtonUp(0))
                StopFire();

            if (isFiring)
            {
                ApplyFireDamage();
            }
        }

        private void StartFire()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (isFiring) return;
                isFiring = true;

            Vector3 mousePosition = GetMousePosition();
            float distance = Vector3.Distance(mousePosition, transform.position);

            if (distance < minDistance)
                return;

            flameEffect.transform.position = shootPosition.position;
            flameEffect.transform.rotation = shootPosition.rotation;

            // Настраиваем параметры длины пламени
            var main = flameEffect.main;
            main.startSpeed = Mathf.Clamp(distance * coefDistanceSpeed, minSpeed, maxSpeed);
            main.startLifetime = Mathf.Clamp(distance * coefDistanceLife, minLife, maxLife);

            flameEffect.Play();
            smokeEffect.Play();
            //flameSound.Stop();
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
            // Наносим урон раз в 0.1 сек (можно настроить)
            damageTimer -= Time.deltaTime;
            if (damageTimer > 0f) return;
            damageTimer = 0.2f;

            Vector3 origin = shootPosition.position;
            Vector3 forward = shootPosition.forward;

            // Находим все цели в радиусе
            Collider[] hits = Physics.OverlapSphere(origin + forward * (fireRange / 2f), fireRange / 2f, damageLayer);

            foreach (var hit in hits)
            {
                Vector3 dirToTarget = (hit.transform.position - origin).normalized;
                float angle = Vector3.Angle(forward, dirToTarget);
                if (angle < spreadAngle)
                {
                    // Проверка прямой видимости (опционально)
                    if (Physics.Raycast(origin, dirToTarget, out RaycastHit hitInfo, fireRange))
                    {
                        var health = hitInfo.collider.GetComponent<IDamagable>();
                        if (health != null)
                        {
                            health.TakeDamage(damage);
                            saveLifecycle.UpdateGunDamageStatistic(gunId, damage);

                            if (health.IsDiedEnemy() && !health.CheckDiedEnemy())
                            {
                                saveLifecycle.UpdateGunKillStatistic(gunId);
                                health.SetCheckDiedEnemy();
                            }
                        }
                    }
                }
            }
        }
    }
}