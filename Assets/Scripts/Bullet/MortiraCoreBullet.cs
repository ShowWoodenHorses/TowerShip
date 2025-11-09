using System.Collections;
using Assets.Scripts.Sound;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public class MortiraCoreBullet : BulletContoller
    {
        [SerializeField] private protected float coefSpeed = 2f;
        [SerializeField] private protected GameObject trackImage;

        private protected Vector3 startPos;
        private protected Vector3 targetPos;
        private protected Vector3 highPoint;
        private protected Vector3 mid1;
        private protected Vector3 mid2;

        private protected float slowSpeed;
        private protected float highSpeed;

        private protected float elapsedTime;
        private protected float flightDuration;
        private protected bool inFlight;

        private void FixedUpdate()
        {
            if (!inFlight) return;

            elapsedTime += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / flightDuration);

            // Получаем позицию по Bezier
            Vector3 position = GetBezierPoint(t);

            // Рассчитываем velocity для Rigidbody
            Vector3 velocity = (position - rb.position) / Time.fixedDeltaTime;
            rb.linearVelocity = velocity;

            // Поворот пушки по движению
            if (velocity.sqrMagnitude > 0.001f)
                rb.rotation = Quaternion.LookRotation(velocity.normalized);

            // Заканчиваем полет
            if (t >= 1f)
            {
                inFlight = false;
                trackImage.SetActive(false);
            }
        }

        public virtual void InitializeCore(Vector3 start, Vector3 target)
        {
            gameObject.SetActive(false);
            startPos = start;
            targetPos = target;
            elapsedTime = 0f;
            inFlight = true;
            trackImage.SetActive(true);

            slowSpeed = speed - coefSpeed;
            highSpeed = speed + coefSpeed;

            // Верхняя точка
            highPoint = new Vector3(
                (start.x + target.x) / 2,
                Mathf.Max(start.y, target.y) + 15f, // поднимаем выше старта/цели
                (start.z + target.z) / 2
            );

            // Дополнительные промежуточные точки для плавного перехода
            mid1 = Vector3.Lerp(start, highPoint, 0.5f);
            mid2 = Vector3.Lerp(highPoint, target, 0.5f);

            // Расчет общей дистанции для определения времени полета
            float distance = Vector3.Distance(start, mid1) +
                             Vector3.Distance(mid1, highPoint) +
                             Vector3.Distance(highPoint, mid2) +
                             Vector3.Distance(mid2, target);

            flightDuration = distance / ((slowSpeed + highSpeed) / 2);
            lifeBeforeDestroy = flightDuration;

            gameObject.SetActive(true);
            SoundPoolManager.Instance.PlaySound(soundShotPrefab);

            StartCoroutine(LifeBeforeDestroy());
        }

        public virtual void InitializeCoreWithDamage(Vector3 start, Vector3 target, int damage)
        {
            damageEnemy = damage;
            gameObject.SetActive(false);
            startPos = start;
            targetPos = target;
            elapsedTime = 0f;
            inFlight = true;
            trackImage.SetActive(true);

            slowSpeed = speed - coefSpeed;
            highSpeed = speed + coefSpeed;

            // Верхняя точка
            highPoint = new Vector3(
                (start.x + target.x) / 2,
                Mathf.Max(start.y, target.y) + 15f, // поднимаем выше старта/цели
                (start.z + target.z) / 2
            );

            // Дополнительные промежуточные точки для плавного перехода
            mid1 = Vector3.Lerp(start, highPoint, 0.5f);
            mid2 = Vector3.Lerp(highPoint, target, 0.5f);

            // Расчет общей дистанции для определения времени полета
            float distance = Vector3.Distance(start, mid1) +
                             Vector3.Distance(mid1, highPoint) +
                             Vector3.Distance(highPoint, mid2) +
                             Vector3.Distance(mid2, target);

            flightDuration = distance / ((slowSpeed + highSpeed) / 2);
            lifeBeforeDestroy = flightDuration;

            gameObject.SetActive(true);
            SoundPoolManager.Instance.PlaySound(soundShotPrefab);

            StartCoroutine(LifeBeforeDestroy());
        }

        /// <summary>
        /// Четырехточечный Bezier для плавной траектории
        /// </summary>
        private protected Vector3 GetBezierPoint(float t)
        {
            // Линейные интерполяции между точками
            Vector3 a = Vector3.Lerp(startPos, mid1, t);
            Vector3 b = Vector3.Lerp(mid1, highPoint, t);
            Vector3 c = Vector3.Lerp(highPoint, mid2, t);
            Vector3 d = Vector3.Lerp(mid2, targetPos, t);

            Vector3 ab = Vector3.Lerp(a, b, t);
            Vector3 bc = Vector3.Lerp(b, c, t);
            Vector3 cd = Vector3.Lerp(c, d, t);

            Vector3 abc = Vector3.Lerp(ab, bc, t);
            Vector3 bcd = Vector3.Lerp(bc, cd, t);

            return Vector3.Lerp(abc, bcd, t);
        }
    }
}
