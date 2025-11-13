using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Configs;
using System;
using Assets.Scripts.Enum;
using YG;

namespace Assets.Scripts.UI.Shop
{
    public class ShopItemData : MonoBehaviour
    {
        public TextMeshProUGUI nameItemText;
        public TextMeshProUGUI costItemText;

        public string idItem;
        public int costItem;
        public string description;

        public Image iconItem;

        public GameObject BuyButton;
        public GameObject SelectButton;
        public GameObject SelectItemText;
        public List<GameObject> listButtons;

        public Action<ShopItemData> OnBuyItem;
        public Action<ShopItemData> OnSelectItem;

        private TextMeshProUGUI descriptionItem;
        [SerializeField] private ItemStatus itemStatus;

        private void AddButtons()
        {
            listButtons.Add(BuyButton);
            listButtons.Add(SelectButton);
            listButtons.Add(SelectItemText);
        }

        public void Initialize(ShopItemConfig shopItemConfig)
        {
            this.idItem = shopItemConfig.idItem;
            this.costItemText.text = shopItemConfig.costItem.ToString();
            this.costItem = shopItemConfig.costItem;
            this.iconItem.sprite = shopItemConfig.iconItem;

            if (YG2.lang == "en")
            {
                this.nameItemText.text = shopItemConfig.nameItemText_EN;
                this.description = shopItemConfig.description_EN;
            }

            else if (YG2.lang == "tr")
            {
                this.nameItemText.text = shopItemConfig.nameItemText_TR;
                this.description = shopItemConfig.description_TR;
            }
            else
            {
                this.nameItemText.text = shopItemConfig.nameItemText;
                this.description = shopItemConfig.description;
            }
        }

        public void BuyClick()
        {
            OnBuyItem?.Invoke(this);
        }

        public void SelectClick()
        {
            OnSelectItem?.Invoke(this);
        }

        public void UpdateButtons(GameObject button)
        {
            foreach (var btn in listButtons)
            {
                btn.SetActive(false);
            }
            button.SetActive(true);
        }

        public void SetDescription()
        {
            descriptionItem.text = description;
        }

        public string GetDescription()
        {
            return description;
        }

        public string GetName()
        {
            return nameItemText.text;
        }

        public void SetItemStatus(ItemStatus newStatus)
        {
            itemStatus = newStatus;
        }

        public ItemStatus GetCurrentStatus()
        {
            return itemStatus;
        }
    }
}