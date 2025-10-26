using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class GunAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject defaultState;
        [SerializeField] private GameObject shootState;
        public float delayResetAnim = 0.3f;

        public virtual void InitializeAnim()
        {
            defaultState.SetActive(true);
            shootState.SetActive(false);
        }
        public virtual void PlayAnim()
        {
            defaultState.SetActive(false);
            shootState.SetActive(true);
        }

        public virtual void ResetAnim()
        {
            StartCoroutine(DelayBeforeResetAnim());
        }

        public virtual IEnumerator DelayBeforeResetAnim()
        {
            yield return new WaitForSeconds(delayResetAnim);
            InitializeAnim();
        }
    }
}