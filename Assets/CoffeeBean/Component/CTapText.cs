using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CoffeeBean;
using System;

/// <summary>
/// 文字打字机
/// </summary>
public class CTapText : MonoBehaviour
{
    /// <summary>
    /// 文字信息们
    /// </summary>
    [SerializeField]
    private string[] m_Words = null;

    /// <summary>
    /// 延时
    /// </summary>
    [SerializeField]
    private float m_Delay = 0f;

    /// <summary>
    /// 每隔多少秒显示一个字
    /// </summary>
    [SerializeField]
    private float m_CharInteval = 0.1f;

    /// <summary>
    /// 是否自动跳到下一句话
    /// </summary>
    [SerializeField]
    private bool m_AutoNext = true;

    /// <summary>
    /// 两句话之间的间隔
    /// </summary>
    [SerializeField]
    private float m_NextInteval = 2f;

    /// <summary>
    /// 文字组件
    /// </summary>
    private Text m_Text = null;

    /// <summary>
    /// 当前显示序号
    /// </summary>
    private int m_NowIndex = 0;

    /// <summary>
    /// 是否可以进入下一句话
    /// </summary>
    private bool m_CanNext = false;

    /// <summary>
    /// 苏醒时
    /// </summary>
    private void Awake()
    {
        m_Text = GetComponent<Text>();
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="words"></param>
    /// <param name="inteval"></param>
    /// <param name="autoNext"></param>
    public void SetData ( string[] words, float inteval = 0.1f, bool autoNext = true )
    {
        m_Words = words;
        m_CharInteval = inteval;
        m_AutoNext = autoNext;
    }

    /// <summary>
    /// 开始显示
    /// </summary>
    public void Start()
    {
        StartCoroutine ( ShowText() );
    }


    /// <summary>
    /// 显示文字
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowText()
    {
        if ( m_Delay > 0 )
        {
            yield return new WaitForSeconds ( m_Delay );
        }

        while ( m_NowIndex < m_Words.Length )
        {
            int index = 0;

            if ( m_CharInteval > 0 )
            {
                while ( index < m_Words[m_NowIndex].Length )
                {
                    m_Text.text += m_Words[m_NowIndex][index++];
                    yield return new WaitForSeconds ( m_CharInteval );
                }
            }
            else
            {
                m_Text.text = m_Words[m_NowIndex];
            }

            m_NowIndex++;
            m_Text.text += "\n";

            if ( m_AutoNext )
            {
                if ( m_NextInteval > 0 )
                {
                    yield return new WaitForSeconds ( m_NextInteval );
                }
            }
            else
            {
                m_CanNext = true;
                yield return new WaitUntil ( () => !m_CanNext );
            }
        }
    }

    /// <summary>
    /// 每帧执行
    /// </summary>
    private void Update()
    {
        if ( m_CanNext && Input.anyKey )
        {
            m_CanNext = false;
        }
    }
}

