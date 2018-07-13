using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// LOGO界面
/// </summary>
[CUIInfo ( PrefabName = "UI_LOGO", IsSigleton = true, IsAnimationUI = true, Description = "LOGO界面" )]
public class CUI_LOGO: CUIBase<CUI_LOGO>
{
    /// <summary>
    /// 背景白
    /// </summary>
    private Image m_BackImage = null;

    /// <summary>
    /// LOGO
    /// </summary>
    private Image m_LOGO = null;

    /// <summary>
    /// 苏醒时
    /// </summary>
    private void Awake()
    {
        m_BackImage = transform.FindChildComponent<Image> ( "Image" );
        m_LOGO = transform.FindChildComponent<Image> ( "LOGO" );
    }

    /// <summary>
    /// 开始时
    /// </summary>
    private IEnumerator Start()
    {
        yield return new WaitForSeconds ( 1.5f );
        m_BackImage.DOColor ( Color.white, 1f );

        yield return new WaitForSeconds ( 3f );
        m_BackImage.rectTransform.FadeOutUINode ( 1f );
        m_LOGO.rectTransform.FadeOutUINode ( 1f );

        yield return new WaitForSeconds ( 1f );

        CSceneManager.Instance.ChangeSceneImmediately ( "02_Main" );
    }
}

