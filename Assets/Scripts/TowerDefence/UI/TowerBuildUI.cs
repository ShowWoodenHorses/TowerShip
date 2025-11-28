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

        private TowerItemUI currentSelectedTiwerItem;

        public void Initialize()
        {
            foreach (var tower in towerOptions)
            {
                GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                Button btn = btnObj.GetComponent<Button>();
                TowerItemUI towerUi = btnObj?.GetComponent<TowerItemUI>();
                btn.onClick.AddListener(() => OnTowerButtonClick(tower, towerUi));

                if(towerUi != null)
                {
                    towerUi.Initialize(tower.baseCost.ToString(), tower.iconItem);
                }
            }
        }

        private void OnTowerButtonClick(TowerData tower, TowerItemUI towerItemUi)
        {
            if(currentSelectedTiwerItem != null)
            {
                currentSelectedTiwerItem.UnselectedItem();
            }

            towerItemUi.SelectedItem();
            currentSelectedTiwerItem = towerItemUi;

            BuildManager.Instance.SelectTowerType(tower, towerItemUi);
        }

        public void UnselectedCurrentItem() // Из инспектора
        {
            TowerActionUI.Instance.Close();
            if (currentSelectedTiwerItem != null)
            {
                currentSelectedTiwerItem.UnselectedItem();
            }
        }

        public void ClosePanel()
        {
            BuildManager.Instance.CancelBuildMode();
            gameObject.SetActive(false);
        }
    }
}