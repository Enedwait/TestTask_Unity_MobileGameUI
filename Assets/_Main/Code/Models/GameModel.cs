using System;
using Newtonsoft.Json;
using OKRT.TechnokomTestTask.Main.Code.Data;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Models
{
    /// <summary>
    /// The <see cref="GameModel"/> class.
    /// This class represents the game model.
    /// </summary>
    internal sealed class GameModel
    {
        #region Properties

        /// <summary> Gets the player data. </summary>
        public PlayerData Data { get; private set; }

        /// <summary> Gets the last bonus acquired. </summary>
        public DailyBonusData LastBonus { get; private set; }

        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="GameModel"/> class.
        /// </summary>
        /// <param name="playerData">player data associated with this model.</param>
        public GameModel(PlayerData playerData)
        {
            if (playerData == null)
                throw new Exception("Player Data should not be null!");

            Data = playerData;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Saves the player data.
        /// </summary>
        public void Save()
        {
            string json = JsonConvert.SerializeObject(Data.ToDto());
            PlayerPrefs.SetString(GlobalIdents.Keys.PlayerData, json);
        }

        /// <summary>
        /// Loads the player data.
        /// </summary>
        public void Load()
        {
            string json = PlayerPrefs.GetString(GlobalIdents.Keys.PlayerData);
            if (string.IsNullOrWhiteSpace(json))
                return;
            
            PlayerDataDTO data = JsonConvert.DeserializeObject<PlayerDataDTO>(json);
            if (data != null)
                Data.Set(data);
        }

        /// <summary>
        /// Sets level of the player.
        /// </summary>
        /// <param name="level">level.</param>
        internal void SetLevel(int level)
        {
            Data.Level = (int)MathF.Max(Data.Level, level);
        }

        /// <summary>
        /// Adds tickets to the player.
        /// </summary>
        /// <param name="tickets">tickets.</param>
        internal void AddTickets(int tickets)
        {
            Debug.Log($"Adding tickets: {tickets}.");
            Data.Tickets += tickets;
        }

        /// <summary>
        /// Adds tickets from daily bonus to the player.
        /// </summary>
        /// <param name="data">daily bonus data.</param>
        internal void AddTicketsFromDailyBonus(DailyBonusData data)
        {
            if (Data.DailyBonusData.TryAdd(data.day, true))
            {
                Debug.Log($"Adding tickets as daily bonus: {data.amount}.");
                Data.Tickets += data.amount;
                LastBonus = data;
            }
        }

        /// <summary>
        /// Adds the specified purchased item to the list.
        /// </summary>
        /// <param name="id">purchased item id.</param>
        internal void AddPurchase(string id)
        {
            Data.AddPurchase(id);
        }

        #endregion
    }
}
