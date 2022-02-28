using System;
using Datapad.Models;
using TMPro;
using UnityEngine;

namespace Datapad.UI
{
    public class MediaControlPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text trackNameText;
        [SerializeField] private TMP_Text artistNameText;

        public void SetMedia(AudioAssetConfig asset)
        {
            trackNameText.text = asset.TrackName;
            artistNameText.text = asset.ArtistName;
        }

        public void ClearMedia()
        {
            trackNameText.text = string.Empty;
            artistNameText.text = string.Empty;
        }

        private void Awake()
        {
            ClearMedia();
        }
    }
}
