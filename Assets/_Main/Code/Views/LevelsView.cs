using System.Collections.Generic;
using OKRT.TechnokomTestTask.Main.Code.Behaviours.UI;
using OKRT.TechnokomTestTask.Main.Code.Controllers;
using OKRT.TechnokomTestTask.Main.Code.Data;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace OKRT.TechnokomTestTask.Main.Code.Views
{
    /// <summary>
    /// The <see cref="LevelsView"/> class.
    /// This class represents the level view.
    /// </summary>
    internal sealed class LevelsView : ViewBase
    {
        #region Private

        [SerializeField] private RectTransform _levelsContainer;
        [SerializeField] private Button _buttonHome;

        private List<LockedButton> _levels;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (_levels == null)
                LoadLevels();

            _buttonHome.onClick.AddListener(() => MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.MenuClicked)));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the levels map elements.
        /// </summary>
        private void LoadLevels()
        {
            _levels = new List<LockedButton>();
            for (int i = 0; i < _levelsContainer.childCount; i++)
            {
                LockedButton button = _levelsContainer.GetChild(i).GetComponent<LockedButton>();
                if (button)
                {
                    _levels.Add(button);

                    button.Button.onClick.AddListener(() =>
                    {
                        if (!button.IsLocked && button.IsInteractable)
                            MessageBroker.Default.Publish(new ButtonEvent(ButtonEventType.LevelButtonClicked, int.Parse(button.Text)));
                    });
                    button.Lock();
                }
            }
        }

        /// <summary>
        /// Sets the player data.
        /// </summary>
        /// <param name="data">data.</param>
        public void Set(PlayerData data)
        {
            int current = -1;
            if (data != null)
                current = data.Level;

            if (_levels == null)
                LoadLevels();

            foreach (var level in _levels)
            {
                int lvl = int.Parse(level.Text);
                if (lvl < current)
                {
                    level.Unlock();
                    level.SetInteractable(false);
                }
                else if (lvl == current)
                {
                    level.Unlock();
                    level.SetInteractable(true);
                }
                else
                {
                    level.Lock();
                }
            }
        }

        #endregion

        #region Events Handling

        private void OnDestroy()
        {
            if (_buttonHome)
                _buttonHome.onClick.RemoveAllListeners();
        }

        #endregion
    }
}