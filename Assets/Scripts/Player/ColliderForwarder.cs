using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderForwarder: MonoBehaviour {

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == "Car") {
            PlayerController.shared.OnCarCollisionEnter(collision);
        } else if (collision.gameObject.name == "MusicalInstrument") {
            var instrument = collision.gameObject.GetComponent<MusicalInstrument>();
            PlayerController.shared.OnInstrumentCollisionEnter(instrument);
        }
    }
}
