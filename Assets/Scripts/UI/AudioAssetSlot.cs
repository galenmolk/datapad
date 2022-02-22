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
            audioNameText.text = _config.FileName;
        }

        public void Play()
        {
            AudioPlayer.Instance.PlayAudio(_config);
        }
    }
}
