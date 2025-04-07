using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class TestManager : MonoBehaviour
{    // Start is called before the first frame update
    public GameObject markerArm;
    public GameObject markerBlock;
    public ArduinoManager arduino;
    public WinZone targetBlock;
    public double hapticThreshold = 2.0;

    public double closestDistance = 100.0;
    public bool doHaptic = true;
    public bool collectData = true;
    public int hapticRate = 0;
    
    System.DateTime begin;
    
    String csvFileName;
    
    float blockOffset = 0.0f;
    
    public List<float> distances = new List<float>();
    public List<float> heights = new List<float>();
    
    bool written;
    void Start()
    {
        blockOffset = markerBlock.transform.position.y;
        written = false;
        begin = System.DateTime.Now;
        
        
    }
    
    float CalculateDistance() {
        if (markerArm && markerBlock) {
            float dist = Vector3.Distance(markerArm.transform.position, markerBlock.transform.position);
            if (dist < closestDistance) {
                closestDistance = dist;
            }
            return dist;
        }
        return 0;
        
    }
    
    void DoHaptic(float dist) {
        int newHapticRate;
        if (doHaptic) {
            if (dist < hapticThreshold) {
                double portion = (hapticThreshold - dist) / hapticThreshold;
                int value = (int) (0xff * portion);
                newHapticRate = (value + 90) & 0xff;
                
            } else {
                newHapticRate = 0;
            }
            //Debug.Log(dist);
            if (newHapticRate != hapticRate) {
                hapticRate = newHapticRate;
                //Debug.Log(hapticRate);
                arduino.WriteByte((byte) hapticRate);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (targetBlock.win && !written) {
            
            System.DateTime current = System.DateTime.Now;
            System.TimeSpan elapsed = current - begin;
            
            Debug.Log($"Test finished!, elapsed {elapsed.TotalSeconds} seconds");
            WriteCSV();
            written = true;
        }
        
    }
    
    void WriteCSV() {
        string time = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        string extra;
        if (doHaptic) {
            extra = "-haptic";
        } else {
            extra = "-standard";
        }
        csvFileName=$"data/{time}{extra}.csv";
        
        using (StreamWriter file = new StreamWriter(csvFileName)) {
            for (int i = 0; i < distances.Count; i++) {
                float distance = distances[i];
                float height = heights[i];
                string line = $"{distance}, {height}, ";
                file.WriteLine(line);
            }
        }
        Debug.Log("Written!");

    }

    void FixedUpdate()
    {
        float d = CalculateDistance();
        DoHaptic(d);
        distances.Add(d);
        float h = markerBlock.transform.position.y - blockOffset;
        heights.Add(h);
    }
}
