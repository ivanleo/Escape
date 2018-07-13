/********************************************************************
    created:    2018/05/31
    created:    31:5:2018   11:27
    filename:   D:\Work\PushCoin\trunk\PushCoin\Assets\CoffeeBean\Core\CUIBase.cs
    file path:  D:\Work\PushCoin\trunk\PushCoin\Assets\CoffeeBean\Core
    file base:  CUIBase
    file ext:   cs
    author:     Leo

    purpose:    UI基类
*********************************************************************/
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// UI基类接口
    /// </summary>
    public interface IUIBase
    {
        /// <summary>
        /// 创建时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        void AnimOnCreate(  );

        /// <summary>
        /// 隐藏时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        void AnimOnHide(  );

        /// <summary>
        /// 显示时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        void AnimOnShow(  );

        /// <summary>
        /// 销毁时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        void AnimOnDestroy ( TweenCallback callback = null );

        /// <summary>
        /// 显示UI
        /// </summary>
        void ShowUI();

        /// <summary>
        /// 隐藏UI
        /// </summary>
        void HideUI();

        /// <summary>
        /// 销毁本UI
        /// </summary>
        void DestroyIt();
    }

    /// <summary>
    /// UI基类
    /// </summary>
    public abstract class CUIBase<T> : MonoBehaviour, IUIBase where T : CUIBase<T>
    {
        /// <summary>
        /// UI单例引用
        /// </summary>
        private static T m_UIInstance = null;

        /// <summary>
        /// UI非单例引用“们”
        /// </summary>
        private static List<T> m_UIInstances = new List<T>();

        /// <summary>
        /// 是否正在移除中
        /// </summary>
        protected bool m_IsRemoving = false;

        /// <summary>
        /// UI的单例引用，非单例获取则为空
        /// </summary>
        public static T UIInstance
        {
            get
            {
                return m_UIInstance;
            }

            private set
            {
                m_UIInstance = value;
            }
        }

        /// <summary>
        /// 矩形变换组件
        /// </summary>
        public RectTransform rectTransform { get { return transform as RectTransform; } }

        /// <summary>
        /// UI非单例引用“们”
        /// </summary>
        public static List<T> UIInstances
        {
            get
            {
                return m_UIInstances;
            }

            private set
            {
                m_UIInstances = value;
            }
        }

        /// <summary>
        /// 被记录的UI信息
        /// </summary>
        public CUIInfo UIInfo { get; private set; }

        /// <summary>
        /// 创建时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        public virtual void AnimOnCreate()
        {
            transform.FadeInUINode ( 0.5f );
        }

        /// <summary>
        /// 隐藏时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        public virtual void AnimOnHide(  )
        {
            //放大,淡化消失
            transform.FadeOutUINode ( 0.5f );
        }

        /// <summary>
        /// 显示时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        public virtual void AnimOnShow( )
        {
            //缩小,淡化进入
            transform.FadeInUINode ( 0.5f );
        }

        /// <summary>
        /// 销毁时的动画
        /// 子类可以继承后修改他实现自己的动画效果
        /// </summary>
        public virtual void AnimOnDestroy ( TweenCallback callback = null )
        {
            //放大显示
            transform.FadeOutUINode ( 0.5f, callback );
        }


        /// <summary>
        /// 销毁本UI
        /// </summary>
        public void DestroyIt()
        {
            DestroyUI ( ( T ) this );
        }


        /// <summary>
        /// 呈现UI
        /// </summary>
        public void ShowUI()
        {
            if ( UIInfo.IsAnimationUI )
            {
                AnimOnShow();
            }
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public void HideUI()
        {
            if ( UIInfo.IsAnimationUI )
            {
                AnimOnHide();
            }
        }


        /// <summary>
        /// 创建层次UI
        /// </summary>
        /// <returns></returns>
        public static T CreateLayoutUI()
        {
            T UI = CreateUI();
            CUILayoutManager.Instance.HideTopAndAddUI ( UI );
            return UI;
        }

        /// <summary>
        /// 创建UI的界面显示
        /// </summary>
        public static T CreateUI()
        {
            Type Tp = typeof ( T );

            // 遍历特性
            foreach ( var attr in Tp.GetCustomAttributes ( false ) )
            {
                //UI预制体特性
                if ( attr.GetType() == typeof ( CUIInfo ) )
                {
                    return CreateUI ( attr as CUIInfo );
                }
            }

            CLOG.E ( "the ui {0} has no CUIInfo attr", typeof ( T ).ToString() );
            return null;
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        /// <param name="PrefabInfo"></param>
        /// <returns></returns>
        private static T CreateUI ( CUIInfo UIInfo )
        {
            if ( UIInfo.IsSigleton ) //单例UI
            {
                if ( m_UIInstance == null ) //实例不存在
                {
                    m_UIInstance = InstantiateUIAndReturnComponent ( UIInfo );
                }

                return m_UIInstance;
            }
            else  //非单例UI
            {
                T TempComp = InstantiateUIAndReturnComponent ( UIInfo );
                UIInstances.Add ( TempComp );
                return TempComp;
            }
        }

        /// <summary>
        /// 创建UI实例并返回其上的UI组件
        ///
        /// </summary>
        /// <param name="PrefabName">预制体名</param>
        /// <param name="NeedAnimation">是否需要动画</param>
        /// <returns></returns>
        private static T InstantiateUIAndReturnComponent ( CUIInfo UIInfo )
        {
            //找当前场景中的画布
            GameObject _Canvas = GameObject.Find ( "Canvas" );

            if ( _Canvas == null )
            {
                CLOG.E ( "now scene has not canvas" );
                return null;
            }

            //创建UI预制体实例
            GameObject CreatedUI = CPrefabManager.Instance.CreatePrefabInstance ( UIInfo.PrefabName );

            if ( CreatedUI == null )
            {
                CLOG.E ( "the UI {0} create failed", UIInfo.PrefabName );
                return null;
            }

            //创建UI界面
            CreatedUI.name = UIInfo.PrefabName;
            RectTransform RT = CreatedUI.GetComponent<RectTransform>();

            //设置父节点
            CreatedUI.transform.SetParent ( _Canvas.transform, false );
            RT.localScale = Vector3.one;

            //向UI上添加自身组件
            T Ins = GetComponentSafe ( CreatedUI );

            //记录UI信息
            Ins.UIInfo = UIInfo;

            if ( UIInfo.IsAnimationUI )
            {
                Ins.AnimOnCreate();
            }

            //返回UI实例
            return Ins;
        }

        /// <summary>
        /// 安全的得到组件
        /// </summary>
        /// <param name="Target">目标游戏对象</param>
        /// <returns></returns>
        private static T GetComponentSafe ( GameObject Target )
        {
            T TempCom = Target.GetComponent<T>();

            if ( TempCom == null )
            {
                TempCom = Target.AddComponent<T>();
            }

            return TempCom;
        }

        /// <summary>
        /// 干掉UI界面
        /// 单例界面干掉单例
        /// 非单例界面干掉所有非单例
        /// </summary>
        public static void DestroyUI()
        {
            if ( UIInstance != null && !UIInstance.m_IsRemoving )
            {
                //单例 UI的销毁

                //先尝试从层级管理器里移除掉
                //如果管理器没有移除成功
                //那么代表不能删除
                if ( !CUILayoutManager.Instance.TryRemoveUI ( m_UIInstance ) )
                {
                    return;
                }

                //删除UI
                if ( UIInstance.UIInfo.IsAnimationUI )
                {
                    UIInstance.m_IsRemoving = true;
                    //支持动画的话就播完动画再销毁
                    UIInstance.AnimOnDestroy ( () =>
                    {
                        UIInstance.m_IsRemoving = false;
                        GameObject.Destroy ( UIInstance.gameObject );
                        UIInstance = null;
                    } );
                }
                else
                {
                    //不支持动画直接销毁
                    GameObject.Destroy ( UIInstance.gameObject );
                    UIInstance = null;
                }
            }
            else if ( m_UIInstances.Count > 0 )
            {
                //非单例 UI的销毁

                for ( int i = 0 ; i < m_UIInstances.Count ; i++ )
                {
                    //先尝试从层级管理器里移除掉
                    //如果管理器没有移除成功
                    //那么代表不能删除
                    if ( !CUILayoutManager.Instance.TryRemoveUI ( UIInstances[i] ) )
                    {
                        continue;;
                    }

                    //支持动画的话就播完动画再销毁
                    if ( UIInstances[i].UIInfo.IsAnimationUI )
                    {
                        UIInstances[i].AnimOnDestroy ( () => {GameObject.Destroy ( UIInstances[i].gameObject ); } );
                    }
                    else
                    {
                        //不支持动画直接销毁
                        GameObject.Destroy ( UIInstances[i].gameObject );
                    }
                }

                m_UIInstances.Clear();
            }

            return;
        }

        /// <summary>
        /// 销毁指定界面
        /// </summary>
        /// <param name="target">目标</param>
        public static void DestroyUI ( T target )
        {
            //先尝试从层级管理器里移除掉
            //如果管理器没有移除成功
            //那么代表不能删除
            if ( !CUILayoutManager.Instance.TryRemoveUI ( target ) )
            {
                return;
            }


            //单例UI直接销毁
            if ( target == UIInstance )
            {
                if ( UIInstance.UIInfo.IsAnimationUI )
                {
                    //支持动画的话就播完动画再销毁
                    UIInstance.AnimOnDestroy ( () =>
                    {
                        GameObject.Destroy ( UIInstance.gameObject );
                        UIInstance = null;
                    } );
                }
                else
                {
                    //不支持动画直接销毁
                    GameObject.Destroy ( UIInstance.gameObject );
                    UIInstance = null;
                }

                return;
            }

            //销毁非单例UI
            T findObj = UIInstances.Find ( ( T obj ) =>
            {
                return obj == target;
            } );

            if ( findObj != null )
            {
                if ( findObj.UIInfo.IsAnimationUI )
                {
                    //支持动画的话就播完动画再销毁
                    findObj.AnimOnDestroy ( () =>
                    {
                        GameObject.Destroy ( findObj.gameObject );
                        UIInstances.Remove ( findObj );
                    } );
                }
                else
                {
                    //不支持动画直接销毁
                    GameObject.Destroy ( findObj.gameObject );
                    UIInstances.Remove ( findObj );
                }
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void OnDestroy()
        {
            if ( m_UIInstance != null )
            {
                m_UIInstance = null;
            }

            if ( m_UIInstances.Contains ( ( T ) this ) )
            {
                m_UIInstances.Remove ( ( T ) this );
            }
        }
    }
}


