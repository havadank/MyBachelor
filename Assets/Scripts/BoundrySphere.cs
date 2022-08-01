using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundrySphere : MonoBehaviour
{
    MeshFilter[] meshFilters;
    BoundryMesh[] hexes;
    public GameObject center;
    public GameObject edge;

    public GameObject target;
    public GameObject placement;

    [SerializeField, Range(0, 256)]
    int meshCount = 10;

    private Vector3 centerVector;
    private float radius;
    private int hexCount;

    private void Start()
    {
        centerVector = center.transform.position;
        radius = Vector3.Distance(centerVector, edge.transform.position);
        Initialize();
        GenerateMesh();
    }

    private void OnValidate()
    {
        centerVector = center.transform.position;
        radius = Vector3.Distance(centerVector, edge.transform.position);
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0 || meshFilters.Length != meshCount)
        {
            var children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));

            meshFilters = new MeshFilter[meshCount];
            hexes = new BoundryMesh[meshCount];
        }

        for (int i = 0; i < meshCount; i++)
        {
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            meshFilters[i].sharedMesh = new Mesh();

            hexes[i] = new BoundryMesh(meshFilters[i].sharedMesh, Vector3.up, radius);
        }
    }

    void GenerateMesh()
    {
        foreach(BoundryMesh mesh in hexes)
        {
            mesh.ConstructMesh();
        }
    }
}
