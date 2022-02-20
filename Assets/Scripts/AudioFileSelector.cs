using UnityEngine;

namespace Datapad
{
    public class AudioFileSelector : MonoBehaviour
    {
        private const string AudioFileType = "audio/*";
        private readonly string[] _fileTypes = { AudioFileType };

        [SerializeField] private AudioImporter audioImporter;
        [SerializeField] private AudioPlayer audioPlayer;
        
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

            StartCoroutine(audioImporter.ImportAudio(path, asset =>
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
                
                audioPlayer.PlayAudio(asset);
            }));
        }
    }
}
