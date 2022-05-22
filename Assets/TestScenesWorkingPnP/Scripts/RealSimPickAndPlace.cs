using System;
using System.Collections;
using System.Linq;
using RosMessageTypes.Geometry;
using RosMessageTypes.Moveit;
using RosMessageTypes.Ur5EMoveitConfig;
using RosMessageTypes.Ur;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

public class RealSimPickAndPlace : MonoBehaviour
{
    const int k_TrajectoryCommandExecution = 7;
    const int k_NumRobotJoints = 6;

    // Hardcoded variables
    const float k_JointAssignmentWait = 0.038f;

    // Variables required for ROS communication
    public string rosJointPublishTopicName = "/ur5e_joints"; // hva skal denne hete?
    public string rosRobotCommandsTopicName = "niryo_one/commander/robot_action/goal";
    //                                          hva skal den oppforbi hete
    [SerializeField]
    GameObject ur5e;
    [SerializeField]
    GameObject m_Target;
    [SerializeField]
    GameObject m_TargetPlacement;

    // Assures that the gripper is always positioned above the m_Target cube before grasping.
    readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);
    readonly Vector3 m_PickPoseOffset = Vector3.up * 0.15f;

    // Articulation Bodies
    ArticulationBody[] jointArticulationBodies;

    ROSConnection m_Ros;

    /// <summary>
    ///     Find all robot joints in Awake() and add them to the jointArticulationBodies array.
    ///     Find left and right finger joints and assign them to their respective articulation body objects.
    /// </summary>
    void Awake()
    {
        jointArticulationBodies = new ArticulationBody[k_NumRobotJoints];

        var linkName = string.Empty;
        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            linkName += MyPublisher.LinkNames[i];
            jointArticulationBodies[i] = ur5e.transform.Find(linkName).GetComponent<ArticulationBody>();
        }

        // Find left and right fingers // dont think i need these
        //var rightGripper = linkName + "/tool_link/gripper_base/servo_head/control_rod_right/right_gripper";
        //var leftGripper = linkName + "/tool_link/gripper_base/servo_head/control_rod_left/left_gripper";

        //m_RightGripper = m_NiryoOne.transform.Find(rightGripper).GetComponent<ArticulationBody>();
        //m_LeftGripper = m_NiryoOne.transform.Find(leftGripper).GetComponent<ArticulationBody>();
    }

    void Start()
    {
        // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.Subscribe<RobotMoveActionGoal>(rosRobotCommandsTopicName, ExecuteRobotCommands);
    }

    /// <summary>
    ///     Publish the robot's current joint configuration, the m_Target object's
    ///     position and rotation, and the m_Target placement for the m_Target object's
    ///     position and rotation.
    ///     Includes conversion from Unity coordinates to ROS coordinates, Forward Left Up
    /// </summary>
    public void PublishJoints() // buttenclick aka toggle switch
    {
        var request = new MoverServiceUr5eRequest
        {
            joints_input = new MyMsgMsg(),
            pick_pose = new PoseMsg
            {
                position = (m_Target.transform.position + m_PickPoseOffset).To<FLU>(),

                // The hardcoded x/z angles assure that the gripper is always positioned above the m_Target cube before grasping.
                orientation = Quaternion.Euler(90, m_Target.transform.eulerAngles.y, 0).To<FLU>()
            },
            place_pose = new PoseMsg
            {
                position = (m_TargetPlacement.transform.position + m_PickPoseOffset).To<FLU>(),
                orientation = m_PickOrientation.To<FLU>()
            }
        };

        for (var i = 0; i < k_NumRobotJoints; i++)
        {
            request.joints_input.joints[i] = jointArticulationBodies[i].jointPosition[0];
        }

        m_Ros.Publish(rosJointPublishTopicName, request);
    }

    /// <summary>
    ///     Execute robot commands receved from ROS Subscriber.
    ///     Gripper commands will be executed immeditately wihle trajectories will be
    ///     executed in a coroutine.
    /// </summary>
    /// <param name="robotAction"> RobotMoveActionGoal of trajectory or gripper commands</param>
    void ExecuteRobotCommands(RobotMoveActionGoal robotAction)
    {
        switch (robotAction.goal.cmd.cmd_type)
        {
            case k_TrajectoryCommandExecution:
                StartCoroutine(ExecuteTrajectories(robotAction.goal.cmd.Trajectory.trajectory));
                break;
        }
    }
    /// <summary>
    ///     Execute trajectories from RobotMoveActionGoal topic.
    ///     Execution will iterate through every robot pose in the trajectory pose
    ///     array while updating the joint values on the simulated robot.
    /// </summary>
    /// <param name="trajectories"> The array of poses for the robot to execute</param>
    IEnumerator ExecuteTrajectories(RobotTrajectoryMsg trajectories) // in unity
    {
        // For every robot pose in trajectory plan
        foreach (var point in trajectories.joint_trajectory.points)
        {
            var jointPositions = point.positions;
            var result = jointPositions.Select(r => (float)r * Mathf.Rad2Deg).ToArray();

            // Set the joint values for every joint
            for (var joint = 0; joint < jointArticulationBodies.Length; joint++)
            {
                var joint1XDrive = jointArticulationBodies[joint].xDrive;
                joint1XDrive.target = result[joint];
                jointArticulationBodies[joint].xDrive = joint1XDrive;
            }

            // Wait for robot to achieve pose for all joint assignments
            yield return new WaitForSeconds(k_JointAssignmentWait);
        }
    }
}
