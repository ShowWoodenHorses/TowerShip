using Assets.Scripts.Interface;
using Assets.Scripts.ObjectPool;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyDemolitionist : MonoBehaviour, IDamagable
    {
        private GameObject prefabRef;

        [Header("Componets")]
        [SerializeField] private NavMeshAgent agent;

        [Header("Settings")]
        [SerializeField] private int startHealth;
        [SerializeField] private int damage;
        [SerializeField] private float minDistanceForDamage = 1f;

        [Header("Effect")]
        [SerializeField] private GameObject effectExplosion;

        private int currentHealth;
        private Transform target;
        private Vector3 finishPos;
        private bool isDied = false;

        public void Initialize(GameObject prefabRef)
        {
            currentHealth = startHealth;
            this.prefabRef = prefabRef;
            isDied = false;

            if (agent == null)
            {
                agent = gameObject.GetComponent<NavMeshAgent>();
            }

        }

        private void Update()
        {
            if (finishPos == null) return;

            if (target == null) return;

            if (isDied) return; 

            float  distance = Vector3.Distance(finishPos, transform.position);
            if(distance < minDistanceForDamage)
            {
                PLayerHealth player = target.gameObject.GetComponent<PLayerHealth>();
                player.TakeDamage(damage);
                Die();
            }
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    var player = other.gameObject.GetComponent<PLayerHealth>();
        //    if (player != null)
        //    {
        //    }
        //}

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void SetStartPosition(Vector3 startPosition)
        {
            agent.Warp(startPosition);
            transform.rotation = Quaternion.identity;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            var playerCollider = target.GetComponent<BoxCollider>();
            Vector3 closestPoint = playerCollider.ClosestPoint(transform.position);
            finishPos = closestPoint;
            agent.SetDestination(closestPoint);
        }

        private void Die()
        {
            isDied = true;
            SpawnEffect();
            Deactive();
        }

        private protected void Deactive()
        {
            EnemyObjectPool.Instance.ReturnObject(prefabRef);
        }
        private void SpawnEffect()
        {
            GameObject effect = EffectObjectPool.Instance.GetObject(effectExplosion);
            effect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            EffectController effectController = effect.GetComponent<EffectController>();
            if (effectController != null)
            {
                effectController.Initialize(effect);
            }
        }

        public bool IsDiedEnemy()
        {
            return false;
        }

        public void SetCheckDiedEnemy()
        {
            return;
        }

        public bool CheckDiedEnemy()
        {
            return false;
        }


    }
}