using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoController : MonoBehaviour
{
    private SerialPort serialPort;
    private string portName = "COM3"; // 需要根据实际Arduino连接的端口修改
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
            Debug.Log("成功连接到Arduino");
        }
        catch (Exception e)
        {
            Debug.LogError($"无法连接到Arduino: {e.Message}");
            isConnected = false;
        }
    }

    private void Update()
    {
        if (!isConnected) return;

        try
        {
            // 读取Arduino发送的数据
            if (serialPort.BytesToRead > 0)
            {
                string message = serialPort.ReadLine();
                ProcessArduinoMessage(message.Trim());
            }

            // 示例：按下空格键发送开灯指令
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TurnOnLight();
            }
            // 按下R键发送关灯指令
            if (Input.GetKeyDown(KeyCode.R))
            {
                TurnOffLight();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"读取数据时出错: {e.Message}");
        }
    }

    private void ProcessArduinoMessage(string message)
    {
        switch (message)
        {
            case "y": // 按钮被按下
                Debug.Log("Arduino按钮被按下");
                // 在这里添加按钮按下时的处理逻辑
                break;
            case "n": // 按钮被释放
                Debug.Log("Arduino按钮被释放");
                // 在这里添加按钮释放时的处理逻辑
                break;
        }
    }

    public void TurnOnLight()
    {
        if (isConnected)
        {
            serialPort.Write("1");
            Debug.Log("发送开灯指令");
        }
    }

    public void TurnOffLight()
    {
        if (isConnected)
        {
            serialPort.Write("0");
            Debug.Log("发送关灯指令");
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