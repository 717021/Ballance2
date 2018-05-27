using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 变球器动画类
 * 
 * 你可以继承此类 并 调用 BallsManager.RegisterBall 来注册自己球的变球器动画。
 */

namespace Assets.Scripts.GameCore
{
    /// <summary>
    /// 默认变球器动画
    /// </summary>
    public class AnimTranfo : MonoBehaviour
    {
        public AnimTranfo()
        {
            ballsManager = GlobalMediator.GetSystemServices(GameServices.BallsManager) as BallsManager;
            animTranfoMgr = GlobalMediator.GetSystemServices(GameServices.AnimTranfo) as AnimTranfoMgr;
        }

        /// <summary>
        /// 已自动获取到的 <see cref="BallsManager"/> 。
        /// </summary>
        protected BallsManager ballsManager;
        /// <summary>
        /// 已自动获取到的 <see cref="AnimTranfoMgr"/> 。
        /// </summary>
        protected AnimTranfoMgr animTranfoMgr;
        /// <summary>
        /// 当前球类型。
        /// </summary>
        protected BallsManager.RegBall targetBallType;


        /// <summary>
        /// 开始播放变球器动画
        /// </summary>
        /// <param name="balltype">球类型</param>
        /// <param name="regBall">球类型实例</param>
        /// <param name="pos">位置</param>
        public virtual void StartPlay(string balltype, BallsManager.RegBall regBall, Vector3 pos)
        {
            targetBallType = regBall;
        }
        /// <summary>
        /// 停止播放动画。你的动画播放完成以后需要调用此方法才能让球变球完成，才能继续。
        /// </summary>
        public virtual void EndPlay()
        {
            animTranfoMgr.SetTranfoAnimPlayEnd(this, targetBallType);
            ballsManager.StartControl();
        }
        /// <summary>
        /// 让球抛出碎片。
        /// 你可以继承重写此函数。
        /// 这一步不是必要的。
        /// </summary>
        public virtual void ThrowPeices()
        {
            animTranfoMgr.SetTranfoAnimThrowPeices(targetBallType);
        }
    }
}
