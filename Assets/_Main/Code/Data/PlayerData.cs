using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace OKRT.TechnokomTestTask.Main.Code.Data
{
    /// <summary>
    /// The <see cref="PlayerData"/> class.
    /// This class represents the player data to be used within the game.
    /// </summary>
    [CreateAssetMenu(menuName = "My Game/Data/Player Data", fileName = "New Player Data", order = 0)]
    [Serializable]
    internal sealed class PlayerData : ScriptableObject
    {
        #region Private

        [field: SerializeField] public int Tickets { get; set; }
        [field: SerializeField] public SerializedDictionary<int, bool> DailyBonusData { get; set; }
        [field: SerializeField] public bool DoPlayMusic { get; set; }
        [field: SerializeField] public bool DoPlaySound { get; set; }
        [field: SerializeField] public int Level { get; set; }
        [field: SerializeField] public List<string> PurchasedItems { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the player data to default values.
        /// </summary>
        public void Reset()
        {
            Tickets = 0;
            DailyBonusData = new SerializedDictionary<int, bool>();
            DoPlayMusic = true;
            DoPlaySound = true;
            Level = 1;
            PurchasedItems = new List<string>();
        }

        /// <summary>
        /// Sets the player data by the specified DTO.
        /// </summary>
        /// <param name="data">data.</param>
        public void Set(PlayerDataDTO data)
        {
            if (data == null)
                throw new NullReferenceException("Player Data DTO should not be null!");

            Tickets = data.Tickets;
            DoPlayMusic = data.DoPlayMusic;
            DoPlaySound = data.DoPlaySound;
            DailyBonusData = new SerializedDictionary<int, bool>();
            Level = data.Level;

            DailyBonusData = new SerializedDictionary<int, bool>();
            if (data.DailyBonusData != null && data.DailyBonusData.Count > 0)
                foreach (var kvp in data.DailyBonusData)
                    DailyBonusData.Add(kvp.Key, kvp.Value);

            PurchasedItems = new List<string>();
            if (data.PurchasedItems != null && data.PurchasedItems.Count > 0)
                foreach (var purchasedItem in data.PurchasedItems)
                    PurchasedItems.Add(purchasedItem);
        }

        /// <summary>
        /// Converts the current player data to DTO.
        /// </summary>
        /// <returns></returns>
        public PlayerDataDTO ToDto()
        {
            PlayerDataDTO data = new PlayerDataDTO();
            data.Tickets = Tickets;
            data.DoPlayMusic = DoPlayMusic;
            data.DoPlaySound = DoPlaySound;
            data.Level = Level;

            data.DailyBonusData = new SerializedDictionary<int, bool>();
            if (DailyBonusData != null && DailyBonusData.Count > 0)
                foreach (var kvp in DailyBonusData)
                    data.DailyBonusData.Add(kvp.Key, kvp.Value);

            data.PurchasedItems = new List<string>();
            if (PurchasedItems != null && PurchasedItems.Count > 0)
                foreach (var purchasedItem in PurchasedItems)
                    data.PurchasedItems.Add(purchasedItem);

            return data;
        }

        /// <summary>
        /// Adds the specified purchased item to the list.
        /// </summary>
        /// <param name="id">purchased item id.</param>
        internal void AddPurchase(string id)
        {
            if (PurchasedItems == null)
                PurchasedItems = new List<string>();

            if (!PurchasedItems.Contains(id))
            {
                PurchasedItems.Add(id);
                Debug.Log($"Item '{id}' added to purchases.");
            }
        }

        /// <summary>
        /// Checks whether the specified purchased item is in the list. 
        /// </summary>
        /// <param name="id">purchased item id.</param>
        /// <returns><value>True</value> if the item is in the purchased items list; otherwise <value>False</value>.</returns>
        internal bool IsItemPurchased(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || PurchasedItems == null)
                return false;

            return PurchasedItems.Contains(id);
        }

        #endregion
    }
}
