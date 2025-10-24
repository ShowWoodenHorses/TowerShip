using System.Collections;
using Assets.Scripts.Save;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Shop
{
    public class ShopPlayerController : ShopController
    {
        public void Initialize(List<string> saveAvaliableItems, string currentItemsId, SaveLifecycle saveLifecycle)
        {
            this.saveLifecycle = saveLifecycle;
            UpdateAvaliableItems(saveAvaliableItems, currentItemsId);
            base.CreateShopItems();
        }
        public override void UpdateItem(string id)
        {
            playerManager.UpgradePlayer(id);
            saveLifecycle.SelectPlayer(id);
        }

        private void UpdateAvaliableItems(List<string> saveAvaliableItems, string currentItemsId)
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