using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneCtrl : MonoBehaviour
{
    public AsyncOperation async;//异步操作
    private int step;//步数
    private float progressValue;

    public Text tip;

    private void Start()
    {
        async = SceneManager.LoadSceneAsync(SceneMgr.Instance.GetSceneNameFromType());
        async.allowSceneActivation = false;

    }

    private void Update()
    {
        step++;
        if (step <= 100)
            tip.text = "正在加载场景......  " + step + "%";

        if (step >= 100 && async.progress >= 0.9f)
            async.allowSceneActivation = true;
    }
}
