using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池管理类
/// </summary>
public class ObjectPoolCtrl : MonoSingleton<ObjectPoolCtrl>
{
    /// <summary>
    /// 管理各个对象池的集合
    /// </summary>
    Dictionary<string, SubPool<Object>> m_PoolCtrl = new Dictionary<string, SubPool<Object>>();

    /// <summary>
    /// 取出对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Object Spawn(string name)
    {
        SubPool<Object> subPool;
        if (!m_PoolCtrl.ContainsKey(name))
            //创建池子
            CreateSubPool(name);

        subPool = m_PoolCtrl[name];
        return subPool.Spawn();
    }

    /// <summary>
    /// 创建新池子
    /// </summary>
    /// <param name="name"></param>
    private void CreateSubPool(string name)
    {
        GameObject obj = Resources.Load<GameObject>(name);
        SubPool<Object> subPool = new SubPool<Object>(obj);
        m_PoolCtrl.Add(name, subPool);
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="obj"></param>
    public void UnSpawn(GameObject obj)
    {
        foreach (SubPool<Object> pool in m_PoolCtrl.Values)
        {
            if (!pool.ContainsObj(obj))
            {
                pool.UnSpawn(obj);
                break;
            }
        }
    }

    /// <summary>
    /// 回收指定池子所有对象
    /// </summary>
    /// <param name="name"></param>
    public void UnSpawnAppointPool(string name)
    {
        if (m_PoolCtrl.ContainsKey(name))
            m_PoolCtrl[name].UnSpawnAllObj();
    }

    /// <summary>
    /// 回收所有池子对象
    /// </summary>
    public void UnSpawnAll()
    {
        foreach (SubPool<Object> pool in m_PoolCtrl.Values)
        {
            pool.UnSpawnAllObj();
        }
    }

    /// <summary>
    /// 清空指定池子
    /// </summary>
    /// <param name="name"></param>
    public void ClearAppointPool(string name)
    {
        if (m_PoolCtrl.ContainsKey(name))
        {
            m_PoolCtrl[name].ClearPool();
        }
    }

    /// <summary>
    /// 清空所有池子
    /// </summary>
    public void ClearAllPool()
    {
        foreach (SubPool<Object> pool in m_PoolCtrl.Values)
        {
            pool.ClearPool();
        }
    }
}
