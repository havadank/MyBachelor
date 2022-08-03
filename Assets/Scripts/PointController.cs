using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour
{
    PointCloud pointCloud;

    [SerializeField]
    private int m_PointAmount = 100;
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
    public Vector4[] PointsInSpace;
    [SerializeField, HideInInspector]
    GameObject[] BorderSpherePrimitives;
    [SerializeField, HideInInspector]
    MeshFilter[] BorderSpheres;

    [SerializeField]
    ComputeShader calcShader;
    ComputeBuffer calcBuffer;
    [SerializeField]
    ComputeShader pointsShader;

    List<GameObject> geometryList;

    // Start is called before the first frame update
    void Start()
    {
        geometryList = new List<GameObject>();
        pointCloud = new PointCloud(m_PointAmount, m_PointsPerUnit, /*m_UseComputeShader*/ false, Target, Placement, Center, Edge, pointsShader);
        pointCloud.Initialize();
        pointCloud.Generate();
        PointsInSpace = pointCloud.getPointsInSpace();

        initializeList();
    }

    // Update is called once per frame
    void Update()
    {
        TargetPos = Target.transform.position;
        PlacementPos = Placement.transform.position;
        if (m_UseComputeShader)
        {
            //stuff += 0.0001f;
            SetShaderParams();
            RunOnGPU();
        }

        else
        {
            RunOnCPU();
        }
    }

    void SetShaderParams()
    {
        calcBuffer = new ComputeBuffer(m_PointAmount * m_PointAmount * m_PointAmount, sizeof(float) * 4);
        calcBuffer.SetData(PointsInSpace);

        calcShader.SetVector("_PlacePos", PlacementPos);
        calcShader.SetVector("_TargetPos", TargetPos);
        calcShader.SetBuffer(0, "_Points", calcBuffer);
    }

    void RunOnGPU()
    {
        calcShader.Dispatch(0, 32, 32, 1);
        calcBuffer.GetData(PointsInSpace);

        calcBuffer.Dispose();

        for (int i = 0; i < PointsInSpace.Length; i++)
        {
            Vector4 p4 = PointsInSpace[i];
            Vector3 p = new Vector3(p4.x, p4.y, p4.z);
            GameObject[] arr = geometryList.ToArray();
            foreach (GameObject obj in arr)
            {
                if (p == obj.transform.position && p4.w > 1f)
                {
                    obj.transform.localScale = new Vector3(0.002f * p4.w, 0.002f * p4.w, 0.002f * p4.w);
                }
            }
        }
    }

    void RunOnCPU()
    {
        GameObject[] geometryArr = geometryList.ToArray();
        for (int i = 0; i < geometryArr.Length; i++)
        {
            if (Vector3.Distance(geometryArr[i].transform.position, TargetPos) < 0.3f)
            {
                float scale = 0.05f;
                geometryArr[i].transform.localScale = new Vector3(scale, scale, scale);
            }
            else if (Vector3.Distance(geometryArr[i].transform.position, PlacementPos) < 0.4f)
            {
                float scale = (Vector3.Distance(geometryArr[i].transform.position, PlacementPos));
                float s = Mathf.Clamp(0.005f / (scale - radius), 0.0f, 0.05f);
                geometryArr[i].transform.localScale = new Vector3(s, s, s);
            }
            else
            {
                geometryArr[i].transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
    }

    void initializeList()
    {
        // //geometryList.RemoveAll(p => p.gameObject == GameObject.CreatePrimitive(PrimitiveType.Sphere));

        //foreach (GameObject obj in geometryList)
        //{
        //    GameObject.Destroy(obj);
        //}

        //geometryList.Clear();
        foreach (Vector4 point in PointsInSpace)
        {

            if (point.w == 0)
            {
                continue;
            }
            else
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(point.x, point.y, point.z);
                sphere.transform.localScale = new Vector3(0.002f * point.w, 0.002f * point.w, 0.002f * point.w);
                geometryList.Add(sphere);
            }
        }
    }

    //private void GenerateSpheresPrims()
    //{
    //    GameObject[] prims = new GameObject[PointsInSpace.Length];
    //    for (int i = 0; i < prims.Length; i++)
    //    {
    //        Vector4 point = PointsInSpace[i];
    //        Vector3 test = new Vector3(point.x, point.y, point.z);

    //        if (point.w == 0)
    //        {
    //            prims[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //            prims[i].transform.position = test;
    //            prims[i].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
    //        }
    //    }
    //    BorderSpherePrimitives = prims;
    //}

    //private void GenerateSpheres()
    //{
    //    if (BorderSpheres == null || BorderSpheres.Length == 0)
    //    {
    //        BorderSpheres = new MeshFilter[PointsInSpace.Length];
    //    }
    //}

    //private void UpdatePrimsActiveState()
    //{
    //    for (int i = 0; i < PointsInSpace.Length; i++)
    //    {
    //        Vector4 point = PointsInSpace[i];
    //        Vector3 test = new Vector3(point.x, point.y, point.z);

    //        if (PointsInSpace[i].w == 1 && BorderSpherePrimitives[i] != null)
    //        {
    //            BorderSpherePrimitives[i].SetActive(true);
    //        }
    //        if (Vector3.Distance(TargetPos, test) <= 0.5f && BorderSpherePrimitives[i] != null)
    //        {
    //            BorderSpherePrimitives[i].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    //        }
    //        //if (BorderSpherePrimitives[i] != null && (Vector3.Distance(test, TargetPos) < 0.02f || Vector3.Distance(test, PlacementPos) < 0.02f))
    //        //{
    //        //    BorderSpherePrimitives[i].SetActive(true);
    //        //}
    //        //else if (BorderSpherePrimitives[i] != null)
    //        //{
    //        //    BorderSpherePrimitives[i].SetActive(false);
    //        //}
    //    }
    //}
}
