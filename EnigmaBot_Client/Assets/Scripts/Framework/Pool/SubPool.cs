using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子对象池
/// </summary>
/// <typeparam name="T"></typeparam>
public class SubPool<T> where T : Object
{
    //池子
    Queue<T> m_SubPool = new Queue<T>();

    ///最大数量
    private int maxObjCount = 10;

    //预制体
    T m_Resource;

    public string ResourceName
    {
        get
        {
            return m_Resource.name;
        }
    }

    public SubPool(T prefab)
    {
        m_Resource = prefab;
    }

    /// <summary>
    /// 取对象
    /// </summary>
    /// <returns></returns>
    public T Spawn()
    {
        T obj = null;

        if (m_SubPool.Count > 0)
            obj = m_SubPool.Dequeue();
        else
            obj = GameObject.Instantiate(m_Resource);

        (obj as GameObject).SetActive(true);
        return obj;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="obj"></param>
    public void UnSpawn(T obj)
    {
        if (m_SubPool.Contains(obj))
            return;

        if (m_SubPool.Count > maxObjCount)
        {
            GameObject.Destroy(obj);
        }
        else
        {
            m_SubPool.Enqueue(obj);
            (obj as GameObject).SetActive(false);
        }
    }

    /// <summary>
    /// 回收所有对象
    /// </summary>
    public void UnSpawnAllObj()
    {
        foreach (T item in m_SubPool)
        {
            if ((item as GameObject).activeSelf)
                UnSpawn(item);
        }
    }

    /// <summary>
    /// 判断是否包含该对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool ContainsObj(T obj)
    {
        return m_SubPool.Contains(obj);
    }

    /// <summary>
    /// 清空池子
    /// </summary>
    public void ClearPool()
    {
        foreach (Object obj in m_SubPool)
        {
            if (obj != null)
                GameObject.Destroy(obj);
        }
        m_SubPool.Clear();
    }
}
