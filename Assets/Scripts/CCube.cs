using UnityEngine;
using DG.Tweening;

public enum ECubeState
{
    IDLE,
    MOVEIN,
    MOVEOUT
}

/// <summary>
/// 盒子控制器
/// </summary>
public class CCube : MonoBehaviour
{
    /// <summary>
    /// 目标Y坐标
    /// </summary>
    private float TargetYPos = Mathf.Infinity;

    /// <summary>
    /// 出生Y坐标
    /// </summary>
    private float StartYPos = Mathf.Infinity;

    /// <summary>
    /// 是否安全
    /// </summary>
    public bool IsSafe { get; set; }

    /// <summary>
    /// 当前状态
    /// </summary>
    public ECubeState State { get; private set; }

    /// <summary>
    /// 刚体
    /// </summary>
    private Rigidbody m_Body = null;

    /// <summary>
    /// 是否被销毁了
    /// </summary>
    private bool m_IsDead = false;

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="startPosY"></param>
    /// <param name="targetPosY"></param>
    public void SetInfo ( float startPosY, float targetPosY )
    {
        TargetYPos = targetPosY;
        StartYPos = startPosY;
    }

    /// <summary>
    /// 开始
    /// </summary>
    private void Start()
    {
        transform.position = new Vector3 ( transform.position.x, StartYPos, transform.position.z );

        m_Body = GetComponentInChildren <Rigidbody>();
        State = ECubeState.IDLE;
        m_IsDead = false;
    }

    /// <summary>
    /// 合拢
    /// </summary>
    public void MoveIn()
    {
        if ( State != ECubeState.IDLE || m_IsDead )
        {
            return;
        }

        transform.DOMoveY ( TargetYPos, 1f ).OnComplete ( () =>
        {
            State = ECubeState.IDLE;
        } );

        State = ECubeState.MOVEIN;
    }

    /// <summary>
    /// 分开
    /// </summary>
    public void MoveOut()
    {
        if ( State != ECubeState.IDLE || m_IsDead )
        {
            return;
        }

        transform.DOLocalMoveY ( StartYPos, 1f ).OnComplete ( () =>
        {
            State = ECubeState.IDLE;
        } );

        State = ECubeState.MOVEOUT;
    }


    /// <summary>
    /// 销毁吧
    /// </summary>
    public void Drop()
    {
        m_IsDead = true;
        m_Body.constraints ^= RigidbodyConstraints.FreezePositionY;
        m_Body.useGravity = true;
        Destroy ( gameObject, 5f );
    }


}

