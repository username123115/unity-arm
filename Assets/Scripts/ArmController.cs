using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct Joint {
    public GameObject obj;
    // Armcontroller will hold the actions each arm part is assigned but the Input Controller is responsible for activating and deactivating them
    public InputAction action;
    public int arduinoOverride;
}
public class ArmController : MonoBehaviour
{
    public Joint[] Joints;

    // Enable if you want to have the controller toggle the direction of selected joints between negative and positive 
    // (May interfere with user input)
    public bool debugTestJoints;
    // List of joints to test if testJoints is enabled, given as an array of indices into the Joints array
    public int[] debugJointsToTest;
    // How many seconds to go before changing the direction when testing joints
    public float debugChange = 1.0f;

    
    float elapsed = 0.0f;
    RotationDirection next = RotationDirection.Positive;
    
    
    public void SetJointDirection(RotationDirection dir, int index) {
        // Does this joint exist?
        if ((0 <= index) && (index < Joints.Length)) {
            GameObject part = Joints[index].obj;
            PartController myController = part.GetComponent<PartController>();
            myController.SetDirection(dir);
        }
    }
    
    public void StopJoint(int index) {
        SetJointDirection(RotationDirection.None, index);
    }
    
    public void StopJoints() {
        for (int i = 0; i < Joints.Length; i++) {
            //If we aren't debugging or a specific joint isn't being debugged, then stop that joint
            if ((!debugTestJoints) || (!debugJointsToTest.Contains(i))) {
                SetJointDirection(RotationDirection.None, i);
            }
        }

    }
    
    void DebugToggleJoints(float dTime) {
        elapsed += Time.deltaTime;
        if (elapsed >= debugChange) {
            // Iterate through all joints to test and toggle them
            for (int i = 0; i < debugJointsToTest.Length; i++) {
                int ind = debugJointsToTest[i];
                SetJointDirection(next, 0);
                
            }
            if (next == RotationDirection.Positive) {
                Debug.Log("Tick");
                next = RotationDirection.Negative;
            } else {
                Debug.Log("Tock");
                next = RotationDirection.Positive;
            }
            elapsed = 0.0f;
        }
    }
    
    void FixedUpdate() {
        if (debugTestJoints) {
            DebugToggleJoints(Time.deltaTime);
        }
    }
    
}
