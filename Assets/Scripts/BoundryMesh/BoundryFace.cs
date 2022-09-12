using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundryFace : MonoBehaviour
{
    Mesh mesh;
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    [SerializeField, Range(0, 256)]
    int resolution = 50;
    float radius;
    Vector3 LocalUp;
    Vector3 AxisA;
    Vector3 AxisB;

    public BoundryFace(/*Mesh mesh,*/ int resolution, Vector3 localUp, float radius)
    {
        //this.mesh = mesh;
        this.resolution = resolution;
        Vector3 vector3 = localUp;
        this.radius = radius;

        AxisA = new Vector3(localUp.y, localUp.z, localUp.x);
        AxisB = Vector3.Cross(localUp, AxisA);
    }

    private void Start()
    {
        ConstructMesh();
    }

    private void OnValidate()
    {
        ConstructMesh();
    }

    public void Initialize()
    {
        
    }

    public void ConstructMesh()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[resolution];
        }

        Vector3[] vertices = new Vector3[resolution * resolution];
        
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = (LocalUp * radius) + (percent.x - .5f) * 2 * AxisA + (percent.y - .5f) * 2 * AxisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = pointOnUnitSphere;
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");

                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            BoundryMesh boundry = new BoundryMesh(mesh, vertices[i]/*, 1 / resolution*/);
            boundry.ConstructTriMesh();
        }

        //foreach (Vector3 point in vertices)
        //{

        //    BoundryMesh boundry = new BoundryMesh(mesh, point/*, 1 / resolution*/);
        //    boundry.ConstructTriMesh();
        //}
    }
}
