using System.Collections;
using Assets.Scripts.Sound;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public class MortiraCoreBullet : BulletContoller
    {
        [SerializeField] private float coefSpeed = 2f;

        private Vector3 directionHighPoint;
        private Vector3 directionTargetPoint;
        private Vector3 HighPoint;

        private float slowSpeed;
        private float highSpeed;
        private bool isGointToTarget;

        private void FixedUpdate()
        {
            if (!isGointToTarget)
            {
                if (Vector3.Distance(transform.position, HighPoint) < 0.5f)
                {
                    rb.linearVelocity = directionTargetPoint.normalized * highSpeed;
                    transform.rotation = Quaternion.LookRotation(directionTargetPoint.normalized);
                    isGointToTarget = true;
                }
            }
        }
        
        public void InitializeCore(Vector3 startPos, Vector3 endPos)
        {
            isGointToTarget = false;
            slowSpeed = speed - coefSpeed;
            highSpeed = speed + coefSpeed;

            float medianX = (startPos.x + endPos.x) / 2;
            float medianY = (startPos.y + endPos.y) / 2;
            float medianZ = (startPos.z + endPos.z) / 2;

            Vector3 median = new Vector3(medianX, medianY + startPos.y, medianZ);

            float distanceToHighPoint = Vector3.Distance(startPos, median);
            float distanceToTargetPoint = Vector3.Distance(median, endPos);
            float allDistance = distanceToHighPoint + distanceToTargetPoint;

            directionHighPoint = median - startPos;
            directionTargetPoint = endPos - median;
            HighPoint = median;

            float time = allDistance / ((slowSpeed + highSpeed) / 2);
            lifeBeforeDestroy = time;
            rb.linearVelocity = directionHighPoint.normalized * slowSpeed;
            transform.rotation = Quaternion.LookRotation(directionHighPoint.normalized);

            SoundPoolManager.Instance.PlaySound(soundShotPrefab);

            StartCoroutine(LifeBeforeDestroy());
        }
    }
}