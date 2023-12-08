using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Behaviours.UI
{
    /// <summary>
    /// The <see cref="SpriteSwitcher"/> class.
    /// This class represents the behaviour which is capable of changing image sprites by their indices.
    /// </summary>
    internal sealed class SpriteSwitcher : MonoBehaviour
    {
        #region Private

        [SerializeField] private Image _image;
        [SerializeField] private Sprite[] _sprites;

        private int _index = 0;

        #endregion

        #region Methods

        /// <summary>
        /// Changes the image sprite by the specified index.
        /// </summary>
        /// <param name="index"></param>
        public void Change(int index)
        {
            _image.sprite = _sprites[index];
            _index = index;
        }

        /// <summary>
        /// Sets the next sprite to image.
        /// </summary>
        public void Next()
        {
            if (_index < _sprites.Length)
                _index++;

            if (_index >= _sprites.Length)
                _index = 0;

            Change(_index);
        }

        #endregion
    }
}
