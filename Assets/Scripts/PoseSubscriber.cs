//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PoseSubscriber : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
//
//    }
//
//    // Update is called once per frame
//    void Update()
//    {
//
//    }
//}

using System;
using System.Collections;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.Ur5EMoveitConfig;
using Sensor = RosMessageTypes.Sensor.JointStateMsg;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class PoseSubscriber : MonoBehaviour
{
    const int k_NumRobotJoints = 6;
    [SerializeField]
    public string topicName = "/joint_states";

    [SerializeField]
    public GameObject ur5e;  // The game object
    [SerializeField]
    GameObject publisher; //The publisher/service
    [SerializeField]
    public bool resetPose = false;
    [SerializeField]
    bool showDebug = false;
    [SerializeField]
    int resetMaxItterations = 50;
    int resetCounter = 0;

    ArticulationBody[] m_JointArticulationBodies;

    //const int numberOfJoints = 6;
    //public static readonly string[] LinkNames = { "world/base_link/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link" };

    //UrdfJointRevolute[] jointArticulationBodies;// Robot Joints

    ROSConnection m_ros;//ROS connector

    // Start is called before the first frame update
    void Start()
    {
        m_ros = ROSConnection.GetOrCreateInstance();
        m_ros.Subscribe<Sensor>(topicName, UpdateJoint);

        m_JointArticulationBodies = new ArticulationBody[k_NumRobotJoints];
        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += MyPublisher.LinkNames[i];
            m_JointArticulationBodies[i] = ur5e.transform.Find(linkName).GetComponent<ArticulationBody>();
        }
    }

    void UpdateJoint(Sensor sensorMsg)
    {
        if (showDebug)
        {
            Debug.Log(sensorMsg);
        }

        if (resetPose)
        {
            //StartCoroutine(ProcessMove(sensorMsg));
            var pose = new float[9];
            for (int i = 0; i < pose.Length; i++)
            {
                pose[i] = 0;
            }
            for (int i = 0; i < k_NumRobotJoints; i++)
            {
                pose[i] = Mathf.Rad2Deg * (float)sensorMsg.position[i];
            }
            publisher.GetComponent<TrajectoryPlanner>().SetPose(pose);
        }
    }

    IEnumerator ProcessMove(Sensor sensorMsg)
    {
        var pose = new float[9];
        for (int i = 0; i < pose.Length; i++)
        {
            pose[i] = 0;
        }
        // Set joint values
        for (int i = 0; i < sensorMsg.position.Length; i++)
        {
            var joint = (float)sensorMsg.position[i];

            //System.Collections.Generic.List<float> jointQw = new System.Collections.Generic.List<float>();
            //jointQw.Add(joint);
            //m_JointArticulationBodies[i].SetJointPositions(jointQw);
            //m_JointArticulationBodies[i].jointPosition = sensorMsg.position[i];

            var jointXDrive = m_JointArticulationBodies[i].xDrive;
            jointXDrive.target = joint;
            m_JointArticulationBodies[i].xDrive = jointXDrive;

            pose[i] = joint;
        }



        //publisher.GetComponent<TrajectoryPlanner>().SetPose(pose);

        // Stop updating pose
        resetCounter++;
        if (resetCounter >= resetMaxItterations)
        {
            resetCounter = 0;
            resetPose = false;
        }
        yield return new WaitForSeconds(5.5f);
    }

    public void OnButtonPress()
    {
        resetPose = true;
    }

    public void OnDebugButtonPress()
    {
        showDebug = true;
    }

    // Update is called once per frame
    void Update()
    {

        //if (resetPose)
        //{
        //    // var robotPose = ;
        //    ur5e.GetComponent<Controller>();


        //    resetPose = false;
        //}
    }

    //// FixedUpdate is called once per frame at a constant rate
    //void FixedUpdate()
    //{

    //}
}