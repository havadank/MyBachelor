//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ur5EMoveitConfig
{
    [Serializable]
    public class MoverServiceUr5eRequest : Message
    {
        public const string k_RosMessageName = "ur5_e_moveit_config/MoverServiceUr5e";
        public override string RosMessageName => k_RosMessageName;

        public MyMsgMsg joints_input;
        public Geometry.PoseMsg pick_pose;
        public Geometry.PoseMsg place_pose;

        public MoverServiceUr5eRequest()
        {
            this.joints_input = new MyMsgMsg();
            this.pick_pose = new Geometry.PoseMsg();
            this.place_pose = new Geometry.PoseMsg();
        }

        public MoverServiceUr5eRequest(MyMsgMsg joints_input, Geometry.PoseMsg pick_pose, Geometry.PoseMsg place_pose)
        {
            this.joints_input = joints_input;
            this.pick_pose = pick_pose;
            this.place_pose = place_pose;
        }

        public static MoverServiceUr5eRequest Deserialize(MessageDeserializer deserializer) => new MoverServiceUr5eRequest(deserializer);

        private MoverServiceUr5eRequest(MessageDeserializer deserializer)
        {
            this.joints_input = MyMsgMsg.Deserialize(deserializer);
            this.pick_pose = Geometry.PoseMsg.Deserialize(deserializer);
            this.place_pose = Geometry.PoseMsg.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.joints_input);
            serializer.Write(this.pick_pose);
            serializer.Write(this.place_pose);
        }

        public override string ToString()
        {
            return "MoverServiceUr5eRequest: " +
            "\njoints_input: " + joints_input.ToString() +
            "\npick_pose: " + pick_pose.ToString() +
            "\nplace_pose: " + place_pose.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
