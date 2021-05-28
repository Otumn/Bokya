using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Otumn.Bokya
{
    public class Shop : Entity, IStoreListener
    {
        public GameObject shopGO;
        public Animator anim;

        private IStoreController controller;
        private IExtensionProvider extensions;
        
        protected override void Start()
        {
            base.Start();
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("com.otumn.bokya.fullversion", ProductType.NonConsumable);
            UnityPurchasing.Initialize(this, builder);
        }

        #region Purchasing functions
        public void RequestBuyFullVersion()
        {
            //BuyProductID("com.otumn.bokya.fullversion"); // <== real payment version
            // Fake payment version  
            GameManager.instance.CallOnChoicePopUpRequested("Beta version", "This is a beta version of the app. No transaction will be done. When the app will be released, you'll still have access the premium content without having to pay.", "Continue", RequestBetaPayment, "Cancel", null);
        }

        public void RequestBetaPayment()
        {
            GameManager.saveManager.settingsData.HasFullVersion = true;
            GameManager.saveManager.SaveSettingsData();
            GameManager.instance.CallOnFullVersionBought();
            QuitShop();
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return controller != null && extensions != null;
        }

        private void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = controller.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    controller.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        /// <summary>
        /// Called when Unity IAP is ready to make purchases.
        /// </summary>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.controller = controller;
            this.extensions = extensions;
        }

        /// <summary>
        /// Called when Unity IAP encounters an unrecoverable initialization error.
        ///
        /// Note that this will not be called if Internet is unavailable; Unity IAP
        /// will attempt initialization until it becomes available.
        /// </summary>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        /// <summary>
        /// Called when a purchase completes.
        ///
        /// May be called at any time after OnInitialized().
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // Or ... a non-consumable product has been purchased by this user.
            if (args.purchasedProduct.definition.id == "com.otumn.bokya.fullversion")
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
                GameManager.saveManager.settingsData.HasFullVersion = true;
                GameManager.saveManager.SaveSettingsData();
                GameManager.instance.CallOnFullVersionBought();
                QuitShop();
            }
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
            else
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 

            return PurchaseProcessingResult.Complete;
        }

        /// <summary>
        /// Called when a purchase fails.
        /// </summary>
        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", i.definition.storeSpecificId, p));
        }
        #endregion

        #region Open/close shop
        public override void OnRequestShopOpen()
        {
            base.OnRequestShopOpen();
            shopGO.SetActive(true);
            anim.SetTrigger("appear");

        }

        public void QuitShop()
        {
            anim.SetTrigger("disappear");
        }

        public void DisableShop()
        {
            shopGO.SetActive(false);
        }
        #endregion
    }
}
