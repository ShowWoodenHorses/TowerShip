using UnityEngine;
using TMPro;
using Assets.Scripts.Animation;
using YG;
using System.Collections;

namespace Assets.Scripts.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private PauseManager pauseManager;
        [SerializeField] private ScoreManager scoreManager;

        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        [SerializeField] private TextMeshProUGUI textMoney;
        [SerializeField] private TextMeshProUGUI textMoneyinStore;

        [Header("Wave text")]
        [SerializeField] private GameObject waveTextObject;
        [SerializeField] private TextMeshProUGUI waveText;
        public string ru, en, tr;
        private string currentWave;
        private string startText;

        public void Initialize(string waveId, ScoreManager scoreManager)
        {
            this.scoreManager = scoreManager;
            currentWave = waveId;

            SwitchLanguage(YG2.lang);
        }

        private void OnEnable()
        {
            scoreManager.OnUpdateWave += UpdateCurrentWave;
        }
        private void OnDisable()
        {
            scoreManager.OnUpdateWave -= UpdateCurrentWave;
        }

        private void Start()
        {
            startText = waveText.text;
            ShowWaveText();
        }

        private void Update()
        {
            textMoney.text = scoreManager.GetCurrentMoney().ToString();
            textMoneyinStore.text = scoreManager.GetCurrentMoney().ToString();
        }

        public void PauseButton()
        {
            pauseManager.Pause();
        }

        public void ResumeButton()
        {
            pauseManager.Resume();
        }

        public void ShowWinPanel()
        {
            YG2.MetricaSend("win");
            winPanel.GetComponent<HopupAnimUI>().Hopup();
            pauseManager.Pause();
        }
        public void ShowLosePanel()
        {
            YG2.MetricaSend("gameOver");
            losePanel.GetComponent<HopupAnimUI>().Hopup();
            pauseManager.Pause();
        }

        public void HideLosePanel()
        {
            losePanel.SetActive(false);
            pauseManager.Resume();
        }

        public void ShowWaveText()
        {
            waveText.text = startText + ": " + currentWave;
            waveTextObject.SetActive(true);

            StartCoroutine(HideWaveText());
        }

        private IEnumerator HideWaveText()
        {
            yield return new WaitForSeconds(2f);
            waveTextObject.SetActive(false);
        }

        private void UpdateCurrentWave(string waveId)
        {
            if (currentWave == waveId) return;

            currentWave = waveId;
            ShowWaveText();
        }

        private void SwitchLanguage(string lang)
        {
            switch (lang)
            {
                case "ru":
                    waveText.text = ru;
                    break;
                case "tr":
                    waveText.text = tr;
                    break;
                default:
                    waveText.text = en;
                    break;
            }
        }
    }
}