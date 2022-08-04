using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 4;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    FaceTerrain[] terrainFaces;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
    }

    void Initialize()
    {
        if (terrainFaces == null || terrainFaces.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new FaceTerrain[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("mesh");
                meshObject.transform.parent = transform;

                meshObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Unlit/Transparent"));
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainFaces[i] = new FaceTerrain(meshFilters[i].sharedMesh, directions[i], resolution);
        }
    }

    void GenerateMesh()
    {
        GameObject.DestroyImmediate(GameObject.Find("Mesh"));
        foreach (FaceTerrain face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }
}