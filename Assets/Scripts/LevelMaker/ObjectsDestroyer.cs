using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsDestroyer: MonoBehaviour {

    public LayerMask layer;

    private void OnTriggerEnter(Collider other) {
        // Checks if layer containt other.gameObject.layer
        if (layer == (layer | (1 << other.gameObject.layer))) {
            Destroy(other.gameObject);
        }
    }
}
