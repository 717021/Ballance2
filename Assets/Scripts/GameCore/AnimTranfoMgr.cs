using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 变球器动画管理器
 */

public class AnimTranfoMgr : MonoBehaviour
{
    public AnimTranfoMgr()
    {
        GlobalMediator.SetSystemServices(GameServices.AnimTranfo, this);
    }

    public AnimTranfo animTranfoDef;

    public void PlayTranfoAnim(BallsManager.RegBall ball, Vector3 pos,Vector3 eulerAngles)
    {
        if (ball.AnimTranfo != null)
        {
            ball.AnimTranfo.gameObject.transform.position = pos;
            ball.AnimTranfo.gameObject.transform.eulerAngles = eulerAngles;
            ball.AnimTranfo.gameObject.SetActive(true);
            ball.AnimTranfo.StartPlay(ball.TypeName, ball, pos);
        }
    }
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
    public void SetTranfoAnimPlayEnd(AnimTranfo anim)
    {
        anim.gameObject.SetActive(false);

    }
    public void SetTranfoAnimThrowPeices(BallsManager.RegBall ball)
    {
        if (ball != null) ball.Pieces.SendMessage("ThrowPieces");
    }
}