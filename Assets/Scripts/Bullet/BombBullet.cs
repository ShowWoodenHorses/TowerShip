using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public class BombBullet : MortiraCoreBullet
    {
        [SerializeField] private Vector3[] bombPositions = new Vector3[5];
        [SerializeField] private float distanceBetweenBombs = 5f;
        [SerializeField] private GameObject prefabBomb;

        [SerializeField] private List<GameObject> bombList = new List<GameObject>();
        [SerializeField] private float delayBetweenDisplayBomb = 0.1f;

        public override void InitializeCore(Vector3 start, Vector3 target)
        {
            HideAllDopBomb();

            base.InitializeCore(start, target);

            if (bombList != null)
            {
                StartCoroutine(ShowBombs());
            }
        }

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

        private protected override IEnumerator LifeBeforeDestroy()
        {
            yield return new WaitForSeconds(lifeBeforeDestroy);
            PLaySoundEffect(soundShotWavePrefab);
            SpawnEffectShotInWater();
            SpawnBombs();
            Deactive();
        }

        private void SpawnBombs()
        {
            bombPositions[0] = new Vector3(transform.position.x, 0f, transform.position.z);
            bombPositions[1] = new Vector3(transform.position.x - distanceBetweenBombs, 0f, transform.position.z);
            bombPositions[2] = new Vector3(transform.position.x + distanceBetweenBombs, 0f, transform.position.z);
            bombPositions[3] = new Vector3(transform.position.x, 0f, transform.position.z - distanceBetweenBombs);
            bombPositions[4] = new Vector3(transform.position.x, 0f, transform.position.z + distanceBetweenBombs);

            foreach (var position in bombPositions)
            {
                GameObject obj = BulletObjectPool.Instance.GetObject(prefabBomb);
                var bomb = obj.GetComponent<BombController>();
                if(bomb != null)
                {
                    bomb.InitializeBomb(position, setStat);
                    bomb.InitializeStatData(saveLifecycle, gunIdStat);
                }
            }
        }

        private void HideAllDopBomb()
        {
            if (bombList == null)
                return;

            foreach(var bomb in bombList)
            {
                bomb.SetActive(false);
            }
        }

        private IEnumerator ShowBombs()
        {
            foreach (var bomb in bombList)
            {
                yield return new WaitForSeconds(delayBetweenDisplayBomb);
                bomb.SetActive(true);
            }
        }
    }
}