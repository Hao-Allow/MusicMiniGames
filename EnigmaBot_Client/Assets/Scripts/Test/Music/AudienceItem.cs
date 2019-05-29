using SonicBloom.Koreo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceItem : MonoBehaviour
{
    /// <summary>
    /// 音频事件ID
    /// </summary>
    [HideInInspector]
    public string musicEventID;
    /// <summary>
    /// 销毁时间
    /// </summary>
    [HideInInspector]
    public float lifeTime = 2f;
    /// <summary>
    /// 当前的事件位置
    /// </summary>
    [HideInInspector]
    public string eventIndex;

    //扫弦幅度
    public int lastStrumRange = 0;
    public string strumDir = "";
    //玩家当前触发的观众
    public string lastAudience = "";

    public string MusicEventID
    {
        get { return musicEventID; }
        set { musicEventID = value; }
    }

    public string EventIndex
    {
        get { return eventIndex; }
        set { eventIndex = value; }
    }

    private void OnEnable()
    {
        ResetState();
        selfAnim = gameObject.GetComponent<Animator>();
    }

    void Start()
    {
        Koreographer.Instance.RegisterForEventsWithTime(musicEventID, MusicEventHandler);
        MessageCenter.AddMsgListener((short)MusicMsgType.Finish, FinishHandler);
        MessageCenter.AddMsgListener(3, OnGuitarKeyHandler);
        MessageCenter.AddMsgListener(4, OnGuitarChordHandler);
    }

    /// <summary>
    /// 蓝牙传输过来的扫弦数据
    /// </summary>
    /// <param name="kv"></param>
    private void OnGuitarChordHandler(KeyValuesUpdate kv)
    {
        byte[] datas = kv.Values as byte[];
        int num = BitConverter.ToInt32(new byte[] { datas[0], datas[1], datas[2], datas[3] }, 0);
        if (num != lastStrumRange)
        {
            int tmp = num - lastStrumRange;
            //滑动区域大于30才算数
            if (Mathf.Abs(tmp) >= 30)
            {
                if (num > lastStrumRange)
                    strumDir = "Up";
                else
                    strumDir = "Down";
            }
            else
            {
                strumDir = "";
            }
        }
        else
        {
            strumDir = "";
        }
        lastStrumRange = num;
    }

    /// <summary>
    /// 蓝牙传输过来的按键数据
    /// </summary>
    /// <param name="kv"></param>
    private void OnGuitarKeyHandler(KeyValuesUpdate kv)
    {
        byte[] datas = kv.Values as byte[];
        int num = BitConverter.ToInt32(new byte[] { datas[0], datas[1], 0, 0 }, 0);
        if (num >= 90 && num < 140)
            lastAudience = "A";
        else if (num >= 60 && num < 90)
            lastAudience = "B";
        else if (num >= 30 && num < 60)
            lastAudience = "C";
        else if (num >= 0 && num < 30)
            lastAudience = "D";
        else
            lastAudience = "";
    }

    /// <summary>
    /// 完成当前练习或游戏回收场景中存在的观众
    /// </summary>
    /// <param name="kv"></param>
    private void FinishHandler(KeyValuesUpdate kv)
    {
        if (gameObject.activeSelf)
        {
            if (selfAnim != null)
                selfAnim.SetTrigger("Miss");
            if (lifeTime > 0)
                StartCoroutine(RecycleTime(lifeTime)); //Invoke("RecycleTime", lifeTime);
        }
    }

    /// <summary>
    /// 重置状态
    /// </summary>
    private void ResetState()
    {
        isSucceed = false; isPerfect = false; isGood = false; guitarAnimIsTrigger = false;
        goodOne = true; perfectOne = true; ordinaryOne = true;
    }

    /// <summary>
    /// 创建出来的观众播放哪种动画
    /// </summary>
    /// <param name="animWay"></param>
    public void AudienceAnimWay(string animWay)
    {
        switch (animWay)
        {
            case "0":
                selfAnim.SetTrigger("Give0");
                break;
            case "1":
                selfAnim.SetTrigger("Give1");
                break;
            case "2":
                selfAnim.SetTrigger("Give2");
                break;
        }
    }

    /// <summary>
    /// 节点判断触发
    /// </summary>
    /// <param name="koreoEvent"></param>
    /// <param name="sampleTime"></param>
    /// <param name="sampleDelta"></param>
    /// <param name="deltaSlice"></param>
    private void MusicEventHandler(KoreographyEvent koreoEvent, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        string[] strs = koreoEvent.GetTextValue().Split('_');

        if (strs[2] == eventIndex)
        {
            if (!guitarAnimIsTrigger)
            {
                guitarAnimIsTrigger = true;
                MessageCenter.Dispatcher((short)MusicMsgType.Guitar, new KeyValuesUpdate(strs[1], null));
            }

            if (strs[0] == "0" && (Input.GetKey(KeyCode.Q) || lastAudience == "A"))
                JudgeCommon(strs, koreoEvent, sampleTime);

            if (strs[0] == "1" && (Input.GetKey(KeyCode.W) || lastAudience == "B"))
                JudgeCommon(strs, koreoEvent, sampleTime);

            if (strs[0] == "2" && (Input.GetKey(KeyCode.E) || lastAudience == "C"))
                JudgeCommon(strs, koreoEvent, sampleTime);

            if (strs[0] == "3" && (Input.GetKey(KeyCode.R) || lastAudience == "D"))
                JudgeCommon(strs, koreoEvent, sampleTime);

            int single = (koreoEvent.EndSample - koreoEvent.StartSample) / 8;
            int point = koreoEvent.EndSample - single;
            if (sampleTime > point && ordinaryOne && !isSucceed)
            {
                ordinaryOne = false;
                selfAnim.SetTrigger("Miss");
                MessageCenter.Dispatcher((short)MusicMsgType.RankCount, new KeyValuesUpdate("C", null));
                if (lifeTime > 0)
                    StartCoroutine(RecycleTime(lifeTime)); //Invoke("RecycleTime", lifeTime);
                                                           //if (isPracticeEnd)
                                                           //    m_CCount++;
            }
        }
    }

    /// <summary>
    /// 判定公共的部分
    /// </summary>
    private void JudgeCommon(string[] strs, KoreographyEvent koreoEvent, int sampleTime)
    {
        if (strs[1] == "Up" && (Input.GetKeyDown(KeyCode.UpArrow) || strumDir == "Up" || strumDir == "Down"))
        {
            MessageCenter.Dispatcher((short)MusicMsgType.LeadAnim, new KeyValuesUpdate(strs[0], "Up"));
            GameJudge(koreoEvent, sampleTime);
        }
        if (strs[1] == "Down" && (Input.GetKeyDown(KeyCode.DownArrow) || strumDir == "Down" || strumDir == "Up"))
        {
            MessageCenter.Dispatcher((short)MusicMsgType.LeadAnim, new KeyValuesUpdate(strs[0], "Down"));
            GameJudge(koreoEvent, sampleTime);
        }
    }

    /// <summary>
    /// 判定
    /// </summary>
    private void GameJudge(KoreographyEvent koreoEvent, int sampleTime)
    {
        if (!isSucceed && sampleTime < koreoEvent.EndSample)
        {
            int single = (koreoEvent.EndSample - koreoEvent.StartSample) / 8;

            int pStart = koreoEvent.StartSample + single * 2;
            int pEnd = koreoEvent.EndSample - single * 2;

            int gStart = koreoEvent.StartSample + single;
            int gEnd = koreoEvent.EndSample - single;

            int point = koreoEvent.StartSample + single;
            if (sampleTime >= pStart && sampleTime <= pEnd && perfectOne)
            {
                isPerfect = true; perfectOne = false; isSucceed = true;
                if (selfAnim != null)
                    selfAnim.SetTrigger("Exact");
                MessageCenter.Dispatcher((short)MusicMsgType.Succeed, null);
                MessageCenter.Dispatcher((short)MusicMsgType.RankCount, new KeyValuesUpdate("A", null));
                if (lifeTime > 0)
                    StartCoroutine(RecycleTime(lifeTime)); //Invoke("RecycleTime", lifeTime);
            }
            if (sampleTime >= gStart && sampleTime <= gEnd && !isPerfect && goodOne)
            {
                isGood = true; goodOne = false; isSucceed = true;
                if (selfAnim != null)
                    selfAnim.SetTrigger("Good");
                MessageCenter.Dispatcher((short)MusicMsgType.Succeed, null);
                MessageCenter.Dispatcher((short)MusicMsgType.RankCount, new KeyValuesUpdate("B", null));
                if (lifeTime > 0)
                    StartCoroutine(RecycleTime(lifeTime)); //Invoke("RecycleTime", lifeTime);
            }
            if (sampleTime <= point && !isPerfect && !isGood && ordinaryOne)
            {
                ordinaryOne = false; isSucceed = true;
                if (selfAnim != null)
                    selfAnim.SetTrigger("Miss");
                MessageCenter.Dispatcher((short)MusicMsgType.RankCount, new KeyValuesUpdate("C", null));
                if (lifeTime > 0)
                    StartCoroutine(RecycleTime(lifeTime)); //Invoke("RecycleTime", lifeTime);
            }
        }
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

    void Update()
    {

    }

    private void OnDisable()
    {
        musicEventID = null;
        eventIndex = null;
    }

    private void OnDestroy()
    {
        //if (gameObject.activeSelf)
        //    Koreographer.Instance.ClearEventRegister();

        //MessageCenter.ClearAllMsgListener();
    }

    private Animator selfAnim;
    //用于只做一次触发,不加标志变量会多次触发
    private bool isSucceed;
    private bool isPerfect;
    private bool isGood;
    bool goodOne = true;
    bool perfectOne = true;
    bool ordinaryOne = true;
    bool guitarAnimIsTrigger = false;
}
