using UnityEngine;
using UnityEngine.Networking;

namespace Datapad.Models
{
    public class AudioAsset
    {
        public string FileName => _fileName;
        public string LocalPath => _localPath;
        public AudioClip AudioClip => _audioClip;
        
        private string _fileName;
        private string _localPath;
        private AudioClip _audioClip;

        public AudioAsset(string fileName, string localPath, UnityWebRequest www)
        {
            _fileName = fileName;
            _localPath = localPath;
            _audioClip = CreateAudioClip(www);
        }

        private AudioClip CreateAudioClip(UnityWebRequest www)
        {
            return DownloadHandlerAudioClip.GetContent(www);
        }
    }
}