using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public class BrakingBullet : BulletContoller
    {
        [SerializeField] private GameObject startBullet;  
        [SerializeField] private GameObject finishBullet;  // сетка раскрыта
        [SerializeField] private float timeBeforeFinish;

        private bool changeStateOnFinishBullet = false;

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

        public override void InitializeWithTimer(Vector3 pos, float distance)
        {
            changeStateOnFinishBullet = false;
            ChangeState(false);
            base.InitializeWithTimer(pos, distance);
        }

        private void ChangeState(bool oneSecondToFinish)
        {
            startBullet.SetActive(!oneSecondToFinish);
            finishBullet.SetActive(oneSecondToFinish);
        }
    }
}