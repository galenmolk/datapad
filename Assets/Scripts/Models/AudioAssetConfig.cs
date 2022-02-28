using System;
using System.IO;
using System.Runtime.Serialization;
using Datapad.Utils;
using Newtonsoft.Json;
using UnityEngine;
using File = TagLib.File;

namespace Datapad.Models
{
    [Serializable]
    public class AudioAssetConfig
    {
        public static AudioAssetConfigEvent OnAudioAssetCreated = new();
        
        [JsonProperty(ConfigKeys.LOCAL_PATH_KEY)]
        public string LocalPath { get; private set; }

        [JsonProperty(ConfigKeys.ARTIST_NAME_KEY)]
        public string ArtistName { get; private set; }

        [JsonProperty(ConfigKeys.TRACK_NAME_KEY)]
        public string TrackName { get; private set; }

        [JsonProperty(ConfigKeys.DURATION_KEY)]
        public TimeSpan Duration { get; private set; }

        [JsonIgnore] 
        public AudioClip AudioClip { get; private set; }

        public AudioAssetConfig()
        {
            TrackName = null;
            LocalPath = null;
        }
        
        public AudioAssetConfig(string localPath)
        {
            LocalPath = localPath;
            CreateMetaData(localPath);
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            AudioClip = audioClip;
        }

        private void CreateMetaData(string path)
        {
            File file = File.Create(path);

            TrackName = GetTrackName(file, path);
            ArtistName = GetArtistName(file);
            Duration = file.Properties.Duration;
        }

        private string GetTrackName(File file, string path)
        {
            string name = file.Tag.Title;
            return !string.IsNullOrWhiteSpace(name) ? name : new FileInfo(path).Name;
        }
        
        private string GetArtistName(File file)
        {
            string artist = file.Tag.FirstPerformer;

            if (!string.IsNullOrWhiteSpace(artist))
                return artist;
           
            artist = file.Tag.FirstAlbumArtist;
            return !string.IsNullOrWhiteSpace(artist) ? artist : file.Tag.FirstComposer;
        }
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            OnAudioAssetCreated.Invoke(this);
        }
    }
}
