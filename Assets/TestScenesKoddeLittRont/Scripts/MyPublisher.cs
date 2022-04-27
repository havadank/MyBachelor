using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using RosMessageTypes.Geometry;
using RosMessageTypes.Ur5EMoveitConfig;

// Source: https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/pick_and_place/Scripts/SourceDestinationPublisher.cs
// modified by Håvar Dankel

public class MyPublisher : MonoBehaviour
{
    const int numberOfJoints = 6;
    public static readonly string[] LinkNames = { "world/base_link/shoulder_link", "/upper_arm_link", "/forearm_link", "/wrist_1_link", "/wrist_2_link", "/wrist_3_link" };


    [SerializeField]
    public string topicName = "/ur5e_joints";
    
    [SerializeField]
    public GameObject ur5e; // The game object
    //[SerializeField]
    //public GameObject target;
    //[SerializeField]
    //public GameObject targetPlacement; //  waiting with pick and place till we have control of the arm established.

    //readonly Quaternion pickOrientation = Quaternion.Euler(90, 90, 0);

    UrdfJointRevolute[] jointArticulationBodies;// Robot Joints

    public float publishMessageFrequency = 0.5f;// Publish the cube's position and rotation every N seconds
    private double timeElapsed; // Used to determine how much time has elapsed since the last message was published

    // i do not need most of these. Most interested in q_actual, and mabye q_target and q_placement later
    // but i need to define them for the RobotStateRTMsgMsg object that i am generating for the messageing to ros

    public double[] q_actual = new double[6]; // i use this one
    public double[] q_target = null; 
    public double[] qd_target = null;
    public double[] qdd_target = null;
    public double[] i_target = null;
    public double[] m_target = null;
    public double[] qd_actual = null;
    public double[] i_actual = null;
    public double[] tool_acc_values = null;
    public double[] tcp_force = null;
    public double[] tool_vector = null;
    public double[] tcp_speed = null;
    public double digital_input_bits = 0;
    public double[] motor_temperatures = null;
    public double controller_timer = 0;
    public double test_value = 0;
    public double robot_mode = 0;
    public double[] joint_modes = null;

    ROSConnection ros;//ROS connector
    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<RobotStateRTMsgMsg>(topicName);

        jointArticulationBodies = new UrdfJointRevolute[numberOfJoints];

        var linkName = string.Empty;
        for (var i = 0; i < numberOfJoints; i++)
        {
            linkName += LinkNames[i];
            jointArticulationBodies[i] = ur5e.transform.Find(linkName).GetComponent<UrdfJointRevolute>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed = Time.realtimeSinceStartupAsDouble;
        var ur5eJointMessage = new RobotStateRTMsgMsg(timeElapsed, q_target, qd_target, qdd_target, i_target, m_target, q_actual, qd_actual, i_actual, tool_acc_values, tcp_force, tool_vector, tcp_speed, digital_input_bits, motor_temperatures, controller_timer, test_value, robot_mode, joint_modes);

        for (var i = 0; i < numberOfJoints; i++)
        {
            ur5eJointMessage.q_actual[i] = Convert.ToDouble(jointArticulationBodies[i].GetPosition());
        }

        ros.Publish(topicName, ur5eJointMessage);
        //// Pick Pose                                                                        //selvkommentar: Her legger vi inn posisjon og orienteringen av objektet som skal hentes
        //ur5eJointMessage.q_target = new PoseMsg
        //{
        //    position = target.transform.position.To<FLU>(),
        //    orientation = Quaternion.Euler(90, ttarget.transform.eulerAngles.y, 0).To<FLU>()
        //};

        //// Place Pose
        //ur5eJointMessage.place_pose = new PoseMsg
        //{
        //    position = m_TargetPlacement.transform.position.To<FLU>(),
        //    orientation = m_PickOrientation.To<FLU>()
        //};

        // Finally send the message to server_endpoint.py running in ROS
        
    }
}
