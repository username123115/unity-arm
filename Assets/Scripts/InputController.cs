using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject robot;
    public ArduinoManager arduinoManager;
    public bool debugPrint = false;
    private ArmController myArm;
    void Start()
    {
        myArm = robot.GetComponent<ArmController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        myArm.StopJoints();
        for (int i = 0; i < myArm.Joints.Length; i++) {
            Joint j = myArm.Joints[i];
            float v;
            if (j.arduinoOverride == 0) {
                InputAction action = j.action;
                v = action.ReadValue<float>();
            } else {
                v = arduinoManager.entries[j.arduinoOverride - 1];
            }
            //Input registered?
            if (Math.Abs(v) >= 0.0001) {
                if (debugPrint) {
                    Debug.Log(v);
                }
                myArm.StopJoints();
                RotationDirection dir;
                if (v < 0) {
                    dir = RotationDirection.Negative;
                } else {
                    dir = RotationDirection.Positive;
                }
                myArm.SetJointDirection(dir, i);
            } else {
                //myArm.StopJoint(i);
            }
        }
        
    }
    void OnEnable() {
        myArm = robot.GetComponent<ArmController>();
        for (int i = 0; i < myArm.Joints.Length; i++) {
            Joint j = myArm.Joints[i];
            InputAction action = j.action;
            action.Enable();

        }
    }
    void OnDisable() {
        for (int i = 0; i < myArm.Joints.Length; i++) {
            Joint j = myArm.Joints[i];
            InputAction action = j.action;
            action.Disable();

        }
    }
}
