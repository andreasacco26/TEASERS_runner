using System.Collections.Generic;
using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public List<string> sfxNames;
    public List<string> musicNames;
    public List<string> ambienceNames;

    List<string> GetSoundNames(List<Sound> sounds) {
        List<string> soundNames = new();
        foreach(Sound sound in sounds) {
            soundNames.Add(sound.name);
        }
        return soundNames;
    }

    void Start() {
        sfxNames = GetSoundNames(AudioManager.Instance.sfxSounds);
        musicNames = GetSoundNames(AudioManager.Instance.musicSounds);
        ambienceNames = GetSoundNames(AudioManager.Instance.ambienceSounds);
    }
}
