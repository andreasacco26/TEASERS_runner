using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuController: MonoBehaviour {

    public CinemachineVirtualCamera menuCamera;
    public Vector3 lightInitialEulerAngles;
    public GameObject lights;
    public CanvasGroup menuUI;
    public CanvasGroup tutorialUI;
    public CanvasGroup settingsUI;
    private Vector3 lightsGameplayEulerAngles;

    private void Start() {
        lightsGameplayEulerAngles = lights.transform.eulerAngles;
        lights.transform.eulerAngles = lightInitialEulerAngles;
        if (!LevelMaker.restartWithMenu) {
            menuCamera.Priority = 1;
            menuUI.alpha = 0;
            enabled = false;
            LevelMaker.shared.StartGameplay();
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if (LevelMaker.restartWithMenu &&
            Input.anyKeyDown &&
            !(Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2))) {
            menuCamera.Priority = 1;
            menuUI.DOFade(0, 0.5f).OnComplete(() => Destroy(menuUI.gameObject));
            LevelMaker.shared.StartGameplay();
            lights.transform.DORotate(lightsGameplayEulerAngles, 3f);
            GetComponent<AudioSource>().DOFade(0f, 1f).OnComplete(() => {
                var animations = DOTween.Sequence();
                animations.AppendInterval(2f);
                animations.Append(tutorialUI.DOFade(1f, 0.5f));
                animations.AppendInterval(3f);
                animations.Append(tutorialUI.DOFade(0f, 0.5f));
                animations.Play();
                Destroy(gameObject);
            });
            enabled = false;
        }
    }

    public void OnSettingsSelected() {
        settingsUI.gameObject.SetActive(true);
    }
}
