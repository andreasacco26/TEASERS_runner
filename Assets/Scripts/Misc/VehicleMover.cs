using UnityEngine;

public class VehicleMover: MonoBehaviour {
    const float minimumSpeed = -3f;
    const float maximumSpeed = 0f;
    public float speed;

    void Start() {
        speed = Random.Range(minimumSpeed, maximumSpeed);
    }
}
