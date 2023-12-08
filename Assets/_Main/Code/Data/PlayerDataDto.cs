using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace OKRT.TechnokomTestTask.Main.Code.Data
{
    /// <summary>
    /// The <see cref="PlayerDataDTO"/> class.
    /// This class is used to store and transfer player data.
    /// </summary>
    [Serializable]
    internal sealed class PlayerDataDTO
    {
        /// <summary> Gets or sets the amount of tickets. </summary>
        public int Tickets { get; set; }

        /// <summary> Gets or sets the daily bonus data. </summary>
        public SerializedDictionary<int, bool> DailyBonusData { get; set; }

        /// <summary> Gets or sets the flag indicating whether play game music or not. </summary>
        public bool DoPlayMusic { get; set; }

        /// <summary> Gets or sets the flag indicating whether play game sound or not. </summary>
        public bool DoPlaySound { get; set; }

        /// <summary> Gets or sets the level. </summary>
        public int Level { get; set; }

        /// <summary> Gets or sets the purchased items. </summary>
        public List<string> PurchasedItems { get; set; }
    }
}
