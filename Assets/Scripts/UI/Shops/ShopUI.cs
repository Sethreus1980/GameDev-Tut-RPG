using RPG.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI.Shops
{

    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName;
        [SerializeField] Transform listRoot;
        [SerializeField] RowUI rowPrefab;

        Shopper shopper = null;
        Shop currentShop = null;

        // Start is called before the first frame update
        void Start()
        {   
            shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (shopper == null) return;

            shopper.activeShopChange += ShopChanged;

            ShopChanged();
        }

        // Update is called once per frame
        private void ShopChanged()
        {            
            currentShop = shopper.GetActiveShop();            
            gameObject.SetActive(currentShop != null);
            if (currentShop == null) return;
            shopName.text = currentShop.GetShopName();

            RefreshUI();
        }

        public void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (ShopItem item in currentShop.GetFilteredItems())
            {
                Instantiate<RowUI>(rowPrefab, listRoot);
            }
        }

        public void Close()
        {
            shopper.SetActiveShop(null);
        }
    }
}