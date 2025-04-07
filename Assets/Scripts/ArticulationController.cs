using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//steal from reference implementation
public enum RotationDirection { None = 0, Positive = 1, Negative = -1, Controlled = 2};

public abstract class PartController : MonoBehaviour
{
    public float speed = 50.0f;
    public RotationDirection direction = RotationDirection.None;
    public abstract void SetDirection(RotationDirection dir);
    public abstract RotationDirection GetDirection();
    public abstract float GetPositionR();
    public abstract float GetPositionD();
    public abstract void SetPositionD(float pos);
    public abstract void AddPositionD(float add);
}

public class ArticulationController : PartController
{
    // Start is called before the first frame update

    
    public bool debugPrint = false;
    private ArticulationBody myJoint;
    
    void Start()
    {
        GetJoints();
    }
    
    void GetJoints()
    {
        myJoint = GetComponent<ArticulationBody>();
    }
    

    // Update is called once per frame
    // Do nothing
    void Update()
    {
        
    }
    public override void SetDirection(RotationDirection dir) {
        direction = dir;
    }
    public override RotationDirection GetDirection() {
        return direction;
    }
    
    public override float GetPositionR() {
        float radians = myJoint.jointPosition[0];
        return radians;
        
    }
    public override float GetPositionD() {
        return Mathf.Rad2Deg * GetPositionR();
    }
    
    public override void SetPositionD(float pos) {
        if (debugPrint) {
            Debug.Log("Setting Position to " + pos);
        }
        ArticulationDrive drive = myJoint.xDrive;
        drive.target = pos;
        myJoint.xDrive = drive;
    }
    
    public override void AddPositionD(float add) {
        float cur = GetPositionD();
        SetPositionD(cur + add);
    }
    
    
    void FixedUpdate()
    {
        if (direction != RotationDirection.Controlled) {
            float change = Time.deltaTime * speed * (float) direction;
            float current = GetPositionD();
            if (debugPrint) {
                Debug.Log("My position is: " + current + ". Change is " + change);
            }
            AddPositionD(change);
        }
        
    }
}


//TODO: direction instead of 
