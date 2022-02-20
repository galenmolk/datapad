using System.Collections;
using System.IO;
using Datapad.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Datapad
{
    public class PlayAudio : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private TMP_Text debugText;

        private const string AUDIO_TYPE = "audio/*";
        
        private readonly string[] _fileTypes = { AUDIO_TYPE };
        
        public void SelectAudioFile()
        {
            debugText.text += "SelectAudioFile\n";
            Debug.Log("SelectAudioFile");
            NativeFilePicker.PickFile(FilePicked, _fileTypes);
        }

        private void FilePicked(string path)
        {
            Debug.Log("FilePicked. Path: " + path);
            debugText.text += "FilePicked. Path: " + path + "\n";

            if (string.IsNullOrWhiteSpace(path))
                return;

            debugText.text += "Not null. Downloading.\n";
            StartCoroutine(DownloadAudioClip(path));
        }

        private IEnumerator DownloadAudioClip(string path)
        {
            Debug.Log("DownloadAudioClip. Path: " + path);
            debugText.text += "DownloadAudioClip\n";

            FileInfo file = new FileInfo(path);
            // using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);
            
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.UNKNOWN);
            
            var localPath = Path.Combine(Application.persistentDataPath, file.Name);
            www.downloadHandler = new DownloadHandlerFile(localPath);

            yield return www.SendWebRequest();
    
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }
            
        }
    }
}
