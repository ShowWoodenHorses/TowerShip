using Assets.Scripts.TowerDefence.UI;
using UnityEngine;

namespace Assets.Scripts.TowerDefence
{
    public class Tile : MonoBehaviour
    {
        [HideInInspector] public Tower tower;

        public bool IsEmpty => tower == null;
        private Renderer rend;
        private Color defaultColor;
        public Color highlightColor = Color.green;

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            defaultColor = rend.material.color;
        }

        private void OnMouseEnter()
        {
            if (BuildManager.Instance.IsInBuildMode && IsEmpty)
                rend.material.color = highlightColor;
        }

        private void OnMouseExit()
        {
            rend.material.color = defaultColor;
        }

        private void OnMouseDown()
        {
            if (BuildManager.Instance.IsInBuildMode)
            {
                BuildManager.Instance.TryBuildTowerOn(this);
            }
            else if (!IsEmpty)
            {
                TowerActionUI.Instance.OpenForTower(this);
            }
        }

        public void PlaceTower(Tower tower)
        {
            this.tower = tower;
        }

        public void ClearTower()
        {
            tower = null;
        }
    }
}