using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 变球器动画管理器
 */
namespace Assets.Scripts.GameCore
{
    /// <summary>
    /// 变球器动画管理器
    /// </summary>
    public class AnimTranfoMgr : MonoBehaviour
    {
        public AnimTranfoMgr()
        {
            GlobalMediator.SetSystemServices(GameServices.AnimTranfo, this);
        }

        //默认 变球器动画
        public AnimTranfo animTranfoDef;

        /// <summary>
        /// 使用球注册实例开始播放变球器动画。
        /// </summary>
        /// <param name="ball">球注册实例</param>
        /// <param name="pos">播放的位置</param>
        /// <param name="eulerAngles">校正旋转</param>
        public void PlayTranfoAnim(BallsManager.RegBall ball, Vector3 pos, Vector3 eulerAngles)
        {
            if (ball.AnimTranfo != null)
            {
                ball.AnimTranfo.gameObject.transform.position = pos;
                ball.AnimTranfo.gameObject.transform.eulerAngles = eulerAngles;
                ball.AnimTranfo.gameObject.SetActive(true);
                ball.AnimTranfo.StartPlay(ball.TypeName, ball, pos);
            }
        }
        /// <summary>
        /// 使用变球器动画实例开始播放变球器动画。
        /// </summary>
        /// <param name="anim">变球器动画实例</param>
        /// <param name="pos">播放的位置</param>
        /// <param name="eulerAngles">校正旋转</param>
        public void PlayTranfoAnim(AnimTranfo anim, Vector3 pos, Vector3 eulerAngles)
        {
            if (anim != null)
            {
                anim.gameObject.transform.position = pos;
                anim.gameObject.transform.eulerAngles = eulerAngles;
                anim.gameObject.SetActive(true);
                anim.StartPlay(null, null, pos);
            }
        }
        /// <summary>
        /// 设置变球器动画播放完成。
        /// </summary>
        /// <param name="anim">变球器动画实例</param>
        public void SetTranfoAnimPlayEnd(AnimTranfo anim, BallsManager.RegBall ball)
        {
            anim.gameObject.SetActive(false);
            if (ball != null)
            {
                //播放 抛出碎片
                if (ball.Pieces != null)
                    SetTranfoAnimThrowPeices(ball);
            }
        }
        /// <summary>
        /// 抛出碎片。
        /// </summary>
        /// <param name="ball">球碎片</param>
        public void SetTranfoAnimThrowPeices(BallsManager.RegBall ball)
        {
            if (ball != null && ball.Pieces != null) ball.Pieces.SendMessage("ThrowPieces");
        }
    }
}