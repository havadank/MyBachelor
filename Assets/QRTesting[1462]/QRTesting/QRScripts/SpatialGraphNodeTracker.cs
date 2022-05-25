using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;

#if MIXED_REALITY_OPENXR
using Microsoft.MixedReality.OpenXR;
#else
using QRTracking.WindowsMR;
#endif

namespace QRTracking
{
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
        private System.Guid _id;
        private SpatialGraphNode node;
        public bool setRobot;
        [SerializeField]
        public GameObject UR5e;
        

        public System.Guid Id
        {
            get => _id;

            set
            {
                if (_id != value)
                {
                    _id = value;
                    InitializeSpatialGraphNode(force: true);
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            InitializeSpatialGraphNode();
        }

        // Update is called once per frame
        void Update()
        {
            InitializeSpatialGraphNode();

            if (node != null)
            {
                if (node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                    // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
                    // to these objects so apply the inverse
                    if (CameraCache.Main.transform.parent != null)
                    {
                        pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
                    }

                    gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
                    Debug.Log("Id= " + Id + " QRPose = " + pose.position.ToString("F7") + " QRRot = " + pose.rotation.ToString("F7"));
                    if (setRobot)
                    {
                        SetRobot(pose);
                        setRobot = false;
                        
                    }
                }
                else
                {
                    Debug.LogWarning("Cannot locate " + Id);
                }
            }
        }
        private void SetRobot(Pose pose)
        {
            UR5e = GameObject.Find("ur5e");
            UR5e.SetActive(false);
            Quaternion svar1 = UR5e.transform.rotation;
            Quaternion svar = Quaternion.Euler(svar1.eulerAngles.x, pose.rotation.eulerAngles.y, svar1.eulerAngles.z);
            
            //svar.x, pose.rotation.eulerAngles.y, svar.z;
            UR5e.transform.SetPositionAndRotation(pose.position+(new Vector3((float)0.9, 0, 0)), svar);
            UR5e.SetActive(true);
        }
        public void SetRobotOnQr()
        {
            setRobot = true;
        }

        private void InitializeSpatialGraphNode(bool force = false)
        {
            if (node == null || force)
            {
                node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                //Debug.Log("Initialize SpatialGraphNode Id= " + Id);
            }
        }
    }
}