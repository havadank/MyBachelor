//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ur
{
    [Serializable]
    public class ObjectPoseMsg : Message
    {
        public const string k_RosMessageName = "ur_msgs/ObjectPose";
        public override string RosMessageName => k_RosMessageName;

        public double x;
        public double y;
        public double z;
        public double roll;
        public double pitch;
        public double yaw;

        public ObjectPoseMsg()
        {
            this.x = 0.0;
            this.y = 0.0;
            this.z = 0.0;
            this.roll = 0.0;
            this.pitch = 0.0;
            this.yaw = 0.0;
        }

        public ObjectPoseMsg(double x, double y, double z, double roll, double pitch, double yaw)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.roll = roll;
            this.pitch = pitch;
            this.yaw = yaw;
        }

        public static ObjectPoseMsg Deserialize(MessageDeserializer deserializer) => new ObjectPoseMsg(deserializer);

        private ObjectPoseMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.x);
            deserializer.Read(out this.y);
            deserializer.Read(out this.z);
            deserializer.Read(out this.roll);
            deserializer.Read(out this.pitch);
            deserializer.Read(out this.yaw);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.x);
            serializer.Write(this.y);
            serializer.Write(this.z);
            serializer.Write(this.roll);
            serializer.Write(this.pitch);
            serializer.Write(this.yaw);
        }

        public override string ToString()
        {
            return "ObjectPoseMsg: " +
            "\nx: " + x.ToString() +
            "\ny: " + y.ToString() +
            "\nz: " + z.ToString() +
            "\nroll: " + roll.ToString() +
            "\npitch: " + pitch.ToString() +
            "\nyaw: " + yaw.ToString();
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
