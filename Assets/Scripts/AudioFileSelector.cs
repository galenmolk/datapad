using UnityEngine;

namespace Datapad
{
    public class AudioFileSelector : MonoBehaviour
    {
        private const string AudioFileType = "audio/*";
        private readonly string[] _fileTypes = { AudioFileType };

        [SerializeField] private AudioPlayer audioPlayer;
        [SerializeField] private AudioLibraryConfigHandler audioLibraryConfigHandler;
        
        public void SelectAudioFile()
        {
            NativeFilePicker.PickFile(AudioFileSelected, _fileTypes);
        }
        
        private void AudioFileSelected(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError($"{nameof(AudioFileSelector)}.{nameof(AudioFileSelected)}: " +
                               $"{nameof(path)} is null or white space. " +
                               $"{nameof(path)}: {path}");
                return;
            }

            StartCoroutine(AudioImporter.Instance.ImportAudioAsset(path, asset =>
            {
                if (asset == null)
                {
                    Debug.LogError($"{nameof(AudioFileSelector)}.{nameof(AudioFileSelected)}: " +
                                   $"AudioAsset is null.");
                    return;
                }

                Debug.Log($"{nameof(AudioFileSelector)}.{nameof(AudioFileSelected)}: " +
                          $"AudioAsset loaded. " +
                          $"Name: {asset.FileName}, " +
                          $"Local Path: {asset.LocalPath}, " +
                          $"AudioClip: {asset.AudioClip}");
                
                audioLibraryConfigHandler.AddNewAudioAsset(asset);
            }));
        }
    }
}
