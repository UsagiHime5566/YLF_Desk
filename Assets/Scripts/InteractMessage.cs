using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using HimeLib;

public class InteractMessage : MonoBehaviour
{
    [Header("IOTTalk API")]
    public string BaseUrl = "ylf2025.iottalk.tw/signal";
    public float checkInterval = 1f;  // 檢查間隔為1秒
    public int lastReceivedValue = 0;

    [Header("Arduino")]
    public MyArduino arduinoInteractive;
    public string lastDataArduino = "";
    public float resetArduinoCD = 3f;

    [Header("CD 線上時間")]
    public float cdTime_online = 90f;
    private float nextCDTime_online = 3f;

    [Header("CD 本地時間")]
    public float cdTime = 90f;
    private float nextCDTime = 3f;

    [Header("UI")]
    public Text RemoteSignalText;
    public Text GlobalCDText;
    public Text LocalCDText;
    public Text TXT_Msg;
    public Text TXT_TotalSignal;
    public Image img_CD;

    public Queue<string> queueMessage = new Queue<string>();

    int totalSignal = 0;

    public void DoQueueMessageStr(string x){
        queueMessage.Enqueue($"{x} ({System.DateTime.Now})");
        if(queueMessage.Count > 5){
            queueMessage.Dequeue();
        }
        totalSignal++;
    }

    void ShowMessage(){
        TXT_Msg.text = "";
        foreach (var item in queueMessage)
        {
            TXT_Msg.text += item + "\n";
        }
    }


    string localLastData;

    public System.Action OnShootingButtonPressed;

    void Start()
    {
        arduinoInteractive.OnRecieveData += (x) => {
            //不知為何不會觸發
            //Debug.Log("Arduino Callback");
        };

        // 初始化遠端數據
        SendIntSignal(0);

        StartCoroutine(EveryCheckIOT());

    }

    //自動迴圈檢查數據, 這邊是主機聽資料自動做噴水與CD
    void HandleReceivedValue(int value){
        RemoteSignalText.text = value.ToString();

        //如果遠端數據變成1的話
        //手機只有變1功能
        //這邊也有按鈕變1功能
        //變0只有這個涵式CD到自動變0, 或是程式重開的初始化變0
        if(value == 1){
            if(GameManager.instance.IsMainComputer){
                arduinoInteractive.SendData("1");
                nextCDTime_online = Time.time + cdTime_online;

                StartCoroutine(ResetData());
            }
            OnShootingButtonPressed?.Invoke();
        }
    }

    IEnumerator ResetData(){
        yield return new WaitForSeconds(resetArduinoCD);
        arduinoInteractive.SendData("0");
    }

    IEnumerator EveryCheckIOT(){
        while(true){
            yield return new WaitForSeconds(checkInterval);
            GetRemoteSignal();

            if(GameManager.instance.IsMainComputer){
                //如果CD時間到就設置遠端數據為0
                if(Time.time >= nextCDTime_online){
                    SendIntSignal(0);
                    arduinoInteractive.SendData("0");
                    nextCDTime_online = 9999999999f;    //避免再次檢查到
                }
            }

            GlobalCDText.text = $"{nextCDTime_online - Time.time:F2}";
            LocalCDText.text = $"{nextCDTime - Time.time:F2}";

            if(NetworkChecker.Instance.IsNetworkAvailable){
                float amount = (nextCDTime_online - Time.time) / cdTime_online;
                img_CD.fillAmount = amount > 1 ? 1 : 1 - amount;
            } else {
                float amount = (nextCDTime - Time.time) / cdTime; 
                img_CD.fillAmount = amount > 1 ? 1 : 1 - amount;
            }
        }
    }

    



    public void OnRecieveData(string data){
        //Debug.Log("data passed");
        if(localLastData == data) return;
        localLastData = data;

        // 按鈕被按下
        if(data == "y"){
            OnShootingButton();
        }
        // 按鈕被釋放
        if(data == "n"){
            //Do Nothing
            Debug.Log("arduino n (Release Button)");
        }
        try{
            ShowMessage();
        } catch {}
    }


    //按下按鈕行為, 與手機相通邏輯
    public void OnShootingButton(){
        //有網路的話
        if(NetworkChecker.Instance.IsNetworkAvailable){
            //如果遠端數據是0的話才能送1
            if(lastReceivedValue == 0){
                SendIntSignal(1);
            }
        } else {
            //沒網路的狀況
            if(Time.time >= nextCDTime){
                arduinoInteractive.SendData("1");
                nextCDTime = Time.time + cdTime;
                StartCoroutine(ResetData());

                OnShootingButtonPressed?.Invoke();
            }
        }
    }












    // 測試相關
    void Update()
    {
        OnRecieveData(lastDataArduino);
        TXT_TotalSignal.text = totalSignal.ToString();

        // 測試用指定
        if (Input.GetKeyDown(KeyCode.O))
        {
            arduinoInteractive.SendData("1");
            Debug.Log("送arduino 1");
        }
        // 測試用指定
        if (Input.GetKeyDown(KeyCode.P))
        {
            arduinoInteractive.SendData("0");
            Debug.Log("送arduino 0");
        }
        // 測試用指定
        if(Input.GetKeyDown(KeyCode.Z)){
            SendIntSignal(1);
        }
        // 測試用指定
        if(Input.GetKeyDown(KeyCode.X)){
            SendIntSignal(0);
        }
    }


    //網路相關可以不用裡

    public void SendIntSignal(int signal)
    {
        string url = "https://" + BaseUrl;
        Debug.Log(url);
        StartCoroutine(SendIntRequest(url, signal));
    }

    private IEnumerator SendIntRequest(string url, int signal)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            // 直接將整數轉換為字串後轉為位元組
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(signal.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "text/plain");  // 改為純文字格式
            request.SetRequestHeader("Authorization", "Bearer fa1a4509915146fdb07ae71645b561cf");
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("發送請求失敗: " + request.error);
            }
            else
            {
                Debug.Log("請求發送成功: " + request.downloadHandler.text);
            }
        }
    }

    // 獲取遠端信號的方法
    public void GetRemoteSignal()
    {
        string url = "https://" + BaseUrl;
        StartCoroutine(GetRemoteRequest(url));
    }

    private IEnumerator GetRemoteRequest(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer fa1a4509915146fdb07ae71645b561cf");
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("獲取數據失敗: " + request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                if (int.TryParse(response, out int value))
                {
                    if (value != lastReceivedValue)
                    {
                        lastReceivedValue = value;
                        Debug.Log("收到新的遠端數據: " + value);
                        HandleReceivedValue(value);
                    }
                }
            }
        }
    }
}
