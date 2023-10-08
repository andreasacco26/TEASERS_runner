using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AudioTester)), CanEditMultipleObjects]
public class AudioTesterEditor : Editor
{
    string GetRandomElement(List<string> list) {
        Debug.Log(list.Count);
        if (list.Count == 1) {
            return list[0];
        }
        return list[Random.Range(0, list.Count)];
    }

    public override void OnInspectorGUI()
    {
        AudioTester tester = (AudioTester)target;
        if (GUILayout.Button("Play Random SFX!")) {
            AudioManager.Instance.PlaySfx(GetRandomElement(tester.sfxNames));
        }

        if (GUILayout.Button("Start Music")) {
            AudioManager.Instance.PlayMusic(GetRandomElement(tester.musicNames));
        }

        if (GUILayout.Button("Toggle Music")) {
            AudioManager.Instance.ToggleMusic();
        }

        if (GUILayout.Button("Start Ambience")) {
            AudioManager.Instance.PlayAmbience(GetRandomElement(tester.ambienceNames));
        }

        if (GUILayout.Button("Toggle Ambience")) {
            AudioManager.Instance.ToggleAmbience();
        }
    }

}
