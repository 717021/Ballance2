using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 核心模块： 管理球的工作
 */

public class BallsManager : MonoBehaviour
{
    public static BallsManager StaticBallsManager { get; private set; }

    private bool isKeyF1Down = false, isKeyF2Down, isKeyF3Down, isKeyF4Down;

    public Camera ballCamera;
    public GameObject ballCamFollowHost;
    public GameObject testRoad;
    public GameObject ballWood, ballStone, ballPaper;
    public GameObject ballPeicesWood, ballPeicesStone, ballPeicesPaper;
    public Color ballWoodTranfoColor, ballStoneTranfoColor, ballPaperTranfoColor;
    public Transform camFollowTarget;
    public float camFollowSmothing = 5f;
    

    public BallsManager()
    {
        StaticBallsManager = this;
        thisVector3Right = Vector3.right;
        thisVector3Left = Vector3.left;
        thisVector3Fornt = Vector3.forward;
        thisVector3Back = Vector3.back;
    }

    public Vector3 thisVector3Right { get; private set; }
    public Vector3 thisVector3Left { get; private set; }
    public Vector3 thisVector3Fornt { get; private set; }
    public Vector3 thisVector3Back { get; private set; }

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
                if(value)
                {
                    isFollowCam = true;
                    if (currertRegBall != null)
                        currertRegBall.Core.StartControl();
                }
                else
                {
                    isFollowCam = false;
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

    private void Start()
    {
        camFollowOffset = new Vector3(0, 0.25f, -0.25f);
        ballCamera.transform.position = new Vector3(0, 0.2f, -0.2f);
#if UNITY_EDITOR
        if (!GlobalSettings.StartInIntro)
            testRoad.SetActive(true);
#endif
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
        if (isFollowCam)
            CamFollow();
        if (isControl)
            CamUpdate();
        if (isControl)
            BallPush();
    }
    private void OnDestroy()
    {
        registeredBall.Clear();
        registeredBall = null;
    }

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

    private bool isFollowCam = false;
    private Vector3 camFollowOffset;
    public static int cameraRoteValue;
    private bool cameraLooking = true;
    private bool cameraSpaced;
    private bool cameraRoteing;
    private bool cameraRoteingX;
    private bool cameraRoteingY;
    private float cameraRoteingOffest = 5f;
    private float cameraRoteingValue;
    private float cameraRoteingRailValue;

    private void CamFollow()
    {
        if (camFollowTarget == null)
        {
            isFollowCam = false;
            return;
        }
        Vector3 targetCampos = camFollowTarget.position + camFollowOffset;
        ballCamFollowHost.transform.position = Vector3.Lerp(ballCamFollowHost.transform.position, targetCampos, camFollowSmothing * Time.deltaTime);
    }
    private void CamRoteLeft()
    {
        if (!this.cameraRoteing)
        {
            if (cameraRoteValue < 3)
                cameraRoteValue++;
            else
                cameraRoteValue = 0;
            this.cameraRoteingValue = 90f;
            this.cameraRoteing = true;
            this.cameraRoteingX = true;
            this.CamRote2();
        }
    }
    private void CamRoteRight()
    {
        if (!this.cameraRoteing)
        {
            if (cameraRoteValue > 0)
            {
                cameraRoteValue--;
            }
            else
            {
                cameraRoteValue = 3;
            }
            this.cameraRoteingValue = -90f;
            this.cameraRoteing = true;
            this.cameraRoteingX = true;
            this.CamRote2();
        }
    }
    private void CamRote2()

    {
        switch (cameraRoteValue)
        {
            case 0:
                this.thisVector3Right = Vector3.right;
                this.thisVector3Left = Vector3.left;
                this.thisVector3Fornt = Vector3.forward;
                this.thisVector3Back = Vector3.back;
                break;
            case 1:
                this.thisVector3Right = Vector3.back;
                this.thisVector3Left = Vector3.forward;
                this.thisVector3Fornt = Vector3.right;
                this.thisVector3Back = Vector3.left;
                break;
            case 2:
                this.thisVector3Right = Vector3.left;
                this.thisVector3Left = Vector3.right;
                this.thisVector3Fornt = Vector3.back;
                this.thisVector3Back = Vector3.forward;
                break;
            case 3:
                this.thisVector3Right = Vector3.forward;
                this.thisVector3Left = Vector3.back;
                this.thisVector3Fornt = Vector3.left;
                this.thisVector3Back = Vector3.right;
                break;
        }
    }
    private void CamRoteSpace()
    {
        if (!this.cameraRoteing && !this.cameraSpaced)
        {
            this.cameraRoteingValue = -27f;
            this.cameraRoteing = true;
            this.cameraRoteingY = true;
        }
    }
    private void CamRoteSpaceBack()
    {
        if (this.cameraSpaced)
        {
            this.cameraRoteingValue = 27f;
            this.cameraRoteing = true;
            this.cameraRoteingY = true;
        }
    }
    private void CamUpdate()
    {
        if (cameraRoteing)
        {
            if (cameraRoteingX)
            {
                if (cameraRoteingValue > 0f)
                {
                    if (cameraRoteingRailValue < cameraRoteingValue)
                    {
                        cameraRoteingRailValue += cameraRoteingOffest;
                        ballCamera.transform.RotateAround(currentBall.transform.position, Vector3.up, cameraRoteingOffest);
                    }
                    else
                    {
                        cameraRoteingRailValue = 0f;
                        cameraRoteingX = false;
                        cameraRoteing = false;
                    }
                }
                else if (cameraRoteingRailValue > cameraRoteingValue)
                {
                    cameraRoteingRailValue -= cameraRoteingOffest;
                    ballCamera.transform.RotateAround(currentBall.transform.position, Vector3.up, -cameraRoteingOffest);
                }
                else
                {
                    cameraRoteingRailValue = 0f;
                    cameraRoteingX = false;
                    cameraRoteing = false;
                }
            }
            else if (cameraRoteingY)
            {
                if (cameraRoteingValue > 0f)
                {
                    if (cameraRoteingRailValue < cameraRoteingValue)
                    {
                        cameraRoteingRailValue += cameraRoteingOffest;
                        ballCamera.transform.RotateAround(currentBall.transform.position, thisVector3Left, cameraRoteingOffest);
                    }
                    else
                    {
                        cameraSpaced = false;
                        cameraRoteingRailValue = 0f;
                        cameraRoteingY = false;
                        cameraRoteing = false;
                    }
                }
                else if (cameraRoteingRailValue > cameraRoteingValue)
                {
                    cameraRoteingRailValue -= cameraRoteingOffest;
                    ballCamera.transform.RotateAround(currentBall.transform.position, thisVector3Left, -cameraRoteingOffest);
                }
                else
                {
                    cameraSpaced = true;
                    cameraRoteingRailValue = 0f;
                    cameraRoteingY = false;
                    cameraRoteing = false;
                }
            }
        }
        if (cameraLooking && !isFollowCam)
        {
            if (currentBall != null)
            {
                ballCamera.transform.LookAt(currentBall.transform);
            }
            else
            {
                //ballCamera.transform.LookAt(startLookAtTransform);
            }
        }
    }
    #endregion

    private GameObject currentBall;
    private Rigidbody rigidbodyCurrent;

    private Vector3 nextRecoverBallPos = Vector3.zero;

    private RegBall currertRegBall;
    private List<RegBall> registeredBall = new List<RegBall>();

    public void RecoverBallDef()
    {
        RecoverBall(nextRecoverBallPos);
    }
    public void RecoverBall(Vector3 pos)
    {
        if (currertRegBall != null)
        {
            currertRegBall.Core.Recover(pos);
            camFollowTarget.position = pos;
        }
    }
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
        }
    }
    public void ActiveBallDef()
    {
        ActiveBall(currertRegBall.TypeName);
    }
    public void ClearBall()
    {
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

    public void StartControl()
    {
        isControl = true;
    }
    public void EndControl()
    {
        isControl = false;
    }

    #region 球注册

    /// <summary>
    /// 球注册类
    /// </summary>
    public class RegBall
    {
        public string TypeName;
        public Ball Core;
        public GameObject Base;
        public GameObject Pieces;
        public Color TrafoColor;
    }

    /// <summary>
    /// 获取注册的球类
    /// </summary>
    /// <param name="type">名字</param>
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
    /// <param name="type">名字</param>
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
    /// <param name="type">名字</param>
    /// <param name="b">球本体</param>
    /// <param name="peices">球的碎片</param>
    /// <param name="trafoColor">变球器颜色</param>
    /// <returns></returns>
    public bool RegisterBall(string type, GameObject b, GameObject peices = null, Color trafoColor = default(Color))
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
                registeredBall.Add(regBall);
            }
        }
        return false;
    }
    #endregion

}
