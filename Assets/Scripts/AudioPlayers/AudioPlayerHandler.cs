using Datapad.Models;
using Datapad.UI;
using UnityEngine;

namespace Datapad.AudioPlayers
{
    public class AudioPlayerHandler : Singleton<AudioPlayerHandler>
    {
        [SerializeField] 
        private MediaControlPanel mediaControlPanel;
        
        private IAudioPlayer _audioPlayer;

        public void PlayAudio(AudioAssetConfig asset)
        {
            mediaControlPanel.SetMedia(asset);
            _audioPlayer.PlayAudio(asset);
        }

        public void StopAudio()
        {
            mediaControlPanel.ClearMedia();
            _audioPlayer.StopAudio();
        }
        
        private void Awake()
        {
            _audioPlayer = GetAudioPlayer();
        }

        private IAudioPlayer GetAudioPlayer()
        {
            #if UNITY_EDITOR
                return gameObject.GetOrAddComponent<EditorAudioPlayer>();
            #elif UNITY_ANDROID
                return new AndroidAudioPlayer();
            #endif
        }
    }
}
