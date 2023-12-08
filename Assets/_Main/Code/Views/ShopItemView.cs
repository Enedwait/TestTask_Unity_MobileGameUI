using OKRT.TechnokomTestTask.Main.Code.Behaviours.UI;
using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Models;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="ShopItemView"/> class.
    /// This class represents the shop item view.
    /// </summary>
    internal class ShopItemView : ViewBase
    {
        #region Private

        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private Image _imageIcon;
        [SerializeField] private RectTransform _locked;
        [SerializeField] private TextMeshProUGUI _textRequiredLevel;
        [SerializeField] private PurchaseItemButton _button;
        [SerializeField] private RectTransform _purchased;
        [SerializeField] private RectTransform _containerTickets;
        [SerializeField] private TextMeshProUGUI _textTicketAmount;
        [SerializeField] private bool _isLocked = false;
        [SerializeField] private ShopItemDto _item;

        private SceneData _sceneData;
        private PlayerData _playerData;

        #endregion

        #region Properties

        /// <summary> Gets the flag indicating whether the shop item view locked or not. </summary>
        public bool IsLocked { get => _isLocked; private set => _isLocked = value; }

        /// <summary> Gets the shop item. </summary>
        public ShopItemDto Item { get => _item; private set => _item = value; }

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            if (IsLocked) Lock();
            else Unlock();
        }

        private void Awake()
        {
            _purchased.gameObject.SetActive(false);

            _button.onClick.AddListener(Purchase);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the item and the required data accordingly.
        /// </summary>
        /// <param name="sceneData">scene data.</param>
        /// <param name="playerData">player data.</param>
        /// <param name="itemData">item data.</param>
        public void Set(SceneData sceneData, PlayerData playerData, ShopItemDto itemData)
        {
            Clear();

            if (itemData == null)
                return;

            _sceneData = sceneData;
            _playerData = playerData;

            UpdateItem(itemData);
        }

        /// <summary>
        /// Updates the view with the current item.
        /// </summary>
        public void UpdateItem() => UpdateItem(Item);

        /// <summary>
        /// Updates the view with the specified item.
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateItem(ShopItemDto itemData)
        {
            if (itemData == null)
                return;

            Item = itemData;

            _textName.text = itemData.Name;
            _imageIcon.sprite = _sceneData.GetIcon(itemData.Icon);
            _textRequiredLevel.text = $"LV. {itemData.RequiredLevel}";

            if (itemData.RequiredLevel > _playerData.Level) Lock();
            else Unlock();

            if (itemData.Content.TryGetValue(GlobalIdents.Keys.Ticket, out var value))
            {
                _containerTickets.gameObject.SetActive(true);
                _textTicketAmount.text = $"x{(int)value}";
            }

            _button.Set(_playerData, itemData.Currency, itemData.Price);

            if (_playerData.IsItemPurchased(itemData.Id))
                _purchased.gameObject.SetActive(true);
            else _purchased.gameObject.SetActive(false);
        }

        /// <summary>
        /// Clears the item data.
        /// </summary>
        public void Clear()
        {
            Item = null;

            _textName.text = null;
            _imageIcon.sprite = null;
            _textRequiredLevel.text = null;
            _textTicketAmount.text = null;
            _containerTickets.gameObject.SetActive(false);
            _purchased.gameObject.SetActive(false);

            _button.Clear();
        }

        /// <summary>
        /// Locks the item view.
        /// </summary>
        private void Lock()
        {
            _imageIcon.enabled = false;
            _locked.gameObject.SetActive(true);
            IsLocked = true;
        }
        
        /// <summary>
        /// Unlocks the item view.
        /// </summary>
        private void Unlock()
        {
            _imageIcon.enabled = true;
            _locked.gameObject.SetActive(false);
            IsLocked = false;
        }

        /// <summary>
        /// Initiates the purchase of the item.
        /// </summary>
        public void Purchase()
        {
            MessageBroker.Default.Publish(new ShopEvent(ShopEventType.PurchaseRequested, Item));
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"{Item.Name} [{Item.Category}]";
        }

        #endregion
    }
}
