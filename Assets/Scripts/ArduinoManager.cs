using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ArduinoManager : MonoBehaviour
{
    public const int numEntries = 2;
    public float[] entries = new float[numEntries];
    public string portName = "/dev/ttyACM0";
    public int baudRate = 115200;
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
            Debug.Log($"Serial: {data}");
        }
        
    }
    void OnApplicationQuit() {
        if (serialPort != null && serialPort.IsOpen) {
            serialPort.Close();
            Debug.Log("Port closed");
        }
    }
}
