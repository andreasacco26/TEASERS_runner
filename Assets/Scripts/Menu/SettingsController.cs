using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsController: MonoBehaviour {

    public void OnEasySelected() {
        LevelMaker.level = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMediumSelected() {
        LevelMaker.level = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnHardSelected() {
        LevelMaker.level = 2;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnBackSelected() {
        gameObject.SetActive(false);
    }
}
