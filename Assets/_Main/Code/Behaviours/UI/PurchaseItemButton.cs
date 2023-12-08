using System;
using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Behaviours.UI
{
    /// <summary>
    /// The <see cref="PurchaseItemButton"/> class.
    /// This class represents the purchase item button.
    /// </summary>
    internal sealed class PurchaseItemButton : Button
    {
        #region Private

        [SerializeField] private TextMeshProUGUI _textPriceCurrency;
        [SerializeField] private RectTransform _priceTickets;
        [SerializeField] private TextMeshProUGUI _textPriceTickets;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the currency and value accordingly.
        /// </summary>
        /// <param name="data">player data.</param>
        /// <param name="currency">currency.</param>
        /// <param name="value">value.</param>
        public void Set(PlayerData data, PriceCurrency currency, double value)
        {
            switch (currency)
            {
                case PriceCurrency.Tickets:
                    int ticketPrice = (int)value;
                    _textPriceCurrency.enabled = false;
                    _priceTickets.gameObject.SetActive(true);
                    _textPriceTickets.text = $"{ticketPrice}";
                    if (data.Tickets < ticketPrice) interactable = false;
                    else interactable = true;
                    break;
                case PriceCurrency.RealMoney:
                    _textPriceCurrency.enabled = true;
                    _textPriceCurrency.text = $"{value:F}$";
                    _priceTickets.gameObject.SetActive(false);
                    break;
                default: throw new Exception($"Currency '{currency}' not supported yet!");
            }
        }

        /// <summary>
        /// Clears the button view.
        /// </summary>
        public void Clear()
        {
            _textPriceCurrency.enabled = false;
            _priceTickets.gameObject.SetActive(false);
            _textPriceTickets.text = null;
            _textPriceCurrency.text = null;
        }

        #endregion
    }
}
