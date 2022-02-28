using Datapad.AudioPlayers;
using Datapad.Models;
using TMPro;
using UnityEngine;

namespace Datapad.UI
{
    public class AudioAssetSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text audioNameText;
        
        private AudioAssetConfig _config;
        
        public void Initialize(AudioAssetConfig config)
        {
            _config = config;
            audioNameText.text = config.TrackName;
        }

        public void Play()
        {
            AudioPlayerHandler.Instance.PlayAudio(_config);
        }
    }
}
