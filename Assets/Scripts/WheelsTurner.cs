using UnityEngine;
using DG.Tweening;

static class AnimationName {
    public static readonly string left = "LEFT";
    public static readonly string right = "RIGHT";
    public static readonly string straight = "STRAIGHT";
}

public class WheelsTurner: MonoBehaviour {
    public GameObject leftTire, rightTire;
    public float rotationAngle = 30f;
    public float bodyRotationAngle = 10f;
    public float rotationTime = 0.2f;

    private Tweener bodyAnimation;

    void RotateTires(float rotationAngle) {
        leftTire.transform.DOLocalRotate(new Vector3(0, rotationAngle, 90), rotationTime, RotateMode.Fast);
        rightTire.transform.DOLocalRotate(new Vector3(0, rotationAngle + 180f, 90), rotationTime, RotateMode.Fast);
    }

    void Update() {
        var movement = PlayerController.shared.movement;
        var parent = transform.parent;
        if (movement.x < 0 && parent.eulerAngles.y >= 0) {
            if (bodyAnimation != null && bodyAnimation.stringId == AnimationName.left) {
                return;
            }
            RotateTires(-rotationAngle);
            bodyAnimation = parent.DOLocalRotate(new Vector3(0, -bodyRotationAngle, 0), rotationTime);
            bodyAnimation.stringId = AnimationName.left;
        } else if (movement.x > 0) {
            if (bodyAnimation != null && bodyAnimation.stringId == AnimationName.right) {
                return;
            }
            RotateTires(rotationAngle);
            bodyAnimation = parent.DOLocalRotate(new Vector3(0, bodyRotationAngle, 0), rotationTime);
            bodyAnimation.stringId = AnimationName.right;
        } else {
            if (bodyAnimation != null && bodyAnimation.stringId == AnimationName.straight) {
                return;
            }
            if (parent.eulerAngles != Vector3.zero &&
                Mathf.Abs(parent.eulerAngles.y) != 10) {
                RotateTires(0);
                bodyAnimation = parent.DOLocalRotate(Vector3.zero, rotationTime);
                bodyAnimation.stringId = AnimationName.straight;
            }
        }
    }
}
