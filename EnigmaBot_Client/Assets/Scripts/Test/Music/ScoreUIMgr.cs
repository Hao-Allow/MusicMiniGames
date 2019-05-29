using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreUIMgr : MonoBehaviour
{
    public Text m_Result;
    public Text m_AScore;
    public Text m_BScore;
    public Text m_CScore;
    public Text m_DScore;

    public Image m_Grade;

    public Button m_LeaveBtn;

    ///玩家游戏结果
    private string m_Appraise;

    private void Start()
    {
        m_LeaveBtn.onClick.AddListener(OnLeaveBtnCallback);
    }

    private void OnLeaveBtnCallback()
    {
        gameObject.SetActive(false);
        SceneMgr.Instance.LoadScene(EnumSceneType.Home);
    }

    /// <summary>
    /// 给结算面板赋值
    /// </summary>
    /// <param name="aCount"></param>
    /// <param name="bCount"></param>
    /// <param name="cCount"></param>
    /// <param name="dCount"></param>
    /// <param name="nodeCount"></param>
    public void SetScoreUI(int aCount, int bCount, int cCount, int dCount, int nodeCount)
    {
        m_AScore.text = "A: " + aCount;
        m_BScore.text = "B: " + bCount;
        m_CScore.text = "C: " + cCount;
        m_DScore.text = "D: " + dCount;

        int single = (100 / nodeCount);
        float score = (aCount * single + 0.8f * single * bCount + 0.6f * single * cCount + 0.5f * single * dCount);

        if (score < 60)
        {
            m_Appraise = "再接再厉";
            m_Result.color = Color.gray;
        }
        else if (score >= 60 && score < 80)
        {
            m_Appraise = "勉勉强强";
            m_Result.color = Color.blue;
        }
        else if (score >= 80 && score < 90)
        {
            m_Appraise = "优秀的节奏感";
            m_Result.color = new Color(255,200,0);
        }
        else if (score >= 90 && score <= 100)
        {
            m_Appraise = "灵魂同步";
            m_Result.color = new Color(33, 208, 210);
        }
        
        m_Grade.DOFillAmount((score / 100), 2f).OnComplete(() => {
            m_Result.text = m_Appraise;
            m_Result.transform.DOLocalJump(new Vector3(-344f, 166f, 0f), 10f, 6, 2);
        }).SetAutoKill(true);
    }
}
