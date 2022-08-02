using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundrySphere : MonoBehaviour
{
    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    [SerializeField, HideInInspector]
    GameObject[] sides;

    BoundryFace[] faces;
    public GameObject center;
    public GameObject edge;

    public GameObject target;
    public GameObject placement;

    [SerializeField, Range(0, 256)]
    int resolution = 10;

    private Vector3 centerVector;
    private float radius;

    private void Start()
    {
        centerVector = center.transform.position;
        radius = Vector3.Distance(centerVector, edge.transform.position);
        Initialize();
        //GenerateMesh();
    }

    private void OnValidate()
    {
        centerVector = center.transform.position;
        radius = Vector3.Distance(centerVector, edge.transform.position);
        Initialize();
        //GenerateMesh();
    }

    void Initialize()
    {
        if (sides == null || sides.Length == 0)
        {
            sides = new GameObject[6];
        }
        //if (meshFilters == null || meshFilters.Length == 0)
        //{
        //    meshFilters = new MeshFilter[6];
        //}
        faces = new BoundryFace[resolution];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        for (int i = 0; i < 6; i++)
        {
            if (sides[i] == null)
            {
                GameObject side = new GameObject("Side");
                side.transform.parent = transform;

                //side.AddComponent<BoundryFace>() = new BoundryFace(resolution, directions[i], radius);
            }
            //if (meshFilters[i] == null)
            //{
            //    GameObject meshObj = new GameObject("mesh");

            //    meshObj.transform.parent = transform;

            //    meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            //    meshFilters[i] = meshObj.AddComponent<MeshFilter>();
            //    meshFilters[i].sharedMesh = new Mesh();
            //}

            faces[i] = new BoundryFace(/*meshFilters[i].sharedMesh,*/ resolution, directions[i], radius);
        }
    }

    //void GenerateMesh()
    //{
    //    foreach(BoundryFace face in faces)
    //    {
    //        face.ConstructMesh();
    //    }
    //}
}
