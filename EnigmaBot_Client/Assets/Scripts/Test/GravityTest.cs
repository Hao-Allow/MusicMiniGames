using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GravityTest : MonoBehaviour
{
    public float speed = 5.0f;

    private Vector3 targetPos;

    private bool isOver = true;
    private bool isWin = false;

    private Vector3 moveTowardPosition;

    /// <summary>
    /// 人物的刚体
    /// </summary>
    public Rigidbody rigB;

    /// <summary>
    /// 电梯的刚体
    /// </summary>
    public Rigidbody gravityRigB;

    private Vector3 gravityChanged;

    public Transform targetEndPos;
    public Button resetBtn;
    public Text playResult;
    public Button backBtn;
    public Button autoBtn;

    private void OnEnable()
    {
        resetBtn.onClick.AddListener(OnResetCallback);
        backBtn.onClick.AddListener(OnBackBtnCallback);
        autoBtn.onClick.AddListener(OnAutoBtnCallback);
    }

    private void OnAutoBtnCallback()
    {
        isWin = true;
    }

    private void OnBackBtnCallback()
    {
        SceneMgr.Instance.LoadScene(EnumSceneType.Home);
    }

    private void OnResetCallback()
    {
        SceneMgr.Instance.LoadScene(EnumSceneType.Gravity);
    }

    void Start()
    {
        Input.gyro.enabled = true;
    }

    Vector3 playerDir = Vector3.zero;
    Vector3 elevatorDir = Vector3.zero;


    void FixedUpdate()
    {
#if UNITY_EDITOR_WIN
        MouseMove();


#elif UNITY_ANDROID || UNITY_IPHONE
        gravityChanged = Physics.gravity;
        gravityChanged.y += Input.acceleration.y;
        Physics.gravity = gravityChanged;
        if (!isWin)
            MouseMove();

        TouchMove();
#endif

        if (Input.acceleration.x > 0.2f)
            playerDir.x = Input.acceleration.x;
        if (Input.acceleration.y > 0.2f)
            playerDir.y = Input.acceleration.y;

        rigB.AddForce(playerDir * 100);
        rigB.velocity = Vector3.zero;
        rigB.rotation = Quaternion.identity;



#if UNITY_ANDROID || UNITY_IPHONE
        //Debug.Log("X :    " + Input.acceleration.x + "    Y:   " + Input.acceleration.y);
        elevatorDir.y = Input.acceleration.y;
        gravityRigB.AddForce(elevatorDir * 50f);

        //Quaternion input = Input.gyro.attitude;
        //elevatorDir.y = input.x;
        //gravityRigB.AddForce(elevatorDir * 50f);
        //gravityRigB.MovePosition(gravityRigB.position + elevatorDir * 50f * Time.deltaTime);
#endif

        //MoveByVector3MoveTowards();

    }


    private void MouseMove()
    {

#if UNITY_EDITOR_WIN
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                targetPos = hitInfo.point;
                targetPos.z = transform.position.z;
                isOver = false;
            }
        }

        MoveToPos(targetPos);

#elif UNITY_ANDROID || UNITY_IPHONE
        if (targetEndPos.position != Vector3.zero)
        {
            //isOver = false;
            SeekTarget(targetEndPos.position);
        }
#endif
    }

    /// <summary>
    /// 寻找目标
    /// </summary>
    /// <param name="pos"></param>
    private void SeekTarget(Vector3 pos)
    {
        //if (!isOver)
        //{
        Vector3 dir = pos - transform.position;
        rigB.MovePosition(rigB.position + dir.normalized * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, pos) <= 0.5f)
        {
            Debug.Log("赢啦:   " + Vector3.Distance(transform.position, pos));
            isWin = true;
            //isOver = true;
            playResult.text = "Win";
            Invoke("Vanish", 3f);
        }
        //}
    }

    /// <summary>
    /// 玩家移动
    /// </summary>
    /// <param name="pos"></param>
    private void MoveToPos(Vector3 pos)
    {
        if (!isOver)
        {
            Vector3 dir = pos - transform.position;
            rigB.MovePosition(rigB.position + dir.normalized * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pos) <= 0.2f)
            {
                isOver = true;
            }
        }
    }

    private void Vanish()
    {
        playResult.text = "";
    }

    /// <summary>
    /// 使用Vector3的MoveTowards 直接进行位置更新
    /// </summary>
    private void MoveByVector3MoveTowards()
    {
        //1、获得当前位置
        Vector3 curenPosition = this.transform.position;
        //2、获得方向
        if (Input.GetMouseButtonDown(0))
        {
            moveTowardPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveTowardPosition.z = 0;
            isOver = false;
        }
        if (!isOver)
        {
            if (Vector3.Distance(curenPosition, moveTowardPosition) < 0.01f)
            {
                transform.position = moveTowardPosition;
                isOver = true;
            }
            else
            {
                //3、插值移动
                //距离就等于 间隔时间乘以速度即可
                float maxDistanceDelta = Time.deltaTime * speed;
                transform.position = Vector3.MoveTowards(curenPosition, moveTowardPosition, maxDistanceDelta);
            }
        }
    }


    /// <summary>
    /// 判断是否为单点触摸
    /// </summary>
    /// <returns></returns>
    public bool SingleTouch()
    {
        if (Input.touchCount == 1)
            return true;

        return false;
    }

    /// <summary>
    /// 手指触摸控制玩家移动
    /// </summary>
    private void TouchMove()
    {
        if (SingleTouch())
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    targetPos = hitInfo.point;
                    targetPos.z = transform.position.z;
                    isOver = false;
                }
            }
        }
        MoveToPos(targetPos);
    }

    private void OnDestroy()
    {
        resetBtn.onClick.RemoveListener(OnResetCallback);
        backBtn.onClick.RemoveListener(OnBackBtnCallback);
        autoBtn.onClick.RemoveListener(OnAutoBtnCallback);
    }

}
