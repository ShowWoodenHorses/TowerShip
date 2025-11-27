using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance;

        [SerializeField] private List<Transform> enemies = new List<Transform>();

        public void Initialize()
        {
            Instance = this;
        }

        public void Register(Transform enemy)
        {
            enemies.Add(enemy);
        }

        public void Unregister(Transform enemy)
        {
            enemies.Remove(enemy);
        }

        public List<Transform> GetEnemies() => enemies;
    }
}