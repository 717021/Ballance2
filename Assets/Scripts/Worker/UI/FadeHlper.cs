using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 屏幕渐变控制脚本（渐渐变黑或渐渐变白）
 */

public class FadeHlper : MonoBehaviour {

    /// <summary>
    /// 渐变速度
    /// </summary>
    public float FadeSpeed = 0.3F;
    /// <summary>
    /// 绘画的遮罩图像
    /// </summary>
    public Texture2D BgImage;
    /// <summary>
    /// 起始 Alpha （0-1）
    /// </summary>
    public float StartAlpha = 0f;

    public enum FadeStatus
    {
        Null,
        FadeIn,
        FadeOut
    }

    /// <summary>
    /// 淡入
    /// </summary>
    public void FadeIn()
    {
        mAlpha = 0.0F;
        mStatus = FadeStatus.FadeIn;
    }
    /// <summary>
    /// 淡出
    /// </summary>
    public void FadeOut()
    {
        mAlpha = 1F;
        mStatus = FadeStatus.FadeOut;
    }

    private FadeStatus mStatus = FadeStatus.Null;
    //当前透明度
    private float mAlpha = 0.0F;

    // Use this for initialization
    void Start () {
        mAlpha = StartAlpha;
    }

    /// <summary>
    /// 当前透明度
    /// </summary>
    public float Alpha
    {
        get { return mAlpha; }
        set { mAlpha = value; }
    }

    // Update is called once per frame
    void Update () {
        switch (mStatus)
        {
            case FadeStatus.FadeIn:
                mAlpha += FadeSpeed * Time.deltaTime;
                break;
            case FadeStatus.FadeOut:
                mAlpha -= FadeSpeed * Time.deltaTime;
                break;
        }
    }

    void OnGUI()
    {
        if (mAlpha > 0F && mAlpha < 1.0F)
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp(mAlpha, 0, 1));
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BgImage);
        }
        else if (mAlpha > 1.0F)
        {
            mAlpha = 1.0F;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp(mAlpha, 0, 1));
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BgImage);
            mStatus = FadeStatus.Null;
        }
        else if (mAlpha < 0F)
        {
            mAlpha = 0F;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, Mathf.Clamp(mAlpha, 0, 1));
            mStatus = FadeStatus.Null;
        }
        else if (mAlpha == 1.0F)
        {
            if(mStatus == FadeStatus.Null)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), BgImage);
        }
    }
}
