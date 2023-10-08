using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Subsystems;
using UnityEngine.VFX;

public class KnightBusEffects: MonoBehaviour {
    // 1. Exahust fire bang
    [Header("Explosion Effect")]
    [Tooltip("The speed at which the parts of the car will fly off when exploding.")]
    public float explosionForce = 30.0f;
    [Tooltip("The angular speed at which the parts of the car will fly off when exploding.")]
    public float explosionTorque = 30.0f;
    [Tooltip("Object that contains the car parts that will fly off when exploding.")]
    public GameObject knightBusPartsContainer;

    [Header("Bus Audio")]
    [Tooltip("The pitch of the engine relative to the speed of the car.")]
    public AnimationCurve enginePitchOverSpeed = new(new Keyframe(0, 0.25f), new Keyframe(1, 1));
    [Tooltip("The volume of the brake sound relative to the speed delta of the car. The speed delta is calculated by dividing the current speed by the max speed.")]
    public AnimationCurve driftOverSpeed = new AnimationCurve(new Keyframe(0.25f, 0), new Keyframe(0.75f, 1));

    public AudioSource engineSource, brakeSource;
    public Sound engineSound, brakeSound;

    [Header("Exhaust fire")]
    [Tooltip("Reference to the exhaust fire VFX to spawn.")]
    public ParticleSystem exhaustFireVfx;
    [Tooltip("Reference to the exhaust location.")]
    public Transform exhaust;

    public bool HasExploded { get; private set; }

    private List<Rigidbody> knightBusRigidbodies;
    private GameObject container;

    private void InitExplosionData() {
        HasExploded = false;
        container = Instantiate(knightBusPartsContainer);
        knightBusRigidbodies = new(container.GetComponentsInChildren<Rigidbody>());
        container.SetActive(false);
    }

    private void InitAudioData() {
        brakeSource.playOnAwake = false;
        brakeSource.loop = true;
        brakeSource.clip = brakeSound.clip;
        brakeSource.volume = 0;
        brakeSource.Play();

        engineSource.playOnAwake = false;
        engineSource.loop = true;
        engineSource.clip = engineSound.clip;
        engineSource.volume = 1;
        engineSource.Play();
    }

    private void Start() {
        InitExplosionData();
        InitAudioData();
    }

    private void Update() {
        if (!engineSource.isPlaying && engineSource.isActiveAndEnabled) engineSource.Play();
        float speedDelta = Mathf.Clamp01(Mathf.Abs(PlayerController.shared.movement.z));
        engineSource.pitch = enginePitchOverSpeed.Evaluate(speedDelta);

        if (Input.GetKeyDown(KeyCode.UpArrow) && PlayerController.shared.controlsEnabled) {
            AudioManager.Instance.PlaySfx("BangInstant");
            ParticleSystem exhaustFire = Instantiate(exhaustFireVfx, exhaust.transform);
            exhaustFire.Play();
            Destroy(exhaustFire.gameObject, 0.5f);
        }

        float brakeDelta = Input.GetKeyDown(KeyCode.DownArrow) ? speedDelta : 0.0f;
        brakeSource.volume = Mathf.Clamp01(driftOverSpeed.Evaluate(brakeDelta));
    }

    public void Explode() {
        HasExploded = true;

        container.transform.SetPositionAndRotation(
           PlayerController.shared.transform.position,
           PlayerController.shared.transform.rotation);
        PlayerController.shared.gameObject.SetActive(false);

        container.SetActive(true);
        LevelMaker.shared.AddObjectToMove(container.transform);
        foreach (Rigidbody body in knightBusRigidbodies) {
            Vector3 forceVector = Random.onUnitSphere;
            forceVector.y = Mathf.Abs(forceVector.y) * 2;
            forceVector.Normalize();
            body.AddForce(forceVector * explosionForce, ForceMode.Impulse);
            body.AddTorque(Random.onUnitSphere * explosionTorque);
        }

        AudioManager.Instance.PlaySfx("Explosion");
    }

    public void Restore() {
        container.SetActive(false);
        PlayerController.shared.gameObject.SetActive(true);
        HasExploded = false;
    }
}
