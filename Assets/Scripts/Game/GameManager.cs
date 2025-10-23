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
        public List<BuildController> buildings;

        [SerializeField] private int countDestroyedBuildingForWin;

        [SerializeField] private UIController uiController;
        [SerializeField] private ScoreManager scoreManager;

        public void Initialize(UIController uiController, ScoreManager scoreManager)
        {
            this.uiController = uiController;
            this.scoreManager = scoreManager;

            PLayerHealth.OnPlayerDie += CheckPlayerHealth;

            //foreach (var build in buildings)
            //{
            //    build.Initialize();
            //    build.OnBuildingDestroyed += CheckCurrentCountBuildings;
            //}

            //countDestroyedBuildingForWin = buildings.Count;
        }

        private void CheckCurrentCountBuildings(BuildController buildController)
        {
            int reward = buildController.GetReward();
            scoreManager.AddMoney(reward);

            countDestroyedBuildingForWin--;
            if (countDestroyedBuildingForWin <= 0)
            {
                YG2.InterstitialAdvShow();
                uiController.ShowWinPanel();
            }
        }

        private void CheckPlayerHealth(GameObject obj)
        {
            YG2.InterstitialAdvShow();
            uiController.ShowLosePanel();
        }

        private void OnDisable()
        {
            //foreach (var build in buildings)
            //{
            //    build.OnBuildingDestroyed -= CheckCurrentCountBuildings;
            //}

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
    }
}