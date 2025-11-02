using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TowerDefence.Configs
{
    [CreateAssetMenu(menuName = "TD/TowerData")]
    public class TowerData : ScriptableObject
    {
        public string towerName;
        public GameObject prefab;

        [Header("Economy")]
        public int baseCost = 100;
        public float upgradeMultiplier = 1.5f;
        public float sellRefundFactor = 0.6f;

        [Header("Stats")]
        public float baseDamage = 10f;
        public float range = 3f;
        public float fireRate = 1f;
    }
}