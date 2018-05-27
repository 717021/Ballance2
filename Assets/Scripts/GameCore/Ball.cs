using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 球 描述类
 */

namespace Assets.Scripts.GameCore
{

    /// <summary>
    /// 球推动类型
    /// </summary>
    public enum BallPushType
    {
        None,
        /// <summary>
        /// 前
        /// </summary>
        Forward = 2,
        /// <summary>
        /// 后
        /// </summary>
        Back = 4,
        /// <summary>
        /// 左
        /// </summary>
        Left = 8,
        /// <summary>
        /// 右
        /// </summary>
        Right = 16,
        /// <summary>
        /// 上
        /// </summary>
        Up = 32,
        /// <summary>
        /// 下
        /// </summary>
        Down = 64
    }

    /// <summary>
    /// 球
    /// </summary>
    public class Ball : MonoBehaviour
    {
        public float PushFroce = 3f;
        public bool UseFallForce = false;
        public float FallForce = 10f;

        public ForceMode FallFroaceMode = ForceMode.Force;
        public ForceMode ForceMode = ForceMode.Force;

        private Rigidbody rigidbodyCurrent;
        private bool debug = false;
        private BallsManager ballsManager;
        private Vector3 oldSpeed;

        private void Start()
        {
            rigidbodyCurrent = GetComponent<Rigidbody>();

#if UNITY_EDITOR
            debug = true;
#else
        debug = GlobalSettings.Debug;
#endif
            ballsManager = GlobalMediator.GetSystemServices(GameServices.BallsManager) as BallsManager;
        }

        /// <summary>
        /// 重新设置位置
        /// </summary>
        /// <param name="pos"></param>
        public virtual void Recover(Vector3 pos)
        {
            Deactive();
            gameObject.transform.position = pos;
            gameObject.transform.eulerAngles = Vector3.zero;
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
                rigidbodyCurrent.velocity = oldSpeed;
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
                oldSpeed = rigidbodyCurrent.velocity;
                if (!rigidbodyCurrent.isKinematic)
                    rigidbodyCurrent.isKinematic = true;
            }
        }

        /// <summary>
        /// 速度清零。
        /// </summary>
        public virtual void RemoveSpeed()
        {
            if (rigidbodyCurrent != null)
            {
                oldSpeed = Vector3.zero;
                rigidbodyCurrent.velocity = oldSpeed;
            }
        }

        /// <summary>
        /// 推动
        /// </summary>
        public virtual void BallPush()
        {
            if (ballsManager.isControl)
            {
                //压力?
                if (UseFallForce)
                {
                    rigidbodyCurrent.AddForce(Vector3.down * FallForce, FallFroaceMode);
                }
                //获取 ballsManager 的球推动类型。
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
                    //调试模式可以上下飞行
                    if (debug)
                    {
                        if ((currentBallPushType & BallPushType.Up) == BallPushType.Up) //上
                            rigidbodyCurrent.AddForce(Vector3.up * PushFroce * 2f, ForceMode);
                        else if ((currentBallPushType & BallPushType.Down) == BallPushType.Down)    //下
                            rigidbodyCurrent.AddForce(Vector3.down * PushFroce, ForceMode);
                    }
                }
            }
        }
    }
}
