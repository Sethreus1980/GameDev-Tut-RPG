using RPG.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPG.UI.Shops
{

    public class RowUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameField;
        [SerializeField] Image iconField;
        [SerializeField] TextMeshProUGUI availabilityField;
        [SerializeField] TextMeshProUGUI priceField;

        Shop currentShop = null;
        ShopItem item = null;

        public void Setup(Shop currentShop, ShopItem item)
        {
            this.currentShop = currentShop;
            this.item = item;
            iconField.sprite = item.GetIcon();
            nameField.text = item.GetName();
            availabilityField.text = $"{item.GetAvailability()}";
            priceField.text = $"${item.GetPrice():N2}";
        }
        public void Add()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), 1);
        }
        public void Remove()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), -1);
        }

    }
}