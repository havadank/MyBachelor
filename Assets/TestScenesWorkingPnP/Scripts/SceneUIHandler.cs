using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneUIHandler : MonoBehaviour
{
    public void ToggleObject(GameObject gameObjectToToggle)
    {
        gameObjectToToggle.SetActive(!gameObjectToToggle.activeSelf);
    }
    public void JointChooserLeft()
    {
        
    }
}
