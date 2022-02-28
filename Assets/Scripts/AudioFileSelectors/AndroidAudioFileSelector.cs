using Datapad.Models;

namespace Datapad
{
    public class AndroidAudioFileSelector : IAudioFileSelector
    {
        private const string AudioFileType = "audio/*";
        private readonly string[] _fileTypes = { AudioFileType };
        
        public void SelectAudioFiles()
        {
            NativeFilePicker.PickMultipleFiles(AudioFilesSelected, _fileTypes);
        }
        
        private void AudioFilesSelected(string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return;

            for (int i = 0, length = paths.Length; i < length; i++)
            {
                AudioAssetConfig asset = new AudioAssetConfig(paths[i]);
                AudioLibraryConfigHandler.Instance.AddNewAudioAsset(asset);
            }
        }
    }
}
