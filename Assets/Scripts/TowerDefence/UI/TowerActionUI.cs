using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TowerDefence.UI
{
    // Панель улучшения/продажи башни
    public class TowerActionUI : MonoBehaviour
    {
        public static TowerActionUI Instance;

        public GameObject panel;
        public TMPro.TextMeshProUGUI towerInfoText;
        public Button upgradeButton;
        public Button sellButton;

        private Tile currentTile;

        private void Awake() => Instance = this;

        public void OpenForTower(Tile tile)
        {
            currentTile = tile;
            Tower tower = tile.tower;

            towerInfoText.text =
                $"{tower.data.towerName} L{tower.level}\n" +
                $"Upgrade: {tower.GetUpgradeCost()}$\n" +
                $"Sell: {tower.GetSellValue()}$";

            panel.SetActive(true);
        }

        public void OnUpgrade()
        {
            if (currentTile == null) return;
            Tower tower = currentTile.tower;

            if (tower.TryUpgrade(ref BuildManager.Instance.playerMoney))
            {
                UIManager.Instance.UpdateMoney();
                OpenForTower(currentTile); // обновить данные
            }
        }

        public void OnSell()
        {
            if (currentTile == null) return;

            Tower tower = currentTile.tower;
            int refund = tower.GetSellValue();
            BuildManager.Instance.playerMoney += refund;

            Destroy(tower.gameObject);
            currentTile.ClearTower();

            UIManager.Instance.UpdateMoney();
            Close();
        }

        public void Close()
        {
            panel.SetActive(false);
            currentTile = null;
        }
    }
}