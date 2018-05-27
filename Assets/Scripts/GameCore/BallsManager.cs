using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 核心模块： 管理球的工作
 */

namespace Assets.Scripts.GameCore
{
    /// <summary>
    /// 球工作管理器
    /// </summary>
    public class BallsManager : MonoBehaviour
    {
        private bool isKeyF1Down, isKeyF2Down, isKeyF3Down, isKeyF4Down, isKeyF5Down;

        public GameObject ballCamMoveY;
        public Camera ballCamera;
        public GameObject ballCamFollowHost;
        public GameObject ballCamFollowTarget;
        public GameObject ballWood, ballStone, ballPaper;
        public GameObject ballLightningSphere;
        public GameObject ballLightningLight;
        public GameObject ballPeicesWood, ballPeicesStone, ballPeicesPaper;
        public Color ballWoodTranfoColor, ballStoneTranfoColor, ballPaperTranfoColor;

        public float camFollowSpeed = 0.1f;
        public float camFollowSpeed2 = 0.05f;

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
            ballLightningSphere.SetActive(false);

            //注册三个默认球
            RegisterBall("BallWood", ballWood, ballPeicesWood, ballWoodTranfoColor);
            RegisterBall("BallStone", ballStone, ballPeicesStone, ballStoneTranfoColor);
            RegisterBall("BallPaper", ballPaper, ballPeicesPaper, ballPaperTranfoColor);

