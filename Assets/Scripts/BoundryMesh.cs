using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryMesh
{
    Mesh mesh;
    float distance;
    Vector3 Center;
    Vector3 LocalUp;
    Vector3 AxisA;
    Vector3 AxisB;

    public BoundryMesh(Mesh mesh, Vector3 localUp, float distance)
    {
        this.mesh = mesh;
        LocalUp = localUp;
        Center = localUp*distance;

        AxisA = new Vector3(localUp.y, localUp.z, localUp.x);
        AxisB = Vector3.Cross(localUp, AxisA);
        this.distance = distance;
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[7];
        vertices[0] = Center;

        for (int y = -1; y < 2; y += 2)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3 pointOnHex = (LocalUp * distance) + (x*AxisA).normalized + (y*AxisB).normalized;
            }
        }

        // Create the triangles in the hexagon
        int[] triangles = new int[18];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        triangles[6] = 0;
        triangles[7] = 3;
        triangles[8] = 6;
        triangles[9] = 0;
        triangles[10] = 6;
        triangles[11] = 5;
        triangles[12] = 0;
        triangles[13] = 5;
        triangles[14] = 4;
        triangles[15] = 0;
        triangles[16] = 4;
        triangles[17] = 1;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
