using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EnumSceneType
{
    Home,
    Gravity,
    Gyro,
    Music
}


public class SceneMgr : MonoSingleton<SceneMgr>
{
    /// <summary>
    /// 当前场景
    /// </summary>
    public EnumSceneType currentScene;

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneType"></param>
    public void LoadScene(EnumSceneType sceneType)
    {
        currentScene = sceneType;
        SceneManager.LoadScene("LoadingScene");
    }

    public string GetSceneNameFromType()
    {
        string sceneName = "";
        switch (currentScene)
        {
            case EnumSceneType.Home:
                sceneName = "HomePackScene";
                break;
            case EnumSceneType.Gravity:
                sceneName = "GravityScene";
                break;
            case EnumSceneType.Gyro:
                sceneName = "GyroScene";
                break;
            case EnumSceneType.Music:
                sceneName = "FinalRush";
                break;
        }
        return sceneName;
    }
    private void OnDestroy()
    {
        Discard();
    }


}
