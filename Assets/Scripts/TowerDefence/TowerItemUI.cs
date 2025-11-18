using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.TowerDefence
{
    public class TowerItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI costItemText;
        [SerializeField] private Image iconItem;
        [SerializeField] private Image baseBackground;

        [Header("Colors")]
        [SerializeField] private Sprite baseColor;
        [SerializeField] private Sprite selectedColor;

        public void Initialize(string costText, Sprite icon)
        {
            baseBackground.sprite = baseColor;
            costItemText.text = costText;
            iconItem.sprite = icon;
        }

        public void SelectedItem()
        {
            baseBackground.sprite = selectedColor;
        }

        public void UnselectedItem()
        {
            baseBackground.sprite = baseColor;
        }
    }
}