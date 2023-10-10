using UnityEngine;
using DG.Tweening;

public enum MusicalInstrumentType {
    BASS,
    DRUM,
    GUITAR,
    MICROPHONE
}

public class MusicalInstrument: MonoBehaviour {

    public MusicalInstrumentType type;

    private Sequence animationSequence;

    void Start() {
        gameObject.layer = 3;
        name = "MusicalInstrument";
        LevelMaker.shared.AddObjectToMove(transform);
        transform.parent = LevelMaker.shared.transform;
        var collider = gameObject.AddComponent<BoxCollider>();
        collider.center = new Vector3(0, 1.4f, 0);
        var child = transform.GetChild(0);
        animationSequence = DOTween.Sequence();
        animationSequence.Append(child.DOLocalMoveY(child.localPosition.y + 2f, 1.0f).SetEase(Ease.InOutQuad));
        animationSequence.Append(child.DOLocalMoveY(child.localPosition.y, 1.0f).SetEase(Ease.InOutQuad));
        animationSequence.SetLoops(-1);
        animationSequence.Play();
    }

    private void OnDestroy() {
        animationSequence.Kill();
        animationSequence = null;
    }
}
