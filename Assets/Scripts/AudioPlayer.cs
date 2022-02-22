using Datapad.Models;
using UnityEngine;

namespace Datapad
{
    public class AudioPlayer : Singleton<AudioPlayer>
    {
        private const string CurrentActivityName = "currentActivity";
        private const string UnityClassName = "com.unity3d.player.UnityPlayer";
        private const string AudioPlayerClassName = "com.datapad.audioplayer.AudioPlayer";
        private const string SetActivityInstanceMethodName = "setActivityInstance";
        private const string PlayAudioMethodName = "PlayAudio";


        private AndroidJavaClass unityClass;
        private AndroidJavaClass audioPlayer;
        
        private AndroidJavaObject unityActivity;

        public void PlayAudio(AudioAssetConfig audioAssetConfig)
        {   
            Debug.Log($"{nameof(AudioPlayer)}.{nameof(PlayAudio)}: " +
                      $"Playing AudioAsset {audioAssetConfig.FileName}.");
            
            audioPlayer.CallStatic(PlayAudioMethodName, audioAssetConfig.LocalPath);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            unityClass = new(UnityClassName);
            audioPlayer = new(AudioPlayerClassName);
            unityActivity = unityClass.GetStatic<AndroidJavaObject>(CurrentActivityName);
            audioPlayer.CallStatic(SetActivityInstanceMethodName, unityActivity);
        }
    }
}
