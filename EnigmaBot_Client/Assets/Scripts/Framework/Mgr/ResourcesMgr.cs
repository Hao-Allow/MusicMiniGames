using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum EnumResType
{
    None,
    Audio,
    UIPrefab,
    TextAsset,
    Texture,
    SceneUI,
    WindowUI,
}

/// <summary>
/// 资源管理,控制资源的加载
/// </summary>
public class ResourcesMgr : MonoSingleton<ResourcesMgr>
{
    /// <summary>
    /// 存储资源引用的字典
    /// </summary>
    private Dictionary<string, UnityEngine.Object> assetDic = new Dictionary<string, UnityEngine.Object>();

    /// <summary>
    /// 加载资源的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resType"></param>
    /// <param name="name"></param>
    /// <param name="isCache"></param>
    /// <returns></returns>
    public T LoadAsset<T>(EnumResType resType, string name, bool isCache) where T : UnityEngine.Object
    {
        string fullPath = GetFullPath(resType, name);
        if (assetDic.ContainsKey(fullPath))
            return assetDic[fullPath] as T;

        T tmpRes = Resources.Load(fullPath) as T;

        if (tmpRes == null)
            Debug.Log("资源不存在/路径错误");

        if (isCache)
            assetDic.Add(fullPath, tmpRes);

        return tmpRes;
    }

    /// <summary>
    /// 创建游戏物体实例
    /// </summary>
    /// <param name="resType"></param>
    /// <param name="name"></param>
    /// <param name="isCache"></param>
    /// <returns></returns>
    public GameObject LoadAsset(EnumResType resType, string name, bool isCache)
    {
        UnityEngine.Object tmpGo = LoadAsset<UnityEngine.Object>(resType, name, isCache);
        GameObject goInstance = GameObject.Instantiate(tmpGo) as GameObject;
        return goInstance;
    }

    /// <summary>
    /// 释放缓存资源
    /// </summary>
    /// <param name="resType"></param>
    /// <param name="name"></param>
    public void ReleaseCache(EnumResType resType, string name)
    {
        string fullPath = GetFullPath(resType, name);
        if (assetDic.ContainsKey(fullPath))
        {
            UnityEngine.Object tmpObj = assetDic[fullPath];
            Resources.UnloadAsset(tmpObj);
            assetDic.Remove(fullPath);
        }
    }

    /// <summary>
    /// 释放单个资源
    /// </summary>
    /// <param name="tmpObj"></param>
    public void ReleaseSingleObj(UnityEngine.Object tmpObj)
    {
        if (tmpObj != null)
            Resources.UnloadAsset(tmpObj);
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void ReleaseAllObj()
    {
        foreach (string path in assetDic.Keys)
        {
            UnityEngine.Object tmpObj = assetDic[path];
            Resources.UnloadAsset(tmpObj);
            assetDic[path] = null;
        }
        assetDic.Clear();
        Discard();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 获取完整路径
    /// </summary>
    /// <param name="resType"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private string GetFullPath(EnumResType resType, string name)
    {
        StringBuilder fullPath = new StringBuilder();
        switch (resType)
        {
            case EnumResType.None:
                break;
            case EnumResType.Audio:
                fullPath.Append("Audio/");
                break;
            case EnumResType.UIPrefab:
                fullPath.Append("UIPrefab/");
                break;
            case EnumResType.TextAsset:
                fullPath.Append("TextAsset/");
                break;
            case EnumResType.Texture:
                fullPath.Append("");
                break;
            case EnumResType.SceneUI:
                fullPath.Append("");
                break;
            case EnumResType.WindowUI:
                fullPath.Append("");
                break;
        }
        fullPath.Append(name);
        return fullPath.ToString();
    }

    private void OnDestroy()
    {
        Discard();
    }
}
