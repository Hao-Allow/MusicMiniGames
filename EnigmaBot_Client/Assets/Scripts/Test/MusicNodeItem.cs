using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNodeItem : MonoBehaviour
{
    private float speed = 10f;

    [HideInInspector]
    public double tirggerTime;

    [HideInInspector]
    public float currTime;

    [HideInInspector]
    public string nodeDir;

    private Vector2 startPos;
    private Vector2 endPos;

    private string dir;
    private bool result;

    private Vector2 startFingerPos;
    private bool startPosFlag;
    private Vector2 nowFingerPos;
    private float xMoveDistance;
    private float yMoveDistance;

    private bool isPrefect = false;

    private void Start()
    {
        gameObject.transform.localScale = new Vector3(45, 15, 1);
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        currTime += Time.deltaTime;

        if (gameObject.activeSelf && Time.timeScale != 0)
            transform.Translate(Vector3.down * MusicGameTest.Instance.speed);

#if UNITY_EDITOR_WIN
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if ((currTime * 1000) >= (tirggerTime - MusicGameTest.Instance.goodArea) && (currTime * 1000) <= (tirggerTime + MusicGameTest.Instance.goodArea))
                dir = "leftDir";
            if ((currTime * 1000) >= (tirggerTime - MusicGameTest.Instance.perfectArea) && (currTime * 1000) <= (tirggerTime + MusicGameTest.Instance.perfectArea))
            {
                dir = "leftDir";
                isPrefect = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if ((currTime * 1000) >= (tirggerTime - MusicGameTest.Instance.goodArea) && (currTime * 1000) <= (tirggerTime + MusicGameTest.Instance.goodArea))
                dir = "rightDir";
            if ((currTime * 1000) >= (tirggerTime - MusicGameTest.Instance.perfectArea) && (currTime * 1000) <= (tirggerTime + MusicGameTest.Instance.perfectArea))
            {
                dir = "rightDir";
                isPrefect = true;
            }
        }

        if ((currTime * 1000) > (tirggerTime + MusicGameTest.Instance.goodArea))
        {
            if (dir != "" && dir == nodeDir && tirggerTime != 0)
            {
                result = true;
                tirggerTime = 0f;
                if (isPrefect)
                    MessageCenter.Dispatcher((short)GameModule.Music, new KeyValuesUpdate("1", "Prefect"));
                else
                    MessageCenter.Dispatcher((short)GameModule.Music, new KeyValuesUpdate("1", "Good"));

                Destroy(gameObject);
            }
        }

        if ((currTime * 1000) > (tirggerTime + MusicGameTest.Instance.missArea) && !result)
        {
            MessageCenter.Dispatcher((short)GameModule.Music, new KeyValuesUpdate("0", "Miss"));
            result = true;
        }

#elif UNITY_ANDROID || UNITY_IPHONE
         if (SingleTouch() && MoveSingleTouch())
        {
            if ((currTime * 1000) >= (tirggerTime - MusicGameTest.Instance.goodArea) && (currTime * 1000) <= (tirggerTime + MusicGameTest.Instance.goodArea))
                dir = MoveSingleTouchDir();
            if ((currTime * 1000) >= (tirggerTime - MusicGameTest.Instance.perfectArea) && (currTime * 1000) <= (tirggerTime + MusicGameTest.Instance.perfectArea))
            {
                dir = MoveSingleTouchDir();
                isPrefect = true;
            }
        }

        if ((currTime * 1000) > (tirggerTime + MusicGameTest.Instance.goodArea))
        {
            if (dir != "" && dir == nodeDir && tirggerTime != 0)
            {
                result = true;
                tirggerTime = 0f;
                if (isPrefect)
                    MessageCenter.Dispatcher((short)GameModule.Music, new KeyValuesUpdate("1", "Prefect"));
                else
                    MessageCenter.Dispatcher((short)GameModule.Music, new KeyValuesUpdate("1", "Good"));

                Destroy(gameObject);
            }
        }

        if ((currTime * 1000) > (tirggerTime + MusicGameTest.Instance.missArea) && !result)
        {
            MessageCenter.Dispatcher((short)GameModule.Music, new KeyValuesUpdate("0", "Miss"));
            result = true;
        }
#endif
    }

    /// <summary>
    /// 判断是否单指触摸
    /// </summary>
    /// <returns></returns>
    private bool SingleTouch()
    {
        if (Input.touchCount == 1)
            return true;

        return false;
    }

    /// <summary>
    /// 判断单指触摸状态下是否是移动
    /// </summary>
    /// <returns></returns>
    private bool MoveSingleTouch()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Moved)
            return true;

        return false;
    }

    /// <summary>
    /// 获取手持触摸移动的方向
    /// </summary>
    /// <returns></returns>
    private string MoveSingleTouchDir()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began && startPosFlag == true)
        {
            ///开始触摸
            startFingerPos = Input.GetTouch(0).position;
            startPosFlag = false;
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            //结束触摸
            startPosFlag = true;
        }

        //当前位置
        nowFingerPos = Input.GetTouch(0).position;
        xMoveDistance = Mathf.Abs(nowFingerPos.x - startFingerPos.x);
        yMoveDistance = Mathf.Abs(nowFingerPos.y - startFingerPos.y);
        if (xMoveDistance > yMoveDistance)
        {
            if (nowFingerPos.x - startFingerPos.x > 0)
                dir = "rightDir";
            else
                dir = "leftDir";
        }
        else
        {
            if (nowFingerPos.y - nowFingerPos.y > 0)
                dir = "upDir";
            else
                dir = "downDir";
        }
        return dir;
    }

}
