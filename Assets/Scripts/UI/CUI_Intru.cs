using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 介绍界面
/// </summary>
[CUIInfo ( PrefabName = "UI_Intru", IsSigleton = true, IsAnimationUI = true, Description = "介绍界面" )]
public class CUI_Intru : CUIBase<CUI_Intru>
{
    private float m_NowPassTime = 0f;

    private void Update()
    {
        m_NowPassTime += Time.deltaTime;

        if ( Input.anyKeyDown || m_NowPassTime > 12f )
        {
            DestroyUI();
        }
    }

}

