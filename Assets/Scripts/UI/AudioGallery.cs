using System;
using Datapad.Models;
using UnityEngine;

namespace Datapad.UI
{
    public class AudioGallery : MonoBehaviour
    {
        [SerializeField] private AudioAssetSlot slotPrefab;
        [SerializeField] private Transform slotParent;

        private void AddAudioAssetSlot(AudioAssetConfig asset)
        {
            Instantiate(slotPrefab, slotParent).Initialize(asset);
        }

        private void OnEnable()
        {
            AudioAssetConfig.OnAudioAssetCreated.AddListener(AddAudioAssetSlot);
        }

        private void OnDisable()
        {
            AudioAssetConfig.OnAudioAssetCreated.RemoveListener(AddAudioAssetSlot);
        }
    }
}
