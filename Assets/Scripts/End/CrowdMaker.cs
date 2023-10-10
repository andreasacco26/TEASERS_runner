using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
public class CrowdMaker: MonoBehaviour {

    public int crowdCount = 100;
    public Vector3[] area;
    public GameObject[] crowdObjects;

    private float groundY = 0.7f;

    void Start() {
        DOTween.SetTweensCapacity(1250, 780);
        EndAnimator.shared.crowd = new(crowdCount);
        for (int i = 0; i < crowdCount; i++) {
            var position = GeneratePointInsidePolygon(area);
            var instance = Instantiate(crowdObjects[Random.Range(0, crowdObjects.Length - 1)]);
            instance.transform.position = position;
            instance.transform.parent = transform;
            EndAnimator.shared.crowd.Add(instance.transform);
        }
    }

    private Vector3 GeneratePointInsidePolygon(Vector3[] polygon) {
        Vector3 MinVec = MinPointOnThePolygon(polygon);
        Vector3 MaxVec = MaxPointOnThePolygon(polygon);
        Vector3 GenVector;

        float x = ((Random.value) * (MaxVec.x - MinVec.x)) + MinVec.x;
        float z = ((Random.value) * (MaxVec.z - MinVec.z)) + MinVec.z;
        GenVector = new Vector3(x, groundY, z);

        while (IsPointInPolygon(polygon, GenVector)) {
            x = ((Random.value) * (MaxVec.x - MinVec.x)) + MinVec.x;
            z = ((Random.value) * (MaxVec.z - MinVec.z)) + MinVec.z;
            GenVector.x = x;
            GenVector.z = z;
        }
        return GenVector;

    }

    private Vector3 MinPointOnThePolygon(Vector3[] polygon) {
        float minX = polygon[0].x;
        float minZ = polygon[0].z;
        for (int i = 1; i < polygon.Length; i++) {
            if (minX > polygon[i].x) {
                minX = polygon[i].x;
            }
            if (minZ > polygon[i].z) {
                minZ = polygon[i].z;
            }
        }
        return new Vector3(minX, groundY, minZ);
    }

    private Vector3 MaxPointOnThePolygon(Vector3[] polygon) {
        float maxX = polygon[0].x;
        float maxZ = polygon[0].z;
        for (int i = 1; i < polygon.Length; i++) {
            if (maxX < polygon[i].x) {
                maxX = polygon[i].x;
            }
            if (maxZ < polygon[i].z) {
                maxZ = polygon[i].z;
            }
        }
        return new Vector3(maxX, groundY, maxZ);
    }

    private bool IsPointInPolygon(Vector3[] polygon, Vector3 point) {
        bool isInside = false;
        for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++) {
            if (((polygon[i].x > point.x) != (polygon[j].x > point.x)) &&
            (point.z < (polygon[j].z - polygon[i].z) * (point.x - polygon[i].x) / (polygon[j].x - polygon[i].x) + polygon[i].z)) {
                isInside = !isInside;
            }
        }
        return !isInside;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CrowdMaker))]
public class MovingBirdBezierControllerEditor: Editor {
    void OnSceneGUI() {
        CrowdMaker t = target as CrowdMaker;
        for (int i = 0; i < t.area.Length; i++) {
            Handles.Label(t.area[i] + Vector3.up * 2, "" + i);
            t.area[i] = Handles.PositionHandle(t.area[i], Quaternion.identity);
        }
        Handles.color = Color.red;
        List<Vector3> closedArea = new(t.area);
        if (closedArea.Count > 2) {
            closedArea.Add(closedArea[0]);
        }
        Handles.DrawPolyLine(closedArea.ToArray());
    }
}

#endif