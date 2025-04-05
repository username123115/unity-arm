
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FingerController : PartController
{

    public GameObject fingerA;
    public GameObject fingerB;
    public bool debugPrint;
    
    public float correctionDistance = Mathf.Deg2Rad * 5.0f;
    
    private float savedAngle = 0.0f;
    private bool shouldSaveAngle = false;
    private ArticulationBody jointA;
    private ArticulationBody jointB;

    
    void Start()
    {
        jointA = fingerA.GetComponent<ArticulationBody>();
        jointB = fingerB.GetComponent<ArticulationBody>();
    }
    public override void SetDirection(RotationDirection dir) 
    {
        // Only save angle if direction is being changed to None
        if (dir == 0) {
            if (shouldSaveAngle) {
                savedAngle = GetPositionD() + correctionDistance;
                shouldSaveAngle = false;
            }
        } else {
            shouldSaveAngle = true;
        }
        direction = dir;
    }
    public override RotationDirection GetDirection() 
    {
        return direction;
    }
    
    // Just get average rotation of two fingers for now
    public override float GetPositionR() 
    {
        float radians = (jointA.jointPosition[0] + jointB.jointPosition[0]) / 2.0f;
        return radians;
        
    }
    public override float GetPositionD() 
    {
        return Mathf.Rad2Deg * GetPositionR();
    }
    public override void SetPositionD(float pos) {
        if (debugPrint) {
            Debug.Log("Setting Position to " + pos);
        }
        ArticulationDrive driveA = jointA.xDrive;
        driveA.target = pos;
        jointA.xDrive = driveA;

        ArticulationDrive driveB = jointB.xDrive;
        driveB.target = pos;
        jointB.xDrive = driveB;
    }
    
    public override void AddPositionD(float add) {
        float cur = GetPositionD();
        SetPositionD(cur + add);
    }
    
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (direction != 0) {
            float change = Time.deltaTime * speed * (float) direction;
            float current = GetPositionD();
            if (debugPrint) {
                Debug.Log("My position is: " + current + ". Change is " + change);
            }
            AddPositionD(change);
        } else {
            SetPositionD(savedAngle);
        }
        
    }

}
