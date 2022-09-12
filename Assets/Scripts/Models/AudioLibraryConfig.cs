using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Datapad.Utils;
using Newtonsoft.Json;

namespace Datapad.Models
{
    [Serializable]
    public class AudioLibraryConfig
    {
        [JsonIgnore] 
        public static AudioLibraryConfigEvent OnLibraryDeserialized = new();
        
        [JsonProperty(ConfigKeys.ASSETS_KEY)]
        public List<AudioAssetConfig> Assets;

        [JsonIgnore] 
        public int Count
        {
            get
            {
                if (_count == -1)
                    _count = Assets.Count;

                return _count;
            }
        }

        [JsonIgnore] 
        private int _count = -1;

        [JsonIgnore] 
        private List<string> _assetPaths = new();

        public bool TryAdd(AudioAssetConfig asset)
        {
            if (_assetPaths.Contains(asset.LocalPath))
                return false;

            AddAsset(asset);
            return true;
        }

        private void AddAsset(AudioAssetConfig asset)
        {
            _assetPaths.Add(asset.LocalPath);
            Assets.Add(asset);
        }
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            for (int i = 0; i < Count; i++)
                _assetPaths.Add(Assets[i].LocalPath);

            OnLibraryDeserialized.Invoke(this);
        }
    }
}
