using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Bullet
{
    public class BombBullet : BulletContoller
    {
        [SerializeField] private Vector3[] bombPositions = new Vector3[5];
        [SerializeField] private float distanceBetweenBombs = 5f;
        [SerializeField] private GameObject prefabBomb;

        private protected override IEnumerator LifeBeforeDestroy()
        {
            yield return new WaitForSeconds(lifeBeforeDestroy);
            PLaySoundEffect(soundShotWavePrefab);
            SpawnEffectShotInWater();
            SpawnBombs();
            Deactive();
        }

        private void SpawnBombs()
        {
            bombPositions[0] = new Vector3(transform.position.x, 0f, transform.position.z);
            bombPositions[1] = new Vector3(transform.position.x - distanceBetweenBombs, 0f, transform.position.z);
            bombPositions[2] = new Vector3(transform.position.x + distanceBetweenBombs, 0f, transform.position.z);
            bombPositions[3] = new Vector3(transform.position.x, 0f, transform.position.z - distanceBetweenBombs);
            bombPositions[4] = new Vector3(transform.position.x, 0f, transform.position.z + distanceBetweenBombs);

            foreach (var position in bombPositions)
            {
                GameObject obj = BulletObjectPool.Instance.GetObject(prefabBomb);
                var bomb = obj.GetComponent<BombController>();
                if(bomb != null)
                {
                    bomb.InitializeBomb(position);
                }
            }
        }
    }
}