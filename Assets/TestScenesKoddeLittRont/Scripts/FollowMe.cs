using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.UrdfImporter.Control;
using Unity.Robotics;
using System.Linq;
using System;

public class FollowMe : MonoBehaviour
{
    public GameObject robot;
    public GameObject Target;
    Controller kontroller;

    Vector3 prePosisjon;
    Quaternion preRotasjon;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        try
        {

            if (Target.transform.localPosition != prePosisjon || Target.transform.localRotation != preRotasjon)
            {
                
            }
        }
        catch
        {

        }

        //Update robot with last target angles
    }
}
