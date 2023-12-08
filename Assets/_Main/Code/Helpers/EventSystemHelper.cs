using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Helpers
{
    /// <summary>
    /// The <see cref="EventSystemHelper"/> class.
    /// This class contains different helper methods for the ease of work with EventSystem.
    /// </summary>
    internal static class EventSystemHelper
    {
        #region SubmitSelected

        /// <summary> Submits the selected button. </summary>
        public static void SubmitSelected()
        {
            if (!EventSystem.current || !EventSystem.current.currentSelectedGameObject)
                return;

            Button button = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (button)
            {
                button.Select();
                button.OnSubmit(new BaseEventData(EventSystem.current));
            }
        }

        #endregion
    }
}
