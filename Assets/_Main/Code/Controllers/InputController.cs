using OKRT.TechnokomTestTask.Main.Code.Controls;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OKRT.TechnokomTestTask.Main.Code.Controllers
{
    /// <summary>
    /// The <see cref="InputController"/> class.
    /// This class represents the input controller which is used as the input data hub.
    /// </summary>
    internal sealed class InputController : MonoBehaviour
    {
        #region Private

        private MainControls _controls;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _controls = new MainControls();

            _controls.Default.Click.performed += OnClickPerformed;
            _controls.Default.Back.performed += OnBackPerformed;

            _controls.Enable();
        }

        #endregion

        #region Events Handling

        /// <summary>
        /// Handles the 'Performed' event of the Back action.
        /// </summary>
        /// <param name="obj">context.</param>
        private void OnBackPerformed(InputAction.CallbackContext obj)
        {
            MessageBroker.Default.Publish(InputEvent.Back);
        }

        /// <summary>
        /// Handles the 'Performed' event of the Click action.
        /// </summary>
        /// <param name="obj">context.</param>
        private void OnClickPerformed(InputAction.CallbackContext obj)
        {
            MessageBroker.Default.Publish(InputEvent.Click);
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="InputEvent"/> enumeration.
    /// </summary>
    public enum InputEvent : byte { Click, Back }
}
