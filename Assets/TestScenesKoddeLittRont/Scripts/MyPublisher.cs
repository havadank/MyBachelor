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
    [SerializeField]
    public GameObject target;
    [SerializeField]
    public GameObject targetPlacement; //  waiting with pick and place till we have control of the arm established.

    readonly Quaternion pickOrientation = Quaternion.Euler(90, 90, 0);

    UrdfJointRevolute[] jointArticulationBodies;// Robot Joints

    ROSConnection ros;//ROS connector
    void Start()
    {
        // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<MyMsgMsg>(topicName);

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
        var ur5eJointMessage = new MyMsgMsg();
        for (var i = 0; i < numberOfJoints; i++)
        {
            ur5eJointMessage.joints[i] = Convert.ToDouble(jointArticulationBodies[i].GetPosition());
        }

        
        // Pick Pose                                                                        //selvkommentar: Her legger vi inn posisjon og orienteringen av objektet som skal hentes
        ur5eJointMessage.pick_pose = new PoseMsg
        {
            position = target.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(90, target.transform.eulerAngles.y, 0).To<FLU>()
        };

        // Place Pose
        ur5eJointMessage.place_pose = new PoseMsg
        {
            position = targetPlacement.transform.position.To<FLU>(),
            orientation = pickOrientation.To<FLU>()
        };
        ros.Publish(topicName, ur5eJointMessage);
        // Finally send the message to server_endpoint.py running in ROS

    }
}
