using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HimeLib;

public class InteractMessage : MonoBehaviour
{
    public ArduinoInteractive arduinoInteractive;

    string lastData = "";

    void Start()
    {
        arduinoInteractive.OnRecieveData += OnRecieveData;
    }

    // Update is called once per frame
    void Update()
    {
        // 示例：按下空格鍵發送開燈指令
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            TurnOnLight();
        }
        // 按下R鍵發送關燈指令
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            TurnOffLight();
        }
    }

    public void OnRecieveData(string data){
        if(lastData == data) return;
        lastData = data;
        
        // 按鈕被按下
        if(data == "y"){
            
        }
        // 按鈕被釋放
        if(data == "n"){
            
        }
    }

    public void TurnOnLight()
    {
        if (arduinoInteractive.IsArduinoConnect())
        {
            arduinoInteractive.SendData("1");
            Debug.Log("發送開燈指令");
        }
    }

    public void TurnOffLight()
    {
        if (arduinoInteractive.IsArduinoConnect())
        {
            arduinoInteractive.SendData("0");
            Debug.Log("發送關燈指令");
        }
    }
}
