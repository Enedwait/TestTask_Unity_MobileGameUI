using OKRT.TechnokomTestTask.Main.Code.Data;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="WeeklyBonusView "/> class.
    /// This class represents the weekly bonus subview.
    /// </summary>
    internal sealed class WeeklyBonusView : ViewBase
    {
        #region Private

        [SerializeField] private ProgressBarPro _progressBar;
        [SerializeField] private DayBonusItemView[] _daysBonusViews;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the player data.
        /// </summary>
        /// <param name="data">data.</param>
        public void Set(PlayerData data)
        {
            _progressBar.Value = 0;
            foreach (var dayBonusView in _daysBonusViews)
            {
                bool taken = false;
                if (data.DailyBonusData.TryGetValue(dayBonusView.Day, out taken))
                    if (taken)
                        _progressBar.Value += 1f / 7f;

                dayBonusView.Activate(!taken);
            }
        }

        #endregion
    }
}
