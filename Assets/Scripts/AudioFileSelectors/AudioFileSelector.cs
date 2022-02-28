using Datapad.Models;
using UnityEditor;
using UnityEngine;

namespace Datapad
{
    public class AudioFileSelector : MonoBehaviour
    {
        private const string AudioFileType = "audio/*";
        private readonly string[] _fileTypes = { AudioFileType };

        [SerializeField] private AudioLibraryConfigHandler audioLibraryConfigHandler;

        public void SelectAudioFiles()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NativeFilePicker.PickMultipleFiles(AudioFilesSelected, _fileTypes);
#elif UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("", "", "");
            
            if (string.IsNullOrWhiteSpace(path))
                return;
            
            AudioAssetConfig asset = new AudioAssetConfig(path);
            audioLibraryConfigHandler.AddNewAudioAsset(asset);
#endif
        }

        private void AudioFilesSelected(string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return;

            for (int i = 0, length = paths.Length; i < length; i++)
            {
                AudioAssetConfig asset = new AudioAssetConfig(paths[i]);
                audioLibraryConfigHandler.AddNewAudioAsset(asset);
            }
        }
    }
}
