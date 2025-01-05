using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : HimeLib.SingletonMono<GameManager>
{
    public RenderTexture DolphinRender;
    public RawImage DolphinImage;
    public Camera DolphinCamera;

    public GameObject PlanMaping;
    public Transform DophinAnim;
    public float DophinScale = 1f;
    public float DophinScaleSpeed = 0.05f;

    public bool PlanMapingActive = false;

    void Start()
    {
        InitDolphinRender();
        LoadDolphScale();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.PageUp)){
            DophinScale += DophinScaleSpeed;
            DophinAnim.localScale = new Vector3(DophinScale, DophinScale, DophinScale); // 放大
            SaveDolphScale();
        }
        if(Input.GetKeyDown(KeyCode.PageDown)){
            DophinScale -= DophinScaleSpeed;
            DophinAnim.localScale = new Vector3(DophinScale, DophinScale, DophinScale); // 縮小 
            SaveDolphScale();
        }
        if(Input.GetKeyDown(KeyCode.F4)){
            PlanMaping.SetActive(!PlanMaping.activeSelf);
            PlanMapingActive = PlanMaping.activeSelf;
        }
    }

    public void InitDolphinRender(){
        DolphinRender = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
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
}
