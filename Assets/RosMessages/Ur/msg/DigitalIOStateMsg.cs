//Do not edit! This file was generated by Unity-ROS MessageGeneration.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Ur
{
    [Serializable]
    public class DigitalIOStateMsg : Message
    {
        public const string k_RosMessageName = "ur_msgs/DigitalIOState";
        public override string RosMessageName => k_RosMessageName;

        //  GPIO pin
        public int[] pins;
        //  PIN names seen by user to make it simpler
        public string[] names;
        //  IN/OUT
        public int[] modes;
        //  HIGH/LOW
        public int[] states;

        public DigitalIOStateMsg()
        {
            this.pins = new int[0];
            this.names = new string[0];
            this.modes = new int[0];
            this.states = new int[0];
        }

        public DigitalIOStateMsg(int[] pins, string[] names, int[] modes, int[] states)
        {
            this.pins = pins;
            this.names = names;
            this.modes = modes;
            this.states = states;
        }

        public static DigitalIOStateMsg Deserialize(MessageDeserializer deserializer) => new DigitalIOStateMsg(deserializer);

        private DigitalIOStateMsg(MessageDeserializer deserializer)
        {
            deserializer.Read(out this.pins, sizeof(int), deserializer.ReadLength());
            deserializer.Read(out this.names, deserializer.ReadLength());
            deserializer.Read(out this.modes, sizeof(int), deserializer.ReadLength());
            deserializer.Read(out this.states, sizeof(int), deserializer.ReadLength());
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.WriteLength(this.pins);
            serializer.Write(this.pins);
            serializer.WriteLength(this.names);
            serializer.Write(this.names);
            serializer.WriteLength(this.modes);
            serializer.Write(this.modes);
            serializer.WriteLength(this.states);
            serializer.Write(this.states);
        }

        public override string ToString()
        {
            return "DigitalIOStateMsg: " +
            "\npins: " + System.String.Join(", ", pins.ToList()) +
            "\nnames: " + System.String.Join(", ", names.ToList()) +
            "\nmodes: " + System.String.Join(", ", modes.ToList()) +
            "\nstates: " + System.String.Join(", ", states.ToList());
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
