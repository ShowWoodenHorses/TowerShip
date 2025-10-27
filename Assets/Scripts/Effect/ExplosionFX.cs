using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Effect
{
    public class ExplosionFX : EffectController
    {
        [SerializeField] private ParticleSystem flameBurst;
        [SerializeField] private ParticleSystem smoke;
        [SerializeField] private ParticleSystem sparks;

        public override void Initialize(GameObject obj)
        {
            PlayExplosion();
            base.Initialize(obj);
        }

        public void PlayExplosion()
        {
            flameBurst.Play();
            smoke.Play();
            sparks.Play();
        }
    }
}