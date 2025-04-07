using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ArduinoManager : MonoBehaviour
{
    public const int numEntries = 4;

    public GameObject distanceA;
    public GameObject distanceB;
    public float[] entries = new float[numEntries];
    public string portName = "/dev/ttyACM0";
    public int baudRate = 115200;
    
    public double closestDistance = 100.0;
    
    public bool haptic = true;
    
    public int hapticRate = 0;
    private SerialPort serialPort;
    // Start is called before the first frame update
    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            serialPort.ReadTimeout = 1000;
            Debug.Log("Opened serial port!");
        } catch (Exception e) {
            Debug.Log($"Failed to open port: {e.Message}");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (serialPort.IsOpen && serialPort.BytesToRead > 0) {
            string data = serialPort.ReadLine();
            string[] values = data.Split(",");
            for (int i = 0; i < numEntries; i++) {
                float result = float.Parse(values[i].Trim());
                entries[i] = result;
            }
            //Debug.Log($"Serial: {data}");
        }
        int newHapticRate = hapticRate;
        if (haptic && distanceA && distanceB) {
            float dist = Vector3.Distance(distanceA.transform.position, distanceB.transform.position);
            if (dist < closestDistance) {
                closestDistance = dist;
            }
            if (dist < 2.0) {
                double portion = (2.0 - dist) / 2.0;
                int value = (int) (0xff * portion);
                newHapticRate = (value + 90) & 0xff;
                
            } else {
                newHapticRate = 0;
            }
            //Debug.Log(dist);
        }
        if (newHapticRate != hapticRate) {
            hapticRate = newHapticRate;
            //Debug.Log(hapticRate);
            byte[] toWrite = new byte[1];
            toWrite[0] = (byte) (hapticRate);
            Debug.Log(toWrite);
            serialPort.Write(toWrite, 0, 1);
        }

        
    }
    void OnApplicationQuit() {
        if (serialPort != null && serialPort.IsOpen) {
            serialPort.Close();
            Debug.Log("Port closed");
        }
    }
}
