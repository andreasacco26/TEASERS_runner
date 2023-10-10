using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EndAnimator: MonoBehaviour {

    public Transform bus;
    public Vector3 busTarget;
    public float busDuration;

    public GameObject[] band;
    public Transform[] bandDestinations;
    public Vector3 bandSpawnPosition;
    public Vector3 bandStageDestination;
    public float bandStageDestinationDuration;
    public float bandSpawnDelay = 1f;

    [HideInInspector]
    public List<Transform> crowd = new();

    private List<GameObject> instantiatedBand = new();

    public static EndAnimator shared { get; private set; }

    void Awake() {
        if (shared != null && shared != this) {
            Destroy(this);
        } else {
            shared = this;
        }
    }

    void Start() {
        var sequence = DOTween.Sequence();
        sequence.Append(bus.DOMove(busTarget, busDuration).SetEase(Ease.OutQuint));
        sequence.AppendCallback(CreateBand);
        sequence.Play();
    }

    void CreateBand() {
        for (int i = 0; i < band.Length; i++) {
            var instance = Instantiate(band[i]);
            instance.transform.position = bandSpawnPosition;
            instantiatedBand.Add(instance);
            instance.SetActive(false);
        }
        MoveBand(0);
    }

    void MoveBand(int index) {
        if (index >= band.Length) { return; }
        instantiatedBand[index].SetActive(true);
        instantiatedBand[index].transform
            .DOMove(bandStageDestination, bandStageDestinationDuration)
            .SetEase(Ease.Linear)
            .SetDelay(bandSpawnDelay * index)
            .OnComplete(() => MoveBandToFinalPosition(index));
        MoveBand(index + 1);
    }

    void MoveBandToFinalPosition(int index) {
        var animation = instantiatedBand[index].transform
            .DOMove(bandDestinations[index].position, bandStageDestinationDuration)
            .SetEase(Ease.Linear);
        if (index == band.Length - 1) {
            animation = animation.OnComplete(StartCrowdAnimation);
        }
        animation.Play();
    }

    public void StartCrowdAnimation() {
        foreach (Transform t in crowd) {
            var time = Random.Range(0.1f, 0.2f);
            var animationSequence = DOTween.Sequence();
            animationSequence.Append(t.DOLocalMoveY(t.localPosition.y + 0.5f, time).SetEase(Ease.InQuad));
            animationSequence.Append(t.DOLocalMoveY(t.localPosition.y, time).SetEase(Ease.OutQuad));
            animationSequence.SetLoops(-1);
            animationSequence.Play();
        }
    }
}
