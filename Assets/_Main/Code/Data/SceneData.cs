using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Data
{
    /// <summary>
    /// The <see cref="SceneData"/> class.
    /// This class represents the scene data to be used across the current scene.
    /// </summary>
    internal sealed class SceneData : MonoBehaviour
    {
        #region Private

        [SerializeField] private SerializedDictionary<string, Sprite> _itemIcons;
        [SerializeField] private ShopData _shopData;
        [SerializeField] private PlayerData _playerData;

        #endregion

        #region Properties

        /// <summary> Gets the shop data. </summary>
        public ShopData ShopData => _shopData;

        /// <summary> Gets the player data. </summary>
        public PlayerData PlayerData => _playerData;

        #endregion

        #region ethods

        /// <summary>
        /// Gets the icon by its' code name.
        /// </summary>
        /// <param name="codeName">code name.</param>
        /// <returns>icon.</returns>
        public Sprite GetIcon(string codeName)
        {
            if (string.IsNullOrWhiteSpace(codeName))
                return null;

            foreach (var icon in _itemIcons)
                if (icon.Key.Equals(codeName))
                    return icon.Value;

            return null;
        }

        #endregion
    }
}
