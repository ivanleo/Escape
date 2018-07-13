using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 主界面
/// </summary>
[CUIInfo ( PrefabName = "UI_Main", IsSigleton = true, IsAnimationUI = true, Description = "主界面" )]

public class CUI_Main : CUIBase<CUI_Main>
{
    /// <summary>
    /// 单人游戏
    /// </summary>
    private Button m_SingleButton = null;

    /// <summary>
    /// 释义
    /// </summary>
    private Button m_Intru = null;

    /// <summary>
    /// 双人游戏
    /// </summary>
    private Button m_DoubleButton = null;

    /// <summary>
    /// 动画控制器
    /// </summary>
    private Animator m_AT = null;

    /// <summary>
    /// 声音
    /// </summary>
    private AudioSource m_Sound = null;

    /// <summary>
    /// 苏醒时
    /// </summary>
    private void Awake()
    {
        m_SingleButton = transform.FindChildComponent<Button> ( "SingleGame" );
        m_DoubleButton = transform.FindChildComponent<Button> ( "DoubleGame" );
        m_Intru = transform.FindChildComponent<Button> ( "Btn_Intru" );

        m_Sound = transform.FindChildComponent<AudioSource> ( "Sound" );

        m_AT = GetComponent<Animator>();

        m_SingleButton.onClick.AddListener ( OnSinglePlayClick );
        m_DoubleButton.onClick.AddListener ( OnDoublePlayClick );
        m_Intru.onClick.AddListener ( OnIntruClick );

    }

    private void OnSinglePlayClick()
    {
        HideAndGoToPlay ( EPlayMode.PLAY_SINGLE );
    }


    /// <summary>
    /// 显示介绍
    /// </summary>
    private void OnIntruClick()
    {
        CUI_Intru.CreateUI();
    }


    private void OnDoublePlayClick()
    {
        HideAndGoToPlay ( EPlayMode.PLAY_DOUBLE );
    }

    /// <summary>
    /// 隐藏界面并延时进入游戏
    /// </summary>
    private void HideAndGoToPlay ( EPlayMode EPM )
    {
        m_Sound.DOFade ( 0f, 1f );
        m_AT.Play ( "Hide" );
        m_DoubleButton.enabled = false;
        m_DoubleButton.enabled = false;
        StartCoroutine ( DelayTOGame ( EPM ) );
    }

    private IEnumerator DelayTOGame ( EPlayMode EPM )
    {
        yield return new WaitForSeconds ( 1 );
        CSceneManager.Instance.ChangeSceneImmediately ( "03_Game" );
    }

}

