using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TowerDefence.Configs
{
    [CreateAssetMenu(menuName = "TD/TowerData")]
    public class TowerData : ScriptableObject
    {
        public string towerName;
        public string towerName_EN, towerName_RU, towerName_TR;
        public GameObject prefab;
        public int maxLevel = 3;

        [Header("Economy")]
        public int baseCost = 100;
        public float upgradeMultiplier = 1.5f;
        public float sellRefundFactor = 0.6f;

        [Header("Level 1")]
        public int damage_level_1;
        public float reload_level_1;
        public float minDistance_level_1;
        public float maxDistance_level_1;
        public int cost_level_1;

        [Header("Level 2")]
        public int damage_level_2;
        public float reload_level_2;
        public float minDistance_level_2;
        public float maxDistance_level_2;
        public int cost_level_2;

        [Header("Level 3")]
        public int damage_level_3;
        public float reload_level_3;
        public float minDistance_level_3;
        public float maxDistance_level_3;
        public int cost_level_3;
    }
}