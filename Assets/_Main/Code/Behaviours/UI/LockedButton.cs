using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Behaviours.UI
{
    /// <summary>
    /// The <see cref="LockedButton"/> class.
    /// This class represents a button which has lock functionality.
    /// </summary>
    internal sealed class LockedButton : MonoBehaviour
    {
        #region Private

        [SerializeField] private bool _isLocked = false;
        [SerializeField] private string _buttonText;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _lockd;

        #endregion

        #region Properties

        /// <summary> Gets the button text. </summary>
        public string Text => _buttonText;

        /// <summary> Gets the button. </summary>
        public Button Button => _button;

        /// <summary> Gets the 'locked' flag. </summary>
        public bool IsLocked { get; private set; }

        /// <summary> Gets the 'interactable' flag. </summary>
        public bool IsInteractable { get; private set; }

        #endregion

        #region Unity Methods

        private void OnValidate()
        {
            _text.text = _buttonText;
            if (_isLocked) Lock();
            else Unlock();
        }

        private void Start()
        {
            _text.text = _buttonText;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Locks the button.
        /// </summary>
        public void Lock()
        {
            if (IsLocked)
                return;

            _text.enabled = false;
            _image.enabled = true;
            IsLocked = true;
        }

        /// <summary>
        /// Unlocks the button.
        /// </summary>
        public void Unlock()
        {
            if (!IsLocked)
                return;

            _text.enabled = true;
            _image.enabled = false;
            IsLocked = false;
        }

        /// <summary>
        /// Set 'interactable' flag.
        /// </summary>
        /// <param name="interactable"></param>
        public void SetInteractable(bool interactable)
        {
            IsInteractable = interactable;
        }

        #endregion

        #region Events Handling

        private void OnDestroy()
        {
            if (_button)
                _button.onClick.RemoveAllListeners();
        }

        #endregion
    }
}
