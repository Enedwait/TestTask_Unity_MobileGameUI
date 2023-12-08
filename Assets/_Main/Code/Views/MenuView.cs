using OKRT.TechnokomTestTask.Main.Code.Controllers;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="MenuView"/> class.
    /// This class represents the menu view.
    /// </summary>
    internal sealed class MenuView : ViewBase
    {
        #region Private

        [SerializeField] private Button _buttonPlay;
        [SerializeField] private Button _buttonSettings;
        [SerializeField] private Button _buttonBonus;
        [SerializeField] private Button _buttonShop;
        [SerializeField] private TextMeshProUGUI _textGoldTicketsAmount;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _buttonPlay.onClick.AddListener(() => MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.PlayClicked)));
            _buttonSettings.onClick.AddListener(() => MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.SettingsClicked)));
            _buttonBonus.onClick.AddListener(() => MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.BonusClicked)));
            _buttonShop.onClick.AddListener(() => MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.ShopClicked)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the amount of tickets on the menu.
        /// </summary>
        /// <param name="amount">total amount of tickets.</param>
        public void SetTicketsAmount(int amount)
        {
            _textGoldTicketsAmount.text = $"{amount}";
        }

        #endregion
    }
}
