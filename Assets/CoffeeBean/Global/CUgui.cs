﻿using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoffeeBean {
    /// <summary>
    /// UGUI工具类
    /// </summary>
    public static class CUgui {
        /// <summary>
        /// 得到UI组件的画布坐标
        /// </summary>
        /// <param name="obj">物体</param>
        /// <param name="can">画布</param>
        /// <returns></returns>
        public static Vector2 GetUICanvasPos ( RectTransform obj, Canvas can = null )
        {
            if ( can == null )
            {
                can = GameObject.Find ( "Canvas" ).GetComponent<Canvas>();
            }

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle ( can.transform as RectTransform, obj.position, can.worldCamera, out pos );
            return pos;
        }

        /// <summary>
        /// 渐入一个UI节点
        /// </summary>
        /// <param name="Target">this扩展</param>
        /// <param name="Duration">持续时间</param>
        /// <param name="Callback">延时回调</param>
        public static void FadeInUINode ( this RectTransform Target, float Duration, TweenCallback Callback = null )
        {
            if ( !Target.gameObject.activeSelf )
            {
                Target.gameObject.SetActive ( true );
            }

            Component[] Comps = Target.GetComponentsInChildren<Component>();
            int CompNum = 0;
            int FinishNum = 0;

            for ( int i = 0 ; i < Comps.Length ; i++ )
            {
                Component Comp = Comps[i];

                if ( Comp is Graphic )
                {
                    ( Comp as Graphic ).SetAlpha ( 0f );

                    CompNum++;

                    ( Comp as Graphic ).DOFade ( 1.0f, Duration ).OnComplete ( () =>
                    {
                        FinishNum++;

                        if ( CompNum != 0 && FinishNum != 0 && FinishNum == CompNum )
                        {
                            if ( Callback != null )
                            {
                                Callback();
                            }
                        }
                    } );
                }
            }
        }

        /// <summary>
        /// 渐出一个UI节点
        /// </summary>
        /// <param name="Target">目标UI节点</param>
        /// <param name="Duration">持续时间</param>
        /// <param name="Callback">延时回调</param>
        public static void FadeOutUINode ( this RectTransform Target, float Duration, TweenCallback Callback = null )
        {
            if ( !Target.gameObject.activeSelf )
            {
                Target.gameObject.SetActive ( true );
            }

            Component[] Comps = Target.GetComponentsInChildren<Component>();
            int CompNum = 0;
            int FinishNum = 0;

            for ( int i = 0 ; i < Comps.Length ; i++ )
            {
                Component Comp = Comps[i];

                if ( Comp is Graphic )
                {
                    ( Comp as Graphic ).SetAlpha ( 1f );

                    CompNum++;
                    ( Comp as Graphic ).DOFade ( 0.0f, Duration ).OnComplete ( () =>
                    {
                        FinishNum++;

                        if ( CompNum != 0 && FinishNum != 0 && FinishNum == CompNum )
                        {
                            Target.gameObject.SetActive ( false );

                            if ( Callback != null )
                            {
                                Callback();
                            }
                        }
                    } );
                }
            }
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Alpha"></param>
        public static void SetAlpha ( this Graphic target, float Alpha )
        {
            Color cr = target.color;
            cr.a = CMath.Clamp ( Alpha, 0f, 1f );
            target.color = cr;
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="target"></param>
        /// <param name="Alpha"></param>
        public static void SetAlpha ( this SpriteRenderer target, float Alpha )
        {
            Color cr = target.color;
            cr.a = CMath.Clamp ( Alpha, 0f, 1f );
            target.color = cr;
        }


        /// <summary>
        /// 得到屏幕指定点上的UI组件
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <param name="ResultList">结果组件</param>
        /// <returns></returns>
        public static bool GetScreenPosOverUIObjects ( Vector2 screenPosition, ref List<RaycastResult> ResultList )
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData ( EventSystem.current );
            eventDataCurrentPosition.position = screenPosition;
            EventSystem.current.RaycastAll ( eventDataCurrentPosition, ResultList );
            return ResultList.Count > 0;
        }

    }
}