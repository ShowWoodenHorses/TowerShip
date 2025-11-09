using Assets.Scripts.TowerDefence.UI;
using Assets.Scripts.TowerDefence;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Save;

public class Tile : MonoBehaviour
{
    [HideInInspector] public Tower tower;
    public Color baseColor = Color.white;
    public Color highlightColor = Color.green;
    public Renderer rend;
    public int index;

    private SaveLifecycle saveLifecycle;

    public void Initialize(SaveLifecycle saveLifecycle)
    {
        this.saveLifecycle = saveLifecycle;
    }

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
        saveLifecycle.DestroyTower(index);
    }

    public void SetHighlight(bool on)
    {
        if (rend == null) return;
        rend.material.color = on && IsEmpty ? highlightColor : baseColor;
    }

    private void OnMouseDown()
    {
        // Игнорируем клики, когда курсор над UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Если на клетке есть башня — открыть меню действий
        if (!IsEmpty && TowerActionUI.Instance != null && BuildManager.Instance.towerList.activeInHierarchy)
        {
            TowerActionUI.Instance.Close();
            TowerActionUI.Instance.OpenForTower(this);
        }
    }
}
