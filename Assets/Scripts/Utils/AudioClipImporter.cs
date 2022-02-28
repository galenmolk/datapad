using System;
using System.Collections;
using Datapad.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Datapad
{
    public class AudioClipImporter : MonoBehaviour
    {
        private const string LocalUriPrefix = "file://";

        public void Import(AudioAssetConfig config)
        {
            if (config.AudioClip != null)
                return;

            StartCoroutine(GetAudioClipFromPath(config.LocalPath, config.SetAudioClip));
        }

        private IEnumerator GetAudioClipFromPath(string path, Action<AudioClip> callback)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip($"{LocalUriPrefix}{path}", AudioType.UNKNOWN);
            
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }
            
            callback(DownloadHandlerAudioClip.GetContent(www));
        }
    }
}
