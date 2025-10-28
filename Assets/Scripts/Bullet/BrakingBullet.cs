using System.Collections;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public class BrakingBullet : BulletContoller
    {
        [SerializeField] private GameObject startBullet;  
        [SerializeField] private GameObject finishBullet;  // сетка раскрыта
        [SerializeField] private float timeBeforeFinish;

        [SerializeField] private float brakingTime;

        private bool changeStateOnFinishBullet = false;
        public override void InitializeWithTimer(Vector3 pos, float distance)
        {
            changeStateOnFinishBullet = false;
            ChangeState(false);
            base.InitializeWithTimer(pos, distance);
        }

        private void Update()
        {
            lifeBeforeDestroy -= Time.deltaTime;
            if (!changeStateOnFinishBullet)
            {
                if(lifeBeforeDestroy < timeBeforeFinish)
                {
                    ChangeState(true);
                    changeStateOnFinishBullet = true;
                }
            }
        }

        private new void OnTriggerEnter(Collider other)
        {
            var objectForDamage = other.gameObject.GetComponent<IDamagable>();
            var building = other.gameObject.GetComponent<IObstaclable>();
            var brakingable = other.gameObject.GetComponent<IBrakingable>();

            if (brakingable != null)
            {
                PLaySoundEffect(soundTakeDamagePrefab);
                SpawnEffect(effectShotInEnemy);
                brakingable.Braking(brakingTime);
                Deactive();
            }

            if (objectForDamage != null)
            {
                PLaySoundEffect(soundTakeDamagePrefab);
                SpawnEffect(effectShotInEnemy);
                objectForDamage.TakeDamage(damageEnemy);
                Deactive();
            }

            else if (building != null)
            {
                PLaySoundEffect(soundTakeDamagePrefab);
                SpawnEffect(effectShotInBuilding);
                Deactive();
            }
        }

        private void ChangeState(bool oneSecondToFinish)
        {
            startBullet.SetActive(!oneSecondToFinish);
            finishBullet.SetActive(oneSecondToFinish);
        }
    }
}