#if UNITY_EDITOR
using System.IO;
using Datapad.Models;
using UnityEditor;

namespace Datapad
{
    public class EditorAudioFileSelector : IAudioFileSelector
    {
        private const string FilePanelTitle = nameof(EditorAudioFileSelector);

        private string _lastDirectory = string.Empty;
        
        public void SelectAudioFiles()
        {
            string path = EditorUtility.OpenFilePanel(FilePanelTitle, _lastDirectory, string.Empty);
            
            if (string.IsNullOrWhiteSpace(path))
                return;

            TryUpdateLastDirectory(path);
            
            AudioAssetConfig asset = new AudioAssetConfig(path);
            AudioLibraryConfigHandler.Instance.AddNewAudioAsset(asset);
        }

        private void TryUpdateLastDirectory(string path)
        {
            DirectoryInfo directoryInfo = new FileInfo(path).Directory;
            
            if (directoryInfo != null)
                _lastDirectory = directoryInfo.FullName;
        }
    }
}
#endif
