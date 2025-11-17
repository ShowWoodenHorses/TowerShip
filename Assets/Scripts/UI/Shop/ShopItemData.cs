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

        [Header("Main info")]
        public string idItem;
        public int costItem;
        public string description;
        public Image iconItem;

        [Header("Settings")]
        [SerializeField] private GameObject lockObject;
        [SerializeField] private Image backImage;
        [SerializeField] private Sprite defaultBackImage;
        [SerializeField] private Sprite selectedBackImage;

        [SerializeField] private ItemStatus itemStatus;

        public void Initialize(ShopItemConfig shopItemConfig)
        {
            this.idItem = shopItemConfig.idItem;
            this.costItemText.text = shopItemConfig.costItem.ToString();
            this.costItem = shopItemConfig.costItem;
            this.iconItem.sprite = shopItemConfig.iconItem;

            UpdateStatus(ItemStatus.CanBuy);

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

        private void UpdateStatus(ItemStatus newstatus)
        {
            switch (newstatus)
            {
                case ItemStatus.CanBuy:
                    lockObject.SetActive(true);
                    backImage.sprite = defaultBackImage;
                    break;
                case ItemStatus.CanSelect:
                    lockObject.SetActive(false);
                    backImage.sprite = defaultBackImage;
                    break;
                case ItemStatus.AlreadySelect:
                    lockObject.SetActive(false);
                    backImage.sprite = selectedBackImage;
                    break;
                default:
                    break;

            }
        }

        public void SetItemStatus(ItemStatus newStatus)
        {
            itemStatus = newStatus;
            UpdateStatus(itemStatus);
        }

        public void DeleteLock()
        {
            lockObject.SetActive(false);
        }

        public ItemStatus GetCurrentStatus()
        {
            return itemStatus;
        }

        public string GetDescription()
        {
            return description;
        }

        public string GetName()
        {
            return nameItemText.text;
        }

        public int GetCost()
        {
            return costItem;
        }
    }
}