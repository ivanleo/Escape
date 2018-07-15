using CoffeeBean;
using DG.Tweening;
using UnityEngine;

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

    /// <summary>
    /// 玩家编号
    /// </summary>
    [SerializeField]
    private int m_PlayerID = 1;

    /// <summary>
    /// 身体碰撞区
    /// </summary>
    private CapsuleCollider m_CapCollider = null;

    /// <summary>
    /// 死亡区域
    /// </summary>
    private SphereCollider m_DeadArea = null;

    /// <summary>
    /// 动画控制器
    /// </summary>
    private Animator m_AT = null;

    /// <summary>
    /// 刚体
    /// </summary>
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
    /// 玩家的半身高
    /// </summary>
    private float m_HalfPlayerHeight;

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

        m_CapCollider = GetComponent<CapsuleCollider>();

        m_AT = GetComponentInChildren<Animator>();
        m_Body = GetComponent<Rigidbody>();

        m_HalfPlayerHeight = m_CapCollider.height / 2;


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
        float HInput = m_PlayerID == 1 ? Input.GetAxisRaw ( "Horizontal" ) : Input.GetAxisRaw ( "Horzontal2p" );

        //处理旋转
        HandlerDirection ( HInput );

        //更新地面状态
        m_IsOnGround = CheckOnLand();

        //处理水平移动和是否在地面的状态修改
        if ( m_IsOnGround )
        {
            HandlerGroundMoving ( HInput );
        }
        else
        {
            HandlerAirMoving ( HInput );
        }

        //限制速度
        LimitVelocity();

        m_AT.SetFloat ( "YSpeed", m_Body.velocity.y );
        m_AT.SetBool ( "IsOnGround", m_IsOnGround );
        m_AT.SetBool ( "IsMove", HInput != 0f );

        //检查是否需要创建新的盒子
        //CCubeManager.Instance.CheckCreateGrid ( transform.position.x );
    }

    /// <summary>
    /// 限制速度
    /// </summary>
    private void LimitVelocity()
    {
        Vector3 temp = m_Body.velocity;

        if ( temp.x > 9 ) { temp.x = 9; }

        if ( temp.x < -9 ) { temp.x = -9; }

        m_Body.velocity = temp;
    }

    /// <summary>
    /// 处理空中移动
    /// </summary>
    /// <param name="hInput"></param>
    private void HandlerAirMoving ( float hInput )
    {
        //添加水平力
        m_Body.AddForce ( Vector3.right * m_MoveForce * hInput );
    }

    /// <summary>
    /// 处理地面移动
    /// </summary>
    /// <param name="hInput"></param>
    private void HandlerGroundMoving ( float hInput )
    {
        //添加水平力
        m_Body.AddForce ( Vector3.right * m_MoveForce * hInput );

        if ( m_PlayerID == 1 && Input.GetKeyDown ( KeyCode.W ) ||
                m_PlayerID == 2 && Input.GetKeyDown ( KeyCode.UpArrow ) )
        {
            Jump();
        }
    }

    /// <summary>
    /// 检查是否落地
    /// </summary>
    private bool CheckOnLand()
    {
        float Length = 0.1f;
        Ray ry = new Ray ( transform.position + m_HalfPlayerHeight * Vector3.up, Vector3.down );
        return Physics.Raycast ( ry, m_HalfPlayerHeight + Length, LayerMask.GetMask ( "FloorCube" ) );
    }

    /// <summary>
    /// 检查是否碰到了不安全的上方箱子
    /// </summary>
    /// <returns></returns>
    private bool CheckHitUnSafeUpper()
    {
        float Length = 0.1f;
        Ray ry = new Ray ( transform.position + m_HalfPlayerHeight * Vector3.up, Vector3.up );
        return Physics.Raycast ( ry, m_HalfPlayerHeight + Length, LayerMask.GetMask ( "UpperCube" ) );
    }

    /// <summary>
    /// 处理方向
    /// </summary>
    private void HandlerDirection ( float HInput )
    {
        if ( HInput > 0 )
        {
            //向前移动
            m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_RUN );

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
    }



    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        CLOG.I ( "Jumping" );
        CSoundManager.Instance.PlayEffect ( "jump" );

        m_AT.SetInteger ( "AnimIndex", ( int ) EActionType.NORMAL_FLY_UP );
        Vector3 Temp = m_Body.velocity;
        Temp.y = 0f;
        Temp += Vector3.up * m_JumpForce;
        m_Body.velocity = Temp;
        m_IsOnGround = false;

        //CParticleManager.PlaySmoke ( transform.position );

        CGame.Instance.JumpCount++;
    }

    /// <summary>
    /// 检查是否死亡
    /// </summary>
    /// <returns></returns>
    private bool CheckDead()
    {
        if ( !CGame.Instance.IsDangerous )
        {
            return false;
        }

        Ray ry = new Ray ( transform.position + m_HalfPlayerHeight * Vector3.up, Vector3.up );
        RaycastHit ryHIt;

        if ( Physics.Raycast ( ry, out ryHIt, 0.3f + m_HalfPlayerHeight, LayerMask.GetMask ( "UpperCube" ) ) )
        {
            var cube = ryHIt.transform.GetComponent<CCube>();

            if ( cube.IsSafe )
            {
                return false;
            }

            var GameRef = CGame.Instance;

            if ( GameRef.PlayMode == EPlayMode.PLAY_SINGLE )
            {
                GameRef.DeadCount++;
                CUI_Relife.CreateUI();
            }
            else
            {
                //双人模式下死亡
                GameRef.DeadCount++;
            }

            return true;
        }

        return false;
    }


}
