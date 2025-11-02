using System.Collections;
using Assets.Scripts.TowerDefence.Configs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TowerDefence
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager Instance;

        [Header("Economy")]
        public int playerMoney = 500;

        [Header("Grid")]
        public List<Tile> allTiles = new List<Tile>();

        private TowerData selectedTowerData;
        public bool IsInBuildMode => selectedTowerData != null;

        [Header("Ghost Settings")]
        private GameObject ghostInstance;
        private Renderer[] ghostRenderers;
        public Color ghostValidColor = new Color(0, 1, 0, 0.3f);
        public Color ghostInvalidColor = new Color(1, 0, 0, 0.3f);

        private Tile hoveredTile;

        private void Awake() => Instance = this;

        private void Update()
        {
            if (!IsInBuildMode) return;

            // Наведение мыши для отображения призрака
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                hoveredTile = tile;

                if (tile != null)
                {
                    UpdateGhost(tile);
                }
            }
            else if (ghostInstance)
            {
                ghostInstance.SetActive(false);
            }
        }

        public void SelectTowerType(TowerData data)
        {
            if (selectedTowerData == data)
            {
                CancelBuildMode();
                return;
            }

            selectedTowerData = data;
            HighlightAvailableTiles(true);
            CreateGhost();
        }

        public void CancelBuildMode()
        {
            selectedTowerData = null;
            HighlightAvailableTiles(false);
            DestroyGhost();
        }

        public void TryBuildTowerOn(Tile tile)
        {
            if (!tile.IsEmpty || selectedTowerData == null)
                return;

            if (playerMoney < selectedTowerData.baseCost)
                return;

            playerMoney -= selectedTowerData.baseCost;

            GameObject towerObj = Instantiate(selectedTowerData.prefab, tile.transform.position, Quaternion.identity);
            Tower tower = towerObj.GetComponent<Tower>();
            tower.Initialize(selectedTowerData);

            tile.PlaceTower(tower);
            UIManager.Instance.UpdateMoney();
        }

        private void HighlightAvailableTiles(bool enable)
        {
            foreach (var tile in allTiles)
            {
                var rend = tile.GetComponent<Renderer>();
                if (enable && tile.IsEmpty)
                    rend.material.color = Color.green;
                else
                    rend.material.color = Color.white;
            }
        }

        #region GHOST

        private void CreateGhost()
        {
            if (ghostInstance != null)
                Destroy(ghostInstance);

            ghostInstance = Instantiate(selectedTowerData.prefab);
            ghostInstance.name = "GhostTower";
            ghostRenderers = ghostInstance.GetComponentsInChildren<Renderer>();
            foreach (var r in ghostRenderers)
            {
                r.material = new Material(r.material);
                r.material.color = ghostInvalidColor;
            }

            DisableTowerScripts(ghostInstance);
        }

        private void UpdateGhost(Tile tile)
        {
            if (ghostInstance == null) return;
            ghostInstance.SetActive(true);
            ghostInstance.transform.position = tile.transform.position;

            bool canBuild = tile.IsEmpty && playerMoney >= selectedTowerData.baseCost;

            foreach (var r in ghostRenderers)
                r.material.color = canBuild ? ghostValidColor : ghostInvalidColor;
        }

        private void DestroyGhost()
        {
            if (ghostInstance != null)
                Destroy(ghostInstance);
        }

        private void DisableTowerScripts(GameObject tower)
        {
            foreach (var mono in tower.GetComponentsInChildren<MonoBehaviour>())
            {
                mono.enabled = false;
            }
            foreach (var collider in tower.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }

        #endregion
    }
}