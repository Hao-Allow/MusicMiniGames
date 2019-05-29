using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoSingleton<AudioMgr>
{
    /// <summary>
    /// 播放器
    /// </summary>
    private AudioSource m_AudioScorce;

    public AudioSource GetAudioSorce
    {
        get { return m_AudioScorce; }
    }


    /// <summary>
    /// 控制音量
    /// </summary>
    public float Audio_Volume
    {
        get
        {
            return m_AudioScorce.volume;
        }
        set
        {
            m_AudioScorce.volume = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (GetComponent<AudioSource>() == null)
        {
            m_AudioScorce = gameObject.AddComponent<AudioSource>();
        }
        m_AudioScorce = GetComponent<AudioSource>();

        ///设置属性 需求(是否循环播放,是否一开是就播放等)
        //m_AudioScorce.loop = true;
        //m_AudioScorce.playOnAwake = true;
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    public void PlayAudioByName(string name, bool isLoop = false)
    {
        string currAudioName = string.Empty;
        if (m_AudioScorce.clip != null)
        {
            currAudioName = m_AudioScorce.clip.name;
        }

        //资源加载管理者加载资源,读配置表
        AudioClip clip = ResourcesMgr.Instance.LoadAsset<AudioClip>(EnumResType.Audio,name,false);
        if (clip != null)
        {
            if (clip.name == currAudioName)//加载的声音等于当前正在播放的声音
            {
                return;
            }
            m_AudioScorce.clip = clip;
            m_AudioScorce.Play();
        }
        else
        {
            Debug.Log("资源加载错误或为空");
        }
    }


    private void OnDestroy()
    {
        Discard();
    }




}
