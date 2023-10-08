using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController: MonoBehaviour {

    public static PlayerController shared;

    public float streetWidth = 4;
    public float numberOfLanes = 4;
    public float zMovement = 1;
    public float speed = 1;
    public float slowmoSpeed = 1;
    public Image slowmoImage;
    public Transform bus;
    public bool controlsEnabled;

    private float initialSpeed;
    [HideInInspector]
    public Vector3 movement = Vector3.zero;
    private Vector3 initialPosition;
    private const float slowmoScale = 0.2f;

    void Start() {
        shared = this;
        initialPosition = transform.position;
        initialSpeed = speed;
    }

    private void Update() {
        if (controlsEnabled) {
            CheckControls();
        }
    }

    void CheckControls() {
        if (Input.GetKey(KeyCode.LeftArrow)) {
            movement.x = -1;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            movement.x = 1;
        } else {
            movement.x = 0;
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            movement.z = -1;
        } else if (Input.GetKey(KeyCode.UpArrow)) {
            movement.z = 1;
        } else {
            movement.z = 0;
        }
        CheckSlowmo();
        movement *= Time.deltaTime * speed;
        ClampMovement();
        transform.Translate(movement, Space.World);
    }

    void CheckSlowmo() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (LevelMaker.shared.IsInSlowmo()) {
                LevelMaker.shared.StopSlowmo();
            } else {
                LevelMaker.shared.StartSlowmo();
            }
        }
    }

    void ClampMovement() {
        var position = transform.position + movement;
        var xLimit = streetWidth / 2;
        var xOffset = streetWidth / numberOfLanes * 0.5f;
        if (position.x < -xLimit + xOffset ||
            position.x > xLimit - xOffset) {
            movement.x = 0;
        }
        if (position.z < initialPosition.z - zMovement ||
            position.z > initialPosition.z + zMovement) {
            movement.z = 0;
        }
    }

    public void StartSlowmo() {
        bus.DOComplete();
        var duration = LevelMaker.stopSlowmoAnimationTime;
        bus.DOScaleX(slowmoScale, duration);
        DOTween.To(() => speed, x => speed = x, slowmoSpeed, duration);
    }

    public void StopSlowmo() {
        bus.DOComplete();
        var duration = LevelMaker.stopSlowmoAnimationTime;
        bus.DOScaleX(1f, duration);
        DOTween.To(() => speed, x => speed = x, initialSpeed, duration);
    }

    public void SetSlowmoProgress(float progress) {
        slowmoImage.fillAmount = progress;
        slowmoImage.enabled = progress < 0.999f;
    }

    public void OnCarCollisionEnter(Collision collision) {
        LevelMaker.shared.OnGameOver();
        GetComponent<KnightBusEffects>().Explode();
    }
}
