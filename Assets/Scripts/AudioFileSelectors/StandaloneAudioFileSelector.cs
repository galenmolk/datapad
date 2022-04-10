using Datapad.Models;
using SFB;

namespace Datapad
{
    public class StandaloneAudioFileSelector : IAudioFileSelector
    {
        public void SelectAudioFiles()
        {
            StandaloneFileBrowser.OpenFilePanelAsync("Import Files", "", "", true, paths =>
            {
                if (paths == null || paths.Length == 0)
                    return;

                for (int i = 0, length = paths.Length; i < length; i++)
                {
                    AudioAssetConfig asset = new AudioAssetConfig(paths[i]);
                    AudioLibraryConfigHandler.Instance.AddNewAudioAsset(asset);
                }
            });
        }
    }
}
