using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Datapad.Models
{
    [Serializable]
    public class AudioLibraryConfig
    {
        public List<AudioAssetConfig> assets = new();
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Debug.Log("AudioLibraryConfig.OnDeserialized. assets.Count: " + assets.Count);
        }
    }
}
