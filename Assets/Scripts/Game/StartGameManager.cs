using Assets.Scripts.Achievements;
using Assets.Scripts.Animation;
using Assets.Scripts.Save;
using Assets.Scripts.Scene;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.Audio;
using YG;

namespace Assets.Scripts.Game
{
    public class StartGameManager : MonoBehaviour
    {
        private SaveData data;

        [SerializeField] private GameObject ContinueButton;
        [SerializeField] private GameObject ConfirmationPanel;

        [SerializeField] private AudioSettingsManager audioSettingsManager;
        [SerializeField] private SoundPoolManager soundPoolManager;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private MusicManager musicManager;
        [SerializeField] private AchievementManager achievementManager;

        [Header("Save")]
        [SerializeField] private SaveLifecycle saveLifecycle;
        private void Awake()
        {
            ContinueButton.SetActive(false);
            ConfirmationPanel.SetActive(false);

            if (SaveSystem.IsExistsSave())
            {
                ContinueButton.SetActive(true);
            }

            data = SaveSystem.Load();

            saveLifecycle.Initialize(data);
        }

        private void Start()
        {
            audioSettingsManager.Initialize();
            soundPoolManager.Initialize(audioMixer);
            musicManager.Initialize(audioSettingsManager, audioMixer);
            achievementManager.Initialize();
        }

        public void LoadGame()
        {
            YG2.MetricaSend("startGame");
            LoadingScreen.LoadScene("SampleScene");

        }

        public void NewGame()
        {
            SaveSystem.New();
            LoadingScreen.LoadScene("SampleScene");
        }

        public void ShowConfirmationOrNewGame()
        {
            if (SaveSystem.IsExistsSave())
            {
                ConfirmationPanel.GetComponent<HopupAnimUI>().Hopup();
            }
            else
            {
                NewGame();
            }
        }
    }
}