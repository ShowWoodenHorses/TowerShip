using System.Collections;
using Assets.Scripts.TowerDefence.Configs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.TowerDefence.UI;

namespace Assets.Scripts.TowerDefence
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance;

        [Header("Economy")]
        public int playerMoney = 500;

        [Header("Grid")]
        public List<Tile> allTiles = new List<Tile>();

        [Header("Ghost Settings")]
        public Color ghostValidColor = new Color(0f, 1f, 0f, 0.5f);
        public Color ghostInvalidColor = new Color(1f, 0f, 0f, 0.5f);
        public LayerMask tileLayerMask; // установить в инспекторе на слой Tile

        // runtime
        private TowerData selectedTowerData;
        private GameObject ghostInstance;
        private Renderer[] ghostRenderers;
        private Tile hoveredTile;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            Instance = this;
        }

        [ContextMenu("Init Tiles")]
        public void initTiles()
        {
            allTiles.Clear();
            allTiles = new List<Tile>(FindObjectsByType<Tile>(FindObjectsSortMode.None));
        }

        private void Update()
        {
            HandleHoverAndGhost();
            HandleMouseInput();
        }

        private void HandleHoverAndGhost()
        {
            if (!IsInBuildMode())
            {
                if (ghostInstance != null && ghostInstance.activeSelf) ghostInstance.SetActive(false);
                hoveredTile = null;
                return;
            }

            // Рендерим луч от камеры к курсору и смотрим, попали ли в Tile (по layer)
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, tileLayerMask))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                hoveredTile = tile;
                if (tile != null)
                {
                    ShowGhostOn(tile);
                }
                else
                {
                    if (ghostInstance != null) ghostInstance.SetActive(false);
                }
            }
            else
            {
                if (ghostInstance != null) ghostInstance.SetActive(false);
                hoveredTile = null;
            }
        }

        private void HandleMouseInput()
        {
            // не реагируем, если курсор над UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                // если режим строительства — пробуем построить
                if (IsInBuildMode())
                {
                    if (hoveredTile != null)
                    {
                        TryBuildTowerOn(hoveredTile);
                    }
                }
                else
                {
                    // не в режиме строительства — клики по Tile обрабатываются Tile (или другим кодом)
                }
            }
        }

        public void SelectTowerType(TowerData data)
        {
            TowerActionUI.Instance?.Close();
            if (selectedTowerData == data)
            {
                CancelBuildMode();
                return;
            }

            selectedTowerData = data;
            CreateGhost();
            HighlightAvailableTiles(true);
        }

        public void CancelBuildMode()
        {
            selectedTowerData = null;
            DestroyGhost();
            HighlightAvailableTiles(false);
        }

        public bool IsInBuildMode() => selectedTowerData != null;

        public void TryBuildTowerOn(Tile tile)
        {
            Debug.Log($"TryBuildTowerOn called. selectedTowerData={(selectedTowerData != null ? selectedTowerData.towerName : "null")}, tile.IsEmpty={tile.IsEmpty}, playerMoney={playerMoney}");

            if (selectedTowerData == null)
                return;

            if (tile == null)
                return;

            if (!tile.IsEmpty)
            {
                Debug.Log("Tile is not empty. Can't build.");
                return;
            }

            if (playerMoney < selectedTowerData.baseCost)
            {
                Debug.Log("Not enough money to build.");
                return;
            }

            // оплачиваем и ставим башню
            playerMoney -= selectedTowerData.baseCost;

            GameObject towerObj = Instantiate(selectedTowerData.prefab, tile.transform.position, Quaternion.identity);
            Tower tower = towerObj.GetComponent<Tower>();
            if (tower == null)
            {
                Debug.LogWarning("Prefab missing Tower component!");
            }
            else
            {
                tower.Initialize(selectedTowerData);
                tile.PlaceTower(tower);
                Debug.Log($"Built {selectedTowerData.towerName} at tile {tile.name}. Remaining money: {playerMoney}");
            }

            UIManager.Instance.UpdateMoney();

            // ghost остаётся (позволяет ставить ещё)
            // если хочешь, чтобы после каждой постройки ghost проверял валидность (цвет)
            UpdateGhostVisualForTile(tile);
        }

        private void HighlightAvailableTiles(bool enable)
        {
            foreach (var tile in allTiles)
            {
                if (tile == null) continue;
                if (enable && tile.IsEmpty)
                    tile.SetHighlight(true);
                else
                    tile.SetHighlight(false);
            }
        }

        #region Ghost

        private void CreateGhost()
        {
            DestroyGhost();

            if (selectedTowerData == null)
                return;

            // Instantiate активный объект (в случае root inactive, явно включаем)
            ghostInstance = Instantiate(selectedTowerData.prefab);
            ghostInstance.name = "Ghost_" + selectedTowerData.towerName;
            ghostInstance.SetActive(true);

            // Получаем рендереры и применяем полупрозрачные материалы
            ghostRenderers = ghostInstance.GetComponentsInChildren<Renderer>();
            foreach (var r in ghostRenderers)
            {
                // клонируем материал чтобы не менять оригинал префаба
                r.material = new Material(r.sharedMaterial);
                r.material.color = ghostInvalidColor;
            }

            // Отключаем скрипты и коллайдеры (чтобы ghost не мешал физике и не реагировал)
            DisableBehaviorAndColliders(ghostInstance);

            // Опционально: поместить ghost в отдельный слой (Ignore Raycast) чтобы он не мешал кликам
            SetLayerRecursively(ghostInstance, LayerMask.NameToLayer("Ignore Raycast"));
        }

        private void ShowGhostOn(Tile tile)
        {
            if (ghostInstance == null) CreateGhost();
            if (ghostInstance == null) return;

            ghostInstance.SetActive(true);
            ghostInstance.transform.position = tile.transform.position;

            bool canBuild = tile.IsEmpty && playerMoney >= selectedTowerData.baseCost;

            Color c = canBuild ? ghostValidColor : ghostInvalidColor;
            if (ghostRenderers != null)
            {
                foreach (var r in ghostRenderers)
                {
                    if (r == null) continue;
                    r.material.color = c;
                }
            }
        }

        private void UpdateGhostVisualForTile(Tile tile)
        {
            // если после постройки нужно обновить подсветку для всех клеток и цвет ghost
            HighlightAvailableTiles(true);
            if (hoveredTile != null && ghostInstance != null)
                ShowGhostOn(hoveredTile);
        }

        private void DestroyGhost()
        {
            if (ghostInstance != null)
            {
                Destroy(ghostInstance);
                ghostInstance = null;
                ghostRenderers = null;
            }
        }

        private void DisableBehaviorAndColliders(GameObject go)
        {
            foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>())
            {
                mb.enabled = false;
            }
            foreach (var col in go.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            if (obj == null) return;
            obj.layer = layer;
            foreach (Transform t in obj.transform)
                SetLayerRecursively(t.gameObject, layer);
        }

        #endregion
    }
}