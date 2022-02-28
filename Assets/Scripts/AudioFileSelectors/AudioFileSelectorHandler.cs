using System;
using UnityEngine;

namespace Datapad
{
    public class AudioFileSelectorHandler : MonoBehaviour
    {
        private IAudioFileSelector _fileSelector;

        public void SelectAudioFiles()
        {
            _fileSelector.SelectAudioFiles();
        }
        
        private void Awake()
        {
            _fileSelector = GetAudioFileSelector();
        }

        private IAudioFileSelector GetAudioFileSelector()
        {
            #if UNITY_EDITOR
                return new EditorAudioFileSelector();
            #elif UNITY_ANDROID
                return new AndroidAudioFileSelector();
            #endif
        }
    }
}
