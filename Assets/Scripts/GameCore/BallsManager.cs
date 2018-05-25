using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 核心模块： 管理球的工作
 */

/// <summary>
/// 球工作管理器
/// </summary>
public class BallsManager : MonoBehaviour
{
    private bool isKeyF1Down, isKeyF2Down, isKeyF3Down, isKeyF4Down;

    public GameObject ballCamMoveY;
    public Camera ballCamera;
    public GameObject ballCamFollowHost;
    public GameObject ballWood, ballStone, ballPaper;
    public GameObject ballPeicesWood, ballPeicesStone, ballPeicesPaper;
    public Color ballWoodTranfoColor, ballStoneTranfoColor, ballPaperTranfoColor;

    public float camFollowSmothing = 5f;
    public float camFollowSpeed = 0.05f;

    public BallsManager()
    {
        thisVector3Right = Vector3.right;
        thisVector3Left = Vector3.left;
        thisVector3Fornt = Vector3.forward;
        thisVector3Back = Vector3.back;

        GlobalMediator.SetSystemServices(GameServices.BallsManager, this);
    }

    private void Start()
    {
        RegisterBall("BallWood", ballWood, ballPeicesWood, ballWoodTranfoColor);
        RegisterBall("BallStone", ballStone, ballPeicesStone, ballStoneTranfoColor);
        RegisterBall("BallPaper", ballPaper, ballPeicesPaper, ballPaperTranfoColor);

        keyListener.AddKeyListen(KeyCode.UpArrow, new KeyListener.VoidDelegate(UpArrow_Key));
        keyListener.AddKeyListen(KeyCode.DownArrow, new KeyListener.VoidDelegate(DownArrow_Key));
        keyListener.AddKeyListen(KeyCode.LeftArrow, new KeyListener.VoidDelegate(LeftArrow_Key));
        keyListener.AddKeyListen(KeyCode.RightArrow, new KeyListener.VoidDelegate(RightArrow_Key));
        keyListener.AddKeyListen(KeyCode.Keypad1, new KeyListener.VoidDelegate(Home_Key));
        keyListener.AddKeyListen(KeyCode.Keypad0, new KeyListener.VoidDelegate(End_Key));
        keyListener.AddKeyListen(KeyCode.Space, new KeyListener.VoidDelegate(Space_Key));
        keyListener.AddKeyListen(KeyCode.LeftShift, new KeyListener.VoidDelegate(Shift_Key));
        keyListener.AddKeyListen(KeyCode.RightShift, new KeyListener.VoidDelegate(Shift_Key));
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1) && !isKeyF1Down)
        {
            isKeyF1Down = true;
            isControl = !isControl;
        }
        else if (Input.GetKeyUp(KeyCode.F1) && isKeyF1Down) isKeyF1Down = false;
        if (Input.GetKeyDown(KeyCode.F2) && !isKeyF2Down)
        {
            isKeyF2Down = true;
            ActiveBall("BallWood");
        }
        else if (Input.GetKeyUp(KeyCode.F2) && isKeyF2Down) isKeyF2Down = false;
        if (Input.GetKeyDown(KeyCode.F3) && !isKeyF3Down)
        {
            isKeyF3Down = true;
            ActiveBall("BallStone");
        }
        else if (Input.GetKeyUp(KeyCode.F3) && isKeyF3Down) isKeyF3Down = false;
        if (Input.GetKeyDown(KeyCode.F4) && !isKeyF4Down)
        {
            isKeyF4Down = true;
            ActiveBall("BallPaper");
        }
        else if (Input.GetKeyUp(KeyCode.F4) && isKeyF4Down) isKeyF4Down = false;
