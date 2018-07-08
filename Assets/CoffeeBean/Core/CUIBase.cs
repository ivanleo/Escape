
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// UI基类
    /// </summary>
    public abstract class CUIBase<T> : MonoBehaviour where T : CUIBase<T> {
        /// <summary>
        /// UI单例引用
        /// </summary>
        protected static T m_UIInstance = null;

        /// <summary>
        /// UI非单例引用“们”
        /// </summary>
        private static List<T> m_UIInstances = new List<T>();

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
        /// 创建UI的界面显示
        /// </summary>
        public static T CreateUI()
        {
            Type Tp = typeof ( T );

            // 遍历特性
            foreach ( var attr in Tp.GetCustomAttributes ( true ) )
            {
                //UI预制体特性
                if ( attr.GetType() == typeof ( CAttrUIBind ) )
                {
                    return CreateUI ( attr as CAttrUIBind );
                }
            }

            return null;
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        /// <param name="PrefabInfo"></param>
        /// <returns></returns>
        private static T CreateUI ( CAttrUIBind PrefabInfo )
        {
            if ( PrefabInfo.IsSigleton ) //单例UI
            {
                if ( m_UIInstance == null ) //实例不存在
                {
                    m_UIInstance = InstantiateUIAndReturnComponent ( PrefabInfo.PrefabName );
                }

                return m_UIInstance;
            }
            else  //非单例UI
            {
                T TempComp = InstantiateUIAndReturnComponent ( PrefabInfo.PrefabName );
                UIInstances.Add ( TempComp );
                return TempComp;
            }
        }

        /// <summary>
        /// 创建UI实例并返回其上的UI组件
        ///
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        private static T InstantiateUIAndReturnComponent ( string PrefabName )
        {
            //找当前场景中的画布
            GameObject _Canvas = GameObject.Find ( "Canvas" );

            if ( _Canvas == null )
            {
                CLOG.E ( "now scene has not canvas" );
                return null;
            }

            //创建UI预制体实例
            GameObject CreatedUI = CPrefabManager.Instance.CreatePrefabInstance ( PrefabName );

            if ( CreatedUI == null )
            {
                CLOG.E ( "the UI {0} create failed", PrefabName );
                return null;
            }

            //创建UI界面
            CreatedUI.name = PrefabName;
            RectTransform RT = CreatedUI.GetComponent<RectTransform>();

            CreatedUI.transform.SetParent ( _Canvas.transform, false );
            RT.localScale = Vector3.one;

            return GetComponentSafe ( CreatedUI );
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
            if ( UIInstance != null )
            {
                GameObject.Destroy ( UIInstance.gameObject );
                UIInstance = null;
                return;
            }
            else if ( m_UIInstances.Count > 0 )
            {
                for ( int i = 0; i < m_UIInstances.Count; i++ )
                {
                    GameObject.Destroy ( UIInstances[i].gameObject );
                }

                m_UIInstances.Clear();

                return;
            }
        }



        /// <summary>
        /// 销毁非单例界面的指定界面
        /// </summary>
        /// <param name="target">目标</param>
        public static void DestroyUI ( T target )
        {
            T findObj = UIInstances.Find ( ( T obj ) =>
            {
                return obj == target;
            } );

            if ( findObj != null )
            {
                GameObject.Destroy ( findObj.gameObject );
                UIInstances.Remove ( findObj );
            }
        }

        /// <summary>
        /// 干掉非单例界面的最后面一个界面
        /// </summary>
        public static void DestroyTheLastUIOfNonSingleton()
        {
            GameObject.Destroy ( UIInstances[m_UIInstances.Count - 1].gameObject );
            UIInstances.RemoveAt ( m_UIInstances.Count - 1 );
        }


        /// <summary>
        /// 得到预制体名字
        /// </summary>
        public static string GetPrefabName()
        {
            Type Tp = typeof ( T );

            // 遍历特性
            foreach ( var attr in Tp.GetCustomAttributes ( false ) )
            {
                //UI预制体特性
                if ( attr.GetType() == typeof ( CAttrUIBind ) )
                {
                    return ( attr as CAttrUIBind ).PrefabName;
                }
            }

            return "";
        }

    }
}


