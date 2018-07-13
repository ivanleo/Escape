//using UnityEngine;
//using UnityEngine.UI;
//using CoffeeBean;
//using System.Collections.Generic;

//public class CVerticalScroll : ScrollRect
//{
//    //是否循环
//    public bool isLoop = false;

//    //content区域
//    private RectTransform m_ContentArea = null;

//    //ScrollView
//    private ScrollRect m_ScrollView = null;

//    private LayoutGroup m_Layout = null;

//    //子项的高
//    private float m_ItemHeight;

//    //子项数量
//    public float m_ItemNum;


//    public void Bind<T1, T2> ( List<T1> list, bool flag ) where T1 : ItemDataBase where T2 : CUIBase<T2>
//    {
//        m_ItemNum = list.Count;
//        isLoop = flag;

//        Init();

//        for ( int i = 0; i < list.Count; i++ )
//        {
//            T2 ui = CUIBase<T2>.CreateUI();
//            ui.SetData ( list[i] );
//            ui.transform.SetParent ( m_ContentArea );
//        }

//    }

//    private void Init()
//    {
//        m_ScrollView = this.gameObject.GetComponent<CVerticalScroll>();
//        m_ContentArea = m_ScrollView.content;
//        m_ScrollView.onValueChanged.AddListener ( OnContentMove );
//        m_Layout = m_ContentArea.GetComponent<LayoutGroup>();
//    }

//    /// <summary>
//    /// 滚动条Content监测的方法
//    /// </summary>
//    /// <param name="param"></param>
//    private void OnContentMove ( Vector2 param )
//    {
//        m_ItemHeight = m_ContentArea.rect.height / m_ItemNum;

//        if ( isLoop )
//        {
//            if ( m_ContentArea.anchoredPosition.y > m_ItemHeight * 2 )
//            {
//                m_ContentArea.GetChild ( 0 ).SetSiblingIndex ( m_ContentArea.childCount - 1 );

//                m_ContentArea.anchoredPosition = new Vector2 ( 0, m_ItemHeight );
//                m_Layout.CalculateLayoutInputVertical();
//                m_Layout.CalculateLayoutInputHorizontal();
//                return;
//            }

//            if ( m_ContentArea.anchoredPosition.y < 0 )
//            {
//                m_ContentArea.GetChild ( m_ContentArea.childCount - 1 ).SetSiblingIndex ( 0 );

//                m_ContentArea.anchoredPosition = new Vector2 ( 0, m_ItemHeight );
//                m_Layout.CalculateLayoutInputVertical();
//                m_Layout.CalculateLayoutInputHorizontal();
//                return;
//            }
//        }

//    }


//}

