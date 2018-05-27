using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 默认变球器动画
 */
namespace Assets.Scripts.GameCore
{
    /// <summary>
    /// 默认变球器动画，不要改动。
    /// 你可以参照此类设计你自己的变球器动画。
    /// </summary>
    public class AnimTranfoDef : AnimTranfo
    {
        public MeshRenderer AnimTrafo_FlashfieldMeshRenderer;
        public GameObject AnimTrafo_Flashfield;
        public GameObject AnimTrafo_Ringparts;
        public GameObject AnimTrafo_Ringparts1;
        public GameObject AnimTrafo_Ringparts2;
        public GameObject AnimTrafo_Ringparts3;
        public GameObject AnimTrafo_Ringparts4;

        public Vector3 AnimTrafo_Ringparts1MoveTarget;
        public Vector3 AnimTrafo_Ringparts2MoveTarget;
        public Vector3 AnimTrafo_Ringparts3MoveTarget;
        public Vector3 AnimTrafo_Ringparts4MoveTarget;

        public AnimationCurve AnimTrafo_RingpartsMoveBigCurve;
        public AnimationCurve AnimTrafo_RingpartsMoveSmallCurve;

        public override void StartPlay(string balltype, BallsManager.RegBall regBall, Vector3 pos)
        {
            base.StartPlay(balltype, regBall, pos);
            if (regBall != null)
                SetTranfoColor(regBall.TrafoColor);
            //移动球到指定位置
            ballsManager.SmoothMoveBallToPos(pos, 0.2f);
            StartCoroutine(WaitBallMoveToPos());
        }
        public override void EndPlay()
        {
            base.EndPlay();
        }

        private IEnumerator WaitBallMoveToPos()
        {
            //等待球移到指定位置
            yield return new WaitUntil(ballsManager.IsSmoothMove);
            StartInnern();
        }
        private void StartInnern()
        {
            rolling = true;
            upValue = transform.position.y;
            toDownValue = upValue;
            toValue = upValue + 0.105f;
            bigIng = true;
            gameObject.SetActive(true);
        }

        private float upValue;
        private float toValue;
        private float toDownValue;
        private bool upIng;
        private bool downIng;
        private bool bigIng;
        private bool smallIng;
        private bool rolling;
        private int rolling2;
        private bool rolling1;

        void Start()
        {
            gameObject.SetActive(false);
        }
        void Update()
        {
            if (upIng)
            {
                if (upValue < toValue)
                {
                    upValue += 15f * Time.deltaTime;
                }
                else
                {
                    upValue = toValue;
                    upIng = false;
                    downIng = true;
                }
                transform.position = new Vector3(transform.position.x, upValue, transform.position.z);
            }
            else if (downIng)
            {
                if (upValue > toDownValue)
                {
                    upValue -= 12f * Time.deltaTime;
                }
                else
                {
                    upValue = toDownValue;
                    downIng = false;
                    smallIng = true;
                    ThrowPeices();
                }
                transform.position = new Vector3(transform.position.x, upValue, transform.position.z);
            }

            if (bigIng)
            {
                float xx = Mathf.Abs(AnimTrafo_Ringparts1.transform.position.x / 300f * Time.deltaTime);
                float speed = AnimTrafo_RingpartsMoveBigCurve.Evaluate(xx);
                AnimTrafo_Ringparts1.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts1.transform.localPosition, AnimTrafo_Ringparts1MoveTarget, speed);
                AnimTrafo_Ringparts2.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts1.transform.localPosition, AnimTrafo_Ringparts2MoveTarget, speed);
                AnimTrafo_Ringparts3.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts1.transform.localPosition, AnimTrafo_Ringparts3MoveTarget, speed);
                AnimTrafo_Ringparts4.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts1.transform.localPosition, AnimTrafo_Ringparts4MoveTarget, speed);
                if (xx >= 1)
                {
                    bigIng = false;
                    upIng = true;
                }
            }
            else if (smallIng)
            {
                float xx = Mathf.Abs(AnimTrafo_Ringparts1.transform.position.x / 300f * Time.deltaTime);
                float speed = AnimTrafo_RingpartsMoveSmallCurve.Evaluate(xx);
                AnimTrafo_Ringparts1.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts1.transform.localPosition, Vector3.zero, speed);
                AnimTrafo_Ringparts2.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts2.transform.localPosition, Vector3.zero, speed);
                AnimTrafo_Ringparts3.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts3.transform.localPosition, Vector3.zero, speed);
                AnimTrafo_Ringparts4.transform.localPosition = Vector3.Lerp(AnimTrafo_Ringparts4.transform.localPosition, Vector3.zero, speed);
                if (xx <= 0) { smallIng = false; EndPlay(); }
            }

            if (rolling && rolling2 > 10)
            {
                rolling2 = 0; rolling1 = !rolling1;
                if (rolling1) AnimTrafo_FlashfieldMeshRenderer.material.mainTextureOffset = new Vector2(0.5f, 0);
                else AnimTrafo_FlashfieldMeshRenderer.material.mainTextureOffset = Vector2.zero;
            }
            else if (rolling) rolling2++;
        }

        private void SetTranfoColor(Color color)
        {
            AnimTrafo_Ringparts1.GetComponent<MeshRenderer>().materials[1].color = color;
            AnimTrafo_Ringparts2.GetComponent<MeshRenderer>().materials[1].color = color;
            AnimTrafo_Ringparts3.GetComponent<MeshRenderer>().materials[1].color = color;
            AnimTrafo_Ringparts4.GetComponent<MeshRenderer>().materials[1].color = color;
        }
    }
}
