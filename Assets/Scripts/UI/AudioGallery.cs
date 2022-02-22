using Datapad.Models;
using UnityEngine;

namespace Datapad.UI
{
    public class AudioGallery : Singleton<AudioGallery>
    {
        [SerializeField] private AudioAssetSlot slotPrefab;
        [SerializeField] private Transform slotParent;
        
        public void DisplayLibrary(AudioLibraryConfig libraryConfig)
        {
            foreach (var asset in libraryConfig.assets)
                AddAudioAssetSlot(asset);
        }

        public void AddAudioAssetSlot(AudioAssetConfig config)
        {
            AudioAssetSlot slot = Instantiate(slotPrefab, slotParent);
            AudioImporter.Instance.LoadAudioAsset(config);
            slot.Initialize(config);
        }
    }
}
