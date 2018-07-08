using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoffeeBean
{
    /// <summary>
    /// 帧回调
    /// </summary>
    /// <param name="frameIndex"></param>
    public delegate void DelegateAnimationFrameEvent( int frameIndex );

    [Serializable]
    public class CFrame
    {
        /// <summary>
        /// 帧精灵
        /// </summary>
        public Sprite SpFrame;

        /// <summary>
        /// 帧延时
        /// </summary>
        public float SpInteval = 0.1f;
    }

    /// <summary>
    /// 动画
    /// </summary>
    [Serializable]
    public class CAnimation
    {
        /// <summary>
        /// 帧
        /// </summary>
        public CFrame[] Frames;

        /// <summary>
        /// 每帧回调
        /// </summary>
        public DelegateAnimationFrameEvent EveryFrameCallback = null;

        /// <summary>
        /// 特定帧回调
        /// </summary>
        public Dictionary<int, DelegateAnimationFrameEvent> FrameCallbacks = new Dictionary<int, DelegateAnimationFrameEvent>();

        /// <summary>
        /// 设置每一帧的回调
        /// </summary>
        /// <param name="callback">回调</param>
        public void SetEveryFrameCallBack( DelegateAnimationFrameEvent callback )
        {
            EveryFrameCallback = callback;
        }

        /// <summary>
        /// 添加帧回调
        /// </summary>
        /// <param name="FrameIndex">特定帧</param>
        /// <param name="callback"></param>
        public void AddFrameCallBack( int FrameIndex, DelegateAnimationFrameEvent callback )
        {
            //替换
            if ( FrameCallbacks.ContainsKey( FrameIndex ) )
            {
                FrameCallbacks[FrameIndex] = callback;
            }
            else
            {
                FrameCallbacks.Add( FrameIndex, callback );
            }
        }

        /// <summary>
        /// 移除特定帧的回调
        /// </summary>
        /// <param name="FrameIndex">特定帧</param>
        public void DeleteFrameCallBack( int FrameIndex )
        {
            //替换
            if ( FrameCallbacks.ContainsKey( FrameIndex ) )
            {
                FrameCallbacks.Remove( FrameIndex );
            }
        }

        /// <summary>
        /// 移除所有帧回掉
        /// </summary>
        public void DeleteAllFrameCallBack()
        {
            FrameCallbacks.Clear();
        }

    }

    /// <summary>
    /// 序列帧动画封装
    /// </summary>
    public class CFrameAnimation : MonoBehaviour
    {
        /// <summary>
        /// 动画数据
        /// </summary>
        public CAnimation AnimationData = null;

        /// <summary>
        /// 是否在开始时默认播放第一个动画
        /// </summary>
        [SerializeField]
        private bool m_AutoPlayOnStart = false;

        /// <summary>
        /// 当前是否重复播放
        /// </summary>
        [SerializeField]
        private bool m_IsLoop = false;

        /// <summary>
        /// 是否正在播放动画
        /// </summary>
        [ReadOnly]
        [SerializeField]
        private bool m_IsPlaying = false;

        /// <summary>
        /// 当前播放头
        /// </summary>
        [ReadOnly]
        [SerializeField]
        private int m_PlayHeadIndex = 0;

        /// <summary>
        /// 操控的图像 支持 SpriteRenderer 和 Image两种
        /// </summary>
        private SpriteRenderer m_SR = null;
        private Image m_Image = null;

        /// <summary>
        /// 播放协程
        /// </summary>
        private Coroutine m_PlayCoroutine = null;

        /// <summary>
        /// 苏醒时
        /// </summary>
        private void Awake()
        {
            m_SR = GetComponent<SpriteRenderer>();
            m_Image = GetComponent<Image>();
        }

        /// <summary>
        /// 开始时
        /// </summary>
        private void Start()
        {
            if ( m_AutoPlayOnStart )
            {
                PlayAnimation( m_IsLoop );
            }
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="IsLoop">是否循环</param>
        public void PlayAnimation(  bool IsLoop = false )
        {
            //如果在播放就停止
            if ( m_IsPlaying )
            {
                StopNowAnimation();
            }

            m_IsLoop = IsLoop;

            //设置播放头为第一帧
            m_PlayHeadIndex = 0;

            int AnimCount = AnimationData.Frames.Length;
            if ( AnimCount > 1 )
            {
                m_PlayCoroutine = StartCoroutine( PlayHandler() );
            }
            else if( AnimCount == 1 )
            {
                if ( m_SR != null )
                {
                    m_SR.sprite = AnimationData.Frames[0].SpFrame;
                }
                else if ( m_Image != null )
                {
                    m_Image.sprite = AnimationData.Frames[0].SpFrame;
                }
            }
            else
            {
                CLOG.E( "none frame to play!" );
                return;
            }

            m_IsPlaying = true;
        }

        /// <summary>
        /// 停止当前动画的播放
        /// </summary>
        /// <param name="SetFrameToZero">是否回到第一帧</param>
        public void StopNowAnimation( bool SetFrameToZero = false )
        {
            if ( !m_IsPlaying )
            {
                return;
            }

            if ( m_PlayCoroutine != null )
            {
                StopCoroutine( m_PlayCoroutine );
                m_PlayCoroutine = null;
            }

            //设置回到第一帧
            if ( SetFrameToZero )
            {
                if ( m_SR != null )
                {
                    m_SR.sprite = AnimationData.Frames[0].SpFrame;
                }
                else if ( m_Image != null )
                {
                    m_Image.sprite = AnimationData.Frames[0].SpFrame;
                }
            }

            m_IsPlaying = false;
        }


        /// <summary>
        /// 播放处理
        /// </summary>
        /// <returns>协程</returns>
        private IEnumerator PlayHandler()
        {
            while ( true )
            {
                //CLOG.I( "now play animation={0} index={1} ", Animations[m_NowPlayAnimIndex].AnimState.GetDescription(), m_PlayHeadIndex );

                if ( m_PlayHeadIndex < 0 || m_PlayHeadIndex >= AnimationData.Frames.Length )
                {
                    CLOG.E( "frame index error!!" );
                    break;
                }

                if ( m_SR != null )
                {
                    m_SR.sprite = AnimationData.Frames[m_PlayHeadIndex].SpFrame;
                }
                else if ( m_Image != null )
                {
                    m_Image.sprite = AnimationData.Frames[m_PlayHeadIndex].SpFrame;
                }

                /************************************************************************/
                /*                              回调处理                                */
                /************************************************************************/
                if ( AnimationData.FrameCallbacks.ContainsKey( m_PlayHeadIndex ) )
                {
                    //如果特定帧回调存在，那么调用它
                    if ( AnimationData.FrameCallbacks[m_PlayHeadIndex] != null )
                    {
                        AnimationData.FrameCallbacks[m_PlayHeadIndex]( m_PlayHeadIndex );
                    }
                }
                else if ( AnimationData.EveryFrameCallback != null )
                {
                    //如果特定帧回调不存在，而帧回调存在，那么调用帧回调
                    AnimationData.EveryFrameCallback( m_PlayHeadIndex );
                }

                //下一帧
                m_PlayHeadIndex++;

                //安全边界处理
                if ( m_PlayHeadIndex >= AnimationData.Frames.Length )
                {
                    if ( m_IsLoop )
                    {
                        m_PlayHeadIndex = 0;
                    }
                    else
                    {
                        break;
                    }
                }

                yield return new WaitForSeconds( AnimationData.Frames[m_PlayHeadIndex].SpInteval );
            }

            m_PlayCoroutine = null;
            m_IsPlaying = false;
        }


        /// <summary>
        /// 设置特定动画每一帧的回调
        /// </summary>
        /// <param name="callback">回调</param>
        public void SetEveryFrameCallBack(  DelegateAnimationFrameEvent callback )
        {
            AnimationData.SetEveryFrameCallBack( callback );
        }

        /// <summary>
        /// 添加特定动画特定帧回调
        /// </summary>
        /// <param name="FrameIndex">特定帧</param>
        /// <param name="callback"></param>
        public void AddFrameCallBack( int FrameIndex, DelegateAnimationFrameEvent callback )
        {
            AnimationData.AddFrameCallBack( FrameIndex, callback );
        }

        /// <summary>
        /// 移除特定动画特定帧的回调
        /// </summary>
        /// <param name="EAS">动画类型</param>
        /// <param name="FrameIndex">特定帧</param>
        public void DeleteFrameCallBack( int FrameIndex )
        {
            AnimationData.DeleteFrameCallBack( FrameIndex );
        }

        /// <summary>
        /// 移除特定动画所有帧回掉
        /// </summary>
        /// <param name="EAS">动画类型</param>
        public void DeleteAllFrameCallBack(  )
        {
            AnimationData.DeleteAllFrameCallBack( );
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {

        }
#endif

    }
}