using System.Collections;
using Assets.Scripts.ObjectPool;
using UnityEngine;

namespace Assets.Scripts
{
    public class EffectController : MonoBehaviour
    {
        [SerializeField] private protected float lifeBeforeDestroy;

        private protected GameObject refObj;

        public virtual void Initialize(GameObject obj)
        {
            refObj = obj;
            StartCoroutine(LifeBeforeDestroy());
        }

        private protected void Deactive()
        {
            EffectObjectPool.Instance.ReturnObject(refObj);
        }

        private protected IEnumerator LifeBeforeDestroy()
        {
            yield return new WaitForSeconds(lifeBeforeDestroy);
            Deactive();
        }
    }
}