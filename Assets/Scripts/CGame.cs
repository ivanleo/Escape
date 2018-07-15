using System.Collections;
using System.Collections.Generic;
using CoffeeBean;
using DG.Tweening;
using UnityEngine;

public enum EPlayMode
{
    /// <summary>
    /// 单人模式
    /// </summary>
    PLAY_SINGLE = 1,

    /// <summary>
    /// 双人模式
    /// </summary>
    PLAY_DOUBLE = 2
}

/// <summary>
/// 格子信息
/// </summary>
public struct SGrid
{
    public GameObject _UpperCube;
    public GameObject _DownerCube;
}

/// <summary>
/// 游戏控制类
/// </summary>
public class CGame: CSingleton<CGame>
{
    public static int LAYER_UPPER = 9;
    public static int LAYER_DOWN = 10;
    public static int LAYER_PLAYER = 11;

    /// <summary>
    /// 是否在运行
    /// </summary>
    private bool IsRunning { get; set; }

    /// <summary>
    /// 游戏模式
    /// </summary>
    public EPlayMode PlayMode { get; private set; }

    /// <summary>
    /// 管理的格子
    /// </summary>
    private Queue<SGrid> Grids { get; set; }

    /// <summary>
    /// 跳跃次数
    /// </summary>
    public int JumpCount { get;  set; }

    /// <summary>
    /// 死亡次数
    /// </summary>
    public int DeadCount { get; set; }


    /// <summary>
    /// 站起次数
    /// </summary>
    public int StandCount { get; set; }

    /// <summary>
    /// 是否危险时刻
    /// </summary>
    public bool IsDangerous { get; set; }

    /// <summary>
    /// 当前格子序号
    /// </summary>
    private int _nowGridIndex;

    /// <summary>
    /// 最后一个Ypos
    /// </summary>
    private int _lastYPos;

    /// <summary>
    /// 安全的格子数
    /// </summary>
    private const int SAFE_GRID_COUNT = 30;

    /// <summary>
    /// 上一个安全的序号
    /// </summary>
    private int _lastSafeIndex;

    /// <summary>
    /// 上方节点
    /// </summary>
    private Transform Root_Upper = null;

    /// <summary>
    /// 下方节点
    /// </summary>
    private Transform Root_Downer = null;

    /// <summary>
    /// 是否可销毁尾巴上的格子
    /// </summary>
    private bool CanDestroyPeekGrid = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    public CGame()
    {
        IsRunning = false;
    }

    /// <summary>
    /// 初始化游戏
    /// 进入场景之前调用
    /// </summary>
    /// <param name="EPM"></param>
    public void InitGame ( EPlayMode EPM )
    {
        PlayMode = EPM;
        _nowGridIndex = 0;
        _lastSafeIndex = 0;

        JumpCount = 0;
        Grids = new Queue<SGrid>();
    }

    /// <summary>
    /// 开始游戏
    /// 进入场景之后调用
    /// </summary>
    public void StartGame(  )
    {
        if ( Grids == null )
        {
            InitGame ( EPlayMode.PLAY_SINGLE );
        }

        if ( CSceneManager.Instance.GetRunningScene() != "03_Game" )
        {
            CLOG.E ( "the scene is not game!" );
            return;
        }

        Root_Upper = GameObject.Find ( "GridNode/Upper" ).transform;
        Root_Downer = GameObject.Find ( "GridNode/Downer" ).transform;

        CreateFirstGrid();
    }

    /// <summary>
    /// 格子开始合拢操作
    /// </summary>
    public void StartGridClose()
    {
        CanDestroyPeekGrid = true;
        CCoroutineManager.Instance.RunCoroutine ( GridClose() );
    }

