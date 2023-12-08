using Newtonsoft.Json.Serialization;
using OKRT.TechnokomTestTask.Main.Code.Controllers;
using OKRT.TechnokomTestTask.Main.Code.Data;
using OKRT.TechnokomTestTask.Main.Code.Models;
using OKRT.TechnokomTestTask.Main.Code.Views;
using UniRx;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace OKRT.TechnokomTestTask.Main.Code.Presenters
{
    /// <summary>
    /// The <see cref="GamePresenter"/> class.
    /// This class represents the game presenter which guides interactions between game model and different view.
    /// </summary>
    internal sealed class GamePresenter : MonoBehaviour
    {
        #region Private

        private GameModel _gameModel;
        private ShopPurchaseModel _purchaseModel;

        #endregion

        #region Init

        /// <summary>
        /// Serves as Zenject's dependency injection method.
        /// </summary>
        /// <param name="playerData">player data.</param>
        /// <param name="purchaseModel">purchase model.</param>
        [Inject]
        private void Construct(PlayerData playerData, ShopPurchaseModel purchaseModel)
        {
            _gameModel = new GameModel(playerData);
            _gameModel.Load();

            _purchaseModel = purchaseModel;
        }

        #endregion

        #region Unity Methods

        private void Start()
        {
            MessageBroker.Default.Receive<GameEvent>().Subscribe(Process);
            MessageBroker.Default.Receive<ShopEvent>().Subscribe(Process);

            MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateSettings, _gameModel.Data));
            MessageBroker.Default.Publish(new AudioEvent(AudioEventType.AudioSettingsChanged, _gameModel.Data));

            // update screens with current data
            OnTicketsAdded(0);
            OnLevelAcquired(0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the specified game event accordingly it's type.
        /// </summary>
        /// <param name="gameEvent">game event.</param>
        private void Process(GameEvent gameEvent)
        {
            switch (gameEvent.Type)
            {
                case GameEventType.TicketsSpent: OnTicketsSpent(gameEvent.UserData); break;
                case GameEventType.TicketsReceived: OnTicketsAdded(gameEvent.UserData); break;
                case GameEventType.ViewShown: OnViewShown(gameEvent.UserData as ViewBase); break;
                case GameEventType.LevelAcquired: OnLevelAcquired(gameEvent.UserData); break;
                case GameEventType.StoreInitialized: OnStoreInitialized(gameEvent.UserData); break;
            }
        }

        /// <summary>
        /// Processes the specified shop event accordingly it's type.
        /// </summary>
        /// <param name="shopEvent">shop event.</param>
        private void Process(ShopEvent shopEvent)
        {
            switch (shopEvent.Type)
            {
                case ShopEventType.PurchaseInitiated:
                    MessageBroker.Default.Publish(new UIEvent(UIEventType.ShowOverlay));
                    break;
                case ShopEventType.PurchaseDenied:
                    MessageBroker.Default.Publish(new UIEvent(UIEventType.HideOverlay));
                    break;
                case ShopEventType.PurchaseCompleted: // after making the purchase we need to update the system accordingly
                    MessageBroker.Default.Publish(new UIEvent(UIEventType.HideOverlay));
                    ShopItemDto item = null;
                    if (shopEvent.UserData is ShopItemDto item0)
                        item = item0;
                    else if (shopEvent.UserData is ProductDefinition product)
                        item = _purchaseModel.GetItem(product.id);

                    if (item == null)
                        break;

                    switch (item.ProductType)
                    {
                        case ProductType.Consumable: break;
                        case ProductType.NonConsumable: _gameModel.AddPurchase(item.Id); break;
                        case ProductType.Subscription: break;
                    }
                    
                    // iterate through all the content of the purchased item and apply required actions
                    foreach (var content in item.Content)
                    {
                        switch (content.Key)
                        {
                            case GlobalIdents.Keys.Ticket: // there is a need to change tickets amount
                                _gameModel.AddTickets((int)content.Value); OnTicketsAdded();
                                break;

                            // unknown key specified
                            default: Debug.LogWarning($"Content key '{content.Key}' is not supported!"); break;
                        }
                    }

                    MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateShopItemsAvailability));
                    break;
            }
        }

        #endregion

        #region Events Handling

        /// <summary>
        /// Handles the 'StoreInitialized' event.
        /// </summary>
        /// <param name="data">event data.</param>
        private void OnStoreInitialized(object data)
        {
            if (data is ShopPurchaseModel purchaseModel)
            {
                _purchaseModel = purchaseModel;
            }
        }

        /// <summary>
        /// Handles the 'LevelAcquired' event.
        /// </summary>
        /// <param name="data">event data.</param>
        private void OnLevelAcquired(object data)
        {
            if (data is int level)
            {
                _gameModel.SetLevel(level + 1);
                MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateLevels, _gameModel.Data));
            }
        }

        /// <summary>
        /// Handles the 'ViewShown' event.
        /// </summary>
        /// <param name="view">view.</param>
        private void OnViewShown(ViewBase view)
        {
            if (view == null)
                return;

            if (view is WeeklyBonusView weeklyBonusView)
                weeklyBonusView.Set(_gameModel.Data);
            else if (view is DailyBonusView dailyBonusView)
                dailyBonusView.Set(_gameModel.LastBonus);
        }

        /// <summary>
        /// Handles the 'TicketsAdded' event.
        /// </summary>
        /// <param name="userData">event data.</param>
        private void OnTicketsAdded(object userData = null)
        {
            if (userData is DailyBonusData data)
            {
                _gameModel.AddTicketsFromDailyBonus(data);
                MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateTickets, _gameModel.Data.Tickets));
            }
            else if (userData is int tickets)
            {
                _gameModel.AddTickets(tickets);
                MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateTickets, _gameModel.Data.Tickets));
            }
            else
                MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateTickets, _gameModel.Data.Tickets));
        }

        /// <summary>
        /// Handles the 'TicketsSpent' event.
        /// </summary>
        /// <param name="userData">event data.</param>
        private void OnTicketsSpent(object userData = null)
        {
            if (userData is int tickets)
            {
                _gameModel.AddTickets(-tickets);
                MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateTickets, _gameModel.Data.Tickets));
            }
            else
                MessageBroker.Default.Publish(new UIEvent(UIEventType.UpdateTickets, _gameModel.Data.Tickets));
        }

        private void OnDestroy()
        {
            _gameModel.Save();
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="GameEvent"/> class.
    /// </summary>
    internal sealed class GameEvent
    {
        /// <summary> Gets the event type. </summary>
        public GameEventType Type { get; private set; }

        /// <summary> Gets the data associated with this event. </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEvent"/> class.
        /// </summary>
        /// <param name="type">event type.</param>
        /// <param name="userData">event data.</param>
        public GameEvent(GameEventType type, object userData)
        {
            Type = type;
            UserData = userData;
        }
    }

    /// <summary>
    /// The <see cref="GameEventType"/> enumeration.
    /// </summary>
    internal enum GameEventType : byte { TicketsReceived, TicketsSpent, ViewShown, ViewHidden, LevelAcquired, StoreInitialized }
}
