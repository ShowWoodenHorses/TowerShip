using System;
using Assets.Scripts.Configs;
using Assets.Scripts.Save;
using UnityEngine;
using YG;

namespace Assets.Scripts
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private int currentMoney;
        [SerializeField] private int allTimeMoney;
        [SerializeField] private EnemyWaveMinValues enemyWaveMinValues;

        public Action<string> OnUpdateWave;

        private SaveLifecycle saveLifecycle;

        public void Initialize(int currentMoney, int allMoney, SaveLifecycle saveLifecycle)
        {
            this.currentMoney = currentMoney;
            this.allTimeMoney = allMoney;
            this.saveLifecycle = saveLifecycle;
        }

        public void AddMoney(int amount)
        {
            currentMoney += amount;
            allTimeMoney += amount;
            saveLifecycle.AddMoney(currentMoney, allTimeMoney);
            YG2.SetLeaderboard("leaderboardTower", allTimeMoney);
            CheckAndSendForUpdate();
        }

        public void RemoveMoney(int amount)
        {
            currentMoney -= amount;
            saveLifecycle.RemoveMoney(currentMoney);
        }

        public int GetCurrentMoney()
        {
            return currentMoney;
        }

        public int GetAllTimeMoney()
        {
            return allTimeMoney;
        }

        private void CheckAndSendForUpdate()
        {
            if (enemyWaveMinValues.minValueEnemies.Length == 0) return;

            string currentWaveId;
            for (int i = enemyWaveMinValues.minValueEnemies.Length - 1; i >= 0; i--)
            {
                if (allTimeMoney >= enemyWaveMinValues.minValueEnemies[i].minScore)
                {
                    currentWaveId = enemyWaveMinValues.minValueEnemies[i].enemyWaveId;
                    saveLifecycle.ChangeWave(currentWaveId);
                    OnUpdateWave?.Invoke(currentWaveId);

                    YG2.MetricaSend("scoreCount", currentWaveId, allTimeMoney.ToString());
                    break;
                }
            }
        }
    }
}