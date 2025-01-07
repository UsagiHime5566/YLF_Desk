using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : HimeLib.SingletonMono<GameManager>
{
    public Toggle MainComputerToggle;
    public Text networkStatusText;
    public Button QuitButton;
    public InputField ResolutionInput;

    [Header("海豚")]
    public RenderTexture DolphinRender;
    public RawImage DolphinImage;
    public Camera DolphinCamera;

    public GameObject PlanMaping;
    public Transform DophinAnim;
    public float DophinScale = 1f;
    public float DophinScaleSpeed = 0.05f;

    public bool PlanMapingActive = false;

    public bool IsMainComputer = true;

    int resolutionParam = 1;

    void Start()
    {
        LoadDolphScale();
        
        MainComputerToggle.onValueChanged.AddListener(x => {
            SystemConfig.Instance.SaveData("MainComputerToggle", x);
            IsMainComputer = x;
        });
        MainComputerToggle.isOn = SystemConfig.Instance.GetData<bool>("MainComputerToggle", true);

        ResolutionInput.onValueChanged.AddListener(x => {
            if(int.TryParse(x, out int result)){
                if(result == 0) return;
                SystemConfig.Instance.SaveData("ResolutionParam", result);
                resolutionParam = result;
            }
        });
        ResolutionInput.text = SystemConfig.Instance.GetData<int>("ResolutionParam", 1).ToString();

        QuitButton.onClick.AddListener(QuitGame);


        InitDolphinRender();

        if(networkStatusText)
            StartCoroutine(CheckNetworkStatus());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.PageUp)){
            DophinScale += DophinScaleSpeed;
            DophinAnim.localScale = new Vector3(DophinScale, DophinScale, DophinScale); // 放大海豚
            SaveDolphScale();
        }
        if(Input.GetKeyDown(KeyCode.PageDown)){
            DophinScale -= DophinScaleSpeed;
            DophinAnim.localScale = new Vector3(DophinScale, DophinScale, DophinScale); // 縮小海豚
            SaveDolphScale();
        }
        if(Input.GetKeyDown(KeyCode.F4)){                                        // 切換海豚可見地板
            PlanMaping.SetActive(!PlanMaping.activeSelf);
            PlanMapingActive = PlanMaping.activeSelf;
        }
    }

    public void InitDolphinRender(){
        DolphinRender = new RenderTexture(Screen.width / resolutionParam, Screen.height / resolutionParam, 24, RenderTextureFormat.ARGB32);
        DolphinImage.texture = DolphinRender;
        DolphinCamera.targetTexture = DolphinRender;
    }

    void LoadDolphScale(){
        DophinScale = SystemConfig.Instance.GetData<float>("DophinScale", 1.0f);
        DophinAnim.localScale = new Vector3(DophinScale, DophinScale, DophinScale);
    }

    void SaveDolphScale(){
        SystemConfig.Instance.SaveData("DophinScale", DophinScale);
    }

    IEnumerator CheckNetworkStatus(){
        while(true){
            if(NetworkChecker.Instance.IsNetworkAvailable)
                networkStatusText.text = "網路連接：正常";
            else
                networkStatusText.text = "網路連接：失敗";

            yield return new WaitForSeconds(1f);
        }
    }

    public void QuitGame(){
        Application.Quit();
    }
}
