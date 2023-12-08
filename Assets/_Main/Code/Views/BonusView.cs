using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Models;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="BonusView"/> class.
    /// This class represents the bonus view.
    /// </summary>
    internal sealed class BonusView : ViewBase
    {
        #region Private

        [SerializeField] private WeeklyBonusView _weekly;
        [SerializeField] private DailyBonusView _daily;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _weekly.Hide();
            _daily.Hide();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the player data.
        /// </summary>
        /// <param name="data">data.</param>
        public void Set(PlayerData data)
        {
            _weekly.Set(data);
        }

        /// <summary>
        /// Sets the daily bonus data.
        /// </summary>
        /// <param name="data"></param>
        public void Set(DailyBonusData data)
        {
            _daily.Set(data);
        }

        /// <summary>
        /// Shows the weekly bonus subview.
        /// </summary>
        public void ShowWeek()
        {
            Show();
            _daily.Hide();
            _weekly.Show();
        }

        /// <summary>
        /// Shows the daily bonus subview.
        /// </summary>
        public void ShowDay()
        {
            Show();
            _weekly.Hide();
            _daily.Show();
        }

        #endregion
    }
}
