using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 主界面
/// </summary>
[CAttrUIBind ( PrefabName = "UI_Main", IsSigleton = true, Description = "主界面" )]
public class CUI_Main : CUIBase<CUI_Main>
{
    /// <summary>
    /// 背景白
    /// </summary>
    private Image m_BackImage = null;

    /// <summary>
    /// Title
    /// </summary>
    private Image m_LOGO = null;

    /// <summary>
    /// 单人游戏
    /// </summary>
    private Button m_SingleButton = null;

    private Button m_Intru = null;

    /// <summary>
    /// 双人游戏
    /// </summary>
    private Button m_DoubleButton = null;

    private Animator m_AT = null;

    private AudioSource m_Sound = null;

    /// <summary>
    /// 苏醒时
    /// </summary>
    private void Awake()
    {
        m_BackImage = transform.FindChildComponent<Image> ( "img_Background" );
        m_LOGO = transform.FindChildComponent<Image> ( "img_Title" );
        m_SingleButton = transform.FindChildComponent<Button> ( "SingleGame" );
        m_DoubleButton = transform.FindChildComponent<Button> ( "DoubleGame" );
        m_Intru = transform.FindChildComponent<Button> ( "Btn_Intru" );

        m_Sound = transform.FindChildComponent<AudioSource> ( "Sound" );

        m_AT = GetComponent<Animator>();

        m_SingleButton.onClick.AddListener ( OnSinglePlayClick );
        m_DoubleButton.onClick.AddListener ( OnDoublePlayClick );
        m_Intru.onClick.AddListener ( OnIntruClick );

        m_SingleButton.GetTouchEventModule().OnTouchEnter += ( PointerEventData eventData ) => CSoundManager.Instance.PlayButtonClick() ;
        m_DoubleButton.GetTouchEventModule().OnTouchEnter += ( PointerEventData eventData ) => CSoundManager.Instance.PlayButtonClick();
        m_Intru.GetTouchEventModule().OnTouchEnter += ( PointerEventData eventData ) => CSoundManager.Instance.PlayButtonClick();
    }

    private void OnSinglePlayClick()
    {
        CSoundManager.Instance.PlayEffect ( "single" );
        m_Sound.DOFade ( 0f, 1f );

        //单人游戏
        //CGame.Instance.PlayMode = 1;
        m_AT.Play ( "Hide" );
        m_DoubleButton.enabled = false;
        m_DoubleButton.enabled = false;
        StartCoroutine ( DelayTOGame() );
    }

    private void OnIntruClick()
    {
        CSoundManager.Instance.PlayEffect ( "single" );
        CUI_Intru.CreateUI();
    }


    private void OnDoublePlayClick()
    {
        CSoundManager.Instance.PlayEffect ( "double" );

        m_Sound.DOFade ( 0f, 1f );

        ////双人游戏
        //CGame.Instance.PlayMode = 2;
        m_AT.Play ( "Hide" );
        m_DoubleButton.enabled = false;
        m_DoubleButton.enabled = false;
        StartCoroutine ( DelayTOGame() );

    }

    private IEnumerator DelayTOGame()
    {
        yield return new WaitForSeconds ( 1 );
        CSceneManager.Instance.ChangeSceneImmediately ( "Game" );
    }

}

