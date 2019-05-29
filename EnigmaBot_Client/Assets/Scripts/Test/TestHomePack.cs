using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHomePack : MonoBehaviour
{
    public Button gravityBtn;
    public Button gyroBtn;
    public Button musicBtn;
    public Button quitBtn;

    private void OnEnable()
    {
        gravityBtn.onClick.AddListener(OnGravityBtnCallback);
        gyroBtn.onClick.AddListener(OnGyroBtnCallback);
        quitBtn.onClick.AddListener(OnQuitGameBtnCallback);
        musicBtn.onClick.AddListener(OnMusicGuitarBtnCallback);
    }

    private void OnMusicGuitarBtnCallback()
    {
        SceneMgr.Instance.LoadScene(EnumSceneType.Music);
    }

    private void OnQuitGameBtnCallback()
    {
        ResourcesMgr.Instance.ReleaseAllObj();
        Application.Quit();
    }

    private void OnGyroBtnCallback()
    {
        SceneMgr.Instance.LoadScene(EnumSceneType.Gyro);
    }

    private void OnGravityBtnCallback()
    {
        SceneMgr.Instance.LoadScene(EnumSceneType.Gravity);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnDestroy()
    {
        gravityBtn.onClick.RemoveListener(OnGravityBtnCallback);
        gyroBtn.onClick.RemoveListener(OnGyroBtnCallback);
        musicBtn.onClick.RemoveListener(OnMusicGuitarBtnCallback);
        quitBtn.onClick.RemoveListener(OnQuitGameBtnCallback);
    }
}
