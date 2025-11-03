using System.Collections;
using Assets.Scripts.Interface;
using Assets.Scripts.ObjectPool;
using DG.Tweening;
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

        [Header("Animation")]
        [SerializeField] private float wobbleAmount;
        [SerializeField] private float wobbleDuration;

        private Tween anim;

        public void InitializeBomb(Vector3 startPos)
        {
            transform.position = new Vector3(startPos.x, posY, startPos.z);
            Animation();
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
            SpawnEffect();
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

        private void Animation()
        {
            anim = transform
                .DOLocalRotate(new Vector3(0, 0, wobbleAmount), wobbleDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            if (anim != null)
                anim.Kill();
        }
    }
}