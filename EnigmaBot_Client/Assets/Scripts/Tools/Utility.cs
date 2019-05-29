

using UnityEngine;

public static class Utility
{
    public static void SetParentAndReset(this GameObject go, Transform parent)
    {
        if (parent != null)
            go.transform.SetParent(parent);

        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 获取或添加
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T tmpComponent = go.GetComponent<T>();
        if (tmpComponent == null)
            tmpComponent = go.AddComponent<T>();

        return tmpComponent;
    }
}
