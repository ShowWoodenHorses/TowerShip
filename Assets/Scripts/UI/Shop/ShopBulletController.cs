using System.Collections.Generic;
using Assets.Scripts.Save;

namespace Assets.Scripts.UI.Shop
{
    public class ShopBulletController : ShopController
    {
        //public void Initialize(List<string> saveAvaliableItems, string currentItemsId, SaveLifecycle saveLifecycle)
        //{
        //    this.saveLifecycle = saveLifecycle;
        //    UpdateAvaliableItems(saveAvaliableItems, currentItemsId);
        //    base.CreateShopItems();
        //}
        public override void UpdateItem(string id)
        {
            //shipManager.UpgradeBullet(id);
            saveLifecycle.SelectBullet(id);
        }

        private void UpdateAvaliableItems(List<string> saveAvaliableItems, string currentItemsId)
        {
            foreach (string item in saveAvaliableItems)
            {
                if(!avaliableItems.Contains(item))
                    avaliableItems.Add(item);
            }

            if(avaliableItems.Contains(currentItemsId))
                currentIdItem = currentItemsId;
        }
    }
}