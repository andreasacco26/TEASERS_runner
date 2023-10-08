using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMaker {

    public enum Orientation {
        Horizontal,
        Vertical
    }
    public int widthSegments = 1;
    public int lengthSegments = 1;
    public float width = 1.0f;
    public float length = 1.0f;
    public Orientation orientation = Orientation.Horizontal;
    public Material material;
    private static Mesh planeMesh;

    public PlaneMaker() {
        UpdateConfig();
    }

    public void UpdateConfig() {
        widthSegments = Mathf.Clamp(widthSegments, 1, 254);
        lengthSegments = Mathf.Clamp(lengthSegments, 1, 254);
    }


    public GameObject GetPlane() {
        GameObject plane = new();
        plane.name = "Road";
        plane.transform.position = Vector3.zero;
        Vector2 anchorOffset = Vector2.zero;

        MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
        plane.AddComponent(typeof(MeshRenderer));
        plane.GetComponent<MeshRenderer>().material = material;
        Mesh m = planeMesh;
        if (m == null) {
            m = new();
            m.name = plane.name;

            int hCount2 = widthSegments + 1;
            int vCount2 = lengthSegments + 1;
            int numTriangles = widthSegments * lengthSegments * 6;
            int numVertices = hCount2 * vCount2;

            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] uvs = new Vector2[numVertices];
            int[] triangles = new int[numTriangles];

            int index = 0;
            float uvFactorX = 1.0f / widthSegments;
            float uvFactorY = 1.0f / lengthSegments;
            float scaleX = width / widthSegments;
            float scaleY = length / lengthSegments;
            for (float y = 0.0f; y < vCount2; y++) {
                for (float x = 0.0f; x < hCount2; x++) {
                    if (orientation == Orientation.Horizontal) {
                        vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, 0.0f, y * scaleY - length / 2f - anchorOffset.y);
                    } else {
                        vertices[index] = new Vector3(x * scaleX - width / 2f - anchorOffset.x, y * scaleY - length / 2f - anchorOffset.y, 0.0f);
                    }
                    uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
                }
            }

            index = 0;
            for (int y = 0; y < lengthSegments; y++) {
                for (int x = 0; x < widthSegments; x++) {
                    triangles[index] = (y * hCount2) + x;
                    triangles[index + 1] = ((y + 1) * hCount2) + x;
                    triangles[index + 2] = (y * hCount2) + x + 1;

                    triangles[index + 3] = ((y + 1) * hCount2) + x;
                    triangles[index + 4] = ((y + 1) * hCount2) + x + 1;
                    triangles[index + 5] = (y * hCount2) + x + 1;
                    index += 6;
                }
            }

            m.vertices = vertices;
            m.uv = uvs;
            m.triangles = triangles;
            m.RecalculateNormals();
            planeMesh = m;
        }

        meshFilter.sharedMesh = m;
        m.RecalculateBounds();
        plane.AddComponent(typeof(BoxCollider));
        return plane;
    }

}
