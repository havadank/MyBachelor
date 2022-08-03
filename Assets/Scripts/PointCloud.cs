using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud
{
    private int m_PointAmount;
    private int m_PointsPerUnit;
    private bool m_UseComputeShader = false;

    //public GameObject Target;
    //public GameObject Placement;
    //Vector3 TargetPos;
    //Vector3 PlacementPos;

    public GameObject Center;
    public GameObject Edge;
    Vector3 CenterPos;
    Vector3 EdgePos;
    float radius;

    Vector4 RobotReach;

    public Vector4[] PointsInSpace;

    ComputeShader pointsShader;
    ComputeBuffer pointsBuffer;

    public PointCloud(int pointAmount, bool useComputeShader, GameObject target, GameObject placement, GameObject center, GameObject edge, ComputeShader pointsShader)
    {
        m_PointAmount = pointAmount;
        m_UseComputeShader = useComputeShader;
        //Target = target;
        //Placement = placement;
        Center = center;
        Edge = edge;
        this.pointsShader = pointsShader;
    }

    public PointCloud(int pointAmount, int pointsPerUnit, bool useComputeShader, GameObject target, GameObject placement, GameObject center, GameObject edge, ComputeShader pointsShader)
    {
        m_PointAmount = pointAmount;
        m_PointsPerUnit = pointsPerUnit;
        m_UseComputeShader = useComputeShader;
        //Target = target;
        //Placement = placement;
        Center = center;
        Edge = edge;
        this.pointsShader = pointsShader;
    }

    public Vector4[] getPointsInSpace()
    {
        return PointsInSpace;
    }

    public void Initialize()
    {
        //TargetPos = Target.transform.position;
        //PlacementPos = Placement.transform.position;

        CenterPos = Center.transform.position;
        EdgePos = Edge.transform.position;
        radius = Vector3.Distance(CenterPos, EdgePos);
    }

    private void SetShaderParams()
    {
        RobotReach = new Vector4(CenterPos.x, CenterPos.y, CenterPos.z, radius);
        PointsInSpace = new Vector4[m_PointAmount * m_PointAmount * m_PointAmount];

        pointsBuffer = new ComputeBuffer(m_PointAmount * m_PointAmount * m_PointAmount, sizeof(float) * 4);
        pointsBuffer.SetData(PointsInSpace);

        pointsShader.SetInt("_PointsAmount", m_PointAmount);
        pointsShader.SetVector("_RobotReach", RobotReach);
        pointsShader.SetBuffer(0, "_Points", pointsBuffer);
    }

    public void Generate()
    {
        if (!m_UseComputeShader)
        {
            GeneratePointsCPU();
        }
        else
        {
            SetShaderParams();
            GeneratePointsGPU();
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
                    if (Vector3.Distance(test, CenterPos) > radius + 0.1f && Vector3.Distance(test, CenterPos) < radius + 0.2f)
                    {
                        point = new Vector4((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit, 1);
                    }
                    else
                    {
                        point = new Vector4((float)i / m_PointsPerUnit, (float)j / m_PointsPerUnit, (float)k / m_PointsPerUnit, 0);
                    }
                    pointsArray[index] = point;
                    index++;
                }
            }
        }

        PointsInSpace = pointsArray;
    }

    private void GeneratePointsGPU()
    {
        pointsShader.Dispatch(0, 1, 1, 1);
        //System.Threading.Thread.Sleep(5);
        pointsBuffer.GetData(PointsInSpace);
        pointsBuffer.Dispose();
    }
}
