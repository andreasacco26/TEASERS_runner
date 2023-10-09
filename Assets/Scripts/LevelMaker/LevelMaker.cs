using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelMaker: MonoBehaviour {

    public const float slowmoAnimationTime = 1.5f;
    public const float stopSlowmoAnimationTime = 0.5f;

    public float currentSpeed = 5f;
    public float worldSpeed = 5f;
    public float worldSlowmoSpeed = 5f;
    public float slowmoTimer = 4f;
    public float cooldownSlowmoTimer = 4f;

    public float streetWidth = 17;
    public int numberOfLanes = 4;

    [HideInInspector]
    public float currentSlowmoTimer = 0;

    public Vector3 movingObjectsSpawnerPosition;
    public GameObject movingObjectsSpawner;
    private MovingObjectsSpawner _spawner;

    public Vector3 objectsDestroyerPosition;
    public GameObject objectsDestroyer;

    public Vector3 playerPosition;
    public GameObject player;

    //public Vector3 leftBuilingsSpawnerPosition;
    public GameObject leftBuilingsSpawner;

    //public Vector3 rightBuilingsSpawnerPosition;
    public GameObject rightBuilingsSpawner;

    public Vector3 streetSpawnerPosition;
    public GameObject streetSpawner;
    private StreetSpawner _streetSpawner;
    private readonly List<Transform> itemsToMove = new();
    private readonly List<Tweener> animators = new();
    private Tweener slowmoTimerAnimation;
    private Tweener slowmoCooldownTimerAnimation;

    private bool listenForGameOver;
    public static bool restartWithMenu = true;

    public static int level = 1;

    public static LevelMaker shared { get; private set; }

    void Awake() {
        if (shared != null && shared != this) {
            Destroy(this);
        } else {
            shared = this;
        }
    }

    void Start() {
        if (level == 0) {
            SetEasyLevel();
        } else if (level == 1) {
            SetMediumLevel();
        } else if (level == 2) {
            SetHardLevel();
        }
        shared = this;
        currentSpeed = worldSpeed;
        BuildStreetSpawner();
        BuildBuildingsSpawners();
        BuildObjectsDestroyer();
        BuildPlayer();
        currentSlowmoTimer = slowmoTimer;
    }

    private void Update() {
        MoveItems();
        if (listenForGameOver) {
            if (Input.anyKeyDown) {
                restartWithMenu = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void FixedUpdate() {
        CleanItemsToMove();
    }

    public void StartGameplay() {
        BuildMovingObjectsSpawner();
        var animations = DOTween.Sequence();
        animations.AppendInterval(restartWithMenu ? 2f : 0.1f);
        animations.AppendCallback(() => {
            ScoreManager.shared.StartRecording();
            PlayerController.shared.controlsEnabled = true;
        });
        animations.Play();
        AudioManager.Instance.PlayRandomMusic();
    }

    void BuildMovingObjectsSpawner() {
        if (_spawner != null) {
            return;
        }
        var instance = Instantiate(movingObjectsSpawner, movingObjectsSpawnerPosition, Quaternion.identity);
        CleanName(instance);
        _spawner = instance.GetComponent<MovingObjectsSpawner>();
        _spawner.speed = worldSpeed;
        _spawner.slowmoSpeed = worldSlowmoSpeed;
        _spawner.numberOfLanes = numberOfLanes;
        _spawner.streetWidth = streetWidth;
    }

    void BuildObjectsDestroyer() {
        var instance = Instantiate(objectsDestroyer, objectsDestroyerPosition, Quaternion.identity);
        CleanName(instance);
    }

    void BuildPlayer() {
        var playerObject = Instantiate(player, playerPosition, Quaternion.identity);
        CleanName(playerObject);
        var playerController = playerObject.GetComponent<PlayerController>();
        playerController.numberOfLanes = numberOfLanes;
        playerController.streetWidth = streetWidth;
    }

    void BuildBuildingsSpawners() {
        var position = movingObjectsSpawnerPosition;
        position.x = -streetWidth * 0.5f - 7f;
        var left = Instantiate(leftBuilingsSpawner, position, Quaternion.identity);
        CleanName(left);
        position.x = streetWidth * 0.5f + 7f;
        var right = Instantiate(rightBuilingsSpawner, position, Quaternion.identity);
        CleanName(right);
    }

    void BuildStreetSpawner() {
        var instance = Instantiate(streetSpawner, streetSpawnerPosition, Quaternion.identity);
        CleanName(instance);
        _streetSpawner = instance.GetComponent<StreetSpawner>();
        _streetSpawner.numberOfLanes = numberOfLanes;
        _streetSpawner.streetWidth = streetWidth;
        _streetSpawner.destroyerZ = objectsDestroyerPosition.z;
    }

    public void StartSlowmo() {
        if (!CanStartSlowmo()) {
            return;
        }
        if (slowmoCooldownTimerAnimation != null) {
            slowmoCooldownTimerAnimation.Kill();
            slowmoCooldownTimerAnimation = null;
        }
        foreach (Tweener animator in animators) {
            animator.Complete();
        }
        animators.Clear();
        animators.Add(DOTween.To(() => currentSpeed,
            x => currentSpeed = x,
            worldSlowmoSpeed, slowmoAnimationTime));
        animators.Add(DOTween.To(() => _spawner.speed,
            x => _spawner.speed = x,
            _spawner.slowmoSpeed, slowmoAnimationTime));

        var timer = slowmoTimer * (currentSlowmoTimer / slowmoTimer);
        slowmoTimerAnimation = DOTween.To(() => currentSlowmoTimer,
            x => currentSlowmoTimer = x,
            0, timer)
            .SetEase(Ease.Linear)
            .OnComplete(() => StopSlowmo())
            .OnUpdate(() => PlayerController.shared.SetSlowmoProgress(currentSlowmoTimer / slowmoTimer)); ;
        PlayerController.shared.StartSlowmo();
        AudioManager.Instance.ChangePitch(0.5f);
    }

    public void StopSlowmo() {
        if (slowmoCooldownTimerAnimation != null) {
            return;
        }
        if (slowmoTimerAnimation != null) {
            slowmoTimerAnimation.Kill();
            slowmoTimerAnimation = null;
        }
        slowmoCooldownTimerAnimation = null;
        foreach (Tweener animator in animators) {
            animator.Complete();
        }
        animators.Clear();
        animators.Add(DOTween.To(() => currentSpeed,
            x => currentSpeed = x,
            worldSpeed, stopSlowmoAnimationTime));
        animators.Add(DOTween.To(() => _spawner.speed,
            x => _spawner.speed = x,
            _spawner.initialSpeed, stopSlowmoAnimationTime));

        var timer = cooldownSlowmoTimer * (1 - (currentSlowmoTimer / slowmoTimer));
        slowmoCooldownTimerAnimation = DOTween.To(() => currentSlowmoTimer,
            x => currentSlowmoTimer = x,
            slowmoTimer, timer)
            .SetEase(Ease.Linear)
            .OnUpdate(() => PlayerController.shared.SetSlowmoProgress(currentSlowmoTimer / slowmoTimer));
        PlayerController.shared.StopSlowmo();
        AudioManager.Instance.ChangePitch(1.0f);
    }

    public bool CanStartSlowmo() {
        return !IsInSlowmo();
    }

    public bool IsInSlowmo() {
        return slowmoTimerAnimation != null;
    }

    public void AddObjectToMove(Transform obj) {
        itemsToMove.Add(obj);
        obj.parent = transform;
    }

    public void OnGameOver() {
        ScoreManager.shared.GameOver();
        var animations = DOTween.Sequence();
        animations.AppendInterval(2f);
        animations.AppendCallback(() => {
            listenForGameOver = true;
        });
        animations.Play();

    }

    private void MoveItems() {
        foreach (Transform t in itemsToMove) {
            if (t == null) continue;
            t.Translate(0, 0, -currentSpeed * Time.deltaTime, Space.World);
        }
    }

    private void CleanItemsToMove() {
        itemsToMove.Remove(null);
    }

    private void CleanName(GameObject obj) {
        obj.name = obj.name.Replace("(Clone)", "");
    }

    public void SetEasyLevel() {
        level = 0;
        numberOfLanes = 3;
        streetWidth = GetStreetWidth(numberOfLanes);
    }

    public void SetMediumLevel() {
        level = 1;
        numberOfLanes = 4;
        streetWidth = GetStreetWidth(numberOfLanes);
    }

    public void SetHardLevel() {
        level = 2;
        numberOfLanes = 5;
        streetWidth = GetStreetWidth(numberOfLanes);
    }

    private float GetStreetWidth(float numberOfLanes) {
        return streetWidth / 4 * numberOfLanes;
    }
}
