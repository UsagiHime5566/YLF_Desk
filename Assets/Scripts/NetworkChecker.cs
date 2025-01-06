using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Net.NetworkInformation;

public class NetworkChecker : MonoBehaviour
{
    public static NetworkChecker Instance { get; private set; }
    
    [SerializeField] private string pingAddress = "8.8.8.8"; // Google DNS服務器
    [SerializeField] private string testWebsite = "http://www.google.com"; // 測試網站
    
    private bool isNetworkAvailable;
    public bool IsNetworkAvailable => isNetworkAvailable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(CheckInternetConnection());
    }

    private IEnumerator CheckInternetConnection()
    {
        while (true)
        {
            // 使用UnityWebRequest進行網路檢查
            using (UnityWebRequest webRequest = UnityWebRequest.Get(testWebsite))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    isNetworkAvailable = true;
                    Debug.Log("網路連接正常");
                }
                else
                {
                    isNetworkAvailable = false;
                    Debug.LogWarning("網路連接失敗: " + webRequest.error);
                }
            }

            // 使用Ping進行備用檢查
            UnityEngine.Ping ping = null;
            try
            {
                ping = new UnityEngine.Ping(pingAddress);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ping 檢查失敗: {e.Message}");
                yield break;
            }

            yield return new WaitForSeconds(2f);

            if (ping != null && ping.isDone && ping.time >= 0)
            {
                isNetworkAvailable = true;
                Debug.Log($"Ping 延遲: {ping.time}ms");
            }

            yield return new WaitForSeconds(30f); // 每30秒檢查一次
        }
    }

    // 提供給其他腳本調用的公開方法
    public void CheckNetworkStatus(System.Action<bool> callback)
    {
        StartCoroutine(CheckNetworkStatusCoroutine(callback));
    }

    private IEnumerator CheckNetworkStatusCoroutine(System.Action<bool> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(testWebsite))
        {
            yield return webRequest.SendWebRequest();
            bool status = (webRequest.result == UnityWebRequest.Result.Success);
            callback?.Invoke(status);
        }
    }

    // 獲取當前網路狀態的方法
    public bool GetCurrentNetworkStatus()
    {
        return isNetworkAvailable;
    }

    // 檢查特定網站是否可訪問
    public void CheckSpecificWebsite(string url, System.Action<bool> callback)
    {
        StartCoroutine(CheckSpecificWebsiteCoroutine(url, callback));
    }

    private IEnumerator CheckSpecificWebsiteCoroutine(string url, System.Action<bool> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            bool status = (webRequest.result == UnityWebRequest.Result.Success);
            callback?.Invoke(status);
        }
    }
} 