using System.Collections;
using Assets.Scripts.Save;
using Assets.Scripts.TowerDefence.Configs;
using Assets.Scripts.TowerDefence.Controllers;
using UnityEngine;

namespace Assets.Scripts.TowerDefence
{
    public class Tower : MonoBehaviour
    {
        public TowerData data;
        public int level = 1;
        public int TotalCost { get; private set; }

        private TowerController towerController;
        private SaveLifecycle saveLifecycle;

        private int indexTile;

        public void Initialize(SaveLifecycle saveLifecycle, TowerData towerData, int indexTile, int level = 1)
        {
            towerController = GetComponent<TowerController>();
            this.saveLifecycle = saveLifecycle;
            data = towerData;
            this.indexTile = indexTile;
            this.level = level;
            UpdateSettings();
            TotalCost = data.baseCost;
        }

        public int GetUpgradeCost() =>
            Mathf.RoundToInt(data.baseCost * Mathf.Pow(data.upgradeMultiplier, level));

        public bool TryUpgrade(ScoreManager scoreManager)
        {
            int playerMoney = scoreManager.GetCurrentMoney();
            int cost = GetUpgradeCost();
            if (playerMoney < cost)
                return false;

            scoreManager.RemoveMoney(cost);
            level++;
            UpdateSettings();
            SaveSettings();
            TotalCost += cost;
            return true;
        }

        public int GetSellValue() =>
            Mathf.RoundToInt(TotalCost * data.sellRefundFactor);

        private void UpdateSettings()
        {
            switch(level)
            {
                case 1:
                    towerController.SetSettings(data.reload_level_1, data.minDistance_level_1, data.maxDistance_level_1, data.damage_level_1);
                    break;
                case 2:
                    towerController.SetSettings(data.reload_level_2, data.minDistance_level_2, data.maxDistance_level_2, data.damage_level_2);
                    break;
                default:
                    towerController.SetSettings(data.reload_level_3, data.minDistance_level_3, data.maxDistance_level_3, data.damage_level_3);
                    break;


            }
        }

        public int GetMaxLEvel()
        {
            return data.maxLevel;
        }

        public int GetCurrentLevel()
        {
            return level;
        }

        public TowerController GetTowerController()
        {
            return towerController;
        }

        private void SaveSettings()
        {
            saveLifecycle.UpdateTileTower(indexTile, data.towerName, level);
        }
    }
}