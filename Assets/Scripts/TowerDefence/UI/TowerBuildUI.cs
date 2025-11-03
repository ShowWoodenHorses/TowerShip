using Assets.Scripts.TowerDefence.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TowerDefence.UI
{
    // Панель со всеми башнями
    public class TowerBuildUI : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public Transform buttonContainer;
        public TowerData[] towerOptions;

        public void Initialize()
        {
            foreach (var tower in towerOptions)
            {
                GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() => OnTowerButtonClick(tower));

                btnObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{tower.towerName}\n{tower.baseCost}$";
            }
        }

        private void OnTowerButtonClick(TowerData tower)
        {
            BuildManager.Instance.SelectTowerType(tower);
        }

        public void ClosePanel()
        {
            BuildManager.Instance.CancelBuildMode();
            gameObject.SetActive(false);
        }
    }
}