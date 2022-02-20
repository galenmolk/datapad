using System.Collections.Generic;

namespace Datapad.Models
{
    public static class AudioAssetCollection
    {
        public static List<AudioAsset> AudioAssets => _audioAssets;

        private static List<AudioAsset> _audioAssets = new();

        public static void AddAudioAsset(AudioAsset audioAsset)
        {
            _audioAssets.Add(audioAsset);
        }
    }
}
