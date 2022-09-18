using GameDevTV.Inventories;
using RPG.Control;
using RPG.Core;
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
        
        // Stock Config
        // Item:
        // InventoryItem
        // Initial Stock
        // buyingDiscount

        [SerializeField] StockItemConfig[] stockConfig;

        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(-50, 100)]
            public float buyingDiscountPercentage;
        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();

        public event Action onChange;

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (StockItemConfig config in stockConfig)
            {
                float price = config.item.GetPrice() * (1- config.buyingDiscountPercentage/100);
                int quantityInTransaction = 0;
                transaction.TryGetValue(config.item, out quantityInTransaction);
                yield return new ShopItem(config.item, config.initialStock, price, quantityInTransaction);
            }
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
            //print($"Added to Transaction: {item.GetDisplayName()} x {quantity}");
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            transaction[item] += quantity;

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }
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
                    callingController.GetComponent<Mover>().MoveTo(this.transform.position, 1.0f);
                    callingController.GetComponent<Shopper>().SetActiveShop(this);
                }
                else
                {
                    callingController.GetComponent<Shopper>().SetActiveShop(this);
                }
            }
            return true;
        }
        //public void Update()
        //{
        //    Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //    if (playerTransform.GetComponent<Shopper>().GetActiveShop() == null)
        //    {                
        //        if (Vector3.Distance(playerTransform.position, this.transform.position) < 2.0f)
        //        {
        //            playerTransform.GetComponent<Mover>().Cancel();
        //            playerTransform.GetComponent<Shopper>().SetActiveShop(this);                    
        //        }
        //    }
        //}
    }
}