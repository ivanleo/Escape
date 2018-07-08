using DG.Tweening;
using UnityEngine;

namespace CoffeeBean {
    /// <summary>
    /// 变换组件扩展
    /// </summary>
    public static class CExpandTransform
    {
        /// <summary>
        /// 直接获得某个孩子身上的指定组件
        /// </summary>
        /// <typeparam name="T">泛型，组件类型</typeparam>
        /// <param name="target">this扩展</param>
        /// <param name="ChildName">孩子名字</param>
        /// <returns></returns>
        public static T FindChildComponent<T> ( this Transform target, string ChildName )
        {
            Transform child = target.Find ( ChildName );

            if ( child == null )
            {
                CLOG.E ( "in {0} can not find child {1}", target.name, ChildName );
                return ( T ) ( System.Object ) null;
            }

            return child.GetComponent<T>();
        }

        /// <summary>
        /// 一个物体是否在屏幕内
        /// </summary>
        /// <param name="obj">this扩展</param>
        /// <param name="cam">摄像机</param>
        /// <returns></returns>
        public static bool IsInScreen ( this Transform obj, Camera cam = null )
        {
            if ( cam == null )
            {
                cam = Camera.main;
            }

            if ( cam == null )
            {
                return false;
            }

            Vector2 ScreenPos = cam.WorldToViewportPoint ( obj.position );
            return ScreenPos.x >= 0 && ScreenPos.x <= 1 && ScreenPos.y >= 0 && ScreenPos.y <= 1;
        }

        /// <summary>
        /// 缩放进入
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="useTime"></param>
        public static void ScaleIn ( this Transform obj, float useTime = 0.5f, TweenCallback callback = null )
        {
            obj.localScale = Vector3.zero;
            obj.DOScale ( Vector3.one, useTime ).OnComplete ( callback );
        }

        /// <summary>
        /// 缩放进入
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="useTime"></param>
        public static void ScaleOut ( this Transform obj, float useTime = 0.5f, TweenCallback callback = null )
        {
            obj.DOScale ( Vector3.zero, useTime ).OnComplete ( callback );
        }

        /// <summary>
        /// 销毁所有孩子
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="Quick"></param>
        public static void DestroyAllChild ( this Transform Root, bool Quick = false )
        {
            for ( int i = Root.childCount - 1 ; i >= 0 ; i-- )
            {
                if ( Quick )
                {
                    GameObject.DestroyImmediate ( Root.GetChild ( i ).gameObject );
                }
                else
                {
                    GameObject.Destroy ( Root.GetChild ( i ).gameObject );
                }
            }
        }

        /// <summary>
        /// 设置X坐标
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="x"></param>
        public static void SetPosX ( this Transform Root, float x )
        {
            Vector3 pos = Root.position;
            pos.x = x;
            Root.position = pos;
        }

        /// <summary>
        /// 设置X坐标
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="x"></param>
        public static void SetPosY ( this Transform Root, float y )
        {
            Vector3 pos = Root.position;
            pos.y = y;
            Root.position = pos;
        }

        /// <summary>
        /// 设置X坐标
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="x"></param>
        public static void SetPosZ ( this Transform Root, float z )
        {
            Vector3 pos = Root.position;
            pos.z = z;
            Root.position = pos;
        }

        /// <summary>
        /// 设置X坐标
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="x"></param>
        public static void SetPosXY ( this Transform Root, float x, float y )
        {
            Vector3 pos = Root.position;
            pos.x = x;
            pos.y = y;
            Root.position = pos;
        }

        /// <summary>
        /// 设置X坐标
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="x"></param>
        public static void SetPosYZ ( this Transform Root, float y, float z )
        {
            Vector3 pos = Root.position;
            pos.y = y;
            pos.z = z;
            Root.position = pos;
        }

        /// <summary>
        /// 设置X坐标
        /// </summary>
        /// <param name="Root"></param>
        /// <param name="x"></param>
        public static void SetPosXZ ( this Transform Root, float x, float z )
        {
            Vector3 pos = Root.position;
            pos.x = x;
            pos.z = z;
            Root.position = pos;
        }

    }
}
