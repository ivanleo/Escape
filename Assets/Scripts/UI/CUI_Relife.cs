using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using System;
using DG;

/// <summary>
/// 复活界面
/// </summary>
[CAttrUIBind ( PrefabName = "UI_Relife", IsSigleton = true, Description = "复活界面" )]
public class CUI_Relife : CUIBase<CUI_Relife>
{
    private Image m_Progress = null;
    private Button m_Click = null;
    private Text m_LeftTime = null;
    private int NowValue = 0;
    private const int needValue = 1000;
    private int LeftTime = 9;
    private RectTransform Root = null;

    /// <summary>
    /// 苏醒时
    /// </summary>
    private void Awake()
    {
        Root = transform.FindChildComponent<RectTransform> ( "Root" );
        m_Progress = transform.FindChildComponent<Image> ( "Root/Image (2)" );
        m_Click = transform.FindChildComponent<Button> ( "Clicker" );
        m_LeftTime = transform.FindChildComponent<Text> ( "Root/LeftTime" );
        m_LeftTime.text = "09";


        UpdateProgress();
    }

    private void Start()
    {
        Root.DOAnchorPosX ( -1000f, 1f ).From().SetEase ( Ease.OutBack ).OnComplete ( () =>
        {
            StartCoroutine ( Sec() );
        } );

    }

    private IEnumerator Sec()
    {
        while ( LeftTime > 0 )
        {
            CSoundManager.Instance.PlayEffect ( "clock" );
            m_LeftTime.text = LeftTime.ToString().PadLeft ( 2, '0' );
            LeftTime--;
            yield return new WaitForSeconds ( 1 );
        }

        Hide ( () =>
        {
            CUI_Result.CreateUI();
        } );
    }

    private void Update()
    {
        if ( Input.GetKeyDown ( KeyCode.S ) )
        {
            OnClick();
        }
    }

    private void OnClick()
    {
        //NowValue +=  ( UnityEngine.Random.Range ( 1, 10 ) * ( 10 - CGame.Instance.RelifeCount ) );
        //UpdateProgress();

        //CSoundManager.Instance.PlayButtonClick();

        //if ( NowValue >= needValue )
        //{
        //    StopAllCoroutines();
        //    CGame.Instance.StandAgain ++;
        //    Hide ( () =>
        //    {
        //        CGame.Instance.RelifeCount++;
        //        CPlayerController.Instance.ReLife();
        //    } );
        //}
    }


    public void UpdateProgress (  )
    {
        m_Progress.fillAmount = NowValue / ( float ) needValue;
    }

    private void Hide ( TweenCallback action )
    {
        ( transform as RectTransform ).DOAnchorPosX ( 0, 0.3f ).OnComplete ( () =>
        {
            transform.DOScaleX ( 100f, 0.3f );
            transform.DOScaleY ( 0.01f, 0.3f ).OnComplete ( action );
            ( transform as RectTransform ).FadeOutUINode ( 0.3f, () =>
            {
                CUI_Relife.DestroyUI();
            } );
        } );

    }



}

