using System;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using Assets.Scripts.Player;
using Assets.Scripts.Scene;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace Assets.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int countLostEnemyForLose;

        [SerializeField] private UIController uiController;
        [SerializeField] private ScoreManager scoreManager;

        private int currentCountLostEnemyForLose;

        public Action<int> OnEnemyLost;

        public void Initialize(UIController uiController)
        {
            this.uiController = uiController;
            currentCountLostEnemyForLose = countLostEnemyForLose;

            PLayerHealth.OnPlayerDie += CheckPlayerHealth;
        }


        private void CheckPlayerHealth(GameObject obj)
        {
            YG2.InterstitialAdvShow();
            uiController.ShowLosePanel();
        }

        private void OnDisable()
        {
            PLayerHealth.OnPlayerDie -= CheckPlayerHealth;
        }
        public void NewGame()
        {
            SaveSystem.New();
            LoadingScreen.LoadScene("SampleScene");
        }

        public void ExitMenu()
        {
            YG2.GameplayStop();
            LoadingScreen.LoadScene("StartScene");
        }

        public void DeleteSave()
        {
            SaveSystem.DeleteSave();
        }

        public void EnemyLost()
        {
            currentCountLostEnemyForLose--;

            OnEnemyLost?.Invoke(currentCountLostEnemyForLose);

            if (currentCountLostEnemyForLose <= 0)
            {
                uiController.ShowLosePanel();
            }
        }

        public int GetCountStartEnemyLost()
        {
            return countLostEnemyForLose;
        }
    }
}