using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Animation
{
    public class RecoilAnimation : GunAnimation
    {
        [SerializeField] private float recoilDistance = 0.5f;
        [SerializeField] private float recoilDuration = 0.1f;
        [SerializeField] private float returnDuration = 0.2f;

        Tween tween;

        public override void InitializeAnim() { }

        public override void PlayAnim()
        {
            Transform cannonVisual = transform;
            Vector3 direction = transform.forward;
            Vector3 startLocalPos = cannonVisual.localPosition;

            tween = cannonVisual.DOLocalMove(startLocalPos - direction * recoilDistance, recoilDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    cannonVisual.DOLocalMove(startLocalPos, returnDuration).SetEase(Ease.OutQuad);
                })
                .SetLink(cannonVisual.gameObject);
        }

        public override void ResetAnim() { }

        private void OnDestroy()
        {
            tween.Kill();
        }
    }
}