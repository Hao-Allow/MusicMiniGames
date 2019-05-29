using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AAA : MonoBehaviour
{
    //private const float lowPassFilterFactor = 0.2f;
    //protected void Start()
    //{
    //    //设置设备陀螺仪的开启/关闭状态，使用陀螺仪功能必须设置为 true  
    //    Input.gyro.enabled = true;
    //    //获取设备重力加速度向量  
    //    Vector3 deviceGravity = Input.gyro.gravity;
    //    //设备的旋转速度，返回结果为x，y，z轴的旋转速度，单位为（弧度/秒）  
    //    Vector3 rotationVelocity = Input.gyro.rotationRate;
    //    //获取更加精确的旋转  
    //    Vector3 rotationVelocity2 = Input.gyro.rotationRateUnbiased;
    //    //设置陀螺仪的更新检索时间，即隔 0.1秒更新一次  
    //    Input.gyro.updateInterval = 0.1f;
    //    //获取移除重力加速度后设备的加速度  
    //    Vector3 acceleration = Input.gyro.userAcceleration;
    //}

    //protected void Update()
    //{
    //    //Input.gyro.attitude 返回值为 Quaternion类型，即设备旋转欧拉角  
    //    transform.rotation = Quaternion.Slerp(transform.rotation, Input.gyro.attitude, lowPassFilterFactor);
    //}

    //public Button backBtn;
    //private void OnEnable()
    //{
    //    backBtn.onClick.AddListener(OnBackBtnCallback);
    //}

    //private void OnBackBtnCallback()
    //{
    //    SceneMgr.Instance.LoadScene(EnumSceneType.Home);
    //}

    //private void OnDestroy()
    //{
    //    backBtn.onClick.RemoveListener(OnBackBtnCallback);
    //}

    public GameObject target;     //被移动的对象   
    public float maxOffsetX = 10f;    // 最大倾斜角 35     
    public float maxOffsetY = 10f;    // 最大倾斜角 35    
    public float maxOffsetZ = 1f;    // 最大倾斜角 35    
    public float posFactor = 0f;    // 位移的系数    
    public float lerpFactor = 5f;    // 差值系数    
    Vector3 m_MobileOrientation;   //手机陀螺仪变化的值    
    Vector3 m_targetTransform;
    Vector3 m_targetPos;

    void Awake()
    {
        m_targetTransform = Vector3.zero;
        m_targetPos = Vector3.zero;
        Input.gyro.enabled = true;   
    }

    void Start()
    {
        if (target == null)
        { target = gameObject; }
    }

    ///运用手机的X轴的变化来改变target对象的Y轴旋转，X轴移动； 
    ///Y轴的变化来改变target对象的X轴旋转，Y轴移动；Z轴的变化只去改变target对象的Z轴位置
    void LateUpdate()
    {
        if (target == null) return;

        m_MobileOrientation = Input.acceleration;

        m_targetTransform.x = Mathf.Lerp(m_targetTransform.x, m_MobileOrientation.y * maxOffsetY, Time.deltaTime * lerpFactor);
        m_targetTransform.y = Mathf.Lerp(m_targetTransform.y, -m_MobileOrientation.x * maxOffsetX, Time.deltaTime * lerpFactor);
        m_targetTransform.z = Mathf.Lerp(m_targetTransform.z, (-m_MobileOrientation.x) * maxOffsetZ, Time.deltaTime * lerpFactor);

        m_targetPos.x = m_targetTransform.x;
        m_targetPos.y = m_targetTransform.y;
        m_targetPos.z = m_targetTransform.z;

        target.transform.localPosition = Vector3.Lerp(target.transform.localPosition, m_targetPos * posFactor, Time.deltaTime * lerpFactor);

        //target.transform.localRotation = Quaternion.Euler(m_targetTransform);

        //target.transform.localRotation = Quaternion.Lerp(target.transform.localRotation, Quaternion.Euler(m_targetTransform), Time.deltaTime * lerpFactor);   
    }







}

