using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud : MonoBehaviour
{
    [SerializeField]
    private int m_PointAmount = 20;
    [SerializeField]
    private int m_PointsPerUnit = 50;
    [SerializeField]
    private bool m_UseComputeShader = false;

    public GameObject Target;
    public GameObject Placement;
    Vector3 TargetPos;
    Vector3 PlacementPos;

    public GameObject Center;
    public GameObject Edge;
    Vector3 CenterPos;
    Vector3 EdgePos;
    float radius;

    Vector4 RobotReach;

    [SerializeField, HideInInspector]
    Vector4[] PointsInSpace;
    [SerializeField, HideInInspector]
    GameObject[] BorderSpherePrimitives;
    [SerializeField, HideInInspector]
    MeshFilter[] BorderSpheres;

    public ComputeShader pointsShader;
    ComputeBuffer pointsBuffer;

    // Start is called before the first frame update
    void Start()
    {
        TargetPos = Target.transform.position;
        PlacementPos = Placement.transform.position;

        CenterPos = Center.transform.position;
        EdgePos = Edge.transform.position;
        radius = Vector3.Distance(CenterPos, EdgePos);

        if (!m_UseComputeShader)
        {
            GeneratePointsCPU();
            GenerateSpheresPrims();
        }
        
    }

    private void SetShaderParams()
    {
        RobotReach = new Vector4(CenterPos.x, CenterPos.y, CenterPos.z, radius);

        pointsBuffer = new ComputeBuffer(m_PointAmount * m_PointAmount * m_PointAmount, sizeof(float) * 4);
        pointsBuffer.SetData(GeneratePointsGPU());

        pointsShader.SetVector("_RobotReach", RobotReach);
        pointsShader.SetBuffer(0, "_Points", pointsBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_UseComputeShader)
        {
            SetShaderParams();
        }
        else
        {
            UpdatePrimsActiveState();
            
        }

    }

    private void UpdatePrimsActiveState()
    {
        for (int i = 0; i < PointsInSpace.Length; i++)
        {
            Vector4 point = PointsInSpace[i];
            Vector3 test = new Vector3(point.x, point.y, point.z);

            if (PointsInSpace[i].w == 1 && BorderSpherePrimitives[i] != null)
            {
                BorderSpherePrimitives[i].SetActive(true);
            }
            if (Vector3.Distance(TargetPos, test) <= 0.5f && BorderSpherePrimitives[i] == null)
            {
                BorderSpherePrimitives[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            //if (BorderSpherePrimitives[i] != null && (Vector3.Distance(test, TargetPos) < 0.02f || Vector3.Distance(test, PlacementPos) < 0.02f))
            //{
            //    BorderSpherePrimitives[i].SetActive(true);
            //}
            //else if (BorderSpherePrimitives[i] != null)
            //{
            //    BorderSpherePrimitives[i].SetActive(false);
            //}
        }
    }

    private void GenerateSpheresPrims()
    {
        GameObject[] prims = new GameObject[PointsInSpace.Length];
        for (int i = 0; i < prims.Length; i++)
        {
            Vector4 point = PointsInSpace[i];
            Vector3 test = new Vector3(point.x, point.y, point.z);
            
            if (point.w == 0)
            {
                prims[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                prims[i].transform.position = test;
                prims[i].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            }
        }
        BorderSpherePrimitives = prims;
    }

    private void GenerateSpheres()
    {
        if (BorderSpheres == null || BorderSpheres.Length == 0)
        {
            BorderSpheres = new MeshFilter[PointsInSpace.Length];
        }
    }

    private void GeneratePointsCPU()
    {
        Vector4[] pointsArray = new Vector4[m_PointAmount * m_PointAmount * m_PointAmount];
        int index = 0;
        for (int i = -m_PointAmount/2; i<m_PointAmount/2; i++)
        {
            for (int j = -m_PointAmount/2; j < m_PointAmount/2; j++)
            {
                for (int k = -m_PointAmount/2; k < m_PointAmount/2; k++)
                {
                    Vector4 point;
                    Vector3 test = new Vector3((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit);
                    if (Vector3.Distance(test, CenterPos) >= radius)
                    {
                        point = new Vector4((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit, 0);
                    }
                    else
                    {
                        point = new Vector4((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit, 1);
                    }
                    pointsArray[index] = point;
                    index++;
                }
            }
        }

        PointsInSpace = pointsArray;
    }

    private Vector4[] GeneratePointsGPU()
    {
        Vector4[] pointsArray = new Vector4[m_PointAmount * m_PointAmount * m_PointAmount];
        int index = 0;
        for (int i = -m_PointAmount / 2; i < m_PointAmount / 2; i++)
        {
            for (int j = -m_PointAmount / 2; j < m_PointAmount / 2; j++)
            {
                for (int k = -m_PointAmount / 2; k < m_PointAmount / 2; k++)
                {
                    Vector4 point;
                    Vector3 test = new Vector3((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit);
                    if (Vector3.Distance(test, CenterPos) > radius)
                    {
                        point = new Vector4((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit, 0);
                    }
                    else
                    {
                        point = new Vector4((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit, 1);
                    }
                    pointsArray[index] = point;
                    index++;
                }
            }
        }

        return pointsArray;
    }
}
