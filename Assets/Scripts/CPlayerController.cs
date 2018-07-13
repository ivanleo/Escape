using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Cinemachine;
using CoffeeBean;

public enum EActionType
{
    // Normal Motion
    NORMAL_STAND = 1,
    NORMAL_WALK = 2,
    NORMAL_RUN = 3,
    NORMAL_IDLE = 4,
    NORMAL_DAMAGE = 5,
    NORMAL_ITEMGET = 6,
    NORMAL_LOSE = 7,
    NORMAL_WIN = 8,
    NORMAL_FLY_IDLE = 9,
    NORMAL_FLY_STRAIGHT = 10,
    NORMAL_FLY_LEFT = 11,
    NORMAL_FLY_RIGHT = 12,
    NORMAL_FLY_UP = 13,
    NORMAL_FLY_DOWN = 14,
    NORMAL_FLY_CIRCLE = 15,
    NORMAL_FLY_TURNBACK = 16,

    // Normal Pose
    NORMAL_POSE_CUTE = 50,
    NORMAL_POSE_HELLO = 51,
    NORMAL_POSE_READY = 52,
    NORMAL_POSE_STOP = 53,
    NORMAL_POSE_BOW = 54,
    NORMAL_POSE_ARMCROSSED = 55,
    NORMAL_POSE_PLEASE = 56,
    NORMAL_POSE_SIT = 57,
    NORMAL_POSE_LAYDOWN = 58,
    NORMAL_POSE_ROMANCE = 59,

    // Black Motion
    BLACK_FIGHTING = 105,
    BLACK_PUNCH = 106,
    BLACK_KICK = 107,

    // Black Pose
    BLACK_POSE_1 = 150,
    BLACK_POSE_2 = 151,
    BLACK_POSE_3 = 152,

    // Osaka Motion
    OSAKA_TUKKOMI = 205,
    OSAKA_BOKE = 206,
    OSAKA_CLAP = 207,

    // Osaka Pose
    OSAKA_POSE_GOAL = 250,
    OSAKA_POSE_TEHEPERO = 251,
    OSAKA_POSE_EXIT = 252,

    // Fukuoka Motion
    FUKUOKA_DANCE_1 = 305,
    FUKUOKA_DANCE_2 = 306,
    FUKUOKA_WAIWAI = 307,

    // Fukuoka Pose
    FUKUOKA_POSE_1 = 350,
    FUKUOKA_POSE_2 = 351,
    FUKUOKA_POSE_HIRUNE = 352,

    // Hokkaido Motion
    HOKKAIDO_SNOWBALLING = 405,
    HOKKAIDO_CLIONE = 406,
    HOKKAIDO_IKADANCE = 407,

    // Hokkaido Pose
    HOKKAIDO_POSE_COLD = 450,
    HOKKAIDO_POSE_BEAMBITIOUS = 451,
    HOKKAIDO_POSE_BEAR = 452

}

public enum EDirection
{
    FORWARD,
    BACK
}

public class CPlayerController : MonoBehaviour
{

    /// <summary>
    /// 是否处于其他玩家的死亡区域
    /// </summary>
    private bool isInDeadArea = false;

    //玩家编号
    [SerializeField]
    private int m_PlayerID = 1;

    /// <summary>
    /// 死亡区域
    /// </summary>
    public SphereCollider m_DeadArea = null;

    //动画控制器
    private Animator m_AT = null;

    //刚体
    private Rigidbody m_Body = null;

    /// <summary>
    /// 移动速度
    /// </summary>
    [SerializeField]
    private float m_MoveForce = 10f;

    /// <summary>
    /// 跳跃力量
    /// </summary>
    [SerializeField]
    private float m_JumpForce = 500f;

    /// <summary>
    /// 脚下粒子
    /// </summary>
    private ParticleSystem m_FootPar = null;

    //是否在地面
    [ReadOnly]
    [SerializeField]
    private bool m_IsOnGround = false;

    /// <summary>
    /// 转身用时
    /// </summary>
    [SerializeField]
    private float m_TurnTime = 0.3f;

    /// <summary>
    /// 当前方向
    /// </summary>
    [ReadOnly]
    [SerializeField]
    private EDirection m_Dir = EDirection.FORWARD;

