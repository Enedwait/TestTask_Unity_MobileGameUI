using OKRT.TechnokomTestTask.Main.Code.Behaviours.UI;
using OKRT.TechnokomTestTask.Main.Code.Controllers;
using OKRT.TechnokomTestTask.Main.Code.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="SettingsView"/> class.
    /// This class represents the settings view.
    /// </summary>
    internal sealed class SettingsView : ViewBase
    {
        #region Private

        [SerializeField] private Button _buttonMusic;
        [SerializeField] private SpriteSwitcher _musicSpriteSwitcher;
        [SerializeField] private Button _buttonSound;
        [SerializeField] private SpriteSwitcher _soundSpriteSwitcher;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            // I decided to abandon idea to make custom class which could combine Button and Sprite Switcher.
            // So that's why I have the separated. But of course, I can do smth with it if necessary.
            _buttonMusic.onClick.AddListener(() =>
            {
                _musicSpriteSwitcher?.Next();
                MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.MusicClicked));
            });

            _buttonSound.onClick.AddListener(() =>
            {
                _soundSpriteSwitcher?.Next();
                MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.SoundClicked));
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the player data.
        /// </summary>
        /// <param name="data">data.</param>
        public void Set(PlayerData data)
        {
            if (data == null)
                return;

            if (_musicSpriteSwitcher) _musicSpriteSwitcher.Change(data.DoPlayMusic ? 0 : 1);
            if (_soundSpriteSwitcher) _soundSpriteSwitcher.Change(data.DoPlaySound ? 0 : 1);
        }

        #endregion

        #region Events Handling

        private void OnDestroy()
        {
            _buttonMusic.onClick.RemoveAllListeners();
            _buttonSound.onClick.RemoveAllListeners();
        }

        #endregion
    }
}