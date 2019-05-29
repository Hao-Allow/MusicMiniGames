using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageCenter
{
    //委托：消息传递
    //public delegate void DelMessageDelivery(params object[] messageData);
    public delegate void DelMessageDelivery(KeyValuesUpdate kv);


    //消息中心缓存集合
    //<short : 数据大的分类，DelMessageDelivery 数据执行委托>
    public static Dictionary<short, DelMessageDelivery> m_DicMessages = new Dictionary<short, DelMessageDelivery>();

    /// <summary>
    /// 增加消息的监听
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    public static void AddMsgListener(short messageType, DelMessageDelivery handler)
    {
        if (!m_DicMessages.ContainsKey(messageType))
            m_DicMessages.Add(messageType, null);

        m_DicMessages[messageType] += handler;
    }

    /// <summary>
    /// 取消消息的监听
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handler"></param>
    public static void RemoveMsgListener(short messageType, DelMessageDelivery handler)
    {
        if (m_DicMessages.ContainsKey(messageType))
            m_DicMessages[messageType] -= handler;
    }

    /// <summary>
    /// 取消所有监听
    /// </summary>
    public static void ClearAllMsgListener()
    {
        if (m_DicMessages != null)
            m_DicMessages.Clear();
    }

    /// <summary>
    /// 派发消息
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="messageData"></param>
    public static void Dispatcher(short messageType, KeyValuesUpdate kv)
    {
        DelMessageDelivery del;
        if (m_DicMessages.TryGetValue(messageType, out del))
        {
            if (del != null)
                del(kv);
                //del(messageData);
        }
    }
}

/// <summary>
/// 键值更新,配合委托,实现数据传输
/// </summary>
public class KeyValuesUpdate
{
    public string Key { get; private set; }
    public object Values { get; private set; }

    public KeyValuesUpdate(string key,object valueObj)
    {
        this.Key = key;
        this.Values = valueObj;
    }
}
