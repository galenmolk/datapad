using System.IO;
using Datapad.Models;
using Datapad.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Datapad
{
    public class AudioLibraryConfigHandler : MonoBehaviour
    {
        private const string LibraryConfigFileName = "AudioLibraryConfig.json";

        private AudioLibraryConfig AudioLibraryConfig { get; set; }

        private string _audioLibraryConfigPath;

        public void AddNewAudioAsset(AudioAssetConfig assetConfig)
        {
            AudioLibraryConfig.assets.Add(assetConfig);
            SaveConfigToDisk();
            AudioGallery.Instance.AddAudioAssetSlot(assetConfig);
        }
        
        private void Awake()
        {
            _audioLibraryConfigPath = Path.Combine(Application.persistentDataPath, LibraryConfigFileName);

            if (!File.Exists(_audioLibraryConfigPath))
            {
                Log(nameof(Awake), "Config Not Found. Creating New One.");
                AudioLibraryConfig = new AudioLibraryConfig();
                SaveConfigToDisk();
                return;
            }

            string libraryJson = File.ReadAllText(_audioLibraryConfigPath);
            Log(nameof(Awake), $"Config Found: \n{libraryJson}");
            AudioLibraryConfig = JsonConvert.DeserializeObject<AudioLibraryConfig>(libraryJson);
            AudioGallery.Instance.DisplayLibrary(AudioLibraryConfig);
        }

        private void SaveConfigToDisk()
        {
            string libraryJson = JsonConvert.SerializeObject(AudioLibraryConfig);
            Log(nameof(SaveConfigToDisk), $"Saving New Config: \n{libraryJson}");
            File.WriteAllText(_audioLibraryConfigPath, libraryJson);
        }

        private void Log(string method, string message)
        {
            Debug.Log($"{nameof(AudioLibraryConfigHandler)}.{method}: {message}");
        }
    }
}
