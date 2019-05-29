using DG.Tweening;
using LitJson;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuitarFinalTest : MonoBehaviour
{
    public string musicEventID;
    public string animatorEventID;

    ///对话框
    public GameObject guideRect;
    public Text guideFrame;
    public Text tip;
    public Text succeedCount;
    public Button backBtn;
    //自身音频
    public AudioSource musicAudio;
    public Animator guitarAnim;

    /// <summary>
    /// 吉他手身上的聚光灯
    /// </summary>
    public Transform spotLight;

    /// <summary>
    /// 下一个练习对象
    /// </summary>
    public GameObject exampleObj;

    public List<Transform> audienceObj = new List<Transform>();

    //吉他手的动画
    public List<Animator> leadAnimList = new List<Animator>();

    //练习需通过的次数
    public int succeedCountNum;
    //获得各个区域分数的个数
    private int m_ACount = 0;
    private int m_BCount = 0;
    private int m_CCount = 0;
    private int m_DCount = 0;
    //共有节点个数
    private int m_NodeCount = 0;

    //扫弦幅度
    public int lastStrumRange = 0;
    public string strumDir = "";
    //玩家当前触发的观众
    public string lastAudience = "";

    //控制结算面板
    public ScoreUIMgr m_ScoreUIMgr;
    //当前练习引导是否结束
    private bool currGuideIsOver;
    //全部练习是否结束
    private bool isPracticeEnd = false;

    public TextAsset guide1;
    public TextAsset guide2;
    public TextAsset guide3;
    public TextAsset guide4;
    private List<string> guide1List = new List<string>();
    private List<string> guide2List = new List<string>();
    private List<string> guide3List = new List<string>();
    private List<string> guide4List = new List<string>();

    public Text g_Key;
    public Text g_Chord;

    private void OnEnable()
    {
        guide1List = JsonMapper.ToObject<List<string>>(guide1.text);
        guide2List = JsonMapper.ToObject<List<string>>(guide2.text);
        guide3List = JsonMapper.ToObject<List<string>>(guide3.text);
        guide4List = JsonMapper.ToObject<List<string>>(guide4.text);

    }

    /// <summary>
    /// 控制练习时候的吉他动画
    /// </summary>
    /// <param name="kv"></param>
    private void OnGuitarHandler(KeyValuesUpdate kv)
    {
        if (!isPracticeEnd)
        {
            if (kv.Key == "Down")
                guitarAnim.SetTrigger("Down"); //guitarAnim.SetBool("IsTrigger", true);
            else if (kv.Key == "Up")
                guitarAnim.SetTrigger("Up");  //guitarAnim.SetBool("IsTrigger", false);
        }
    }

    /// <summary>
    /// 用于判断练习是否通过
    /// </summary>
    /// <param name="kv"></param>
    private void OnSucceedHandler(KeyValuesUpdate kv)
    {
        if (!isPracticeEnd)
        {
            succeedCountNum--;
            if (succeedCount != null)
                succeedCount.text = succeedCountNum + "次";
            FinishCurrPractice();
        }
    }

    /// <summary>
    /// 正式游戏中计分
    /// </summary>
    /// <param name="kv"></param>
    private void OnRankCountHandler(KeyValuesUpdate kv)
    {
        if (isPracticeEnd)
        {
            switch (kv.Key)
            {
                case "A":
                    m_ACount++;
                    break;
                case "B":
                    m_BCount++;
                    break;
                case "C":
                    m_CCount++;
                    break;
            }
        }
    }

    /// <summary>
    /// 吉他手动画触发
    /// </summary>
    /// <param name="kv"></param>
    private void OnLeadAnimHandler(KeyValuesUpdate kv)
    {
        leadAnimList[Int32.Parse(kv.Key)].SetTrigger(kv.Values.ToString());
    }

    private void Start()
    {
        currGuideIsOver = false;
        guideRect.SetActive(true);
        Koreographer.Instance.RegisterForEvents(animatorEventID, AnimatorEventHandler);
        MessageCenter.AddMsgListener((short)MusicMsgType.LeadAnim, OnLeadAnimHandler);
        MessageCenter.AddMsgListener((short)MusicMsgType.RankCount, OnRankCountHandler);
        MessageCenter.AddMsgListener((short)MusicMsgType.Succeed, OnSucceedHandler);
        MessageCenter.AddMsgListener((short)MusicMsgType.Guitar, OnGuitarHandler);
        MessageCenter.AddMsgListener(3, OnGuitarKeyHandler);
        MessageCenter.AddMsgListener(4, OnGuitarChordHandler);
        backBtn.onClick.AddListener(OnBackBtnCallback);
        Guide();
    }

    /// <summary>
    /// 蓝牙传输过来的吉他扫弦的数据
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
        g_Chord.text = num.ToString();
        if (strumDir == "Up" || strumDir == "Down")
            Guide();
    }

    /// <summary>
    /// 蓝牙传过来的吉他按键的数据
    /// </summary>
    /// <param name="kv"></param>
    private void OnGuitarKeyHandler(KeyValuesUpdate kv)
    {
        byte[] datas = kv.Values as byte[];
        int num = BitConverter.ToInt32(new byte[] { datas[0], datas[1], 0, 0 }, 0);
        if (num > 90 && num < 140)
            lastAudience = "A";
        else if (num > 60 && num <= 90)
            lastAudience = "B";
        else if (num > 30 && num <= 60)
            lastAudience = "C";
        else if (num >= 0 && num <= 30)
            lastAudience = "D";
        else
            lastAudience = "";
        g_Key.text = num.ToString();
    }

    private void OnBackBtnCallback()
    {
        SceneMgr.Instance.LoadScene(EnumSceneType.Home);
    }

    /// <summary>
    /// 引导框
    /// </summary>
    private void Guide()
    {
        switch (gameObject.name)
        {
            case "Guide1":
                if (guide1List.Count > 0)
                {
                    guideFrame.text = guide1List[0] + "\n\r\n\r" + "按回车键继续";
                    guide1List.RemoveAt(0);
                }
                else if (guide1List.Count < 1 && !currGuideIsOver)
                {
                    guideRect.SetActive(false);
                    EnterGuide("开始练习", true);
                }
                break;
            case "Guide2":
                if (guide2List.Count > 0)
                {
                    guideFrame.text = guide2List[0] + "\n\r\n\r" + "按回车键继续";
                    guide2List.RemoveAt(0);
                }
                else if (guide2List.Count < 1 && !currGuideIsOver)
                {
                    guideRect.SetActive(false);
                    EnterGuide("开始练习", true);
                }
                break;
            case "Guide3":
                if (guide3List.Count > 0)
                {
                    guideFrame.text = guide3List[0] + "\n\r\n\r" + "按回车键继续";
                    guide3List.RemoveAt(0);
                }
                else if (guide3List.Count < 1 && !currGuideIsOver)
                {
                    guideRect.SetActive(false);
                    EnterGuide("开始练习", true);
                }
                break;
            case "Guide4":
                isPracticeEnd = true;
                if (guide4List.Count > 0)
                {
                    guideFrame.text = guide4List[0] + "\n\r\n\r" + "按回车键继续";
                    guide4List.RemoveAt(0);
                }
                else if (guide4List.Count < 1 && !currGuideIsOver)
                {
                    guideRect.SetActive(false);
                    EnterGuide("演奏 开始！", false);
                }
                break;
        }
    }

    /// <summary>
    /// 播放引导提示  
    /// </summary>
    /// <param name="str"></param>
    /// <param name="isGuide"></param>
    private void EnterGuide(string str, bool isGuide)
    {
        if (isGuide)
        {
            musicAudio.loop = true;
            succeedCount.text = succeedCountNum + "次";
        }
        currGuideIsOver = true;
        guideFrame.text = "";
        tip.text = str;
        tip.DOFade(1, 0.1f);
        tip.transform.DOScale(5, 0.5f).OnComplete(() => { tip.DOFade(0, 1); }).SetAutoKill(true);
        //Invoke("StartGame", 2f);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    //void StartGame()
    {
        yield return new WaitForSeconds(2f);
        transform.GetComponent<SimpleMusicPlayer>().Play();
        //transform.GetComponent<Koreographer>();
    }

    private void Update()
    {
        AtWill();

        if (Input.GetKeyDown(KeyCode.Return))
            Guide();
    }

    /// <summary>
    /// 随时可以按下按键以及扫弦,播放对应吉他手的动画
    /// </summary>
    private void AtWill()
    {
        if (Input.GetKey(KeyCode.Q) || lastAudience == "A")
        {
            spotLight.transform.position = new Vector3(-10f,10f,-15.5f);
            if (Input.GetKeyDown(KeyCode.UpArrow) || strumDir == "Up")
                leadAnimList[0].SetTrigger("Up");
            if (Input.GetKeyDown(KeyCode.DownArrow) || strumDir == "Down")
                leadAnimList[0].SetTrigger("Down");
        }
        if (Input.GetKey(KeyCode.W) || lastAudience == "B")
        {
            spotLight.transform.position = new Vector3(-10f, 10f, -17.5f);
            if (Input.GetKeyDown(KeyCode.UpArrow) || strumDir == "Up")
                leadAnimList[1].SetTrigger("Up");
            if (Input.GetKeyDown(KeyCode.DownArrow) || strumDir == "Down")
                leadAnimList[1].SetTrigger("Down");
        }
        if (Input.GetKey(KeyCode.E) || lastAudience == "C")
        {
            spotLight.transform.position = new Vector3(-10f, 10f, -20f);
            if (Input.GetKeyDown(KeyCode.UpArrow) || strumDir == "Up")
                leadAnimList[2].SetTrigger("Up");
            if (Input.GetKeyDown(KeyCode.DownArrow) || strumDir == "Down")
                leadAnimList[2].SetTrigger("Down");
        }
        if (Input.GetKey(KeyCode.R) || lastAudience == "D")
        {
            spotLight.transform.position = new Vector3(-10f, 10f, -22.5f);
            if (Input.GetKeyDown(KeyCode.UpArrow) || strumDir == "Up")
                leadAnimList[3].SetTrigger("Up");
            if (Input.GetKeyDown(KeyCode.DownArrow) || strumDir == "Down")
                leadAnimList[3].SetTrigger("Down");
        }
    }

    /// <summary>
    /// 前奏动画触发
    /// </summary>
    /// <param name="koreoEvent"></param>
    private void AnimatorEventHandler(KoreographyEvent koreoEvent)
    {
        string[] strs = koreoEvent.GetTextValue().Split('_');
        if (isPracticeEnd)
        {
            if (koreoEvent.GetTextValue() == "Over" && m_ScoreUIMgr != null)
            {
                m_ScoreUIMgr.gameObject.SetActive(true);
                m_ScoreUIMgr.SetScoreUI(m_ACount, m_BCount, m_CCount, m_DCount, m_NodeCount);
                return;
            }
            else
            {
                m_NodeCount++;
            }
        }

        if (strs[1] != null)
            CreateAudience(Int32.Parse(strs[0]), strs[1], strs[2]);
    }

    /// <summary>
    /// 创建观众
    /// </summary>
    /// <param name="index"></param>
    /// <param name="animWay"></param>
    /// <param name="eventIndex"></param>
    private void CreateAudience(int index, string animWay, string eventIndex)
    {
        GameObject obj = ObjectPoolCtrl.Instance.Spawn("Audience") as GameObject;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        AudienceItem item = obj.GetComponent<AudienceItem>();
        item.MusicEventID = musicEventID;
        item.EventIndex = eventIndex;
        switch (index)
        {
            case 0:
                obj.transform.SetParent(audienceObj[0], false);
                break;
            case 1:
                obj.transform.SetParent(audienceObj[1], false);
                break;
            case 2:
                obj.transform.SetParent(audienceObj[2], false);
                break;
            case 3:
                obj.transform.SetParent(audienceObj[3], false);
                break;
        }
        item.AudienceAnimWay(animWay);
    }

    /// <summary>
    /// 判断当前练习是否结束
    /// </summary>
    private void FinishCurrPractice()
    {
        if (succeedCountNum <= 0)
        {
            MessageCenter.Dispatcher((short)MusicMsgType.Finish, null);
            Koreographer.Instance.ClearEventRegister();
            succeedCount.text = "";
            tip.text = "5秒后进入下一个练习";
            tip.DOFade(1, 0.1f);
            tip.transform.DOScale(5, 1f).OnComplete(() => { tip.DOFade(0, 1); }).SetAutoKill(true);
            StartCoroutine(NextOne()); //Invoke("NextOne", 5f);
        }
    }

    /// <summary>
    /// 进入下一个
    /// </summary>
    IEnumerator NextOne()
    //private void NextOne()
    {
        yield return new WaitForSeconds(5f);
        ObjectPoolCtrl.Instance.ClearAppointPool("Audience");

        foreach (Transform item in audienceObj)
        {
            int num = item.childCount;
            for (int i = 0; i < num; i++)
            {
                Destroy(item.GetChild(i).gameObject);
            }
        }
        MessageCenter.ClearAllMsgListener();
        Destroy(gameObject);
        if (exampleObj != null)
            exampleObj.SetActive(true);

        //ObjectPoolCtrl.Instance.UnSpawnAppointPool("Audience");
    }

    private void OnDestroy()
    {

    }
}

/// <summary>
/// 对话框内容
/// </summary>
public class GuideInfo
{
    //对话框内容
    public string tip;
    //按键(品)
    public string key;
    //扫弦
    public string chord;
}

/// <summary>
/// 自定义事件类型
/// </summary>
public enum MusicMsgType
{
    RankCount = 10,//用于计分
    LeadAnim,//用于控制吉他手动画
    Finish,//用于是否完成当前游戏或练习
    Succeed,//用于判断是否触发成功,无论是P还是G
    Guitar//控制练习吉他的动画
}
