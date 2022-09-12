//using System;
//using Unity.Robotics;
//using UnityEngine;
//using System.Collections.Generic;
//using Unity.Robotics.UrdfImporter;
//using System.Linq;

//namespace Unity.Robotics.UrdfImporter.Control
//{
//    public enum RotationDirection { None = 0, Positive = 1, Negative = -1 };
//    public enum ControlType { PositionControl };

//    public class Controller : MonoBehaviour
//    {
//        public ArticulationBody[] articulationChain;
//        // Stores original colors of the part being highlighted
//        private Color[] prevColor;
//        private int previousIndex;

//        [InspectorReadOnly(hideInEditMode: true)]
//        public string selectedJoint;
//        [HideInInspector]
//        public int selectedIndex;

//        public ControlType control = ControlType.PositionControl;
//        public float stiffness;
//        public float damping;
//        public float forceLimit;
//        public float speed = 5f; // Units: degree/s
//        public float torque = 100f; // Units: Nm or N
//        public float acceleration = 5f;// Units: m/s^2 / degree/s^2

//        public bool activateEEIK = false;          // do i want the arm to follow the EE mark?
//        public GameObject Target;           //from MS_MR_demo1
//        public double threshold = 0.1;
//        public float gain = 20f;
//        //Vector3 lastPos;
//        //Quaternion lastRot;
//        [Range(0, 1)]
//        public float Lambda = 0.001f;
//        [Range(0, 1)]
//        public float Alpha = 0.05f;
//        public int MaxIter = 100;

//        [Tooltip("Color to highlight the currently selected join")]
//        public Color highLightColor = new Color(1.0f, 0, 0, 1.0f);

//        void Start()
//        {

//            previousIndex = selectedIndex = 1;
//            this.gameObject.AddComponent<FKRobot>();
//            articulationChain = this.GetComponentsInChildren<ArticulationBody>();
//            int defDyanmicVal = 10;
//            foreach (ArticulationBody joint in articulationChain)
//            {
//                joint.gameObject.AddComponent<JointControl>();
//                joint.jointFriction = defDyanmicVal;
//                joint.angularDamping = defDyanmicVal;
//                ArticulationDrive currentDrive = joint.xDrive;
//                currentDrive.forceLimit = forceLimit;
//                joint.xDrive = currentDrive;
//            }
//            DisplaySelectedJoint(selectedIndex);
//            StoreJointColors(selectedIndex);
//        }
//        void SetSelectedJointIndex(int index)
//        {
//            if (articulationChain.Length > 0)
//            {
//                selectedIndex = (index + articulationChain.Length) % articulationChain.Length;
//            }
//        }

//        void Update()
//        {
//            bool SelectionInput1 = Input.GetKeyDown("right"); //
//            bool SelectionInput2 = Input.GetKeyDown("left"); // 

//            SetSelectedJointIndex(selectedIndex); // to make sure it is in the valid range
//            UpdateDirection(selectedIndex);
//            if (activateEEIK) InverseKinematics();
//            else
//            {
//                if (SelectionInput2)
//                {
//                    SetSelectedJointIndex(selectedIndex - 1);
//                    Highlight(selectedIndex);
//                }
//                else if (SelectionInput1)
//                {
//                    SetSelectedJointIndex(selectedIndex + 1);
//                    Highlight(selectedIndex);
//                }
//                UpdateDirection(selectedIndex);
//            }
//        }

//        /// <summary>
//        /// Highlights the color of the robot by changing the color of the part to a color set by the user in the inspector window
//        /// </summary>
//        /// <param name="selectedIndex">Index of the link selected in the Articulation Chain</param>
//        private void Highlight(int selectedIndex)
//        {
//            if (selectedIndex == previousIndex || selectedIndex < 0 || selectedIndex >= articulationChain.Length)
//            {
//                return;
//            }

//            // reset colors for the previously selected joint
//            ResetJointColors(previousIndex);

//            // store colors for the current selected joint
//            StoreJointColors(selectedIndex);

//            DisplaySelectedJoint(selectedIndex);
//            Renderer[] rendererList = articulationChain[selectedIndex].transform.GetChild(0).GetComponentsInChildren<Renderer>();

//            // set the color of the selected join meshes to the highlight color
//            foreach (var mesh in rendererList)
//            {
//                MaterialExtensions.SetMaterialColor(mesh.material, highLightColor);
//            }
//        }

