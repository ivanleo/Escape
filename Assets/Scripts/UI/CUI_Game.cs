using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 游戏界面
/// </summary>
[CAttrUIBind ( PrefabName = "UI_Game", IsSigleton = true, Description = "游戏界面" )]
public class CUI_Game : CUIBase<CUI_Game>
{
    private Text m_Time = null;
    private Image m_HintRoot = null;


    /// <summary>
    /// 苏醒时
    /// </summary>
    private void Awake()
    {
        m_Time = transform.FindChildComponent<Text> ( "txt_Time" );

        m_HintRoot = transform.FindChildComponent<Image> ( "img_Hint" );

    }

    private void Update()
    {
        if ( Input.anyKeyDown )
        {
            m_HintRoot.gameObject.SetActive ( false );
            //CGame.Instance.StartGame();
        }

        //m_Time.text = CGame.Instance.RunningDis.ToString ( "0.0" ).Replace ( '.', ':' );
    }

}

