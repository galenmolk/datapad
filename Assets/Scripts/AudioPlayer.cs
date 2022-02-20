using Datapad.Models;
using UnityEngine;

namespace Datapad
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        
        public void PlayAudio(AudioAsset audioAsset)
        {
            Debug.Log($"{nameof(AudioPlayer)}.{nameof(PlayAudio)}: " +
                      $"Playing AudioAsset {audioAsset.FileName}.");


            audioSource.clip = audioAsset.AudioClip;
            audioSource.Play();
        }
    }
}
