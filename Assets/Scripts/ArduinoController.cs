using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoController : MonoBehaviour
{
    private SerialPort serialPort;
    private string portName = "COM3"; // 需要根據實際Arduino連接的連接埠修改
    private const int baudRate = 9600;
    private bool isConnected = false;

    private void Start()
    {
        ConnectToArduino();
    }

    private void ConnectToArduino()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 100;
            serialPort.WriteTimeout = 100;
            serialPort.Open();
            isConnected = true;
            Debug.Log("成功連接到Arduino");
        }
        catch (Exception e)
        {
            Debug.LogError($"無法連接到Arduino: {e.Message}");
            isConnected = false;
        }
    }

    private void Update()
    {
        if (!isConnected) return;

        try
        {
            // 讀取Arduino發送的數據
            if (serialPort.BytesToRead > 0)
            {
                string message = serialPort.ReadLine();
                ProcessArduinoMessage(message.Trim());
            }

            // 示例：按下空格鍵發送開燈指令
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TurnOnLight();
            }
            // 按下R鍵發送關燈指令
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                TurnOffLight();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"讀取數據時出錯: {e.Message}");
        }
    }

    private void ProcessArduinoMessage(string message)
    {
        switch (message)
        {
            case "y": // 按鈕被按下
                Debug.Log("Arduino按鈕被按下");
                // 在這裡添加按鈕按下時的處理邏輯
                break;
            case "n": // 按鈕被釋放
                Debug.Log("Arduino按鈕被釋放");
                // 在這裡添加按鈕釋放時的處理邏輯
                break;
        }
    }

    public void TurnOnLight()
    {
        if (isConnected)
        {
            serialPort.Write("1");
            Debug.Log("發送開燈指令");
        }
    }

    public void TurnOffLight()
    {
        if (isConnected)
        {
            serialPort.Write("0");
            Debug.Log("發送關燈指令");
        }
    }

    private void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
} 