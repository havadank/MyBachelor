using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Sphere
{
    Vector3 position;
    float radius;
    //Vector3 albedo;
    //Vector3 specular;

    public Sphere(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
}

public class ReachVisualizer : MonoBehaviour
{
    public GameObject Center;
    public GameObject Edge;

    public GameObject Target;
    public GameObject Placement;

    private Vector3 centerPosition;
    private Vector3 edgePosition;
    private float radius;

    public ComputeShader ReachShader;


    // Start is called before the first frame update
    void Start()
    {
        centerPosition = Center.transform.position;
        edgePosition = Edge.transform.position;
        radius = Vector3.Distance(centerPosition, edgePosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
