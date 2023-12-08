using OKRT.TechnokomTestTask.Main.Code.Data;
using UniRx;
using UnityEngine;

namespace OKRT.TechnokomTestTask.Main.Code.Controllers
{
    /// <summary>
    /// The <see cref="AudioController"/> class.
    /// This class represents the audio controller which is responsible for proper audio system setup and operation.
    /// </summary>
    internal sealed class AudioController : MonoBehaviour
    {
        #region Private

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _soundSource;

        private PlayerData _playerData;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            MessageBroker.Default.Receive<AudioEvent>().Subscribe(Process);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the audio event accordingly.
        /// </summary>
        /// <param name="audioEvent">audio event.</param>
        private void Process(AudioEvent audioEvent)
        {
            switch (audioEvent.Type)
            {
                case AudioEventType.AudioSettingsChanged: ApplySettings(audioEvent.UserData as PlayerData); break;
                case AudioEventType.PlayClick: PlayClickSound(); break;
                case AudioEventType.TurnMusic: TurnMusic(); break;
                case AudioEventType.TurnSound: TurnSound(); break;
            }
        }

        /// <summary>
        /// Applies the settings of te player.
        /// </summary>
        /// <param name="data">player data.</param>
        private void ApplySettings(PlayerData data)
        {
            if (data == null)
                return;

            _playerData = data;
            if (_playerData.DoPlayMusic) _musicSource.Play();
            else _musicSource.Stop();
        }

        /// <summary>
        /// Plays the click sound.
        /// </summary>
        private void PlayClickSound()
        {
            bool playSound = !(_playerData != null && !_playerData.DoPlaySound);

            if (playSound) 
                _soundSource.Play();
        }

        /// <summary>
        /// Turns the music on or off according to the player data.
        /// </summary>
        private void TurnMusic()
        {
            if (_playerData == null)
                return;

            _playerData.DoPlayMusic = !_playerData.DoPlayMusic;

            if (_playerData.DoPlayMusic) _musicSource.Play();
            else _musicSource.Stop();
        }

        /// <summary>
        /// Turns the sound on and off according to the player data.
        /// </summary>
        private void TurnSound()
        {
            if (_playerData == null)
                return;

            _playerData.DoPlaySound = !_playerData.DoPlaySound;

            if (_playerData.DoPlaySound) _soundSource.Play();
            else _soundSource.Stop();
        }

        #endregion
    }

    /// <summary>
    /// The <see cref="AudioEvent"/> class.
    /// This class represent the event which is being fired when some audio interaction occurs.
    /// </summary>
    internal class AudioEvent
    {
        /// <summary> Gets the event type. </summary>
        public AudioEventType Type { get; private set; }

        /// <summary> Gets the user data associated with this event. </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioEvent"/> class.
        /// </summary>
        /// <param name="type">event type.</param>
        /// <param name="userData">user data associated with this event.</param>
        public AudioEvent(AudioEventType type, object userData = null)
        {
            Type = type;
            UserData = userData;
        }
    }

    /// <summary>
    /// The <see cref="AudioEventType"/> enumeration.
    /// </summary>
    internal enum AudioEventType : byte { AudioSettingsChanged, PlayClick, TurnMusic, TurnSound }
}
