using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticObjectsSpawner: MonoBehaviour {

    public Vector3 objectsRotation;
    public bool isLeft = false;
    public float offset = 0f;
    public GameObject[] itemsToSpawn;
    public float destroyerZ = -35;

    private Vector3 lastExtents = Vector3.zero;
    private int layer;
    private Transform lastSpawned;

    void Start() {
        layer = LayerMask.NameToLayer("Obstacle");
        SpawnFirst();
    }

    void Update() {
        if (!lastSpawned ||
            lastSpawned.position.z + lastExtents.x < transform.position.z) {
            Spawn(Vector3.zero, false);
        }
    }

    private void SpawnFirst() {
        do {
            var position = lastSpawned ? lastSpawned.position : new Vector3(transform.position.x, transform.position.y, destroyerZ);
            position.x = transform.position.x;
            position.z += lastExtents.x;
            Spawn(position, true);
        } while (lastSpawned.position.z + lastExtents.x < transform.position.z);
    }

    private void Spawn(Vector3 customPosition, bool useCustomPosition = false) {
        var item = itemsToSpawn[Random.Range(0, itemsToSpawn.Length - 1)];
        var instantiatedItem = Instantiate(item, transform.position, Quaternion.identity);
        instantiatedItem.layer = layer;
        var boxCollider = instantiatedItem.AddComponent<BoxCollider>();
        boxCollider.size = Vector3.one;
        var extents = instantiatedItem.GetComponent<Renderer>().bounds.extents;
        extents.x += offset;
        boxCollider.center = new(extents.x * (isLeft ? -1.1f : 1.1f), 0, 0);
        var position = transform.position;
        if (useCustomPosition) {
            position = customPosition;
        }
        position.z += extents.x;
        position.x += extents.z * (isLeft ? -1 : 1);
        instantiatedItem.transform.position = position;
        instantiatedItem.transform.localEulerAngles = objectsRotation;
        LevelMaker.shared.AddObjectToMove(instantiatedItem.transform);
        lastSpawned = instantiatedItem.transform;
        lastExtents = extents;
    }
}
