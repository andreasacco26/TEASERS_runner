using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingObjectsSpawner: MonoBehaviour {

    public float streetWidth = 4;
    public int numberOfLanes = 4;
    public float speed = 5;
    public float slowmoSpeed = 1;
    public float minSpawnTime = 2;
    public float maxSpawnTime = 2;
    public int spawnCooldownCarsWall = 2;
    public GameObject[] itemsToSpawn;
    public GameObject[] itemsToCollect;

    [HideInInspector]
    public float initialSpeed = 5;

    private List<GameObject> remainingItemsToCollect = new();
    private float currentSpawnTime = 2;
    private readonly List<VehicleMover> itemsToMove = new();
    private int layer;
    private int currentSpawnCooldownCarsWall = 0;
    private bool CanSpawnCarsWall {
        get {
            return currentSpawnCooldownCarsWall >= spawnCooldownCarsWall;
        }
    }

    void Start() {
        layer = LayerMask.NameToLayer("Obstacle");
        initialSpeed = speed;
        currentSpawnTime = 5f; // Delay
        remainingItemsToCollect = new(itemsToCollect);
    }

    void Update() {
        if (speed != 0) {
            MoveItems();
        }
        currentSpawnTime -= Time.deltaTime * (speed / initialSpeed);
        if (currentSpawnTime <= 0) {
            Spawn();
            currentSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    private void Spawn() {
        var canSpawnMusicalInstrument = CanSpawnCarsWall;
        var numberOfItemsToRemove = Random.Range(1, numberOfLanes - 1);
        if (canSpawnMusicalInstrument) {
            numberOfItemsToRemove = 1;
            currentSpawnCooldownCarsWall = 0;
        }
        currentSpawnCooldownCarsWall += 1;
        var availablePositions = Enumerable.Range(0, numberOfLanes).ToList();
        var lastRemoved = 0;
        while (numberOfItemsToRemove > 0) {
            var toRemove = Random.Range(0, availablePositions.Count - 1);
            lastRemoved = availablePositions[toRemove];
            availablePositions.RemoveAt(toRemove);
            numberOfItemsToRemove--;
        }

        foreach (int position in availablePositions) {
            SpawnCarAtPosition(position);
        }

        if (canSpawnMusicalInstrument) {
            SpawnMusicalInstrumentAtPosition(lastRemoved);
        }
    }

    private void SpawnCarAtPosition(int position) {
        var itemPosition = PositionFromIndex(position);
        var item = itemsToSpawn[Random.Range(0, itemsToSpawn.Length - 1)];
        var instantiatedItem = Instantiate(item, itemPosition, Quaternion.identity);
        instantiatedItem.name = "Car";
        instantiatedItem.layer = layer;
        var collider = instantiatedItem.AddComponent<BoxCollider>();
        var center = collider.center;
        center.z -= collider.size.z * 0.05f;
        collider.center = center;
        var size = collider.size;
        size.x *= 0.78f;
        size.z *= 0.9f;
        collider.size = size;
        var mover = instantiatedItem.AddComponent<VehicleMover>();
        itemsToMove.Add(mover);
        mover.transform.parent = LevelMaker.shared.transform;
    }

    private void SpawnMusicalInstrumentAtPosition(int position) {
        if (remainingItemsToCollect.Count == 0) { return; }
        var itemPosition = PositionFromIndex(position);
        var itemIndex = Random.Range(0, remainingItemsToCollect.Count - 1);
        var item = remainingItemsToCollect[itemIndex];
        Instantiate(item, itemPosition, Quaternion.identity);
        remainingItemsToCollect.RemoveAt(itemIndex);
    }

    private Vector3 PositionFromIndex(int index) {
        var position = transform.position;
        position.x = (streetWidth / numberOfLanes * index) - (position.x + streetWidth * 0.5f);
        position.x += streetWidth / numberOfLanes * 0.5f;
        return position;
    }

    private void MoveItems() {
        foreach (VehicleMover t in itemsToMove) {
            if (t == null) continue;
            t.transform.Translate(0, 0, (-speed + t.speed) * Time.deltaTime);
        }
        CleanItemsToMove();
    }

    private void CleanItemsToMove() {
        itemsToMove.Remove(null);
    }
}
