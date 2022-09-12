using Datapad.Models;
using UnityEngine.Events;

namespace Datapad.Utils
{
    public class AudioAssetConfigEvent : UnityEvent<AudioAssetConfig> { }
    public class AudioLibraryConfigEvent : UnityEvent<AudioLibraryConfig> { }

    public class StringEvent : UnityEvent<string> { }
}
