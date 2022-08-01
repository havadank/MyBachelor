using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPositioning : MonoBehaviour
{
    [SerializeField]
    GameObject QrCodeManager; //Unused at this time
    [SerializeField]
    GameObject RobotPositionCordinates;
    [SerializeField]
    GameObject Robot;
    [SerializeField]
    GameObject publisher;

    GameObject qrCode;
    int qrCodeInstance;

    bool RobotMove = false;

    // Start is called before the first frame update
    void Start()
    {
        qrCodeInstance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        qrCode = GameObject.Find("QRCode(Clone)");
        if (qrCode != null && qrCodeInstance == 0)
        {
            qrCodeInstance = qrCode.GetInstanceID();
        }
        if (qrCodeInstance != 0)
        {
            if (RobotMove)
            {
                var pose = new float[9];
                for (int i = 0; i < 9; i++)
                {
                    pose[i] = 0;
                }
                publisher.GetComponent<TrajectoryPlanner>().SetPose(pose);

                Robot.SetActive(false);
                //RobotPositionCordinates.transform.position = qrCode.transform.position;
                Robot.transform.position = qrCode.transform.position;
                qrCode.SetActive(false);
                Robot.SetActive(true);
                Robot = GameObject.Find("ur5e");
                RobotMove = false;
            }
        }
    }

    public void MoveRobot()
    {
        RobotMove = true;
    }
}
