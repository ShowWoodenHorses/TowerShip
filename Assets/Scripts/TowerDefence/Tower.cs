using System.Collections;
using Assets.Scripts.TowerDefence.Configs;
using UnityEngine;

namespace Assets.Scripts.TowerDefence
{
    public class Tower : MonoBehaviour
    {
        public TowerData data;
        public int level = 1;
        public int TotalCost { get; private set; }

        public void Initialize(TowerData towerData)
        {
            data = towerData;
            level = 1;
            TotalCost = data.baseCost;
        }

        public int GetUpgradeCost() =>
            Mathf.RoundToInt(data.baseCost * Mathf.Pow(data.upgradeMultiplier, level));

        public bool TryUpgrade(ref int playerMoney)
        {
            int cost = GetUpgradeCost();
            if (playerMoney < cost)
                return false;

            playerMoney -= cost;
            level++;
            TotalCost += cost;
            return true;
        }

        public int GetSellValue() =>
            Mathf.RoundToInt(TotalCost * data.sellRefundFactor);
    }
}