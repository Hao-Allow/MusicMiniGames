using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BLEController : MonoSingleton<BLEController>
{
    public delegate void MessageAction(byte[] bytes);
    public static event MessageAction OnReceive;

    // ----------------------------------------------------------------- 
    // 更改这些以匹配您要连接的蓝牙设备:
    // ----------------------------------------------------------------- 
    // private string _FullUID = "713d****-503e-4c75-ba94-3148f18d941e"; // redbear module pattern 
    private string _FullUID = "0000****-0000-1000-8000-00805f9b34fb";     // BLE-CC41a module pattern 
    private string _serviceUUID = "FFE0";
    private string _readCharacteristicUUID = "FFE1";
    private string _writeCharacteristicUUID = "FFE1";
    private string deviceToConnectTo = "EnigmaBot-B";


    private bool _readFound = false;
    private bool _writeFound = false;
    private string _connectedID = null;


    private Dictionary<string, string> _peripheralList;
    private float _subscribingTimeout = 0f;

    private bool isConnected = false;
    private bool connecting;
    private float lastReceiveTime;
    private string stringReceive;

    void Start()
    {
        //DontDestroyOnLoad(gameObject);
        //BluetoothLEHardwareInterface.Initialize (true, false, () => {}, 
        //                              (error) => {} 
        //);       
        //Invoke ("scan", 1f);
        //connecting = false;

        //通常把app设置为centeral端，第一个参数表示是否把app设置为centeral端，第二个参数表示是否设置为peripheral端
        BluetoothLEHardwareInterface.Initialize(true, false,
            () => { Debug.Log("蓝牙初始化成功"); Invoke("ScanBluetooth", 0.1f); },
            (error) => { Debug.Log("蓝牙初始化失败：" + error); }
        );
        connecting = false;
    }

    void Update()
    {
        if (_readFound && _writeFound)
        {
            _readFound = false;
            _writeFound = false;
            _subscribingTimeout = 1.0f;//超时设定
            Debug.Log("已成功连接......");
        }
        //BluetoothLEHardwareInterface.DisconnectPeripheral()
        if (_subscribingTimeout > 0f)
        {
            _subscribingTimeout -= Time.deltaTime;
            if (_subscribingTimeout <= 0f)
            {
                _subscribingTimeout = 0f;
                //订阅设备
                BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(
                  _connectedID, FullUUID(_serviceUUID), FullUUID(_readCharacteristicUUID),
                  (deviceAddress, notification) =>
                  {
                      // Debug.Log("订阅通知： " + deviceAddress + "    >>   " + notification);
                  },
                  (deviceAddress2, characteristic, data) =>
                  {
                      //Debug.Log("订阅特征改变：  " + deviceAddress2 + "  >>  " + characteristic + "  >>　" + ASCIIEncoding.UTF8.GetString(data));
                      //Debug.Log(">>>>>>id: " + _connectedID);
                      if (deviceAddress2.CompareTo(_connectedID) == 0)
                      {
                          //Debug.Log(string.Format(">>>>>>data length: {0}", data.Length));
                          if (data.Length == 0)
                          {
                              // do nothing 
                              Debug.Log("The data length is 0");
                          }
                          else
                          {
                              //OnReceive(data);
                              //string s = ASCIIEncoding.UTF8.GetString(data);
                              //Debug.Log("Current data string:  " + s);
                              UnpackDispatcher(data);
                              //receiveText(s);
                          }
                      }
                  });
            }
        }
    }

    /// <summary>
    /// 将收到的包体数据进行解析,然后进行分发
    /// </summary>
    /// <param name="data"></param>
    private void UnpackDispatcher(byte[] data)
    {
        string messageData = "";
        for (int i = 0; i < data.Length; i++)
        {
            messageData += data[i] + "   ";
        }

        //Debug.Log("messageData     " + messageData);
        //Debug.Log("dataLength：   " + dataLength);
        if (data[0] == 0x55 && data[1] == 0xAA)
        {
            byte id = data[3];
            Debug.Log("dataID：   " + id + "    " + "messageData     " + messageData  + " \n\r");
            byte dataLength = data[2];
            byte[] realityData = new byte[dataLength];

            for (int i = 0; i < dataLength; i++)
            {
                realityData[i] = data[4 + i];
            }

            //TODO: 重新对数据包,改变结构,带有模块或类型,然后是消息id在是数据
            KeyValuesUpdate kv = new KeyValuesUpdate(null, realityData);
            MessageCenter.Dispatcher(id, kv);
        }
        else
        {
            Debug.Log("not 0x55 0xAA");
        }
    }

    /// <summary>
    /// 蓝牙初始化成功，开始扫描蓝牙设备，
    /// 第一个参数表示是否根据UUID来搜索指定蓝牙设备，null表示搜索所有蓝牙设备
    /// </summary>
    public void ScanBluetooth()
    {

        //第一次回调只会在第一次看到此设备时被调用 
        //这是因为它被添加到BluetoothDeviceScript中的列表 
        //之后只有第二次回调才会被调用，并且只有在
        //广告数据时才会被调用  

        Debug.Log("Starting scan \r\n");
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, (address, name) =>
        {
            AddPeripheral(name, address);
        }, (address, name, rssi, advertisingInfo) => { });

    }

    /// <summary>
    /// 添加外围设备
    /// </summary>
    /// <param name="name"></param>
    /// <param name="address"></param>
    public void AddPeripheral(string name, string address)
    {
        Debug.Log("Found " + name + " \r\n");
        if (_peripheralList == null)
            _peripheralList = new Dictionary<string, string>();

        if (!_peripheralList.ContainsKey(address) && !connecting)// && !connecting
        {
            if (name.Trim().ToLower() == deviceToConnectTo.Trim().ToLower())
            {
                //BluetoothLEHardwareInterface.StopScan (); 
                _peripheralList[address] = name;
                Debug.Log("Connecting to " + address + "   ");
                ConnectBluetooth(address);
            }
            else
            {
                if (!connecting)
                    Debug.Log("Not what we're looking for ");
            }
        }
        else
        {

            Debug.Log("No address found ");
        }
    }

    /// <summary>
    /// 连接蓝牙
    /// </summary>
    /// <param name="addr"></param>
    public void ConnectBluetooth(string addr)
    {
        Debug.Log("Connection to ..." + name + "with address: " + addr + ", in progress... \n");
        
        BluetoothLEHardwareInterface.ConnectToPeripheral(addr, (address) =>
        {
            Debug.Log("Succeed  Name : " + address);
        },
           (address, serviceUUID) =>
           {
               Debug.Log("address:   " + address + "           serviceUUID:   " + serviceUUID);
               connecting = true;
           },
           (address, serviceUUID, characteristicUUID) =>
           {
               // discovered characteristic 
               if (IsEqual(serviceUUID, _serviceUUID))
               {
                   _connectedID = address;
                   isConnected = true;

                   if (IsEqual(characteristicUUID, _readCharacteristicUUID))
                   {
                       Debug.Log("Read Characteristic");
                       _readFound = true;
                   }
                   if (IsEqual(characteristicUUID, _writeCharacteristicUUID))
                   {
                       Debug.Log("Write Characteristic");
                       _writeFound = true;
                   }
                   Debug.Log("Connected");

                   BluetoothLEHardwareInterface.StopScan();
               }
           }, (address) =>
           {
               //当设备断开连接时会调用它 
               //请注意，当上面调用断开连接时， 也会调用它。两个方法都调用相同的操作
               //这是为了向后兼容
               Debug.Log("断开连接");
               isConnected = false;
               connecting = false;
               if (_peripheralList != null)
                   _peripheralList.Clear();
               ScanBluetooth();
           });
    }

    void parseSource(string data)
    {
        string head = string.Format("{0}{1}{2}{3}", data[0], data[1], data[2], data[3]);
    }

    void receiveText(string s)
    {
        Debug.Log("Received: " + s + " \n");
        stringReceive = s;
        lastReceiveTime = Time.time;
    }

    /// <summary>
    /// PointFunction发送数据
    /// </summary>
    public void SendData()
    {
        //OnReceive(new byte[] { 0x55, 0xAA, 0x01, 0x01, 0x01, 0x03 });
        SendDataBluetooth("123");
    }

    /// <summary>
    /// 通过蓝牙向硬件发送数据
    /// </summary>
    /// <param name="sData"></param>
    public void SendDataBluetooth(string sData)
    {
        if (sData.Length > 0)
        {
            byte[] bytes = ASCIIEncoding.UTF8.GetBytes(sData);
            if (bytes.Length > 0)
                SendBytesBluetooth(bytes);
        }
    }

    public void SendBytesBluetooth(byte[] data)
    {
        //为BLE UART将数据包分割为20个字节块，也可以使用长写，但是字节缓冲区会有开销
        // 发送数据，不超过20byte的字节数组,否则在Android 很多机型上会发送不出去
        byte[] toSend = new byte[20];
        byte[] nextPacket = new byte[1];
        bool needsNextPacket = data.Length > 20;
        if (needsNextPacket)
        {
            nextPacket = new byte[data.Length - 20];
            for (int i = 0; i < data.Length; i++)
            {
                if (i < 20)
                    toSend[i] = data[i];
                else
                    nextPacket[i - 20] = data[i];
            }
        }
        else
        {
            toSend = data;
        }
        Debug.Log(string.Format("data length: {0} uuid {1}", data.Length.ToString(), FullUUID(_writeCharacteristicUUID)));
        BluetoothLEHardwareInterface.WriteCharacteristic(_connectedID, FullUUID(_serviceUUID), FullUUID(_writeCharacteristicUUID),
           toSend, toSend.Length, false, (characteristicUUID) =>
           {
               Debug.Log(">>>>>>Write succeeded");
               if (needsNextPacket)
                   SendBytesBluetooth(nextPacket);
           }
        );


        //Debug.Log(string.Format(">>>>>>data length: {0} uuid {1}", data.Length.ToString(), FullUUID(_writeCharacteristicUUID)));
        //BluetoothLEHardwareInterface.WriteCharacteristic(_connectedID, FullUUID(_serviceUUID), FullUUID(_writeCharacteristicUUID),
        //   data, data.Length, false, (characteristicUUID) =>
        //   {
        //       Debug.Log(">>>>>>Write succeeded");
        //   }
        //);
    }




    // ------------------------------------------------------- 
    // 用于处理替换连接字符串的辅助函数 
    // ------------------------------------------------------- 
    string FullUUID(string uuid)
    {
        return _FullUID.Replace("****", uuid);
    }

    bool IsEqual(string uuid1, string uuid2)
    {
        if (uuid1.Length == 4)
            uuid1 = FullUUID(uuid1);
        if (uuid2.Length == 4)
            uuid2 = FullUUID(uuid2);

        return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
    }

    private void OnDestroy()
    {
        if (_peripheralList != null)
            _peripheralList.Clear();
    }
}
