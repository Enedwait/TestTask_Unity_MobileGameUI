using PlayfulSystems.ProgressBar;
using TMPro;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="ProgressBarTextView"/> class.
    /// This class represents the text progress bar view.
    /// </summary>
    internal sealed class ProgressBarTextView : ProgressBarProView
    {
        #region Private

        [SerializeField] private TextMeshProUGUI _textProgress;
        [SerializeField] private float _maxValue;

        #endregion

        #region Methods

        public override void UpdateView(float currentValue, float targetValue)
        {
            _textProgress.text = $"{currentValue * _maxValue}/{_maxValue}";
        }

        #endregion
    }
}