    /// <summary>
    /// 是否死亡
    /// </summary>
    public bool IsDead { get; set; }

    /// <summary>
    /// 开始X
    /// </summary>
    private static float m_StartX = 0f;
    public static float StartX { get { return m_StartX; } private set { m_StartX = value; } }

    /// <summary>
    /// 跑步的距离
    /// </summary>
    public float RunningDis { get; set; }

    /// <summary>
    /// 苏醒是
    /// </summary>
    private void Awake()
    {
        //得到死亡球区域，并隐藏
        m_DeadArea = GetComponent<SphereCollider>();
        m_DeadArea.enabled = false;

        m_AT = GetComponentInChildren<Animator>();
        m_Body = GetComponent<Rigidbody>();

        //开始X
        if ( transform.position.x > StartX )
        {
            StartX = transform.position.x;
        }
    }

    /// <summary>
    /// 固定更新
    /// </summary>
    private void FixedUpdate()
    {
        //死亡放弃
        if ( IsDead )
        {
            return;
        }

        //更新前进距离
        if ( transform.position.x - StartX > RunningDis )
        {
            RunningDis = transform.position.x - StartX;
        }


        //检查是否死亡
        if ( CheckDead() )
        {
            //死了

            //播放死亡音效
            CSoundManager.Instance.PlayEffect ( "fail" );
            //淡出背景音乐
            //GameObject.Find ( "BGSound" ).GetComponent<AudioSource>().DOFade ( 0f, 1f );

            //播放死亡动画
            m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_POSE_LAYDOWN );
            IsDead = true;
            return;
        }

        //输入轴
        float HInput = m_PlayerID == 1 ? Input.GetAxis ( "Horizontal" ) : Input.GetAxis ( "Horzontal2p" );

        if ( m_IsOnGround )
        {
            HandlerGroundMoving ( HInput )
        }
        else
        {
            HandlerAirMoving ( HInput );
        }

        if ( CheckIsInAir() )
        {
            ratio = 0.3f;
            m_Body.drag = 0.3f;

            if ( m_FootPar.isPlaying )
            { m_FootPar.Stop(); }
        }
        else
        {
            ratio = 1f;
            m_Body.drag = 0.05f;

            if ( !m_FootPar.isPlaying )
            { m_FootPar.Play(); }
        }


        if ( HInput > 0 )
        {
            //向前移动
            m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_RUN );
            m_Body.AddForce ( Vector3.right * m_MoveForce * ratio );

