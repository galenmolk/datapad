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
            #elif UNITY_STANDALONE_OSX
                return new StandaloneAudioFileSelector();
            #else
                return new UnsupportedAudioFileSelector();
            #endif
        }
    }
}
