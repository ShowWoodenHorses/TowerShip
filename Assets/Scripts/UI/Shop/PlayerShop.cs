using System.Collections.Generic;
using Assets.Scripts.Animation;
using Assets.Scripts.Configs;
using Assets.Scripts.Save;
using TMPro;
using UnityEngine;
using Assets.Scripts.Enum;
using UnityEngine.UI;
using YG;
using Assets.Scripts.Player;

namespace Assets.Scripts.UI.Shop
{
    public class PlayerShop : MonoBehaviour
    {
        private ItemStatus itemStatus;
        private ShopItemData selectedShopItemData;

        [Header("Buttons")]
        [SerializeField] private GameObject buyButton;
        [SerializeField] private GameObject selectButton;
        [SerializeField] private GameObject alreadySelectButton;
        [SerializeField] private List<GameObject> buttons = new();

        [Header("Main info")]
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI nameItem;
        [SerializeField] private TextMeshProUGUI descriptionItem;
        [SerializeField] private protected string currentIdItem; //Для сохранения

        [Header("Create shop")]
        [SerializeField] private protected ShopItemConfig[] shopItemConfigs;
        [SerializeField] private protected GameObject prefabShopItem;
        [SerializeField] private protected Transform parentPosition;

        [Header("Items")]
        [SerializeField] private protected List<string> avaliableItems; //Для сохранения
        [SerializeField] private protected List<GameObject> allItems;

        [Header("Another")]
        [SerializeField] private GameObject noMoneyObj;

        private ScoreManager scoreManager;
        private SaveLifecycle saveLifecycle;
        private PlayerManager playerManager;

        public void Initialize(ScoreManager scoreManager, SaveLifecycle saveLifecycle, PlayerManager playerManager, List<string> saveAvaliableItems, string currentItemsId)
        {
            this.scoreManager = scoreManager;
            this.saveLifecycle = saveLifecycle;
            this.playerManager = playerManager;

            LoadItemFromSave(saveAvaliableItems, currentItemsId);
            CreateShopItems();
            UpdateAvaliableItems();

            buyButton.GetComponent<Button>().onClick.AddListener(() => PurchaseHandler(selectedShopItemData));
            selectButton.GetComponent<Button>().onClick.AddListener(() => ChoiceItemHandler(selectedShopItemData));
        }

        private void UpdateAvaliableItems()
        {
            foreach (var item in allItems)
            {
                var data = item.GetComponent<ShopItemData>();

                if(data.idItem == currentIdItem)
                {
                    data.SetItemStatus(ItemStatus.AlreadySelect);
                }
                else if (avaliableItems.Contains(data.idItem))
                {
                    data.SetItemStatus(ItemStatus.CanSelect);
                }
                else
                {
                    data.SetItemStatus(ItemStatus.CanBuy);
                }
            }
        }


        private void CreateShopItems()
        {
            for (int i = 0; i < shopItemConfigs.Length; i++)
            {
                ShopItemConfig config = shopItemConfigs[i];

                GameObject newItem = Instantiate(prefabShopItem, parentPosition);
                ShopItemData shopItemData = newItem.GetComponent<ShopItemData>();

                if (shopItemData == null) continue;

                shopItemData.Initialize(config);
                newItem.GetComponent<Button>().onClick.AddListener(() => UpdateItemInfo(shopItemData));

                allItems.Add(newItem);
            }
        }

        private void PurchaseHandler(ShopItemData itemData)
        {
            Debug.Log(itemData.idItem);

            if (scoreManager.GetCurrentMoney() < itemData.costItem)
            {
                noMoneyObj.GetComponent<HopupAnimUI>().Hopup();
                return;
            }

            scoreManager.RemoveMoney(itemData.costItem);
            avaliableItems.Add(itemData.idItem);
            saveLifecycle.BuyItem(itemData.idItem);

            UpdateStatus(ItemStatus.CanSelect);
            itemData.SetItemStatus(ItemStatus.CanSelect);

            YG2.MetricaSend("buyItem", itemData.idItem, itemData.costItem.ToString());
        }

        private void ChoiceItemHandler(ShopItemData itemData)
        {
            Debug.Log(itemData.idItem);

            if (avaliableItems.Contains(itemData.idItem))
            {
                UpdateItem(itemData.idItem);
                currentIdItem = itemData.idItem;

                UpdateStatus(ItemStatus.AlreadySelect);
                itemData.SetItemStatus(ItemStatus.AlreadySelect);

                YG2.MetricaSend("selectItem", itemData.idItem, itemData.costItem.ToString());

                UpdateAvaliableItems();
            }
        }

        private void UpdateItemInfo(ShopItemData data)
        {
            selectedShopItemData = data;
            itemImage.sprite = data.iconItem.sprite;
            descriptionItem.text = data.GetDescription();
            nameItem.text = data.GetName();
            UpdateStatus(data.GetCurrentStatus());
        }

        private void UpdateStatus(ItemStatus status)
        {
            itemStatus = status;

            foreach (var button in buttons)
            {
                button.SetActive(false);
            }

            switch (status)
            {
                case ItemStatus.CanBuy:
                    buyButton.SetActive(true);
                    break;
                case ItemStatus.CanSelect:
                    selectButton.SetActive(true); 
                    break;
                case ItemStatus.AlreadySelect:
                    alreadySelectButton.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void UpdateItem(string id)
        {
            playerManager.UpgradePlayer(id);
            saveLifecycle.SelectPlayer(id);
        }

        private void LoadItemFromSave(List<string> saveAvaliableItems, string currentItemsId)
        {
            foreach (string item in saveAvaliableItems)
            {
                if (!avaliableItems.Contains(item))
                    avaliableItems.Add(item);
            }

            if (avaliableItems.Contains(currentItemsId))
                currentIdItem = currentItemsId;
        }
    }
}