            if ( m_Dir == EDirection.BACK )
            {
                transform.DOKill();
                //转到前方
                transform.DORotate ( new Vector3 ( 0f, 90f, 0f ), m_TurnTime );
                m_Dir = EDirection.FORWARD;
            }
        }
        else if ( HInput < 0 )
        {
            //向后移动
            m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_RUN );
            m_Body.AddForce ( Vector3.left * m_MoveForce * ratio );

            if ( m_Dir == EDirection.FORWARD )
            {
                transform.DOKill();
                //转到后方
                transform.DORotate ( new Vector3 ( 0f, 270f, 0f ), m_TurnTime, RotateMode.FastBeyond360 );
                m_Dir = EDirection.BACK;
            }
        }
        else
        {
            m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_STAND );
        }



        if ( m_Body.velocity.x > 9 )
        {
            Vector3 temp = m_Body.velocity;
            temp.x = 9f;
            m_Body.velocity = temp;
        }

        if ( m_Body.velocity.y > 9 )
        {
            Vector3 temp = m_Body.velocity;
            temp.y = 9f;
            m_Body.velocity = temp;
        }

        //检查是否需要创建新的盒子
        CCubeManager.Instance.CheckCreateGrid ( transform.position.x );

        //Debug.Log ( m_Body.velocity );
    }


    /// <summary>
    /// 处理空中移动
    /// </summary>
    /// <param name="hInput"></param>
    private void HandlerAirMoving ( float hInput )
    {
        //添加水平力
        m_Body.AddForce ( Vector3.right * m_MoveForce * 0.3f );

        //检查是否落地
    }

    /// <summary>
    /// 处理地面移动
    /// </summary>
    /// <param name="hInput"></param>
    private void HandlerGroundMoving ( float hInput )
    {
        if ( m_PlayerID == 1 && Input.GetKeyDown ( KeyCode.W ) ||
                m_PlayerID == 2 && Input.GetKeyDown ( KeyCode.UpArrow ) )
        {
            Jump();
        }

        //添加水平力
        m_Body.AddForce ( Vector3.right * m_MoveForce  );
    }

    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        CLOG.I ( "Jumping" );
        CSoundManager.Instance.PlayEffect ( "jump" );

        m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_WIN );
        m_Body.AddForce ( Vector3.up * m_JumpForce );
        m_IsOnGround = false;

        //CParticleManager.PlaySmoke ( transform.position );

        CGame.Instance.JumpCount++;
    }

    ///// <summary>
    ///// 检查是否死亡
    ///// </summary>
    ///// <returns></returns>
    //private bool CheckDead()
    //{
    //    Ray ry = new Ray ( CheckPoint.position + Vector3.back * 5, Vector3.forward );
    //    RaycastHit ryHIt;

    //    if ( Physics.Raycast ( ry, out ryHIt, 10f, LayerMask.GetMask ( "Upper" ) ) )
    //    {
    //        if ( !ryHIt.transform.parent.GetComponent<CCubeController>().IsSafe )
    //        {
    //            CParticleManager.PlayHit ( ryHIt.point + Vector3.back * 1f );
    //            m_FootPar.Stop();

    //            if ( CGame.Instance.PlayMode == 1 )
    //            {
    //                CGame.Instance.DeadCount++;
    //                CUI_Relife.CreateUI();
    //            }
    //            else
    //            {
    //                //双人模式下死亡
    //                CGame.Instance.NowDeadCount++;
    //                CGame.Instance.DeadCount++;
    //                Debug.Log ( CGame.Instance.NowDeadCount );

    //                m_DeadArea.enabled = true;
    //                m_Body.isKinematic = true;
    //                var Par = CParticleManager.PlayNeedHelp ( transform.position );
    //                Par.transform.SetParent ( transform );

    //                CGame.Instance.DeadPeople = this;

    //            }

    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //private void OnCollisionEnter ( Collision collision )
    //{
    //    if ( collision.gameObject.layer == 9 && m_IsInAir )
    //    {
    //        m_IsInAir = false;
    //        m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_STAND );

    //        CParticleManager.PlaySmoke ( transform.position );
    //    }
    //    else if ( collision.gameObject.layer == 11 )
    //    {
    //        Vector3 force1 = UnityEngine.Random.insideUnitSphere.normalized ;
    //        Vector3 force2 = UnityEngine.Random.insideUnitSphere.normalized;
    //        force1.y = Mathf.Abs ( force1.y );
    //        force2.y = Mathf.Abs ( force1.y );
    //        force1.z = 0f;
    //        force2.z = 0f;

    //        int force = 300;
    //        m_Body.AddForce ( force1 * force );
    //        collision.rigidbody.AddForce ( force2 * force );
    //    }
    //}

    //private bool CheckIsInAir()
    //{
    //    Ray ry = new Ray ( CheckPoint.position, Vector3.down * 2 );

    //    m_IsInAir =  !Physics.Raycast ( ry, 2f, LayerMask.GetMask ( "Floor" ) );


    //}

    //public void ReLife()
    //{
    //    CLOG.I ( "5" );
    //    m_Body.isKinematic = false;
    //    Jump();
    //    CGame.Instance.NowDeadCount--;
    //    GameObject.Find ( "BGSound" ).GetComponent<AudioSource>().DOFade ( 1f, 1f );
    //    IsDead = false;
    //    CParticleManager.PlayRelife ( transform.position );
    //    CSoundManager.Instance.PlayEffect ( "relife" );
    //}



    //private void OnTriggerEnter ( Collider other )
    //{
    //    if (  other.gameObject.layer == 11 )
    //    {
    //        isInDeadArea = true;
    //    }
    //}

    //private void OnTriggerExit ( Collider other )
    //{
    //    if ( other.gameObject.layer == 11 )
    //    {
    //        isInDeadArea = false;
    //    }
    //}


}
