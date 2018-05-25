using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTranfo : MonoBehaviour
{
    public AnimTranfo()
    {
        ballsManager = GlobalMediator.GetSystemServices(GameServices.BallsManager) as BallsManager;
        animTranfoMgr = GlobalMediator.GetSystemServices(GameServices.AnimTranfo) as AnimTranfoMgr;
    }

    protected BallsManager ballsManager;
    protected AnimTranfoMgr animTranfoMgr;
    protected BallsManager.RegBall targetBallType;

    public virtual void StartPlay(string balltype, BallsManager.RegBall regBall, Vector3 pos)
    {
        targetBallType = regBall;
    }
    public virtual void EndPlay()
    {
        animTranfoMgr.SetTranfoAnimPlayEnd(this);
    }
    public virtual void ThrowPeices()
    {
        animTranfoMgr.SetTranfoAnimThrowPeices(targetBallType);
    }
}
