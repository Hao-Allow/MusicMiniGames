using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyroTest : MonoBehaviour
{
    public Button backBtn;

    private float gravityX;
    private float gravityY;
    private float gravityZ;

    private float rotateSpeed = 50f;

    private Quaternion quatMap;
    private Quaternion quatMult = new Quaternion(0, 0, 1, 0);
    private Gyroscope gyro;

    private void OnEnable()
    {
        backBtn.onClick.AddListener(OnBackBtnCallback);
    }

    private void OnBackBtnCallback()
    {
        SceneMgr.Instance.LoadScene(EnumSceneType.Home);
    }

    private void Start()
    {
        gyro = Input.gyro;
        gyro.enabled = true;
    }

    private void Update()
    {
        Quaternion input = Input.gyro.attitude;
        input = Quaternion.Euler(90, 0, 0) * (new Quaternion(-input.x, -input.y, input.z, input.w));
        transform.localRotation = input;
        //quatMap = new Quaternion(gyro.attitude.x, gyro.attitude.y, gyro.attitude.z, gyro.attitude.w);
        //Quaternion qt = quatMap;
        //transform.rotation = qt;
    }

    private void OnDestroy()
    {
        backBtn.onClick.RemoveListener(OnBackBtnCallback);
    }
}