    /// <summary>
    /// 合拢操作
    /// </summary>
    /// <returns></returns>
    private IEnumerator GridClose()
    {
        float minInteval = CConfig.GetValue ( "move_in_inteval_min" );
        float maxInteval = CConfig.GetValue ( "move_in_inteval_max" );

        float minWait = CConfig.GetValue ( "move_in_wait_min" );
        float maxWait = CConfig.GetValue ( "move_in_wait_max" );

        while ( true )
        {
            //等待一定时间
            yield return new WaitForSeconds ( CMath.Rand ( minInteval, maxInteval )  );

            //设置不允许掉落尾巴上的格子
            CanDestroyPeekGrid = false;

            //振动上方格子
            for ( int i = 0 ; i < Root_Upper.childCount ; i++ )
            {
                Root_Upper.GetChild ( i ).DOShakePosition ( 1f, 0.1f, 30, 90, false, true );
            }

            //等待1秒
            yield return new WaitForSeconds ( 1f );

            //上下格子合拢
            for ( int i = 0 ; i < Root_Upper.childCount ; i++ )
            {
                CCube Down = Root_Upper.GetChild ( i ).GetComponent<CCube>();
                CCube up = Root_Downer.GetChild ( i ).GetComponent<CCube>();

                Down.MoveIn();
                up.MoveIn();
            }

            yield return new WaitForSeconds ( 0.8f );
            IsDangerous = true;

            yield return new WaitForSeconds ( 0.8f );
            IsDangerous = false;

            //随机等待一定时间
            yield return new WaitForSeconds ( CMath.Rand ( minWait, maxWait ) - 0.6f );

            //上方格子震动
            for ( int i = 0 ; i < Root_Upper.childCount ; i++ )
            {
                Root_Upper.GetChild ( i ).DOShakePosition ( 1f, 0.1f, 30, 90, false, true );
            }

            //等待1秒
            yield return new WaitForSeconds ( 1f );

            //格子分开
            for ( int i = 0 ; i < Root_Upper.childCount ; i++ )
            {
                CCube Down = Root_Upper.GetChild ( i ).GetComponent<CCube>();
                CCube up = Root_Downer.GetChild ( i ).GetComponent<CCube>();

                Down.MoveOut();
                up.MoveOut();
            }

            //等待1秒
            yield return new WaitForSeconds ( 1f );

            //设置尾巴上的格子可以掉落
            CanDestroyPeekGrid = false;
        }

    }

    /// <summary>
    /// 创建第一批障碍
    /// </summary>
    private void CreateFirstGrid()
    {
        for ( int i = 0 ; i < 40 ; i++ )
        {
            Grids.Enqueue ( CreateGrid() );
        }
    }

    /// <summary>
    /// 创建一个格子
    /// </summary>
    /// <returns></returns>
    public SGrid CreateGrid()
    {
        SGrid grid = new SGrid();
        GameObject Upper = CreateCube();
        GameObject Downer = CreateCube();

        Upper.layer = LayerMask.NameToLayer ( "UpperCube" );
        Downer.layer = LayerMask.NameToLayer ( "FloorCube" );

        CCube UpperCube = Upper.AddComponent<CCube>();
        CCube DownerCube = Downer.AddComponent<CCube>();

        int xpos = _nowGridIndex * 2;
        float zpos = 0f;
        int ypos;
        int SafeOffset;

        if ( _nowGridIndex < SAFE_GRID_COUNT )
        {
            //前10个Y轴固定为10
            ypos = 9;
            SafeOffset = 2;
            UpperCube.IsSafe = true;
        }
        else
        {
            //后面的依照前面的进行偏移
            int offset = CMath.Rand ( -2, 3 );
            ypos = _lastYPos + offset;

            bool isSafe = CheckIsSafe();

            if ( isSafe ) { _lastSafeIndex = _nowGridIndex; }

            UpperCube.IsSafe = isSafe;
            SafeOffset = isSafe ? CMath.Rand ( 1, 3 ) : 0 ;
        }

        _lastYPos = ypos;

        //下方 目标坐标
        int downTargetPosY = ypos - 10;

        //上方 目标坐标
        int upTargetPosY = ypos + 10 + SafeOffset;

        //下方 开始坐标
        int downStartPosY = downTargetPosY - 3;

        //上方开始坐标
        int upStartPosY = upTargetPosY + 3;

        Upper.transform.position = new Vector3 ( xpos, upStartPosY, zpos );
        Downer.transform.position = new Vector3 ( xpos, downStartPosY, zpos );

        Upper.transform.SetParent ( Root_Upper );
        Downer.transform.SetParent ( Root_Downer );

        UpperCube.SetInfo ( upStartPosY, upTargetPosY );
        DownerCube.SetInfo ( downStartPosY, downTargetPosY );

        grid._UpperCube = Upper;
        grid._DownerCube = Downer;

        _nowGridIndex++;

        return grid;
    }

    /// <summary>
    /// 检查是否安全
    /// </summary>
    /// <returns></returns>
    private bool CheckIsSafe()
    {
        if ( _nowGridIndex < SAFE_GRID_COUNT )
        {
            return true;
        }

        int offset = _nowGridIndex - _lastSafeIndex;
        int randomValue = CMath.Rand ( 20 );
        return randomValue <= offset;
    }

    /// <summary>
    /// 创建格子
    /// </summary>
    /// <returns></returns>
    public GameObject CreateCube()
    {
        if ( CMath.CanRatioBingo ( 50, EPrecentType.PRECENT_100 ) )
        {
            return CPrefabManager.Instance.CreatePrefabInstance ( "BlackCube" );
        }

        return CPrefabManager.Instance.CreatePrefabInstance ( "WhiteCube" );

    }

}

