using Assets.Scripts.TowerDefence.UI;
using UnityEngine;

namespace Assets.Scripts.TowerDefence
{
    [RequireComponent(typeof(Collider))]
    public class Tile : MonoBehaviour
    {
        [HideInInspector] public Tower tower;
        public Color baseColor = Color.white;
        public Color highlightColor = Color.green;
        public Renderer rend;

        private void Awake()
        {
            if (rend == null) rend = GetComponent<Renderer>();
            if (rend != null) rend.material.color = baseColor;
        }

        public bool IsEmpty => tower == null;

        public void PlaceTower(Tower t)
        {
            tower = t;
        }

        public void ClearTower()
        {
            tower = null;
        }

        // BuildManager вызывает это для подсветки всех пустых клеток
        public void SetHighlight(bool on)
        {
            if (rend == null) return;
            rend.material.color = on && IsEmpty ? highlightColor : baseColor;
        }
    }
}