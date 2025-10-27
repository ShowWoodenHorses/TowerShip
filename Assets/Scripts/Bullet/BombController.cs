using System.Collections;
using Assets.Scripts.Interface;
using Assets.Scripts.ObjectPool;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public class BombController : MonoBehaviour
    {
        [SerializeField] private int damageEnemy;
        [SerializeField] private float rangeDamage;
        [SerializeField] private float lifeBeforeDestroy;
        [SerializeField] private float posY = 1.5f;
        [SerializeField] private LayerMask layerMask;

        [Header("Effect")]
        [SerializeField] private GameObject effectObj;

        public void InitializeBomb(Vector3 startPos)
        {
            transform.position = new Vector3(startPos.x, posY, startPos.z);
            StartCoroutine(LifeBeforeDestroy());
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.GetComponent<IDamagable>() != null)
            {
                Explosion();
            }
        }

        private void Explosion()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, rangeDamage, layerMask);

            foreach (Collider hit in hits)
            {
                hit.gameObject.GetComponent<IDamagable>()?.TakeDamage(damageEnemy);
            }

            SpawnEffect();
            Deactive();
        }

        private IEnumerator LifeBeforeDestroy()
        {
            yield return new WaitForSeconds(lifeBeforeDestroy);
            Deactive();
        }

        private protected void Deactive()
        {
            BulletObjectPool.Instance.ReturnObject(gameObject);
        }
        private void SpawnEffect()
        {
            GameObject effect = EffectObjectPool.Instance.GetObject(effectObj);
            effect.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            EffectController effectController = effect.GetComponent<EffectController>();
            if (effectController != null)
            {
                effectController.Initialize(effect);
            }
        }
    }
}