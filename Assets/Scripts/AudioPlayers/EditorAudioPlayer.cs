using Datapad.Models;
using UnityEngine;

namespace Datapad.AudioPlayers
{
    public class EditorAudioPlayer : MonoBehaviour, IAudioPlayer
    {
        private AudioSource _audioPlayer;
        private AudioClipImporter _audioClipImporter;
        
        public void PlayAudio(AudioAssetConfig asset)
        {
            if (asset.AudioClip == null || asset.AudioClip == _audioPlayer.clip)
                return;

            _audioPlayer.clip = asset.AudioClip;
            _audioPlayer.Play();
        }

        public void StopAudio()
        {
            _audioPlayer.Stop();
            _audioPlayer.clip = null;
        }

        private void Awake()
        {
            _audioPlayer = gameObject.GetOrAddComponent<AudioSource>();
            _audioClipImporter = gameObject.GetOrAddComponent<AudioClipImporter>();
        }

        private void PrepareAudioClip(AudioAssetConfig asset)
        {
            _audioClipImporter.Import(asset);
        }

        private void OnEnable()
        {
            AudioAssetConfig.OnAudioAssetCreated.AddListener(PrepareAudioClip);
        }

        private void OnDisable()
        {
            AudioAssetConfig.OnAudioAssetCreated.RemoveListener(PrepareAudioClip);
        }
    }
}
