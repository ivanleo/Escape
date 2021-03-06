﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CoffeeBean
{

    /// <summary>
    /// 异步任务系统
    /// </summary>
    public class CMissionSystem : CSingletonMono<CMissionSystem>
    {
        //所有的任务序列
        private List<CMissionSequence> MissionSeqs = null;

#if UNITY_EDITOR
        [ReadOnly]
        [SerializeField]
        private int m_SequenceCount = 0;
#endif

        /// <summary>
        /// 构造函数
        /// </summary>
        public CMissionSystem()
        {
            MissionSeqs = new List<CMissionSequence>();
        }

        /// <summary>
        /// 创建序列
        /// </summary>
        /// <returns></returns>
        public static CMissionSequence CreateSequence()
        {
            CMissionSequence CMS = new CMissionSequence();
            Instance.MissionSeqs.Add( CMS );
            return CMS;
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        private void Update()
        {
            for ( int i = 0 ; i < MissionSeqs.Count ; i++ )
            {
                if ( MissionSeqs[i].Update( ) )
                {
                    CLOG.I( "the mission Seq index {0} has finish", i );
                    MissionSeqs.RemoveAt( i );
                }
            }

#if UNITY_EDITOR
            Instance.m_SequenceCount = Instance.MissionSeqs.Count;
#endif
        }

    }

    /// <summary>
    /// 任务
    /// </summary>
    public class CMission
    {
        /// <summary>
        /// 完成回调
        /// </summary>
        private Action m_CompleteCallBack = null;

        /// <summary>
        /// 任务是否已完成
        /// </summary>
        private bool m_HasFinish = false;

        /// <summary>
        /// 任务是否已完成
        /// </summary>
        public bool HasFinish
        {
            get { return m_HasFinish; }
            set { m_HasFinish = value; }
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        public virtual void Start()
        {
            m_HasFinish = false;
            OnStart();
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        public void Start( Action CompleteCallBack )
        {
            m_HasFinish = false;
            m_CompleteCallBack = CompleteCallBack;
            OnStart();
        }

        /// <summary>
        /// 开始时
        /// 子类需要重写来实现自己的任务
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// 完成任务
        /// 子类在合适的时候调用此函数来控制任务队列向后移动
        /// </summary>
        public virtual void Finish()
        {
            m_HasFinish = true;
            OnFinish();

            if ( m_CompleteCallBack != null )
            {
                m_CompleteCallBack();
            }
        }

        /// <summary>
        /// 结束时
        /// 子类需要重写来实现自己的任务
        /// </summary>
        public virtual void OnFinish() { }

        /// <summary>
        /// 加入同步任务方法
        /// </summary>
        public virtual void Join( CMission Mission ) { }

        /// <summary>
        /// 更新状态
        /// </summary>
        public virtual void UpdateState( ) { }

    }

    /// <summary>
    /// 任务堆
    /// 一堆可一起执行的任务
    /// </summary>
    public class CMissionHeap: CMission
    {
        /// <summary>
        /// 同步执行的任务堆
        /// </summary>
        private List<CMission> Missions = null;

        /// <summary>
        /// 创建一个任务堆
        /// </summary>
        /// <param name=""></param>
        public static CMissionHeap Create( CMission Mission )
        {
            CMissionHeap CMH = new CMissionHeap();
            CMH.Missions = new List<CMission>();
            CMH.Missions.Add( Mission );
            return CMH;
        }

        /// <summary>
        /// 开始
        /// </summary>
        public override void Start()
        {
            for ( int i = 0 ; i < Missions.Count ; i++ )
            {
                Missions[i].Start();
            }
        }

        /// <summary>
        /// 添加一个同步任务到队列
        /// </summary>
        /// <param name="Mission"></param>
        public override void Join( CMission Mission )
        {
            Missions.Add( Mission );
        }

        /// <summary>
        /// 检查任务堆是否完成
        /// </summary>
        public override void UpdateState( )
        {
            if ( HasFinish )
            {
                return;
            }

            for ( int i = 0 ; i < Missions.Count ; i++ )
            {
                if ( !Missions[i].HasFinish )
                {
                    HasFinish = false;
                    return;
                }
            }

            HasFinish = true;
        }
    }

    /// <summary>
    /// 任务回调
    /// </summary>
    public class CMissionCallFunc : CMission
    {
        /// <summary>
        /// 回调函数
        /// </summary>
        private Action m_Action = null;

        /// <summary>
        /// 创建方法
        /// </summary>
        /// <param name="AC"></param>
        /// <returns></returns>
        public static CMissionCallFunc Create( Action AC )
        {
            CMissionCallFunc CMCF = new CMissionCallFunc();
            CMCF.m_Action = AC;
            return CMCF;
        }

        /// <summary>
        /// 开始时
        /// </summary>
        public override void OnStart()
        {
            if ( m_Action != null )
            {
                m_Action();
            }

            Finish();
        }
    }

    /// <summary>
    /// 任务延时
    /// </summary>
    public class CMissionInteval : CMission
    {
        /// <summary>
        /// 延时
        /// </summary>
        private float DelayTime = 0f;

        /// <summary>
        /// 当前经过时间
        /// </summary>
        private float NowTime = 0f;

        /// <summary>
        /// 创建延时
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static CMissionInteval Create( float delayTime )
        {
            CMissionInteval CMI = new CMissionInteval();
            CMI.DelayTime = delayTime;
            CMI.NowTime = 0f;
            return CMI;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public override void UpdateState()
        {
            if ( NowTime < DelayTime )
            {
                NowTime += Time.deltaTime;
                if ( NowTime >= DelayTime )
                {
                    Finish();
                }
            }
        }
    }

    /// <summary>
    /// 任务序列
    /// </summary>
    public class CMissionSequence
    {
        /// <summary>
        /// 任务队列
        /// </summary>
        private Queue<CMission> m_MissionQueue = new Queue<CMission>();

        /// <summary>
        /// 当前处理的任务
        /// </summary>
        private CMission m_NowExeMission = null;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool IsRunning = false;

        /// <summary>
        /// 是否完成了
        /// </summary>
        private bool IsFinish = false;

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            IsRunning = true;
            IsFinish = false;
        }

        /// <summary>
        /// 结束
        /// </summary>
        public void Finish()
        {
            IsRunning = false;
            IsFinish = true;
        }

        /// <summary>
        /// 增加一个任务到队列
        /// </summary>
        /// <param name=""></param>
        public void Append( CMission Mission )
        {
            CMissionHeap missionHeap =  CMissionHeap.Create( Mission );
            m_MissionQueue.Enqueue( missionHeap );
        }

        /// <summary>
        /// 添加一个同步任务到队列
        /// </summary>
        /// <param name="Mission"></param>
        public void Join( CMission Mission )
        {
            CMission CM = m_MissionQueue.Last();
            if ( CM is CMissionHeap )
            {
                CM.Join( Mission );
            }
            else
            {
                CLOG.E( "the last mission is not mission heap,so can not join mission to it" );
            }

        }

        /// <summary>
        /// 增加一个函数调用
        /// </summary>
        public void AppendCallFunc( Action Callfunc  )
        {
            CMissionCallFunc CMCF = CMissionCallFunc.Create( Callfunc );
            m_MissionQueue.Enqueue( CMCF );
        }

        /// <summary>
        /// 增加一个延时
        /// </summary>
        /// <param name="WaitTime"></param>
        public void AppendInteval( float WaitTime )
        {
            CMissionInteval CMI = CMissionInteval.Create( WaitTime );
            m_MissionQueue.Enqueue( CMI );
        }



        /// <summary>
        /// 每帧刷新
        /// 如果本任务序列被执行完毕了，则返回true
        /// 未执行完毕，则返回false
        /// </summary>
        public bool Update( )
        {
            if ( IsFinish )
            {
                return true;
            }

            if ( IsRunning )
            {
                if ( m_NowExeMission != null )
                {
                    //更新当前了任务状态
                    m_NowExeMission.UpdateState( );

                    //队列为空，且任务堆已完成，代表队列执行完毕
                    if ( m_MissionQueue.Count == 0 && m_NowExeMission.HasFinish )
                    {
                        Finish();
                        return true;
                    }
                }

                //检查是否需要进入下一个任务
                if ( m_MissionQueue.Count > 0 && CanExecuteNextMission() )
                {
                    ExecuteNextMission();
                }


            }

            return false;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        private bool ExecuteNextMission()
        {
            //任务队列为空，当前任务为空，则代表无法进行下一步
            if ( m_MissionQueue.Count == 0 && m_NowExeMission == null )
            {
                return false;
            }

            m_NowExeMission = m_MissionQueue.Dequeue();
            m_NowExeMission.Start();

            return true;
        }

        /// <summary>
        /// 是否能执行下一个任务堆
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteNextMission()
        {
            if ( m_NowExeMission == null )
            {
                return true;
            }

            if ( m_NowExeMission.HasFinish )
            {
                return true;
            }

            return false;
        }
    }




}
