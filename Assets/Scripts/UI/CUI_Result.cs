using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// "
/// </summary>
[CAttrUIBind ( PrefabName = "UI_Result", IsSigleton = true, Description = "结束界面" )]
public class CUI_Result : CUIBase<CUI_Result>
{
    private Image m_Back = null;
    private RectTransform Root = null;
    private Text m_NumberText = null;
    private Button m_EndButton = null;
    private Tweener m_Tween = null;

    /// <summary>
    /// 苏醒时
    /// </summary>
    private void Awake()
    {
        //CGame.Instance.EndGame();
        //m_Back = transform.FindChildComponent<Image> ( "img_Black" );
        //Root = transform.FindChildComponent<RectTransform> ( "Root" );
        //m_NumberText = Root.FindChildComponent<Text> ( "Number" );
        //m_EndButton = Root.FindChildComponent<Button> ( "img_Click" );

        //m_NumberText.text = string.Format ( m_NumberText.text, CGame.Instance.RunningDis.ToString ( "0.00" ), CGame.Instance.JumpCount, CGame.Instance.DeadCount, CGame.Instance.StandAgain );

        //CGame.Instance.RunningDis = 0;
        //CGame.Instance.JumpCount = 0;
        //CGame.Instance.DeadCount = 0;
        //CGame.Instance.StandAgain = 0;
    }

    private void Update()
    {
        if ( Input.anyKey )
        {
            Time.timeScale = 10;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private IEnumerator Start()
    {
        m_Back.SetAlpha ( 0f );
        m_Back.DOFade ( 1f, 1f );

        Root.gameObject.SetActive ( false );

        yield return new WaitForSeconds ( 1f );
        Root.gameObject.SetActive ( true );
        Root.FadeInUINode ( 1f );

        yield return new WaitForSeconds ( 1f );

        m_EndButton.GetComponent<Image>().SetAlpha ( 0.01f );

        m_Tween = Root.DOAnchorPosY ( 1200, 50f ).SetEase ( Ease.Linear ).SetUpdate ( false ).OnComplete ( () =>
        {
            m_EndButton.interactable = true;
            m_EndButton.onClick.AddListener ( OnNextClick );
        } );
    }

    private void OnNextClick()
    {
        CSceneManager.Instance.ChangeSceneImmediately ( "Main" );
    }


}