//        void DisplaySelectedJoint(int selectedIndex)
//        {
//            if (selectedIndex < 0 || selectedIndex >= articulationChain.Length)
//            {
//                return;
//            }
//            selectedJoint = articulationChain[selectedIndex].name + " (" + selectedIndex + ")";
//        }

//        /// <summary>
//        /// Sets the direction of movement of the joint on every update
//        /// </summary>
//        /// <param name="jointIndex">Index of the link selected in the Articulation Chain</param>
//        private void UpdateDirection(int jointIndex)
//        {
//            if (jointIndex < 0 || jointIndex >= articulationChain.Length)
//            {
//                return;
//            }

//            float moveDirection = Input.GetAxis("Vertical");
//            JointControl current = articulationChain[jointIndex].GetComponent<JointControl>();
//            if (previousIndex != jointIndex)
//            {
//                JointControl previous = articulationChain[previousIndex].GetComponent<JointControl>();
//                previous.direction = RotationDirection.None;
//                previousIndex = jointIndex;
//            }

//            if (current.controltype != control)
//            {
//                UpdateControlType(current);
//            }

//            if (moveDirection > 0)
//            {
//                current.direction = RotationDirection.Positive;
//            }
//            else if (moveDirection < 0)
//            {
//                current.direction = RotationDirection.Negative;
//            }
//            else
//            {
//                current.direction = RotationDirection.None;
//            }
//        }

//        /// <summary>
//        /// Stores original color of the part being highlighted
//        /// </summary>
//        /// <param name="index">Index of the part in the Articulation chain</param>
//        private void StoreJointColors(int index)
//        {
//            Renderer[] materialLists = articulationChain[index].transform.GetChild(0).GetComponentsInChildren<Renderer>();
//            prevColor = new Color[materialLists.Length];
//            for (int counter = 0; counter < materialLists.Length; counter++)
//            {
//                prevColor[counter] = MaterialExtensions.GetMaterialColor(materialLists[counter]);
//            }
//        }

//        /// <summary>
//        /// Resets original color of the part being highlighted
//        /// </summary>
//        /// <param name="index">Index of the part in the Articulation chain</param>
//        private void ResetJointColors(int index)
//        {
//            Renderer[] previousRendererList = articulationChain[index].transform.GetChild(0).GetComponentsInChildren<Renderer>();
//            for (int counter = 0; counter < previousRendererList.Length; counter++)
//            {
//                MaterialExtensions.SetMaterialColor(previousRendererList[counter].material, prevColor[counter]);
//            }
//        }

//        public void UpdateControlType(JointControl joint)
//        {
//            joint.controltype = control;
//            if (control == ControlType.PositionControl)
//            {
//                ArticulationDrive drive = joint.joint.xDrive;
//                drive.stiffness = stiffness;
//                drive.damping = damping;
//                joint.joint.xDrive = drive;
//            }
//        }

//        public void OnGUI()
//        {
//            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
//            centeredStyle.alignment = TextAnchor.UpperCenter;
//            GUI.Label(new Rect(Screen.width / 2 - 200, 10, 400, 20), "Press left/right arrow keys to select a robot joint.", centeredStyle);
//            GUI.Label(new Rect(Screen.width / 2 - 200, 30, 400, 20), "Press up/down arrow keys to move " + selectedJoint + ".", centeredStyle);
//        }
//        public void ButtonLeftUpdate() // av Håvar
//        {

//            bool SelectionInput2 = true;

//            SetSelectedJointIndex(selectedIndex); // to make sure it is in the valid range
//            UpdateDirection(selectedIndex);

//            if (SelectionInput2)
//            {
//                SetSelectedJointIndex(selectedIndex - 1);
//                Highlight(selectedIndex);
//            }

//            UpdateDirection(selectedIndex);
//        }
//        public void ButtonRightUpdate() // av Håvar
//        {

//            bool SelectionInput2 = true;

//            SetSelectedJointIndex(selectedIndex); // to make sure it is in the valid range
//            UpdateDirection(selectedIndex);

//            if (SelectionInput2)
//            {
//                SetSelectedJointIndex(selectedIndex + 1);
//                Highlight(selectedIndex);
//            }

