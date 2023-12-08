using System.Collections.Generic;
using OKRT.TechnokomTestTask.Main.Code.Controllers;
using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Helpers;
using OKRT.TechnokomTestTask.Main.Code.Views;
using UniRx;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Presenters
{
    /// <summary>
    /// The <see cref="UIPresenter"/> class.
    /// This class manages the proper display of views.
    /// </summary>
    internal sealed class UIPresenter : MonoBehaviour
    {
        #region Private

        [SerializeField] private MenuView _menuView;
        [SerializeField] private SettingsView _settingsView;
        [SerializeField] private BonusView _bonusView;
        [SerializeField] private LevelsView _levelsView;
        [SerializeField] private ShopView _shopView;
        [SerializeField] private OverlayView _overlayView;

        private Stack<UIState> _stateStack = new Stack<UIState>();

        #endregion

        #region Properties

        /// <summary> Gets the current state of UI. </summary>
        public UIState CurrentState { get; private set; }

        /// <summary> Gets the locked flag of the UI. </summary>
        public bool IsLocked => _overlayView.IsShown;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _menuView.Hide();
            _shopView.Hide();
            _settingsView.Hide();
            _bonusView.Hide();
            _levelsView.Hide();
            _overlayView.Hide();

            MessageBroker.Default.Receive<UIState>().Subscribe(ChangeState);
            MessageBroker.Default.Receive<InputEvent>().Subscribe(ProcessInput);
            MessageBroker.Default.Receive<UIEvent>().Subscribe(Process);
        }

        private void Start()
        {
            ChangeState(UIState.Menu);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the UI event accordingly.
        /// </summary>
        /// <param name="uiEvent">UI event.</param>
        private void Process(UIEvent uiEvent)
        {
            switch (uiEvent.Type)
            {
                case UIEventType.UpdateTickets:
                    _menuView.SetTicketsAmount((int)uiEvent.UserData);
                    break;
                case UIEventType.UpdateLevels:
                    _levelsView.Set(uiEvent.UserData as PlayerData);
                    break;
                case UIEventType.UpdateSettings:
                    _settingsView.Set(uiEvent.UserData as PlayerData);
                    break;
                case UIEventType.ShowDailyBonus:
                    ChangeState(UIState.DailyBonus);
                    break;
                case UIEventType.UpdateShop:
                    _shopView.RefillShop();
                    break;
                case UIEventType.UpdateShopItem:
                    _shopView.UpdateShopItem(uiEvent.UserData as ShopItemDto);
                    break;
                case UIEventType.UpdateShopItemsAvailability:
                    _shopView.UpdateShopItems();
                    break;
                case UIEventType.ShowOverlay: _overlayView.Show(); break;
                case UIEventType.HideOverlay:_overlayView.Hide(); break;
            }
        }

        /// <summary>
        /// Processes the input event accordingly.
        /// </summary>
        /// <param name="input">input event.</param>
        private void ProcessInput(InputEvent input)
        {
            if (IsLocked)
                return;

            switch (input)
            {
                case InputEvent.Click: EventSystemHelper.SubmitSelected();
                    break;
                case InputEvent.Back: GoBack();
                    break;
            }
        }

        /// <summary>
        /// Changes the UI state (if possible).
        /// </summary>
        /// <param name="state">new state.</param>
        private void ChangeState(UIState state)
        {
            switch (state)
            {
                case UIState.Menu:
                    switch (CurrentState)
                    {
                        case UIState.Shop: _shopView.Hide(); break;
                        case UIState.DailyBonus:
                        case UIState.WeeklyBonus: _bonusView.Hide(); break;
                        case UIState.Levels: _levelsView.Hide(); break;
                        case UIState.Settings: _settingsView.Hide(); break;
                        case UIState.None: break;
                        default: return;
                    }
                    _menuView.Show();
                    break;

                case UIState.Settings:
                    switch (CurrentState)
                    {
                        case UIState.Menu: break;
                        default: return;
                    }
                    _settingsView.Show();
                    break;

                case UIState.WeeklyBonus:
                    switch (CurrentState)
                    {
                        case UIState.Menu: break;
                        case UIState.DailyBonus: break;
                        default: return;
                    }
                    _bonusView.ShowWeek();
                    break;

                case UIState.DailyBonus:
                    switch (CurrentState)
                    {
                        case UIState.Menu: break;
                        case UIState.WeeklyBonus: break;
                        default: return;
                    }
                    _bonusView.ShowDay();
                    break;

                case UIState.Levels:
                    switch (CurrentState)
                    {
                        case UIState.Menu:
                            _menuView.Hide();
                            break;
                        default: return;
                    }
                    _levelsView.Show();
                    break;


                case UIState.Shop:
                    switch (CurrentState)
                    {
                        case UIState.Menu: _menuView.Hide();
                            break;
                        default: return;
                    }
                    _shopView.Show();
                    break;
            }

            _stateStack.Push(CurrentState);
            CurrentState = state;
        }

        /// <summary>
        /// Goes to the previous UI state (if possible).
        /// </summary>
        private void GoBack()
        {
            if (_stateStack.TryPeek(out UIState previousState))
            {
                if (previousState == UIState.None)
                    _stateStack.Pop();
                else
                {
                    if (CurrentState == UIState.DailyBonus && previousState == UIState.WeeklyBonus)
                    {
                        ChangeState(previousState);
                        while(_stateStack.TryPeek(out previousState) && (previousState == UIState.DailyBonus || previousState == UIState.WeeklyBonus))
                            _stateStack.Pop();

                        return;
                    }

                    ChangeState(previousState);
                    if (CurrentState == previousState)
                        _stateStack.Pop();
                }
            }
        }

        #endregion
    }
    
    /// <summary>
    /// The <see cref="UIState"/> enumeration.
    /// </summary>
    public enum UIState : byte { None, Menu, Settings, WeeklyBonus, DailyBonus, Levels, Shop }

    /// <summary>
    /// The <see cref="UIEvent"/> class.
    /// This class represents the UI event which is occurs when some UI interaction is happening.
    /// </summary>
    internal sealed class UIEvent
    {
        /// <summary> Gets the UI event type. </summary>
        public UIEventType Type { get; private set; }

        /// <summary> Gets the data associated with this event. </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIEvent"/> class.
        /// </summary>
        /// <param name="type">event type.</param>
        /// <param name="userData">event data.</param>
        public UIEvent(UIEventType type, object userData = null)
        {
            Type = type;
            UserData = userData;
        }
    }

    /// <summary>
    /// The <see cref="UIEventType"/> enumeration.
    /// </summary>
    public enum UIEventType : byte { UpdateTickets, UpdateLevels, ShowDailyBonus, UpdateSettings, UpdateShop, UpdateShopItem, UpdateShopItemsAvailability, ShowOverlay, HideOverlay }
}