            //添加按键事件
            keyListener.AddKeyListen(KeyCode.UpArrow, KeyCode.W, new KeyListener.VoidDelegate(UpArrow_Key));
            keyListener.AddKeyListen(KeyCode.DownArrow, KeyCode.S, new KeyListener.VoidDelegate(DownArrow_Key));
            keyListener.AddKeyListen(KeyCode.LeftArrow, KeyCode.A, new KeyListener.VoidDelegate(LeftArrow_Key));
            keyListener.AddKeyListen(KeyCode.RightArrow, KeyCode.D, new KeyListener.VoidDelegate(RightArrow_Key));
            keyListener.AddKeyListen(KeyCode.Q, new KeyListener.VoidDelegate(Home_Key));
            keyListener.AddKeyListen(KeyCode.E, new KeyListener.VoidDelegate(End_Key));
            keyListener.AddKeyListen(KeyCode.Space, new KeyListener.VoidDelegate(Space_Key));
            keyListener.AddKeyListen(KeyCode.LeftShift, KeyCode.RightShift, new KeyListener.VoidDelegate(Shift_Key));
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
            if (Input.GetKeyDown(KeyCode.F5) && !isKeyF5Down)
            {
                isKeyF5Down = true;
                PlayLighting(true);
            }
            else if (Input.GetKeyUp(KeyCode.F5) && isKeyF5Down) isKeyF5Down = false;
#endif
            if (isControl) keyListener.ListenKey();
            //闪电球
            if (lighing)
            {
                if (ballLightningSphere.transform.localEulerAngles.y > 360f)
                    ballLightningSphere.transform.localEulerAngles = new Vector3(-90, 0, 0);
                ballLightningSphere.transform.localEulerAngles = new Vector3(-90, ballLightningSphere.transform.localEulerAngles.y + 60f * Time.deltaTime, 0);
                if (secxx < 0.2)
                    secxx += Time.deltaTime;
                else
                {
                    if (currentBallLightningSphereTexture >= 3)
                        currentBallLightningSphereTexture = 1;
                    else currentBallLightningSphereTexture++;
                    switch (currentBallLightningSphereTexture)
                    {
                        case 1:
                            ballLightningSphereMaterial.mainTexture = ballLightningSphere1;

                            break;
                        case 2:
                            ballLightningSphereMaterial.mainTexture = ballLightningSphere2;
                            break;
                        case 3:
                            ballLightningSphereMaterial.mainTexture = ballLightningSphere3;
                            break;
                    }
                    secxx = 0;
                }
            }
            //闪电球 放大
            if (lighingBig)
            {
                if (ballLightningSphere.transform.localScale.x < 1f)
                {
                    ballLightningSphere.transform.localScale = new Vector3(ballLightningSphere.transform.localScale.x + 0.8f * Time.deltaTime,
                    ballLightningSphere.transform.localScale.y + 0.8f * Time.deltaTime,
                    ballLightningSphere.transform.localScale.z + 0.8f * Time.deltaTime);
                }
                else
                {
                    ballLightningSphere.transform.localScale = new Vector3(1f, 1f, 1f);
                    lighingBig = false;
                }
            }
            //平滑移动
            if (isBallSmoothMove)
            {
                if (currentBall != null)
                {
                    currentBall.transform.position = Vector3.SmoothDamp(currentBall.transform.position, ballSmoothMoveTarget, ref ballSmoothMoveVelocityTarget, ballSmoothMoveTime);
                    if (currentBall.transform.position == ballSmoothMoveTarget)
                        isBallSmoothMove = false;
                }
                else isBallSmoothMove = false;
            }
        }
        private void FixedUpdate()
        {
            if (isControl)
            {
                BallPush();
                CamUpdate();
            }
            if (isFollowCam)
            {
                CamFollow();
            }
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

        //按键侦听器
        private KeyListener keyListener = new KeyListener();
        private bool shiftPressed = false;

        //一些按键事件
        private void Shift_Key(bool down)
        {
            shiftPressed = down;
        }
        private void Space_Key(bool down)
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
        private void RightArrow_Key(bool down)
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
                else if (isControl)
                {
                    pushType |= BallPushType.Right;
                }
            }
            else if (isControl && (pushType & BallPushType.Right) == BallPushType.Right)
            {
                pushType ^= BallPushType.Right;
            }
        }
        private void LeftArrow_Key(bool down)
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
                else if (isControl)
                {
                    pushType |= BallPushType.Left;
                }
            }
            else if (isControl && (pushType & BallPushType.Left) == BallPushType.Left)
            {
                pushType ^= BallPushType.Left;
            }
        }
        private void DownArrow_Key(bool down)
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
        private void UpArrow_Key(bool down)
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
        private void Home_Key(bool down)
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
        private void End_Key(bool down)
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

        //一些摄像机的旋转动画曲线
        public AnimationCurve animationCurveCamera;
        public AnimationCurve animationCurveCameraY;
        public AnimationCurve animationCurveCameraMoveY;
        public AnimationCurve animationCurveCameraMoveYDown;

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
        public float cameraSpaceMaxOffest = 80f;
        public float cameraSpaceOffest = 10f;

        private const float pushVector = 0.05f;
        private Material ballLightningSphereMaterial;
        private Vector3 camVelocityTarget2 = new Vector3();
        private Transform camFollowTarget;
        private Vector3 camVelocityTarget = new Vector3();
        private float cameraNeedRoteingValue;
        private float cameraRoteingRealValue;



        /// <summary>
        /// 让摄像机不看着球
        /// </summary>
        public void CamSetNoLookAtBall()
        {
            isLookingBall = false;
        }
        /// <summary>
        /// 让摄像机看着球
        /// </summary>
        public void CamSetLookAtBall()
        {
            if (currentBall != null)
                isLookingBall = true;
        }
        /// <summary>
        /// 让摄像机只看着球
        /// </summary>
        public void CamSetJustLookAtBall()
        {
            if (currentBall != null)
            {
                isFollowCam = false;
                isLookingBall = true;
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
        //摄像机面对向量重置
        private void CamRote2()
        {
            //根据摄像机朝向重置几个球推动的方向向量
            //    这4个方向向量用于球
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
        //摄像机旋转偏移重置
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

        //几个动画计算曲线
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
            return animationCurveCameraMoveY.Evaluate(Mathf.Abs(cameraRoteingRealValue / cameraSpaceMaxOffest)) * cameraSpaceOffest;
        }
        private float CamMoveSpeedFunYDown(float cameraRoteingRealValue)
        {
            return animationCurveCameraMoveYDown.Evaluate(Mathf.Abs((cameraRoteingRealValue) / cameraSpaceMaxOffest)) * cameraSpaceOffest;
        }

        //摄像机跟随 每帧
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
                    ballCamFollowTarget.transform.position = Vector3.SmoothDamp(ballCamFollowTarget.transform.position, currentBall.transform.position, ref camVelocityTarget2, camFollowSpeed2);
                    //ballCamFollowHost.transform.position = Vector3.SmoothDamp(ballCamFollowHost.transform.position, ballCamFollowTarget.transform.position, ref camVelocityTarget, camFollowSpeed);
                    ballCamFollowHost.transform.position = Vector3.SmoothDamp(ballCamFollowHost.transform.position, currentBall.transform.position, ref camVelocityTarget, camFollowSpeed);
                }
            }
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
                            ballCamera.transform.RotateAround(ballCamFollowHost.transform.position, Vector3.up, cameraCurRoteingOffest);
                        }
                        else
                        {
                            float f = cameraNeedRoteingValue - cameraRoteingRealValue;
                            if (f > 0) ballCamera.transform.RotateAround(ballCamFollowHost.transform.position, Vector3.up, -f);
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
                            ballCamera.transform.RotateAround(ballCamFollowHost.transform.position, Vector3.up, -cameraCurRoteingOffest);
                        }
                        else
                        {
                            float f = cameraNeedRoteingValue - cameraRoteingRealValue;
                            if (f < 0) ballCamera.transform.RotateAround(ballCamFollowHost.transform.position, Vector3.up, -f);
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
                        if (ballCamMoveY.transform.localPosition.y > 0)
                            ballCamMoveY.transform.localPosition = new Vector3(0, (ballCamMoveY.transform.localPosition.y - CamMoveSpeedFunYDown(ballCamMoveY.transform.localPosition.y)), 0);
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
                            ballCamMoveY.transform.localPosition = new Vector3(0, ballCamMoveY.transform.localPosition.y + CamMoveSpeedFunY(ballCamMoveY.transform.localPosition.y), 0);
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
            if (isLookingBall && isFollowCam)
            {
                ballCamera.transform.LookAt(ballCamFollowTarget.transform);
            }
            else if (isLookingBall && !isFollowCam)
            {
                if (currentBall != null)
                    ballCamera.transform.LookAt(currentBall.transform);
            }
        }

        #endregion

        #region Lighting

        private bool lighing = false;
        private bool lighingBig = false;

        public Texture ballLightningSphere1,
            ballLightningSphere2,
            ballLightningSphere3;

        private int currentBallLightningSphereTexture = 1;
        private float secxx = 0;

        /// <summary>
        /// 播放球 闪电动画
        /// </summary>
        /// <param name="smallToBig">是否由小变大。</param>
        /// <param name="lightEnd">是否在之后闪一下。</param>
        public void PlayLighting(bool smallToBig = false, bool lightEnd = false)
        {
            //播放闪电声音
            GlobalMediator.CallAction(GameActions.PlaySound("sounds_*TYPE*", "assets/audios/misc_lightning.wav", 3));

            lighing = true;

            ballLightningSphereMaterial = ballLightningSphere.GetComponent<MeshRenderer>().material;
            ballLightningSphere.SetActive(true);
            ballLightningSphere.transform.position = nextRecoverBallPos;
            if (smallToBig)
            {
                ballLightningSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                lighingBig = true;
            }

            StartCoroutine(PlayLightingWait(lightEnd));
        }
        private IEnumerator PlayLightingWait(bool lightEnd)
        {
            yield return new WaitForSeconds(2);
            ballLightningSphereMaterial.mainTexture = ballLightningSphere1;
            ballLightningSphere.transform.localScale = new Vector3(1f, 1f, 1f);
            ballLightningSphere.SetActive(false);
            if (lightEnd)
            {
                ballLightningLight.SetActive(true);
                yield return new WaitForSeconds(0.35f);
                ballLightningLight.SetActive(false);
            }
            lighing = false;
            yield break;
        }

        #endregion

        #region 球控制

        //当前球
        private GameObject currentBall;
        private Rigidbody rigidbodyCurrent;

        //下一次恢复球的位置
        private Vector3 nextRecoverBallPos = Vector3.zero;
        private bool isBallControl = false;
        private bool isBallSmoothMove = false;
        private Vector3 ballSmoothMoveTarget;
        private Vector3 ballSmoothMoveVelocityTarget;
        private float ballSmoothMoveTime = 0.2f;

        public bool IsSmoothMove() { return isBallSmoothMove; }

        /// <summary>
        /// 获取设置是否可以控制球
        /// </summary>
        public bool isControl
        {
            get { return isBallControl; }
            set
            {
                if (isBallControl != value)
                {
                    isBallControl = value;
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

        /// <summary>
        /// 指定球速度清零。
        /// </summary>
        /// <param name="ball">指定球</param>
        public void RemoveBallSpeed(RegBall ball)
        {
            if (ball != null && ball.Core != null)
                ball.Core.RemoveSpeed();
        }
        public void AddBallPush(BallPushType t)
        {
            if ((pushType & t) != t)
            {
                pushType |= t;
            }
        }
        public void RemoveBallPush(BallPushType t)
        {
            if ((pushType & t) == t)
            {
                pushType ^= t;
            }
        }
        /// <summary>
        /// 设置球下次激活的位置。
        /// </summary>
        /// <param name="pos">下次激活的位置</param>
        public void RecoverSetPos(Vector3 pos)
        {
            nextRecoverBallPos = pos;
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
        /// 平滑移动球到指定位置。
        /// </summary>
        /// <param name="pos">指定位置。</param>
        /// <param name="off">动画平滑时间</param>
        public void SmoothMoveBallToPos(Vector3 pos, float off = 2f)
        {
            if (currertRegBall != null)
            {
                if (isControl)
                    isControl = false;
                RemoveBallSpeed(currertRegBall);
                ballSmoothMoveTarget = pos;
                ballSmoothMoveTime = off;

                isBallSmoothMove = true;
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
            if (rigidbodyCurrent != null)
            {
                rigidbodyCurrent.Sleep();
            }
        }

        #endregion

        #region 球注册

        //当前球
        private RegBall currertRegBall;
        //已注册的球
        private List<RegBall> registeredBall = new List<RegBall>();

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

}