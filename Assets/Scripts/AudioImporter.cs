using System;
using System.Collections;
using System.IO;
using Datapad.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Datapad
{
    public class AudioImporter : Singleton<AudioImporter>
    {
        private const string UriPrefix = "file://";
        
        public IEnumerator ImportAudioAsset(string path, Action<AudioAssetConfig> callback)
        {
            yield return StartCoroutine(GetAudioClipFromPath(path, audioClip =>
            {
                var fileName = new FileInfo(path).Name;
                callback(new AudioAssetConfig(fileName, path, audioClip));
            }));
        }

        public IEnumerator GetAudioClipFromPath(string path, Action<AudioClip> callback)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip($"{UriPrefix}{path}", AudioType.UNKNOWN);
            
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }
            
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            callback(clip);
        }

        public void LoadAudioAsset(AudioAssetConfig config)
        {
            if (config.AudioClip != null)
                return;

            StartCoroutine(GetAudioClipFromPath(config.LocalPath, audioClip =>
            {
                config.AudioClip = audioClip;
            }));
        }
    }
}