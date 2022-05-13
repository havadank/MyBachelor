//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ur
{
    [Serializable]
    public class JoystickJointsResult : Message
    {
        public const string k_RosMessageName = "ur_msgs/JoystickJoints";
        public override string RosMessageName => k_RosMessageName;

        //  result
        public int has_succeeded;

        public JoystickJointsResult()
        {
            this.has_succeeded = 0;
        }

        public JoystickJointsResult(int has_succeeded)
        {
            this.has_succeeded = has_succeeded;
        }

        public static JoystickJointsResult Deserialize(MessageDeserializer deserializer) => new JoystickJointsResult(deserializer);

        private JoystickJointsResult(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.has_succeeded);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.has_succeeded);
        }

        public override string ToString()
        {
            return "JoystickJointsResult: " +
            "\nhas_succeeded: " + has_succeeded.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Result);
        }
    }
}
