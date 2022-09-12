using Datapad.Models;
using UnityEngine;

namespace Datapad.AudioPlayers
{
    public class AndroidAudioPlayer : IAudioPlayer
    {
        private const string CurrentActivityName = "currentActivity";
        private const string UnityClassName = "com.unity3d.player.UnityPlayer";
        private const string AudioPlayerClassName = "com.datapad.audioplayer.AudioPlayer";
        
        private const string SetActivityInstanceMethodName = "setActivityInstance";
        private const string PlayAudioMethodName = "PlayAudio";
        private const string StopAudioMethodName = "StopAudio";
        private const string UpdateLibraryMethodName = "UpdateLibrary";

        private AndroidJavaClass _unityClass;
        private AndroidJavaClass _audioPlayer;
        private AndroidJavaObject _unityActivity;

        private AudioAssetConfig lastAudio;
        
        public AndroidAudioPlayer()
        {
            InitializeAudioPlayerPlugin();
        }
        
        public void PlayAudio(AudioAssetConfig asset)
        {
            lastAudio = asset;
            _audioPlayer.CallStatic(PlayAudioMethodName, asset.LocalPath);
        }

        public void StopAudio()
        {
            _audioPlayer.CallStatic(StopAudioMethodName);
        }

        public void TrackComplete()
        {
            Debug.Log("TrackComplete");
        }

        public void UpdateNativeLibrary(string libraryJson)
        {
            Debug.Log("UpdateNativeLibrary Unity");
            _audioPlayer.CallStatic(UpdateLibraryMethodName, libraryJson);
        }

        public void MessageTest(string msg)
        {
            Debug.Log("This Message Worked: " + msg);
        }

        public void AudioPlaying()
        {
            Debug.Log("AUDIO IS PLAYING - JANE");
        }

        private void InitializeAudioPlayerPlugin()
        {
            _unityClass = new(UnityClassName);
            _audioPlayer = new(AudioPlayerClassName);
            _unityActivity = _unityClass.GetStatic<AndroidJavaObject>(CurrentActivityName);
            _audioPlayer.CallStatic(SetActivityInstanceMethodName, _unityActivity);
        }
    }
}
