using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Assets.Scripts.TowerDefence.UI
{
    /// <summary>
    /// Панель улучшения/продажи башни. Появляется над выбранной башней.
    /// </summary>
    public class TowerActionUI : MonoBehaviour
    {
        public static TowerActionUI Instance;

        [Header("UI References")]
        public GameObject panel;                        // Родитель панели
        public TextMeshProUGUI towerInfoText;           // Текст с данными башни
        public Button upgradeButton;
        public Button sellButton;

        [Header("Offset")]
        public Vector3 worldOffset = new Vector3(0, 2f, 0); // Смещение панели от башни

        private Tile currentTile;
        private Camera mainCamera;
        private bool isVisible => panel.activeSelf;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            mainCamera = Camera.main;
            Close();
        }

        private void Update()
        {
            if (!isVisible || currentTile == null)
                return;

            // Следим за позицией башни на экране
            Vector3 screenPos = mainCamera.WorldToScreenPoint(currentTile.transform.position + worldOffset);
            panel.transform.position = screenPos;

            // Автоматическое закрытие, если клик в пустое место
            if (Input.GetMouseButtonDown(0))
            {
                // Игнорируем клики по UI
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                // Raycast по сцене — если кликнули не по текущей башне, закрываем
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Tile clickedTile = hit.collider.GetComponent<Tile>();
                    if (clickedTile == null || clickedTile != currentTile)
                    {
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// Открывает панель для указанной башни.
        /// </summary>
        public void OpenForTower(Tile tile)
        {
            currentTile = tile;
            Tower tower = tile.tower;

            if (tower == null)
            {
                Close();
                return;
            }

            // Обновляем информацию
            towerInfoText.text =
                $"{tower.data.towerName} L{tower.level}\n" +
                $"Upgrade: {tower.GetUpgradeCost()}$\n" +
                $"Sell: {tower.GetSellValue()}$";

            // Устанавливаем позицию UI над башней
            Vector3 screenPos = mainCamera.WorldToScreenPoint(tile.transform.position + worldOffset);
            panel.transform.position = screenPos;

            // Активируем панель
            panel.SetActive(true);

            // Перепривязываем кнопки
            upgradeButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();

            upgradeButton.onClick.AddListener(OnUpgrade);
            sellButton.onClick.AddListener(OnSell);
        }

        public void OnUpgrade()
        {
            if (currentTile == null) return;
            Tower tower = currentTile.tower;
            if (tower == null) return;

            if (tower.TryUpgrade(ref BuildManager.Instance.playerMoney))
            {
                UIManager.Instance.UpdateMoney();
                OpenForTower(currentTile); // обновляем данные
            }
        }

        public void OnSell()
        {
            if (currentTile == null) return;

            Tower tower = currentTile.tower;
            if (tower == null) return;

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
