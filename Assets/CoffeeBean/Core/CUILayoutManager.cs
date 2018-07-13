using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CoffeeBean
{
    /// <summary>
    /// UI层级管理器
    /// </summary>
    public class CUILayoutManager : CSingleton<CUILayoutManager>
    {
        /// <summary>
        /// UI栈
        /// </summary>
        private Stack<IUIBase> m_UIStack = null;

        /// <summary>
        /// 无参构造
        /// </summary>
        public CUILayoutManager()
        {
            m_UIStack = new Stack<IUIBase>();
        }

        /// <summary>
        /// 清理所有管理的UI
        /// 一般在场景切换后调用
        /// </summary>
        public void Clear()
        {
            m_UIStack.Clear();
        }


        /// <summary>
        /// 当前栈顶UI隐藏并增加一个新的UI
        /// </summary>
        /// <param name="UI"></param>
        public void HideTopAndAddUI( IUIBase UI )
        {
            if ( m_UIStack.Count > 0 )
            {
                IUIBase Top = m_UIStack.Peek();
                if ( Top != null )
                {
                    Top.HideUI();
                }
            }

            m_UIStack.Push( UI );
        }

        /// <summary>
        /// 移除栈顶并且显示栈顶的UI
        /// </summary>
        /// <param name="UI"></param>
        public void RemoveTopAndShowTop()
        {
            IUIBase UITop = m_UIStack.Pop();
            if ( UITop != null )
            {
                UITop.DestroyIt();
            }

            //新的栈顶显示动画
            if( m_UIStack.Count > 0 )
            {
                UITop = m_UIStack.Peek();

                if ( UITop != null )
                {
                    UITop.ShowUI();
                }
            }
        }



        /// <summary>
        /// 尝试移除UI
        /// 返回是否从堆栈中成功移除
        /// 如果成功了
        /// 那么代表可以删除
        /// 如果移除失败了
        /// 那么代表不能删除
        /// </summary>
        /// <param name="UI"></param>
        public bool TryRemoveUI( IUIBase UI )
        {
            // 栈未初始化
            // 则默认可以删除UI
            // 因为此时UI尚未纳入层级管理
            // 因此可以自由删除
            if ( m_UIStack == null )
            {
                return true;
            }

            // 栈不包含目标UI
            // 则默认可以删除UI
            // 因为此时UI尚未纳入层级管理
            // 因此可以自由删除
            if ( !m_UIStack.Contains( UI ) )
            {
                return true;
            }

            // 栈顶就是目标UI
            // 则可以删除UI
            // 因为栈只能从栈顶一个一个的删除
            if ( m_UIStack.Peek() == UI )
            {
                m_UIStack.Pop();
                return true;
            }

            // 其余情况无法删除UI
            // 原因是改UI处于UI层级管理器的管理
            // 且该UI不是栈顶UI
            // 因此不能删除
            CLOG.W( "the ui {0} can not destroy because it is an layoutUI , its life was manager by UI Layout Manager" );
            return false;

        }
    }
}