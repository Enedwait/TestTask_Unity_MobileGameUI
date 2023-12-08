using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Purchasing;

namespace OKRT.TechnokomTestTask.Main.Code.Data
{
    /// <summary>
    /// The <see cref="ShopItemDto"/> class.
    /// This class represents the shop item DTO used in the game.
    /// </summary>
    [Serializable]
    internal sealed class ShopItemDto
    {
        #region Properties

        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public string Category { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Icon { get; set; }
        [field: SerializeField] public double Price { get; set; }
        [field: SerializeField] public PriceCurrency Currency { get; set; }
        [field: SerializeField] public int RequiredLevel { get; set; }
        [field: SerializeField] public ProductType ProductType { get; set; }
        [field: SerializeField] public SerializedDictionary<string, double> Content { get; set; }

        #endregion

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopItemDto"/> class.
        /// </summary>
        public ShopItemDto()
        {
            Content = new SerializedDictionary<string, double>();
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return $"{Name} [{Id}]";
        }

        public override bool Equals(object obj)
        {
            if (obj is ShopItemDto item)
                return Id.Equals(item.Id);

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="PriceCurrency"/> enumeration.
    /// </summary>
    internal enum PriceCurrency : byte { RealMoney, Tickets }
}
