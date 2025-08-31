using System;
using System.Collections.Generic;
using SGGames.Scripts.Core;
using UnityEngine;

public enum SFX
{
    ClickCard,
    ScoreCounting,
    MulCounting,
}

[Serializable]
public class SFXData
{
    public SFX SFXID;
    public AudioClip Clip;
}

public class GameplaySoundManager : MonoBehaviour, IGameService, IBootStrap
{
    [Header("SFX")]
    [SerializeField] private AudioSource m_sfxSource_1;
    [SerializeField] private SFXData[] m_sfxData;
    [Header("BGM")]
    [SerializeField] private AudioSource m_bgmSource;
    [SerializeField] private AudioClip[] m_bgm;
    
    private int m_currentBGMIndex;
    private Dictionary<SFX,SFXData> m_sfxDictionary;
    
    private void Update()
    {
        // Check if BGM has finished playing and automatically play next
        if (m_bgmSource.clip != null && !m_bgmSource.isPlaying && m_bgm.Length > 0)
        {
            PlayNextBGM();
        }

    }

    public void StopBGM()
    {
        m_bgmSource.Stop();      
    }

    public void PlayBGM()
    {
        if (m_bgm.Length == 0) return;
        
        m_currentBGMIndex = m_currentBGMIndex % m_bgm.Length;
        m_bgmSource.clip = m_bgm[m_currentBGMIndex];
        m_bgmSource.Play();
   
    }
    
    private void PlayNextBGM()
    {
        m_currentBGMIndex = (m_currentBGMIndex + 1) % m_bgm.Length;
        m_bgmSource.clip = m_bgm[m_currentBGMIndex];
        m_bgmSource.Play();
    }


    public void PlaySfx(SFX sfxID)
    {
        m_sfxSource_1.clip = m_sfxDictionary[sfxID].Clip;
        m_sfxSource_1.Play(); 
    }

    public void Install()
    {
        ServiceLocator.RegisterService<GameplaySoundManager>(this);
        m_sfxDictionary = new Dictionary<SFX, SFXData>();
        foreach (var data in m_sfxData)
        {
            m_sfxDictionary.Add(data.SFXID, data);
        }
        
        m_bgmSource.loop = false;
        PlayBGM();
    }

    public void Uninstall()
    {
        
    }
}
