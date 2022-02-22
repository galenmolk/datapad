using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Datapad.Models
{
    [Serializable]
    public class AudioAssetConfig
    {
        public string FileName { get; private set; }

        public string LocalPath { get; private set; }

        [JsonIgnore]
        public AudioClip AudioClip { get; set; }

        public AudioAssetConfig(string fileName, string localPath, AudioClip clip)
        {
            FileName = fileName;
            LocalPath = localPath;
            AudioClip = clip;
        }
    }
}