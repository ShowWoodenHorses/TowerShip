using Assets.Scripts.Animation;
using Assets.Scripts.Enemy;
using UnityEngine;

namespace Assets.Scripts.Spawner
{
    public class SpawnFromShip : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private Transform spawnPos;
        [SerializeField] private GameObject prefabEnemy;

        [Header("")]
        [SerializeField] private float startReloadTime;

        private float reloadTime;
        private Transform playerTransform;

        public void Initialize(Transform target)
        {
            playerTransform = target;
            reloadTime = startReloadTime;
        }

        private void Update()
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime < 0)
            {
                SpawnEnemy();
                reloadTime = startReloadTime;
            }
        }

        private void SpawnEnemy()
        {
            GameObject enemy = EnemyObjectPool.Instance.GetObject(prefabEnemy);

            if (enemy == null) return;

            EnemyDemolitionist demolitionist = enemy.GetComponent<EnemyDemolitionist>();
            if(demolitionist != null)
            {
                demolitionist.Initialize(enemy);
                demolitionist.SetStartPosition(spawnPos.position);
                demolitionist.SetTarget(playerTransform);
            }
        }
    }
}