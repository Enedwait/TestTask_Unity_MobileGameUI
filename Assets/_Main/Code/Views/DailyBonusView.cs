using OKRT.TechnokomTestTask.Main.Code.Data;
using TMPro;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="DailyBonusView"/> class.
    /// This class represents the daily bonus subview.
    /// </summary>
    internal sealed class DailyBonusView : ViewBase
    {
        #region Private

        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private TextMeshProUGUI _textTicketsAmount;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the daily bonus data.
        /// </summary>
        /// <param name="data">data.</param>
        public void Set(DailyBonusData data)
        {
            _textTitle.text = $"Day {data.day}";
            _textTicketsAmount.text = $"x{data.amount}";
        }

        #endregion
    }
}
