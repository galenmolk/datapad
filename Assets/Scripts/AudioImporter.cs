using System;
using System.Collections;
using System.IO;
using Datapad.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Datapad
{
    public class AudioImporter : MonoBehaviour
    {
        private const string UriPrefix = "file://";
        
        public IEnumerator ImportAudio(string path, Action<AudioAsset> callback)
        {
            using var www = UnityWebRequestMultimedia.GetAudioClip($"{UriPrefix}{path}", AudioType.UNKNOWN);
            var fileName = new FileInfo(path).Name;
            var localPath = Path.Combine(Application.persistentDataPath, fileName);
            www.downloadHandler = new DownloadHandlerFile(localPath);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }

            callback(new AudioAsset(fileName, localPath, www));
        }
    }
}