//            UpdateDirection(selectedIndex);
//        }
//        public void ButtonJointAdditionUpdate()
//        {
//            ArticulationBody current = articulationChain[selectedIndex].GetComponent<ArticulationBody>();
//            ArticulationDrive xtarget = current.xDrive;
//            xtarget.target += 10f;
//            current.xDrive = xtarget;
//            //JointControl current = articulationChain[selectedIndex].GetComponent<JointControl>();
//            //for (int i = 0; i < 20; i++)
//            //{
//            //    current.direction = RotationDirection.Positive;
//            //}
//        }
//        public void ButtonJointSubtractionUpdate()
//        {
//            ArticulationBody current = articulationChain[selectedIndex].GetComponent<ArticulationBody>();
//            ArticulationDrive xtarget = current.xDrive;
//            xtarget.target -= 10f;
//            current.xDrive = xtarget;
//            //JointControl current = articulationChain[selectedIndex].GetComponent<JointControl>();
//            //for (int i = 0; i < 20; i++)
//            //{
//            //    current.direction = RotationDirection.Negative;
//            //}
//        }
//        private void InverseKinematics()
//        {
//            JointControl current;
//            float slope;
//            ArticulationBody snakeHead = articulationChain[articulationChain.Length - 1];
//            if (GetDistance(snakeHead.transform.position, Target.transform.position) > threshold)
//            {
//                for (int i = 0; i < 7; i++)
//                {
//                    foreach (ArticulationBody joint in articulationChain)
//                    {
//                        current = joint.GetComponent<JointControl>();
//                        float d1 = GetDistance(joint.transform.position, Target.transform.position);
//                        current.direction = RotationDirection.Negative;
//                        //var jxDrive = joint.xDrive;
//                        //jxDrive.target += Lambda;
//                        float d2 = GetDistance(joint.transform.position, Target.transform.position);
//                        //jxDrive.target -= Lambda;
//                        current.direction = RotationDirection.Positive;
//                        slope = (d1 - d2) / Lambda;
//                        if (slope > ((float)0 + (float)threshold)) current.direction = RotationDirection.Positive;
//                        else if (slope < ((float)0 - (float)threshold)) current.direction = RotationDirection.Negative;
//                        else current.direction = RotationDirection.None;
//                        //current.direction = RotationDirection.None;
//                        //current.direction = (double)(-slope * gain);
//                    }
//                }
//            }
//            //try
//            //{
//            //    if (Target.transform.localPosition != lastPos || Target.transform.localRotation != lastRot)
//            //    {
//            //        //skipcounter?
//            //        Vector3 r_des = Target.transform.localPosition;
//            //        Vector3 r = Target.transform.localEulerAngles;
//            //        Matrix3x3 rotationMatrix = EulerToRotation(r.x, r.y, r.z);
//            //        lastPos = Target.transform.localPosition;
//            //        lastRot = Target.transform.localRotation;
//            //        //ik

//            //    }
//            //}
//            //catch
//            //{

//            //}
//        }
//        float GetDistance(Vector3 a, Vector3 b)
//        {
//            return Vector3.Distance(a, b);
//        }
//        //private Matrix3x3 EulerToRotation(float x, float y, float z)
//        //{
//        //    Matrix3x3 svar = null;
//        //    float xr = x * ((float)Math.PI / (float)180);
//        //    float yr = y * ((float)Math.PI / (float)180);
//        //    float zr = z * ((float)Math.PI / (float)180);

//        //    Vector3[] placeHolder = new Vector3[2];
//        //    //Rz
//        //    placeHolder[0] = new Vector3((float)Math.Cos(zr), (float)-Math.Sin(zr), (float)0);
//        //    placeHolder[1] = new Vector3((float)Math.Sin(zr), (float)Math.Cos(zr), (float)0);
//        //    placeHolder[2] = new Vector3((float)0, (float)0, (float)1);
//        //    Matrix3x3 Rz = new Matrix3x3(placeHolder);
//        //    //Rx
//        //    placeHolder[0] = new Vector3((float)1, (float)0, (float)0);
//        //    placeHolder[1] = new Vector3((float)0, (float)Math.Cos(xr), (float)-Math.Sin(xr));
//        //    placeHolder[2] = new Vector3((float)0, (float)Math.Sin(xr), (float)Math.Cos(xr));
//        //    Matrix3x3 Rx = new Matrix3x3(placeHolder);
//        //    //Ry
//        //    placeHolder[0] = new Vector3((float)Math.Cos(yr), (float)0, (float)Math.Sin(yr));
//        //    placeHolder[1] = new Vector3((float)0, (float)1, (float)0);
//        //    placeHolder[2] = new Vector3((float)-Math.Sin(yr), (float)0, (float)Math.Cos(yr));
//        //    Matrix3x3 Ry = new Matrix3x3(placeHolder);

//        //    svar = Rx * Ry * Rz;

//        //    return svar;
//        //}
//    }
//}

