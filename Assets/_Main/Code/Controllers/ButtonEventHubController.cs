using OKRT.TechnokomTestTask.Main.Code.Presenters;
using UniRx;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Controllers
{
    /// <summary>
    /// The <see cref="ButtonEventHubController"/> class.
    /// This class represents the event hub.
    /// </summary>
    internal sealed class ButtonEventHubController : MonoBehaviour
    {
        #region Unity Methods

        private void Awake()
        {
            MessageBroker.Default.Receive<ButtonEvent>().Subscribe(Process);
        }

        #endregion

        #region Unity Methods

        /// <summary>
        /// Processes the button event accordingly.
        /// </summary>
        /// <param name="buttonEvent">button event.</param>
        private void Process(ButtonEvent buttonEvent)
        {
            switch (buttonEvent.Type)
            {
                case ButtonEventType.MenuClicked: MessageBroker.Default.Publish(UIState.Menu); break;
                case ButtonEventType.PlayClicked: MessageBroker.Default.Publish(UIState.Levels); break;
                case ButtonEventType.SettingsClicked: MessageBroker.Default.Publish(UIState.Settings); break;
                case ButtonEventType.ShopClicked: MessageBroker.Default.Publish(UIState.Shop); break;
                case ButtonEventType.BonusClicked: MessageBroker.Default.Publish(UIState.WeeklyBonus); break;
                case ButtonEventType.MusicClicked: MessageBroker.Default.Publish(new AudioEvent(AudioEventType.TurnMusic)); break;
                case ButtonEventType.SoundClicked: MessageBroker.Default.Publish(new AudioEvent(AudioEventType.TurnSound)); break;
                case ButtonEventType.DailyBonusClicked: 
                    MessageBroker.Default.Publish(new GameEvent(GameEventType.TicketsReceived, buttonEvent.UserData));
                    MessageBroker.Default.Publish(new UIEvent(UIEventType.ShowDailyBonus, buttonEvent.UserData));
                    break;
                case ButtonEventType.LevelButtonClicked:
                    MessageBroker.Default.Publish(new GameEvent(GameEventType.LevelAcquired, buttonEvent.UserData));
                    break;
                default: return;
            }

            MessageBroker.Default.Publish(new AudioEvent(AudioEventType.PlayClick));
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="ButtonEvent"/> class.
    /// This class represent the event which is being fired when some audio interaction occurs.
    /// </summary>
    internal class ButtonEvent
    {
        /// <summary> Gets the event type. </summary>
        public ButtonEventType Type { get; private set; }

        /// <summary> Gets the user data associated with this event. </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonEvent"/> class.
        /// </summary>
        /// <param name="type">event type.</param>
        /// <param name="userData">user data associated with this event.</param>
        public ButtonEvent(ButtonEventType type, object userData = null)
        {
            Type = type;
            UserData = userData;
        }
    }

    /// <summary>
    /// The <see cref="ButtonEventType"/> enumeration.
    /// Enumerates the available kinds of button events.
    /// </summary>
    internal enum ButtonEventType { MenuClicked, PlayClicked, SettingsClicked, ShopClicked, BonusClicked, LevelButtonClicked, MusicClicked, SoundClicked, DailyBonusClicked }
}