#endif
        if (isControl) keyListener.ListenKey();

    }
    private void FixedUpdate()
    {
        if (isControl) BallPush();
        if (isFollowCam) CamFollow();
        if (isControl) CamUpdate();
    }
    private void OnDestroy()
    {
        registeredBall.Clear();
        registeredBall = null;
    }

    //球推检测
    private void BallPush()
    {
        if (currertRegBall != null)
            currertRegBall.Core.BallPush();
    }

    #region Key Events

    private KeyListener keyListener = new KeyListener();
    private bool shiftPressed = false;

    private void Shift_Key(bool down)
    {
        if (isControl)
        {
            shiftPressed = down;
        }
    }
    private void Space_Key(bool down)
    {
        if (isControl)
        {
            if (down)
            {
                CamRoteSpace();
            }
            else
            {
                CamRoteSpaceBack();
            }
        }
    }
    private void RightArrow_Key(bool down)
    {
        if (isControl)
        {
            if (down)
            {
                if (shiftPressed)
                {
                    if ((pushType & BallPushType.Right) == BallPushType.Right)
                    {
                        pushType ^= BallPushType.Right;
                    }
                    CamRoteRight();
                }
                else
                {
                    pushType |= BallPushType.Right;
                }
            }
            else if ((pushType & BallPushType.Right) == BallPushType.Right)
            {
                pushType ^= BallPushType.Right;
            }
        }
    }
    private void LeftArrow_Key(bool down)
    {
        if (isControl)
        {
            if (down)
            {
                if (shiftPressed)
                {
                    if ((pushType & BallPushType.Left) == BallPushType.Left)
                    {
                        pushType ^= BallPushType.Left;
                    }
                    CamRoteLeft();
                }
                else
                {
                    pushType |= BallPushType.Left;
                }
            }
            else if ((pushType & BallPushType.Left) == BallPushType.Left)
            { 
                pushType ^= BallPushType.Left;
            }
        }
    }
    private void DownArrow_Key(bool down)
    {
        if (isControl)
        {
            if (down)
            {
                pushType |= BallPushType.Back;
            }
            else if ((pushType & BallPushType.Back) == BallPushType.Back)
            {
                pushType ^= BallPushType.Back;
            }
        }
    }
    private void UpArrow_Key(bool down)
    {
        if (isControl)
        {
            if (down)
            {
                pushType |= BallPushType.Forward;
            }
            else if ((pushType & BallPushType.Forward) == BallPushType.Forward)
            {
                pushType ^= BallPushType.Forward;
            }
        }
    }
    private void Home_Key(bool down)
    {
        if (isControl)
        {
            if (down)
            {
                pushType |= BallPushType.Up;
            }
            else
            {
                pushType ^= BallPushType.Up;
            }
        }
    }
    private void End_Key(bool down)
    {
        if (isControl)
        {
            if (down)
            {
                pushType |= BallPushType.Down;
            }
            else
            {
                pushType ^= BallPushType.Down;
            }
        }
    }

    #endregion

    #region Cam

    /// <summary>
    /// 摄像机面对的右方向量
    /// </summary>
    public Vector3 thisVector3Right { get; private set; }
    /// <summary>
    /// 摄像机面对的左方向量
    /// </summary>
    public Vector3 thisVector3Left { get; private set; }
    /// <summary>
    /// 摄像机面对的前方向量
    /// </summary>
    public Vector3 thisVector3Fornt { get; private set; }
    /// <summary>
    /// 摄像机面对的后方向量
    /// </summary>
    public Vector3 thisVector3Back { get; private set; }

    public AnimationCurve animationCurveCamera;
    public AnimationCurve animationCurveCameraY;
    public AnimationCurve animationCurveCameraMoveY;

    private bool isFollowCam = false;
    private bool isLookingBall = false;
    private bool isCameraSpaced;
    private bool isCameraRoteing;
    private bool isCameraRoteingX;
    private bool isCameraRoteingY;
    private bool isCameraMovingY;
    private bool isCameraMovingYDown;

    public int cameraRoteValue;
    public float cameraMaxRoteingOffest = 5f;
    public float cameraCurRoteingOffest = 1f;
    public float cameraSpaceMaxOffest = 0.8f;
    public float cameraSpaceMinOffest = 0;
    public float cameraSpaceOffest = 0.1f;

    private Transform camFollowTarget;
    private Vector3 camVelocityTarget = new Vector3();
    private float cameraNeedRoteingValue;
    private float cameraRoteingRealValue;

    private void CamFollow()
    {
        if (camFollowTarget == null)
        {
            isFollowCam = false;
            return;
        }
        if (isFollowCam)
        {
            if (currentBall != null)
            {
                ballCamFollowHost.transform.position = Vector3.SmoothDamp(ballCamFollowHost.transform.position, currentBall.transform.position, ref camVelocityTarget, camFollowSpeed);
            }
            else
            {
                //BallsCamera.get_transform().LookAt(startLookAtTransform);
                //BallsCamera.get_transform().set_position(Vector3.SmoothDamp(BallsCamera.get_transform().get_position(), targetPosition, ref cameraVelocity, camFollowSpeed));
            }
        }
    }

    /// <summary>
    /// 摄像机向左旋转
    /// </summary>
    public void CamRoteLeft()
    {
        if (!isCameraRoteing)
        {
            if (cameraRoteValue < 3)
                cameraRoteValue++;
            else
                cameraRoteValue = 0;
            cameraNeedRoteingValue = 90f;
            isCameraRoteing = true;
            isCameraRoteingX = true;
            CamRote2();
        }
    }
    /// <summary>
    /// 摄像机向右旋转
    /// </summary>
    public void CamRoteRight()
    {
        if (!isCameraRoteing)
        {
            if (cameraRoteValue > 0)
            {
                cameraRoteValue--;
            }
            else
            {
                cameraRoteValue = 3;
            }
            cameraNeedRoteingValue = -90f;
            isCameraRoteing = true;
            isCameraRoteingX = true;
            CamRote2();
        }
    }

    private void CamRote2()
    {
        switch (cameraRoteValue)
        {
            case 0:
                thisVector3Right = Vector3.right;
                thisVector3Left = Vector3.left;
                thisVector3Fornt = Vector3.forward;
                thisVector3Back = Vector3.back;
                break;
            case 1:
                thisVector3Right = Vector3.back;
                thisVector3Left = Vector3.forward;
                thisVector3Fornt = Vector3.right;
                thisVector3Back = Vector3.left;
                break;
            case 2:
                thisVector3Right = Vector3.left;
                thisVector3Left = Vector3.right;
                thisVector3Fornt = Vector3.back;
                thisVector3Back = Vector3.forward;
                break;
            case 3:
                thisVector3Right = Vector3.forward;
                thisVector3Left = Vector3.back;
                thisVector3Fornt = Vector3.left;
                thisVector3Back = Vector3.right;
                break;
        }
    }
    private void CamRote3()
    {
        switch (cameraRoteValue)
        {
            case 0:
            case 2:
                if (ballCamera.transform.localPosition.x != 0) ballCamera.transform.localPosition = new Vector3(0, ballCamera.transform.localPosition.y, ballCamera.transform.localPosition.z);
                break;
            case 1:
            case 3:
                if (ballCamera.transform.localPosition.z != 0) ballCamera.transform.localPosition = new Vector3(ballCamera.transform.localPosition.x, ballCamera.transform.localPosition.y, 0);
                break;
        }
    }

    /// <summary>
    /// 摄像机 按住 空格键 上升
    /// </summary>
    public void CamRoteSpace()
    {
        if (!isCameraRoteing && !isCameraSpaced)
        {
            cameraNeedRoteingValue = -27f;
            isCameraRoteing = true;
            isCameraRoteingY = true;
            isCameraMovingY = true;
            isCameraMovingYDown = false;
        }
    }
    /// <summary>
    /// 摄像机 放开 空格键 下降
    /// </summary>
    public void CamRoteSpaceBack()
    {
        if (isCameraSpaced)
        {
            cameraNeedRoteingValue = 27f;
            isCameraRoteing = true;
            isCameraRoteingY = true;
            isCameraMovingY = true;
            isCameraMovingYDown = true;
        }
    }

    private float CamRoteSpeedFun(float cameraRoteingRealValue)
    {
        return animationCurveCamera.Evaluate(Mathf.Abs(cameraRoteingRealValue / 90)) * cameraMaxRoteingOffest;
    }
    private float CamRoteSpeedFunY(float cameraRoteingRealValue)
    {
        return animationCurveCameraY.Evaluate(Mathf.Abs(cameraRoteingRealValue / 27)) * cameraMaxRoteingOffest;
    }
    private float CamMoveSpeedFunY(float cameraRoteingRealValue)
    {
        return animationCurveCameraMoveY.Evaluate(Mathf.Abs(cameraRoteingRealValue)) * cameraSpaceOffest;
    }
    private void CamUpdate()
    {
        if (isCameraRoteing)
        {
            //水平旋转
            if (isCameraRoteingX)
            {
                if (cameraNeedRoteingValue > 0f)
                {
                    if (cameraRoteingRealValue < cameraNeedRoteingValue)
                    {
                        cameraCurRoteingOffest = CamRoteSpeedFun(cameraRoteingRealValue);
                        cameraRoteingRealValue += cameraCurRoteingOffest;
                        ballCamera.transform.RotateAround(camFollowTarget.position, Vector3.up, cameraCurRoteingOffest);
                    }
                    else
                    {
                        //float f = cameraNeedRoteingValue - cameraRoteingRealValue;
                        //if (f > 0) ballCamera.transform.RotateAround(camFollowTarget.position, Vector3.up, -cameraCurRoteingOffest);
                        CamRote3();
                        cameraRoteingRealValue = 0f;
                        isCameraRoteingX = false;
                        isCameraRoteing = false;
                    }
                }
                else
                {
                    if (cameraRoteingRealValue > cameraNeedRoteingValue)
                    {
                        cameraCurRoteingOffest = CamRoteSpeedFun(cameraRoteingRealValue);
                        cameraRoteingRealValue -= cameraCurRoteingOffest;
                        ballCamera.transform.RotateAround(camFollowTarget.position, Vector3.up, -cameraCurRoteingOffest);
                    }
                    else
                    {
                        //float f = cameraNeedRoteingValue - cameraRoteingRealValue;
                        //if (f < 0) ballCamera.transform.RotateAround(camFollowTarget.position, Vector3.up, -cameraCurRoteingOffest);
                        CamRote3();
                        cameraRoteingRealValue = 0f;
                        isCameraRoteingX = false;
                        isCameraRoteing = false;
                    }
                }
            }
            //垂直旋转
            if (isCameraRoteingY)
            {
                if (cameraNeedRoteingValue > 0f)
                {
                    if (cameraRoteingRealValue < cameraNeedRoteingValue)
                    {
                        cameraCurRoteingOffest = CamRoteSpeedFunY(cameraRoteingRealValue);
                        cameraRoteingRealValue += cameraCurRoteingOffest;
                        ballCamera.transform.RotateAround(ballCamMoveY.transform.position, thisVector3Left, cameraCurRoteingOffest);
                    }
                    else
                    {
                        isCameraSpaced = false;
                        cameraRoteingRealValue = 0f;
                        isCameraRoteingY = false;
                        if (!isCameraMovingY)
                            isCameraRoteing = false;
                    }
                }
                else if (cameraNeedRoteingValue < 0f)
                {
                    if (cameraRoteingRealValue > cameraNeedRoteingValue)
                    {
                        cameraCurRoteingOffest = CamRoteSpeedFunY(cameraRoteingRealValue);
                        cameraRoteingRealValue -= cameraCurRoteingOffest;
                        ballCamera.transform.RotateAround(ballCamMoveY.transform.position, thisVector3Left, -cameraCurRoteingOffest);
                    }
                    else
                    {
                        isCameraSpaced = true;
                        cameraRoteingRealValue = 0f;
                        isCameraRoteingY = false;
                        if (!isCameraMovingY)
                            isCameraRoteing = false;
                    }
                }
            }
            //空格键 垂直上升
            if (isCameraMovingY)
            {
                if (isCameraMovingYDown)
                {
                    if (ballCamMoveY.transform.position.y > 0)
                        ballCamMoveY.transform.localPosition = new Vector3(0, ballCamMoveY.transform.localPosition.y - CamMoveSpeedFunY(ballCamMoveY.transform.localPosition.y), 0);
                    else
                    {
                        ballCamMoveY.transform.localPosition = new Vector3(0, 0, 0);
                        isCameraMovingY = false;
                        if (!isCameraRoteingY)
                            isCameraRoteing = false;
                    }
                }
                else
                {
                    if (ballCamMoveY.transform.localPosition.y < cameraSpaceMaxOffest)
                        ballCamMoveY.transform.localPosition = new Vector3(0, ballCamMoveY.transform.localPosition.y + CamMoveSpeedFunY(ballCamMoveY.transform.localPosition.y),0);
                    else
                    {
                        ballCamMoveY.transform.localPosition = new Vector3(0, cameraSpaceMaxOffest, 0);
                        isCameraMovingY = false;
                        if (!isCameraRoteingY)
                            isCameraRoteing = false;
                    }
                }
            }
        }
        //看着球
        if (isLookingBall)
        {
            if (currentBall != null)
                ballCamera.transform.LookAt(currentBall.transform);
        }
    }

    #endregion

    #region 球控制

    private GameObject currentBall;
    private Rigidbody rigidbodyCurrent;

    private Vector3 nextRecoverBallPos = Vector3.zero;

    private RegBall currertRegBall;
    private List<RegBall> registeredBall = new List<RegBall>();

    private bool _isControl = false;

    /// <summary>
    /// 获取设置是否可以控制球
    /// </summary>
    public bool isControl
    {
        get { return _isControl; }
        set
        {
            if (_isControl != value)
            {
                _isControl = value;
                if (value)
                {
                    isLookingBall = true;
                    isFollowCam = true;
                    if (currertRegBall != null)
                        currertRegBall.Core.StartControl();
                }
                else
                {
                    isFollowCam = false;
                    isLookingBall = false;
                    if (currertRegBall != null)
                        currertRegBall.Core.EndControl();
                }
            }
        }
    }
    /// <summary>
    /// 获取当前球推动方向
    /// </summary>
    public BallPushType pushType { get; private set; }

    public void RemoveBallPush(BallPushType t)
    {
        if ((pushType & t) == t)
        {
            pushType ^= t;
        }
    }

    /// <summary>
    /// 重新设置默认球位置并激活
    /// </summary>
    public void RecoverBallDef()
    {
        RecoverBall(nextRecoverBallPos);
    }
    /// <summary>
    /// 重新设置指定球位置并激活
    /// </summary>
    /// <param name="pos">球名字</param>
    public void RecoverBall(Vector3 pos)
    {
        if (currertRegBall != null)
        {
            currertRegBall.Core.Recover(pos);
            camFollowTarget.position = pos;
        }
    }
    /// <summary>
    /// 激活指定的球
    /// </summary>
    /// <param name="type">球名字</param>
    public void ActiveBall(string type)
    {
        RecoverBallDef();
        RegBall regBall = GetRegBall(type);
        if (regBall != null)
        {
            currertRegBall = regBall;
            currertRegBall.Core.Active(nextRecoverBallPos);
            currentBall = currertRegBall.Base;
            rigidbodyCurrent = currentBall.GetComponent<Rigidbody>();
            camFollowTarget = currentBall.GetComponent<Transform>();
            isFollowCam = true;
            isLookingBall = true;
        }
    }
    /// <summary>
    /// 激活默认球
    /// </summary>
    public void ActiveBallDef()
    {
        if (currertRegBall != null)
            ActiveBall(currertRegBall.TypeName);
        else ActiveBall("BallWood");
    }
    /// <summary>
    /// 清除已激活的球
    /// </summary>
    public void ClearBall()
    {
        isControl = false;
        if (currertRegBall != null)
        {
            currertRegBall.Core.Deactive();
        }
        if (currentBall != null)
        {
            currentBall.SetActive(false);
            currentBall = null;
        }
    }

    /// <summary>
    /// 开始控制
    /// </summary>
    public void StartControl()
    {
        isControl = true;
    }
    /// <summary>
    /// 停止控制
    /// </summary>
    public void EndControl()
    {
        isControl = false;
    }

    #endregion

    #region 球注册

    /// <summary>
    /// 球注册类
    /// </summary>
    public class RegBall
    {
        /// <summary>
        /// 类型名字
        /// </summary>
        public string TypeName;
        /// <summary>
        /// 球基本类
        /// </summary>
        public Ball Core;
        /// <summary>
        /// 球基本模型
        /// </summary>
        public GameObject Base;
        /// <summary>
        /// 球碎片
        /// </summary>
        public GameObject Pieces;
        /// <summary>
        /// 变球器颜色
        /// </summary>
        public Color TrafoColor;
        /// <summary>
        /// 变球器动画类
        /// </summary>
        public AnimTranfo AnimTranfo;
    }

    /// <summary>
    /// 获取注册的球类
    /// </summary>
    /// <param name="type">球名字</param>
    /// <returns></returns>
    public RegBall GetRegBall(string type)
    {
        RegBall result = null;
        foreach (RegBall current in registeredBall)
        {
            if (current.TypeName == type)
            {
                result = current;
                break;
            }
        }
        return result;
    }
    /// <summary>
    /// 获取球类是否注册
    /// </summary>
    /// <param name="type">名字</param>
    /// <returns></returns>
    public bool IsBallRegisted(string type)
    {
        bool result = false;
        foreach (RegBall current in registeredBall)
        {
            if (current.TypeName == type)
            {
                result = true;
                break;
            }
        }
        return result;
    }
    /// <summary>
    /// 取消注册球类
    /// </summary>
    /// <param name="type">球名字</param>
    /// <returns></returns>
    public bool UnRegistedBall(string type)
    {
        bool result = false;
        RegBall t = null;
        foreach (RegBall current in registeredBall)
        {
            if (current.TypeName == type)
            {
                t = current;
                result = true;
                break;
            }
        }
        if (t != null)
            registeredBall.Remove(t);
        return result;
    }
    /// <summary>
    /// 注册球类
    /// </summary>
    /// <param name="type">球名字</param>
    /// <param name="b">球本体</param>
    /// <param name="peices">球的碎片</param>
    /// <param name="trafoColor">变球器颜色</param>
    /// <returns></returns>
    public bool RegisterBall(string type, GameObject b, GameObject peices = null, Color trafoColor = default(Color), AnimTranfo animTranfo = null)
    {
        if (!IsBallRegisted(type))
        {
            Ball component = b.GetComponent<Ball>();
            if (component != null)
            {
                RegBall regBall = new RegBall();
                regBall.Base = b;
                regBall.Core = component;
                regBall.TypeName = type;
                regBall.Pieces = peices;
                regBall.TrafoColor = trafoColor;
                regBall.AnimTranfo = animTranfo;
                registeredBall.Add(regBall);
            }
        }
        return false;
    }
    #endregion

}
