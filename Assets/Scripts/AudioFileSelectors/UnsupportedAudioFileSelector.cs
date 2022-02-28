namespace Datapad
{
    public class UnsupportedAudioFileSelector : IAudioFileSelector
    {
        public void SelectAudioFiles()
        {
            throw new System.PlatformNotSupportedException();
        }
    }
}
