using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class KatapultaAnimation : GunAnimation
    {
        [SerializeField] private GameObject coreForAnim;

        public override void InitializeAnim()
        {
            base.InitializeAnim();
            coreForAnim.SetActive(true);
        }

        public override void PlayAnim()
        {
            base.PlayAnim();
            coreForAnim.SetActive(false);
        }

        public override void ResetAnim()
        {
            StartCoroutine(DelayBeforeResetAnim());
        }

        public override IEnumerator DelayBeforeResetAnim()
        {
            yield return new WaitForSeconds(delayResetAnim);
            InitializeAnim();
        }
    }
}