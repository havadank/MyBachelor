//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ur
{
    [Serializable]
    public class JoystickJointsGoal : Message
    {
        public const string k_RosMessageName = "ur_msgs/JoystickJoints";
        public override string RosMessageName => k_RosMessageName;

        //  goal
        public double[] joy_values;
        public double duration;

        public JoystickJointsGoal()
        {
            this.joy_values = new double[0];
            this.duration = 0.0;
        }

        public JoystickJointsGoal(double[] joy_values, double duration)
        {
            this.joy_values = joy_values;
            this.duration = duration;
        }

        public static JoystickJointsGoal Deserialize(MessageDeserializer deserializer) => new JoystickJointsGoal(deserializer);

        private JoystickJointsGoal(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.joy_values, sizeof(double), deserializer.ReadLength());
            deserializer.Read(out this.duration);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.WriteLength(this.joy_values);
            serializer.Write(this.joy_values);
            serializer.Write(this.duration);
        }

        public override string ToString()
        {
            return "JoystickJointsGoal: " +
            "\njoy_values: " + System.String.Join(", ", joy_values.ToList()) +
            "\nduration: " + duration.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize, MessageSubtopic.Goal);
        }
    }
}
