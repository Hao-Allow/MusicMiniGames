using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式通用基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T:MonoSingleton<T>
{
    private static T _instance = null;

    /// <summary>
    /// 获得单一实例，没有会自动在Helper在创建
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObj = new GameObject(typeof(T).Name);
                _instance = gameObj.AddComponent<T>();

                GameObject helper = GameObject.Find("Helper");

                if (helper == null)
                {
                    helper = new GameObject("Helper");
                    DontDestroyOnLoad(helper);
                }

                gameObj.transform.SetParent(helper.transform);
                gameObj.transform.localPosition = Vector3.zero;
                gameObj.transform.localRotation = Quaternion.identity;
                gameObj.transform.localScale = Vector3.one;
            }
            return _instance;
        }
    }

    /// <summary>
    /// 销毁单例
    /// </summary>
    public virtual void Discard()
    {
        Destroy(gameObject);
        _instance = null;
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }

    private void OnDestroy()
    {
        Discard();
    }
}
