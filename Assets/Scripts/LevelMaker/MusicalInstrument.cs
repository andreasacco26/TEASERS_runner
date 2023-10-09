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

    private Sequence animation;

    void Start() {
        gameObject.layer = 3;
        name = "MusicalInstrument";
        LevelMaker.shared.AddObjectToMove(transform);
        transform.parent = LevelMaker.shared.transform;
        var collider = gameObject.AddComponent<BoxCollider>();
        collider.center = new Vector3(0, 1.4f, 0);
        var child = transform.GetChild(0);
        animation = DOTween.Sequence();
        animation.Append(child.DOLocalMoveY(child.localPosition.y + 2f, 1.0f).SetEase(Ease.InOutQuad));
        animation.Append(child.DOLocalMoveY(child.localPosition.y, 1.0f).SetEase(Ease.InOutQuad));
        animation.SetLoops(-1);
        animation.Play();
    }

    private void OnDestroy() {
        animation.Kill();
        animation = null;
    }
}
