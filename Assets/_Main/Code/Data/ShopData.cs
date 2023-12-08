using System;
using System.Collections.Generic;
using OKRT.TechnokomTestTask.Main.Code.Views;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Data
{
    /// <summary>
    /// The <see cref="ShopData"/> class.
    /// This class contains the shop specific data alongside with the shop items available to purchase.
    /// </summary>
    [CreateAssetMenu(menuName = "My Game/Data/Shop Data", fileName = "New Shop Data", order = 2)]
    [Serializable]
    internal sealed class ShopData : ScriptableObject
    {
        #region Properties

        [field: SerializeField] public List<ShopItemDto> Items { get; private set; }

        #endregion
    }
}
