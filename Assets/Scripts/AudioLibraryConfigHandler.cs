using System.Collections.Generic;
using System.IO;
using Datapad.Models;
using Datapad.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Datapad
{
    public class AudioLibraryConfigHandler : Singleton<AudioLibraryConfigHandler>
    {
        public static StringEvent OnLibraryJsonChanged = new();
        
        private const string LibraryConfigFileName = "AudioLibraryConfig.json";

        public string LibraryJson
        {
            get
            {
                return libraryJson;
            }
            private set
            {
                libraryJson = value;
                OnLibraryJsonChanged.Invoke(libraryJson);
                Debug.Log("OnLibraryJsonChanged");
            }
        }

        private string libraryJson;
        
        private string _audioLibraryConfigPath;

        private AudioLibraryConfig AudioLibraryConfig { get; set; }

        public void AddNewAudioAsset(AudioAssetConfig assetConfig)
        {
            if (!AudioLibraryConfig.TryAdd(assetConfig))
                return;
            
            SaveConfigToDisk();
            AudioAssetConfig.OnAudioAssetCreated.Invoke(assetConfig);
        }
        
        private void Awake()
        {
            _audioLibraryConfigPath = Path.Combine(Application.persistentDataPath, LibraryConfigFileName);
            TryLoadLibraryConfig();
        }

        private void TryLoadLibraryConfig()
        {
            if (!File.Exists(_audioLibraryConfigPath))
            {
                CreateNewLibraryConfig();
                return;
            }

            LibraryJson = File.ReadAllText(_audioLibraryConfigPath);
            Log(nameof(Awake), $"Config Found: \n{LibraryJson}");
            AudioLibraryConfig = JsonConvert.DeserializeObject<AudioLibraryConfig>(LibraryJson);
        }

        private void CreateNewLibraryConfig()
        {
            Log(nameof(Awake), "Config Not Found. Creating New One.");
            
            AudioLibraryConfig = new AudioLibraryConfig
            {
                Assets = new List<AudioAssetConfig>()
            };
            
            SaveConfigToDisk();
        }
        
        private void SaveConfigToDisk()
        {
            LibraryJson = JsonConvert.SerializeObject(AudioLibraryConfig);
            Log(nameof(SaveConfigToDisk), $"Saving New Config: \n{LibraryJson}");
            File.WriteAllText(_audioLibraryConfigPath, LibraryJson);
        }

        private void Log(string method, string message)
        {
            Debug.Log($"{nameof(AudioLibraryConfigHandler)}.{method}: {message}");
        }
    }
}
