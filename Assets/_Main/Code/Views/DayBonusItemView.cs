using OKRT.TechnokomTestTask.Main.Code.Controllers;
using OKRT.TechnokomTestTask.Main.Code.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="DayBonusItemView"/> class.
    /// This item represents the day bonus item view.
    /// </summary>
    internal sealed class DayBonusItemView : ViewBase
    {
        #region Private

        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _textDay;
        [SerializeField] private TextMeshProUGUI _textAmount;
        [SerializeField] private int _day;
        [SerializeField] private int _amount;

        #endregion

        #region Properties

        /// <summary> Gets the day. </summary>
        public int Day => _day;

        /// <summary> Gets the amount. </summary>
        public int Amount => _amount;

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            _textDay.text = $"DAY{Day}";
            _textAmount.text = $"x{Amount}";
        }

        private void Start()
        {
            _button.onClick.AddListener(() =>
            {
                MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.DailyBonusClicked, new DailyBonusData(){amount = Amount, day = Day}));
                Activate(false);
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Activates (deactivates) the button.
        /// </summary>
        /// <param name="active">activation flag.</param>
        public void Activate(bool active)
        {
            _button.interactable = active;
        }

        #endregion
    }
}
