using System.Collections.Generic;
using Assets.Scripts.Animation;
using Assets.Scripts.Configs;
using Assets.Scripts.Player;
using Assets.Scripts.Save;
using Assets.Scripts.UI.Shop;
using TMPro;
using UnityEngine;
using YG;

namespace Assets.Scripts.UI
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private protected ScoreManager scoreManager;
        [SerializeField] private protected PlayerManager playerManager;

        [SerializeField] private protected ShopItemConfig[] shopItemConfigs;
        [SerializeField] private protected List<string> avaliableItems; //Для сохранения
        [SerializeField] private protected List<GameObject> allItems;
        [SerializeField] private protected GameObject prefabShopItem;
        [SerializeField] private protected Transform parentPosition;

        [SerializeField] private protected string currentIdItem; //Для сохранения

        [SerializeField] private protected GameObject noMoneyObj;
        [SerializeField] private protected TextMeshProUGUI descriptionItem;

        private protected SaveLifecycle saveLifecycle;

        //Bootstrap
        //private protected void Start()
        //{
        //    CreateShopItems();
        //}

        private protected void UpdateAvaliableItems()
        {
            foreach(var item in allItems)
            {
                var data = item.GetComponent<ShopItemData>();
                foreach(var id in avaliableItems)
                {
                    if (data.idItem == currentIdItem)
                    {
                        data.UpdateButtons(data.SelectItemText);
                        continue;
                    }

                    if (data.idItem == id)
                    {
                        data.UpdateButtons(data.SelectButton);
                        continue;
                    }

                }
            }
        }

        private protected void CreateShopItems()
        {
            for (int i = 0; i < shopItemConfigs.Length; i++)
            {
                ShopItemConfig config = shopItemConfigs[i];

                GameObject newItem = Instantiate(prefabShopItem, parentPosition);
                ShopItemData shopItemData = newItem.GetComponent<ShopItemData>();

                if (shopItemData == null) continue;

                shopItemData.Initialize(config, descriptionItem);

                shopItemData.OnBuyItem += PurchaseHandler;
                shopItemData.OnSelectItem += ChoiceItemHandler;

                allItems.Add(newItem);
            }
            UpdateAvaliableItems();
        }

        private protected void PurchaseHandler(ShopItemData itemData)
        {
            Debug.Log(itemData.idItem);

            if (scoreManager.GetCurrentMoney() < itemData.costItem)
            {
                noMoneyObj.GetComponent<HopupAnimUI>().Hopup();
                return;
            }

            scoreManager.RemoveMoney(itemData.costItem);
            itemData.UpdateButtons(itemData.SelectButton);
            avaliableItems.Add(itemData.idItem);
            saveLifecycle.BuyItem(itemData.idItem);

            YG2.MetricaSend("buyItem", itemData.idItem, itemData.costItem.ToString());
        }

        private protected void ChoiceItemHandler(ShopItemData itemData)
        {
            Debug.Log(itemData.idItem);

            if (avaliableItems.Contains(itemData.idItem))
            {
                UpdateItem(itemData.idItem);
                currentIdItem = itemData.idItem;
                itemData.UpdateButtons(itemData.SelectItemText);

                YG2.MetricaSend("selectItem", itemData.idItem, itemData.costItem.ToString());

                UpdateAvaliableItems();
            }
        }

        public virtual void UpdateItem(string id)
        {
        }

        private protected void OnDestroy()
        {
            foreach(var item in allItems)
            {
                var data = item.GetComponent<ShopItemData>();
                if (data != null)
                {
                    data.OnSelectItem -= ChoiceItemHandler;
                    data.OnBuyItem -= PurchaseHandler;
                }
            }
        }
    }
}