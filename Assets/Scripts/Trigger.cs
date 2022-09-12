using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField]
    GameObject RobotReach;
    private void OnTriggerEnter(Collider other)
    {
    //    RobotReach.GetComponent<RobotReachVisual>().DrawBoundryInWorld(true);
    }

    private void OnTriggerExit(Collider other)
    {
    //    RobotReach.GetComponent<RobotReachVisual>().DrawBoundryInWorld(false);
    }
}
