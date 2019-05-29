using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLEDataCenter : MonoBehaviour
{

    void Start()
    {
        //BLEController.OnReceive += Receive;
        //MessageCenter.AddMsgListener(1, PrepareRealButton01Action);
        //MessageCenter.AddMsgListener(2, PrepareSubColorAction);
        //MessageCenter.AddMsgListener(4, PrepareMotor02Action);
        //MessageCenter.AddMsgListener(4, PrepareMotor01Action);
        //MessageCenter.AddMsgListener(5, PrepareAxisAccelerationAction);
        MessageCenter.AddMsgListener((short)GameModule.Dir, PrepareHandler);
    }

    /// <summary>
    /// 在这里再去把对应的响应进行拆分实现
    /// </summary>
    /// <param name="kv"></param>
    private void PrepareHandler(KeyValuesUpdate kv)
    {
        
    }

    public void SetData(string data)
    {
        string[] dataCells = data.Split(' ');
    }

    //public void Receive(byte[] bytes)
    //{
    //    string messageData = "";
    //    for (int i = 0; i < bytes.Length; i++)
    //    {
    //        messageData += bytes[i] + "   ";
    //    }

    //    Debug.Log("messageData     " + messageData);
    //    #region 之前的
    //    //byte length = bytes[2];
    //    //int lengthInt = System.BitConverter.ToInt32(new byte[] { length, 0x00, 0x00, 0x00 }, 0);
    //    //Debug.Log("length     " + lengthInt + "\r\n");
    //    //byte id;
    //    ////byte sum = bytes[3 + lengthInt + 1];
    //    //byte realSum = 0x00;
    //    //for (int i = 0; i < 3 + length; i++)
    //    //{
    //    //    realSum += bytes[i];
    //    //}
    //    //bool sumCheck = realSum == sum;
    //    #endregion
    //    byte dataLength =  bytes[2];
    //    Debug.Log("dataLength：   " + dataLength);
    //    byte id;
    //    if (bytes[0] == 0x55 && bytes[1] == 0xAA)
    //    {
    //        id = bytes[3];
    //        Debug.Log("dataID：   " + id);
    //        byte[] data = new byte[dataLength];

    //        for (int i  = 0; i< dataLength; i++)
    //        {
    //            data[i] = bytes[4 + i];
    //        }
    //        SendMessage(id, data);
    //    }
    //    else
    //    {
    //        Debug.Log("not 0x55 0xAA");
    //    }
    //}

    //public void SendMessage(byte id, byte[] data)
    //{
    //    switch (id)
    //    {
    //        case 1:
    //            PrepareRealButton01Action(data);
    //            break;
    //        case 2:
    //            PrepareSubColorAction(data);
    //            break;
    //        case 4:
    //            PrepareMotor01Action(data);
    //            PrepareMotor02Action(data);
    //            break;
    //        case 5:
    //            PrepareAxisAccelerationAction(data);
    //            break;
    //    }
    //}

    public delegate void RealButton01Action(int buttonState);
    public static event RealButton01Action OnPushButton01;
    public void PrepareRealButton01Action(object[] messageData)//  byte[] data
    {
        string message = "";
        for (int i = 0; i < messageData.Length; i++)
        {
            message += messageData[i];
        }
        Debug.Log("messageData_Length:   " + messageData.Length + "  messageData[0]_Length:   " + (messageData[0] as byte[]).Length + "  >>  " + message);
        //int buttonState = System.BitConverter.ToInt32(new byte[] { data[0], 0x00, 0x00, 0x00 }, 0);
        //OnPushButton01(buttonState);
        //Debug.Log("buttonState    " + buttonState + "\r\n");
    }

    public delegate void SubColorAction(Color color);
    public static event SubColorAction OnSubColor;
    public void PrepareSubColorAction(object[] messageData)
    {
        string message = "";
        for (int i = 0; i < messageData.Length; i++)
        {
            message += messageData[i];
        }
        Debug.Log("messageData_Length:   " + messageData.Length + "  messageData[0]_Length:   " + (messageData[0] as byte[]).Length + "  >>  " + message);
        //byte colorByte = data[0];
        //float colorDate = System.BitConverter.ToInt32(new byte[] { colorByte, 0x00, 0x00, 0x00 }, 0) / 255f;
        //Color color = Color.grey;
        //switch (colorDate)
        //{
        //    case 0:
        //        color = new Color(0, 0, 0);
        //        break;
        //    case 1:
        //        color = new Color(225f / 255f, 15f / 255f, 15f / 255f);
        //        break;
        //    case 2:
        //        color = new Color(15f / 255f, 100f / 255f, 225f / 255f);
        //        break;
        //    case 3:
        //        color = new Color(15f / 255f, 225f / 255f, 15f / 255f);
        //        break;
        //    case 4:
        //        color = new Color(225f / 255f, 225f / 255f, 15f / 255f);
        //        break;
        //    case 5:
        //        color = new Color(225f / 255f, 15f / 255f, 15f / 255f);
        //        break;
        //    case 6:
        //        color = new Color(1, 1, 1);
        //        break;
        //}
        //OnSubColor(color);
        //Debug.Log("color    " + color + "\r\n");
    }


    public delegate void Motor01Action(int angle);
    public static event Motor01Action OnMotor01;
    public void PrepareMotor01Action(object[] messageData)
    {
        string message = "";
        for (int i = 0; i < messageData.Length; i++)
        {
            message += messageData[i];
        }
        Debug.Log("messageData_Length:   " + messageData.Length + "  messageData[0]_Length:   " + (messageData[0] as byte[]).Length + "  >>  " + message);
        //int angle = System.BitConverter.ToInt32(new byte[] { data[0], data[1], data[2], data[3] }, 0);
        //OnMotor01(angle);
        //Debug.Log("angle01    " + angle + "\r\n");
    }


    public delegate void Motor02Action(int angle);
    public static event Motor02Action OnMotor02;
    public void PrepareMotor02Action(object[] messageData)
    {
        string message = "";
        for (int i = 0; i < messageData.Length; i++)
        {
            message += messageData[i];
        }
        Debug.Log("messageData_Length:   " + messageData.Length + "  messageData[0]_Length:   " + (messageData[0] as byte[]).Length + "  >>  " + message);
        //int angle = System.BitConverter.ToInt32(new byte[] { data[4], data[5], data[6], data[7] }, 0);
        //OnMotor02(-angle);
        //Debug.Log("angle02    " + angle + "\r\n");
    }

    public delegate void AxisAccelerationAction(Vector3 acceleration);
    public static event AxisAccelerationAction OnAcceleration;
    public void PrepareAxisAccelerationAction(object[] messageData)
    {
        string message = "";
        for (int i = 0; i < messageData.Length; i++)
        {
            message += messageData[i];
        }
        Debug.Log("messageData_Length:   " + messageData.Length + "  messageData[0]_Length:   " + (messageData[0] as byte[]).Length + "  >>  " + message);
        //byte x = data[0];
        //byte y = data[1];
        //byte z = data[2];
        //int X = System.BitConverter.ToInt32(new byte[] { x, 0x00, 0x00, 0x00 }, 0);
        //int Y = System.BitConverter.ToInt32(new byte[] { y, 0x00, 0x00, 0x00 }, 0);
        //int Z = System.BitConverter.ToInt32(new byte[] { z, 0x00, 0x00, 0x00 }, 0);
        //Vector3 acceleration = new Vector3(X, Y, Z);
        //OnAcceleration(acceleration);
        //Debug.Log("acceleration     " + acceleration + "\r\n");
    }

    public void OnDestroy()
    {
        MessageCenter.RemoveMsgListener((short)GameModule.Dir, PrepareHandler);
        //MessageCenter.RemoveMsgListener(1, PrepareRealButton01Action);
        //MessageCenter.RemoveMsgListener(2, PrepareSubColorAction);
        //MessageCenter.RemoveMsgListener(4, PrepareMotor02Action);
        //MessageCenter.RemoveMsgListener(4, PrepareMotor01Action);
        //MessageCenter.RemoveMsgListener(5, PrepareAxisAccelerationAction);
    }
}
