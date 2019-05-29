using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInfo : MonoBehaviour
{
    //回收时间
    public float lifeTime = 0;

    private void OnEnable()
    {
        if (lifeTime > 0)
            StartCoroutine(RecycleTime(lifeTime));
    }

    /// <summary>
    /// 等待回收携程
    /// </summary>
    /// <param name="lifeTime"></param>
    /// <returns></returns>
    IEnumerator RecycleTime(float lifeTime)
    {
        //等待回收时间结束后进行回收
        yield return new WaitForSeconds(lifeTime);
        ObjectPoolCtrl.Instance.UnSpawn(gameObject);
    }
}
