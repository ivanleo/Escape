using CoffeeBean;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 介绍界面
/// </summary>
[CAttrUIBind ( PrefabName = "UI_Intru", IsSigleton = true, Description = "介绍界面" )]
public class CUI_Intru : CUIBase<CUI_Intru>
{
    private IEnumerator Start()
    {
        ( transform as RectTransform ).FadeInUINode ( 0.5f );
        yield return new WaitForSeconds ( 10f );
        ( transform as RectTransform ).FadeOutUINode ( 1f, () => { DestroyUI(); } );
    }

    private void Update()
    {
        if ( Input.anyKeyDown )
        {
            StopAllCoroutines();
            ( transform as RectTransform ).FadeOutUINode ( 1f, () => { DestroyUI(); } );
        }
    }

}

