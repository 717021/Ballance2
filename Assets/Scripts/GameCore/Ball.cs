using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 球 描述类
 */

/// <summary>
/// 球推动类
/// </summary>
public enum BallPushType
{
    None,
    Forward = 2,
    Back = 4,
    Left = 8,
    Right = 16,
    Up = 32,
    Down = 64
}

/// <summary>
/// 球
/// </summary>
public class Ball : MonoBehaviour
{
    public float PushFroce = 3f;
    public ForceMode ForceMode = ForceMode.Force;

    private Rigidbody rigidbodyCurrent;
    private bool debug = false;
    private BallsManager ballsManager;

    private void Start()
    {
        rigidbodyCurrent = GetComponent<Rigidbody>();
        debug = GlobalSettings.Debug;
        ballsManager = GlobalMediator.GetSystemServices(GameServices.BallsManager) as BallsManager;
        if (ballsManager == null) ballsManager = BallsManager.StaticBallsManager;
    }
    private void Update()
    {
    }

    /// <summary>
    /// 重新设置位置
    /// </summary>
    /// <param name="pos"></param>
    public virtual void Recover(Vector3 pos)
    {
        Deactive();
        gameObject.transform.position = pos;
        gameObject.transform.eulerAngles=Vector3.zero;
    }
    /// <summary>
    /// 激活
    /// </summary>
    /// <param name="pos"></param>
    public virtual void Active(Vector3 pos)
    {
        gameObject.transform.position = pos;
        if (rigidbodyCurrent != null)
        {
            rigidbodyCurrent.WakeUp();
            if (rigidbodyCurrent.isKinematic)
                rigidbodyCurrent.isKinematic = false;
        }
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 使其不活动
    /// </summary>
    public virtual void Deactive()
    {
        if (rigidbodyCurrent != null)
            rigidbodyCurrent.Sleep();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 开始控制
    /// </summary>
    public virtual void StartControl()
    {
        if (rigidbodyCurrent != null)
        {
            rigidbodyCurrent.WakeUp();
            if (rigidbodyCurrent.isKinematic)
                rigidbodyCurrent.isKinematic = false;
        }
    }
    /// <summary>
    /// 取消控制
    /// </summary>
    public virtual void EndControl()
    {
        if (rigidbodyCurrent != null)
        {
            rigidbodyCurrent.Sleep();
            if (!rigidbodyCurrent.isKinematic)
                rigidbodyCurrent.isKinematic = true;
        }
    }

    /// <summary>
    /// 推动
    /// </summary>
    public virtual void BallPush()
    {
        if (ballsManager.isControl)
        {
            BallPushType currentBallPushType = ballsManager.pushType;
            if (currentBallPushType != BallPushType.None)
            {
                if ((currentBallPushType & BallPushType.Forward) == BallPushType.Forward)
                {
                    rigidbodyCurrent.AddForce(ballsManager.thisVector3Fornt * PushFroce, ForceMode);
                }
                else if ((currentBallPushType & BallPushType.Back) == BallPushType.Back)
                {
                    rigidbodyCurrent.AddForce(ballsManager.thisVector3Back * PushFroce, ForceMode);
                }
                if ((currentBallPushType & BallPushType.Left) == BallPushType.Left)
                {
                    rigidbodyCurrent.AddForce(ballsManager.thisVector3Left * PushFroce, ForceMode);
                }
                else if ((currentBallPushType & BallPushType.Right) == BallPushType.Right)
                {
                    rigidbodyCurrent.AddForce(ballsManager.thisVector3Right * PushFroce, ForceMode);
                }
                if (debug)
                {
                    if ((currentBallPushType & BallPushType.Up) == BallPushType.Up)
                        rigidbodyCurrent.AddForce(Vector3.up * this.PushFroce *  5f, 0);
                    else if ((currentBallPushType & BallPushType.Down) == BallPushType.Down)
                        rigidbodyCurrent.AddForce(Vector3.down * this.PushFroce);
                }
            }
        }
    }
}
