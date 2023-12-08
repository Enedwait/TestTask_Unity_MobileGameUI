using System.Collections.Generic;
using OKRT.TechnokomTestTask.Main.Code.Controllers;
using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="ShopView"/> class.
    /// This class represents a shop view which contains a list of items available to purchase.
    /// </summary>
    internal sealed class ShopView : ViewBase
    {
        #region Private

        [SerializeField] private Button _buttonHome;
        [SerializeField] private RectTransform _container;
        [SerializeField] private ShopCategoryView _shopCategoryViewPrefab;

        private SceneData _sceneData;
        private PlayerData _playerData;
        private ShopPurchaseModel _purchaseModel;
        private Dictionary<string, ShopCategoryView> _categories = new Dictionary<string, ShopCategoryView>();

        #endregion

        #region Init

        /// <summary>
        /// Serves as Zenject's dependency injection method.
        /// </summary>
        /// <param name="sceneData">scene data.</param>
        /// <param name="playerData">player data.</param>
        /// <param name="purchaseModel">purchase model.</param>
        [Inject]
        private void Construct(SceneData sceneData, PlayerData playerData, ShopPurchaseModel purchaseModel)
        {
            _sceneData = sceneData;
            _playerData = playerData;
            _purchaseModel = purchaseModel;
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _buttonHome.onClick.AddListener(() => MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.MenuClicked)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="item">item.</param>
        public void UpdateShopItem(ShopItemDto item)
        {
            if (item == null)
                return;

            foreach (var category in _categories)
                category.Value.UpdateShopItem(item);
        }

        /// <summary>
        /// Updates all the current shop items.
        /// </summary>
        public void UpdateShopItems()
        {
            foreach (var category in _categories)
                category.Value.UpdateShopItems();
        }

        /// <summary>
        /// Refills the shop items entirely.
        /// </summary>
        public void RefillShop()
        {
            Clear();

            if (_sceneData == null)
                return;

            HashSet<ShopItemDto> items = new HashSet<ShopItemDto>();

            // check if the purchasing model is initialized
            if (_purchaseModel.IsInitialized) // then we can use it to obtain information about the items which user can purchase for real money
            {
                var products = _purchaseModel.GetProducts();
                foreach (var product in products)
                {
                    bool found = false;
                    for (int i = 0; i < _sceneData.ShopData.Items.Count; i++)
                    {
                        ShopItemDto item = _sceneData.ShopData.Items[i];
                        if (product.id.Equals(item.Id))
                        {
                            found = true;
                            _purchaseModel.UpdateItem(ref item, product);
                            items.Add(item);
                            Debug.Log($"Loaded '{product.id}' item from catalog.");
                            break;
                        }
                    }

                    if (found)
                        continue;

                    ShopItemDto newItem = _purchaseModel.GetItem(product);
                    if (newItem != null)
                        items.Add(newItem);
                }
            }

            // iterate through all the shop items and add unique to the list
            foreach (var item in _sceneData.ShopData.Items)
                items.Add(item);

            // iterate through all the items and create the appropriate categories and respectful views
            foreach (var item in items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Category))
                    continue;
                
                if (_categories.TryGetValue(item.Category, out var category))
                {
                    category.Add(_sceneData, _playerData, item);
                }
                else
                {
                    ShopCategoryView view = Instantiate(_shopCategoryViewPrefab, Vector3.zero, Quaternion.identity, _container);
                    view.Set(item.Category);
                    view.Add(_sceneData, _playerData, item);
                    _categories.Add(item.Category, view);
                }
            }

            Debug.Log($"Found {_categories.Count} categories.");
        }

        /// <summary>
        /// Clears the shop categories one by one.
        /// </summary>
        public void Clear()
        {
            foreach (var kvp in _categories)
            {
                kvp.Value.Clear();
                Destroy(kvp.Value.gameObject);
            }

            _categories.Clear();
        }

        #endregion

        #region Events Handling

        private void OnEnable()
        {
            RefillShop();
        }

        #endregion
    }
}
