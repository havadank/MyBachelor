using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryMesh
{
    Mesh mesh;
    float size;
    Vector3 Center;
    Vector3 AxisA;
    Vector3 AxisB;

    public BoundryMesh(Mesh mesh, Vector3 center/*, float size*/)
    {
        this.mesh = mesh;
        Center = center;
        //this.size = size;

        AxisA = new Vector3(Center.y, Center.z, Center.x);
        AxisB = Vector3.Cross(Center, AxisA);
    }

    public void ConstructTriMesh()
    {
        Vector3[] vertices = new Vector3[3];
        int[] triangle = new int[3];

        int i = 0;
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 1; y++)
            {
                Vector2 percent = new Vector2(x, y) / 1;
                Vector3 pointOnTriangle = Center + (percent.x - .5f) * AxisA + (percent.y - .5f) * AxisB;
                vertices[i] = pointOnTriangle;

                i++;
            }
        }

        triangle[0] = 0;
        triangle[1] = 1;
        triangle[2] = 2;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangle;
        mesh.RecalculateNormals();
    }

    public void ConstructHexMesh()
    {
        Vector3[] vertices = new Vector3[7];
        vertices[0] = Center;

        for (int y = -1; y < 2; y += 2)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3 pointOnHex = Center + (x*AxisA*size).normalized + (y*AxisB*size).normalized;
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
