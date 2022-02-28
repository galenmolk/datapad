using Datapad.Models;

namespace Datapad.AudioPlayers
{
    public interface IAudioPlayer
    {
        public void PlayAudio(AudioAssetConfig asset);
        public void StopAudio();
    }
}
