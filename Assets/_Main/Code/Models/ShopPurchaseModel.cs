using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Presenters;
using OKRT.TechnokomTestTask.Main.Code.Views;
using UniRx;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace OKRT.TechnokomTestTask.Main.Code.Models
{
    /// <summary>
    /// The <see cref="ShopPurchaseModel"/> class;
    /// This class serves as the shop purchase model.
    /// </summary>
    internal sealed class ShopPurchaseModel : IStoreListener
    {
        #region Private

        private PlayerData _playerData;
        private ConfigurationBuilder _builder;

        private List<string> _consentIdentifiers;
        private static IStoreController _storeController; 
        private static IExtensionProvider _storeExtensionProvider;

        private IGooglePlayStoreExtensions _googlePlayStoreExtensions;

        private UnityEngine.Purchasing.Product _tmProduct = null;
        private ShopItemDto _purchasedItem;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the flag indicating whether the shop is initialized or not.
        /// </summary>
        public bool IsInitialized => _storeController != null && _storeExtensionProvider != null;

        #endregion

        #region Init

        /// <summary>
        /// Serves as Zenject's dependency injection method.
        /// </summary>
        /// <param name="playerData">player data.</param>
        [Inject]
        public ShopPurchaseModel(PlayerData playerData)
        {
            _playerData = playerData;

            MessageBroker.Default.Receive<ShopEvent>().Subscribe(Process);

            Init();
        }

        /// <summary>
        /// Initializes the Unity Services and IAP.
        /// </summary>
        private async void Init()
        {
            _tmProduct = null;
            _consentIdentifiers = null;

            try
            {
                await UnityServices.InitializeAsync();
                _consentIdentifiers = await AnalyticsService.Instance.CheckForRequiredConsents();

                InitializePurchasing();

                MessageBroker.Default.Publish(new GameEvent(GameEventType.StoreInitialized, this));
            }
            catch (ConsentCheckException ex)
            {
                Debug.LogError($"Consent: '{ex}'");
            }
        }

        /// <summary>
        /// Initializes the IAP.
        /// </summary>
        private void InitializePurchasing()
        {
            if (IsInitialized)
                return;

            _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            ProductCatalog catalog = ProductCatalog.LoadDefaultCatalog();
            IAPConfigurationHelper.PopulateConfigurationBuilder(ref _builder, catalog);

            UnityPurchasing.Initialize(this, _builder);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the list of all the products available for purchasing.
        /// </summary>
        /// <returns>all the products.</returns>
        public HashSet<ProductDefinition> GetProducts()
        {
            return _builder?.products;
        }

        /// <summary>
        /// Gets the product definition by its id.
        /// </summary>
        /// <param name="id">product id.</param>
        /// <returns>product definition.</returns>
        public ProductDefinition GetProductDefinition(string id)
        {
            foreach (var product in _builder.products)
                if (product.id.Equals(id))
                    return product;

            return null;
        }

        /// <summary>
        /// Gets item by the specified product's id.
        /// </summary>
        /// <param name="id">product id.</param>
        /// <returns>item.</returns>
        public ShopItemDto GetItem(string id) => GetItem(GetProductDefinition(id));

        /// <summary>
        /// Gets item by the specified product's definition.
        /// </summary>
        /// <param name="product">product definition.</param>
        /// <returns>item.</returns>
        public ShopItemDto GetItem(ProductDefinition product)
        {
            ShopItemDto item = new ShopItemDto();
            UpdateItem(ref item, product);
            return item;
        }

        /// <summary>
        /// Updates the specified item with the values from the product definition.
        /// </summary>
        /// <param name="item">item.</param>
        /// <param name="product">product.</param>
        public void UpdateItem(ref ShopItemDto item, ProductDefinition product)
        {
            item.Id = product.id;
            item.Name = GetProductName(product);
            item.Currency = PriceCurrency.RealMoney;
            item.Price = (double)GetProductPrice(product);
            item.Content = new SerializedDictionary<string, double>();
            item.ProductType = product.type;
            foreach (var payout in product.payouts)
            {
                if (payout.type == PayoutType.Other && payout.subtype.Equals(GlobalIdents.Keys.Data) && !string.IsNullOrWhiteSpace(payout.data))
                {
                    OtherPayoutData data = JsonConvert.DeserializeObject<OtherPayoutData>(payout.data);
                    if (data != null)
                    {
                        item.Category = data.Category;
                        item.Icon = data.Icon;
                        item.RequiredLevel = data.RequiredLevel;
                    }
                    continue;
                }

                item.Content.Add(payout.subtype, payout.quantity);
            }
        }

        /// <summary>
        /// Gets the product name.
        /// </summary>
        /// <param name="product">product.</param>
        /// <returns>product name.</returns>
        public string GetProductName(ProductDefinition product) => GetProductName(product?.id);

        /// <summary>
        /// Gets the product name.
        /// </summary>
        /// <param name="id">product id.</param>
        /// <returns>product name.</returns>
        public string GetProductName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("The product id should be specified in order to retrieve the product name");

            return _storeController.products.WithID(id).metadata.localizedTitle;
        }

        /// <summary>
        /// Gets the product price.
        /// </summary>
        /// <param name="product">product.</param>
        /// <returns>product price.</returns>
        public decimal GetProductPrice(ProductDefinition product) => GetProductPrice(product?.id);

        /// <summary>
        /// Gets the product price.
        /// </summary>
        /// <param name="id">product id.</param>
        /// <returns>product price.</returns>
        public decimal GetProductPrice(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("The product id should be specified in order to retrieve the product price");

            decimal price = _storeController.products.WithID(id).metadata.localizedPrice;
            return price;
        }

        /// <summary>
        /// Processes the shop event accordingly.
        /// </summary>
        /// <param name="shopEvent">shop event.</param>
        private void Process(ShopEvent shopEvent)
        {
            switch (shopEvent.Type)
            {
                case ShopEventType.PurchaseRequested: Buy(shopEvent.UserData as ShopItemDto); break;
            }
        }

        /// <summary>
        /// Attempts to buy the specified item.
        /// </summary>
        /// <param name="item">item.</param>
        private void Buy(ShopItemDto item)
        {
            _purchasedItem = null;
            if (item == null)
                return;

            _purchasedItem = item;
            switch (item.Currency)
            {
                case PriceCurrency.RealMoney: BuyProduct(item); break;
                case PriceCurrency.Tickets: BuyItem(item); break;
            }
        }

        /// <summary>
        /// Attempts to buy the specified item.
        /// </summary>
        /// <param name="item">item.</param>
        private void BuyItem(ShopItemDto item)
        {
            if (item.Currency == PriceCurrency.RealMoney)
                return;

            if (_playerData.Tickets >= item.Price && _playerData.Level >= item.RequiredLevel)
            {
                MessageBroker.Default.Publish(new GameEvent(GameEventType.TicketsSpent, (int)item.Price));
                MessageBroker.Default.Publish(new ShopEvent(ShopEventType.PurchaseCompleted, _purchasedItem));
            }
        }

        /// <summary>
        /// Attempts to buy the specified item.
        /// </summary>
        /// <param name="item">item.</param>
        private void BuyProduct(ShopItemDto item)
        {
            if (item == null)
                return;

            if (_playerData.Level >= item.RequiredLevel)
            {
                BuyProduct(item.Id);
            }
        }

        /// <summary>
        /// Attempts to buy the specified product.
        /// </summary>
        /// <param name="productId">product id.</param>
        private void BuyProduct(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return;

            _tmProduct = null;

            if (IsInitialized)
            {
                UnityEngine.Purchasing.Product product = _storeController.products.WithID(productId);

                if (product != null)
                {
                    if (product.availableToPurchase)
                    {
                        Debug.Log($"Purchasing product: {product.definition.id}");

                        MessageBroker.Default.Publish(new ShopEvent(ShopEventType.PurchaseInitiated));
                        _storeController.InitiatePurchase(product);
                    }
                    else
                        Debug.Log($"Can't purchase '{productId}': the specified product is not available for purchase!");
                }
                else
                {
                    Debug.Log($"Can't purchase '{productId}': the specified product is not found!");
                }
            }
            else
            {
                Debug.Log("Purchasing is not initialized!");
            }
        }

        /// <summary>
        /// Gets the list of player's purchases.
        /// </summary>
        /// <returns>list of player's purchases.</returns>
        public List<string> GetPurchases()
        {
            List<string> ids = new List<string>();
            foreach (UnityEngine.Purchasing.Product item in _storeController.products.all)
                if (item.hasReceipt)
                    ids.Add(item.definition.id);

            return ids;
        }

        #endregion

        #region Events Handling

        /// <summary>
        /// Handles the 'Initialized' event of the IStoreListener.
        /// </summary>
        /// <param name="controller">controller.</param>
        /// <param name="extensions">extensions.</param>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("Store Initialized.");

            _storeController = controller;
            _storeExtensionProvider = extensions;
            _googlePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        }

        /// <summary>
        /// Handles the 'InitializeFailed' event of the IStoreListener.
        /// </summary>
        /// <param name="error">error.</param>
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"Store Initialization failed: '{error}'");
        }

        /// <summary>
        /// Handles the 'DeferredPurchase' event of the IStoreListener.
        /// </summary>
        /// <param name="product">product.</param>
        void OnDeferredPurchase(UnityEngine.Purchasing.Product product)
        {
            MessageBroker.Default.Publish(new ShopEvent(ShopEventType.PurchaseDenied));
            Debug.Log($"Purchase of '{product.definition.id}' is deferred.");
        }

        /// <summary>
        /// Handles the 'PurchaseFailed' event of the IStoreListener.
        /// </summary>
        /// <param name="product">product.</param>
        /// <param name="failureReason">failure reason.</param>
        public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
        {
            MessageBroker.Default.Publish(new ShopEvent(ShopEventType.PurchaseDenied, failureReason));
            Debug.Log($"Purchase Failed. Product: '{product.definition.storeSpecificId}', Failure Reason: '{failureReason}'");
        }

        /// <summary>
        /// Handles the 'ProcessPurchase' callback/event of the IStoreListener.
        /// </summary>
        /// <param name="purchaseEvent">purchase event.</param>
        /// <returns>result.</returns>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            _tmProduct = purchaseEvent.purchasedProduct;

            if (_googlePlayStoreExtensions.IsPurchasedProductDeferred(_tmProduct))
            {
                OnDeferredPurchase(purchaseEvent.purchasedProduct);
                return PurchaseProcessingResult.Pending;
            }

            if (purchaseEvent.purchasedProduct.hasReceipt)
            {
                Debug.Log($"Purchasing '{_tmProduct.definition.id}': complete!");
                if (_purchasedItem != null)
                    MessageBroker.Default.Publish(new ShopEvent(ShopEventType.PurchaseCompleted, _purchasedItem));
                else
                    MessageBroker.Default.Publish(new ShopEvent(ShopEventType.PurchaseCompleted, _tmProduct.definition));
                return PurchaseProcessingResult.Complete;
            }

            OnDeferredPurchase(purchaseEvent.purchasedProduct);
            return PurchaseProcessingResult.Pending;
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="ShopEvent"/> class.
    /// This class represent the event which is being fired when some shop interaction occurs.
    /// </summary>
    internal class ShopEvent
    {
        /// <summary> Gets the event type. </summary>
        public ShopEventType Type { get; private set; }

        /// <summary> Gets the user data associated with this event. </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopEvent"/> class.
        /// </summary>
        /// <param name="type">event type.</param>
        /// <param name="userData">user data associated with this event.</param>
        public ShopEvent(ShopEventType type, object userData = null)
        {
            Type = type;
            UserData = userData;
        }
    }

    /// <summary>
    /// The <see cref="ShopEventType"/> enumeration.
    /// </summary>
    internal enum ShopEventType : byte { PurchaseRequested, PurchaseCompleted, PurchaseInitiated, PurchaseDenied }
}
