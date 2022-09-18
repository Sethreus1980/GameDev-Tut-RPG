using GameDevTV.Inventories;
using RPG.Control;
using RPG.Dialogue;
using RPG.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{

    public class Shop : MonoBehaviour, IRaycastable
    {
        [SerializeField] string shopName = "";        
        
        public event Action onChange;

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            yield return new ShopItem(InventoryItem.GetFromID("8363e21c-20fb-439b-89d5-abda75359373"),
                10, 10.0f, 0);
            yield return new ShopItem(InventoryItem.GetFromID("c7b79eb7-015c-40e7-8ee2-5b759a1c9385"),
                10, 10.0f, 0);
            yield return new ShopItem(InventoryItem.GetFromID("75f453bc-4649-40fd-baad-5ffc402bca54"),
                10, 10.0f, 0);
            yield return new ShopItem(InventoryItem.GetFromID("00c8c34a-f19a-4432-abd0-2c96d14c4d83"),
                10, 10.0f, 0);
        }
        public void SelectFilter(ItemCategory category)
        {

        }
        public ItemCategory GetFilter()
        {
            return ItemCategory.None;
        }
        public void SelectMode(bool isBuying)
        {

        }
        public bool IsBuyingMode()
        {
            return true;
        }
        public bool CanTransact()
        {
            return true;
        }

        public string GetShopName()
        {
            shopName = gameObject.GetComponent<AIConversant>().GetName() + "'s Shop";
            return shopName;
        }

        public void ConfirmTransaction()
        {

        }
        public float TransactionTotal()
        {
            return 0;
        }
        public void AddToTransaction(InventoryItem item, int quantity)
        {

        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Vector3.Distance(callingController.transform.position, this.transform.position) > 3.0f)
                {
                    callingController.GetComponent<Mover>().StartMoveAction(this.transform.position, 1.0f);                
                    callingController.GetComponent<Shopper>().SetActiveShop(this);
                }
                else
                    callingController.GetComponent<Shopper>().SetActiveShop(this);
            }
            return true;
        }
    }
}