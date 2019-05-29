using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;
using System.IO;

public enum ExpressionType
{
    Good,
    Perfect,
    Miss
}

public class MusicGameTest : MonoSingleton<MusicGameTest>
{
    public Button backBtn;
    public Transform mountPoint;
    public Text score;
    public Text currTime;
    public Text expression;
    public Text gameOverTip;

    public float speed = 10f;
    [Tooltip("Good得分判定区域")]
    public int goodArea = 300;
    [Tooltip("Perfect得分判定区域")]
    public int perfectArea = 200;
    [Tooltip("Miss判定区域")]
    public int missArea = 300;
    public int createNodeTime = 1500;
    private float startTime = 0f;
    private float currGameTime = 0f;
    private bool isOver = true;

    private float perfectGrade;

    private List<MusicTimeNode> musicTimeNodeList;
    private List<MusicTimeNode> createList;

    private StreamWriter infoWriter;
    private List<MusicTimeNode> saveMusicTimeNodeList = new List<MusicTimeNode>();

    private void OnEnable()
    {
        backBtn.onClick.AddListener(OnBackBtnCallback);
        MessageCenter.AddMsgListener((short)GameModule.Music, MusicExpressionHandler);

        //解析json文件
        TextAsset textAsset = ResourcesMgr.Instance.LoadAsset<TextAsset>(EnumResType.TextAsset, "MusicTimeNodeInfo", false);
        musicTimeNodeList = JsonMapper.ToObject<List<MusicTimeNode>>(textAsset.text);
        createList = JsonMapper.ToObject<List<MusicTimeNode>>(textAsset.text);
        perfectGrade = (100 / musicTimeNodeList.Count);
    }

    /// <summary>
    /// 监听游戏回调
    /// </summary>
    /// <param name="kv"></param>
    private void MusicExpressionHandler(KeyValuesUpdate kv)
    {

        GameGrade(kv.Values.ToString());
        if (expression != null)
        {
            expression.text = kv.Values.ToString();
            Tweener tr = expression.transform.DOScale(2, 0.3f).OnComplete(() =>
            {
                expression.text = "";
                expression.transform.localScale = new Vector3(1, 1, 1);
            }).SetAutoKill(true);
        }
    }

    /// <summary>
    /// 游戏结束的表现
    /// </summary>
    private void GameOverSumGrade()
    {
        if (score != null && expression != null)
        {
            float sum = float.Parse(score.text);
            string tmp = "";
            if (sum < 60)
                tmp = "还需努力";
            else if (sum >= 60 && sum < 80)
                tmp = "平凡";
            else if (sum >= 80 && sum < 90)
                tmp = "优秀";
            else if (sum >= 90)
                tmp = "天才";

            expression.text = tmp;
        }
    }

    /// <summary>
    /// 计算玩家得分
    /// </summary>
    /// <param name="value"></param>
    private void GameGrade(string value)
    {
        float currGrade = 0f;
        switch (value)
        {
            case "Perfect":
                currGrade = perfectGrade;
                break;
            case "Good":
                AudioMgr.Instance.Audio_Volume = 1f;
                currGrade = perfectGrade * 0.8f;
                break;
            case "Miss":
                AudioMgr.Instance.Audio_Volume = 0.5f;
                currGrade = perfectGrade * 0.5f;
                break;
        }
        if (score != null)
            score.text = (float.Parse(score.text) + currGrade).ToString();
    }


    private void OnBackBtnCallback()
    {
        gameOverTip.gameObject.SetActive(false);
        expression.text = "";
        SceneMgr.Instance.LoadScene(EnumSceneType.Home);
    }

    void Start()
    {
        startTime = Time.time;
        AudioMgr.Instance.PlayAudioByName("Battle");
    }

    void Update()
    {
        currGameTime = Time.time - startTime;
        currTime.text = currGameTime.ToString();

        if (createList.Count > 0)
        {
            MusicTimeNode node = createList[0];
            if ((currGameTime * 1000) >= (node.tirggerTime - createNodeTime))
            {
                GameObject obj = ResourcesMgr.Instance.LoadAsset(EnumResType.UIPrefab, node.fallDir, false);
                MusicNodeItem item = obj.GetComponent<MusicNodeItem>();
                item.currTime = currGameTime;
                item.nodeDir = node.fallDir;
                item.tirggerTime = node.tirggerTime;
                Utility.SetParentAndReset(obj, mountPoint);
                createList.Remove(node);
            }
        }

        if (!AudioMgr.Instance.GetAudioSorce.isPlaying && isOver)
        {
            GameOverSumGrade();
            gameOverTip.gameObject.SetActive(true);
            Invoke("OnBackBtnCallback", 5f);
            isOver = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
            SaveMusicTimeNode("leftDir", Mathf.Round(currGameTime * 1000));

        if (Input.GetKeyDown(KeyCode.D))
            SaveMusicTimeNode("rightDir", Mathf.Round(currGameTime * 1000));

        if (Input.GetKeyDown(KeyCode.Space))
            SaveToFile();
    }

    /// <summary>
    /// 保存音乐时间节点
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="time"></param>
    private void SaveMusicTimeNode(string dir, double time)
    {
        MusicTimeNode node = new MusicTimeNode();
        node.fallDir = dir;
        node.tirggerTime = time;
        saveMusicTimeNodeList.Add(node);
    }

    /// <summary>
    /// 最终存入文件
    /// </summary>
    private void SaveToFile()
    {
        string str = JsonMapper.ToJson(saveMusicTimeNodeList);
        FileInfo nodeInfo = new FileInfo(Application.dataPath + "/Resources/TextAsset/MusicTimeNodeInfo.json");
        if (nodeInfo.Exists)
            nodeInfo.Delete();

        if (!nodeInfo.Exists)
            infoWriter = nodeInfo.CreateText();
        else
            infoWriter = nodeInfo.AppendText();

        infoWriter.Write(str);
        infoWriter.Flush();
        infoWriter.Dispose();
        infoWriter.Close();
        nodeInfo.Refresh();
    }

    private void OnDestroy()
    {
        AudioMgr.Instance.Discard();
        backBtn.onClick.RemoveListener(OnBackBtnCallback);
    }


}

/// <summary>
/// 音乐时间节点信息
/// </summary>
public class MusicTimeNode
{
    public string fallDir;
    public double tirggerTime;

    public new void ToString()
    {
        Debug.Log("fallDir:   " + fallDir + "   tirggerTime:   " + tirggerTime);
    }
}
