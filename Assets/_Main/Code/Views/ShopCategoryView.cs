using System.Collections.Generic;
using System.Text;
using OKRT.TechnokomTestTask.Main.Code.Data;
using TMPro;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="ShopCategoryView"/> class.
    /// This class represents the shop category view.
    /// </summary>
    internal sealed class ShopCategoryView : ViewBase
    {
        #region Private

        [SerializeField] private string _category;
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private RectTransform _itemContainer;
        [SerializeField] private ShopItemView _itemViewPrefab;

        private List<ShopItemView> _items = new List<ShopItemView>();

        #endregion

        #region Methods

        /// <summary>
        /// Sets the category for the current view.
        /// </summary>
        /// <param name="category">category.</param>
        public void Set(string category)
        {
            _category = category;
            _textName.text = category;
        }

        /// <summary>
        /// Adds the item and the required data accordingly
        /// </summary>
        /// <param name="sceneData">scene data.</param>
        /// <param name="playerData">player data.</param>
        /// <param name="item">item.</param>
        public void Add(SceneData sceneData, PlayerData playerData, ShopItemDto item)
        {
            if (item == null)
                return;

            ShopItemView view = Instantiate(_itemViewPrefab, Vector3.zero, Quaternion.identity, _itemContainer);
            view.Set(sceneData, playerData, item);
            _items.Add(view);
        }

        /// <summary>
        /// Clears the category from items.
        /// </summary>
        public void Clear()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                Destroy(_items[i].gameObject);
                _items[i] = null;
                _items.RemoveAt(i);
            }
        }

        /// <summary>
        /// Updates the specified item if exist.
        /// </summary>
        /// <param name="item">item.</param>
        public void UpdateShopItem(ShopItemDto item)
        {
            if (item == null)
                return;

            foreach (var view in _items)
                if (view.Item.Equals(item))
                    view.UpdateItem(item);
        }

        /// <summary>
        /// Updates all the current items.
        /// </summary>
        public void UpdateShopItems()
        {
            foreach (var view in _items)
                view.UpdateItem();
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (_items != null && _items.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in _items)
                    sb.Append($"{item}, ");
                return $"{_category}: {sb.ToString()}";
            }

            return $"{_category}: empty";
        }

        #endregion
    }
}
