using OKRT.TechnokomTestTask.Main.Code.Presenters;
using UniRx;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="ViewBase"/> class.
    /// This class represents a base view used it the game.
    /// </summary>
    internal abstract class ViewBase : MonoBehaviour
    {
        #region Properties

        /// <summary> Gets the visibility flag of the view. </summary>
        public bool IsShown => gameObject.activeInHierarchy;

        #endregion

        #region Methods

        /// <summary>
        /// Shows the current view.
        /// </summary>
        public virtual void Show()
        {
            this.gameObject.SetActive(true);
            MessageBroker.Default.Publish(new GameEvent(GameEventType.ViewShown, this));
        }

        /// <summary>
        /// Hides the current view.
        /// </summary>
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
            MessageBroker.Default.Publish(new GameEvent(GameEventType.ViewHidden, this));
        }

        #endregion
    }
}
