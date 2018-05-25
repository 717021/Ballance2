using Assets.Scripts.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * 天空盒生成器
 *   skys_*TYPE* 资源包内的资源
 */

namespace Assets.Scripts.Hlper
{
    /// <summary>
    /// 天空盒生成器
    /// </summary>
    public class SkyMaker
    {
        /// <summary>
        /// 创建预制的天空盒
        /// </summary>
        /// <param name="s">天空盒名字，（必须是 A~K ，对应原版游戏12个关卡的天空）</param>
        /// <returns>返回创建好的天空盒</returns>
        public static Material MakeSkyBox(string s)
        {
            Texture SkyLeft = GlobalAssetPool.GetResource("skys_*TYPE*", "Sky_"+s+"_Left.BMP") as Texture;
            Texture SkyRight = GlobalAssetPool.GetResource("skys_*TYPE*", "Sky_" + s + "_Right.BMP") as Texture;
            Texture SkyFront = GlobalAssetPool.GetResource("skys_*TYPE*", "Sky_" + s + "_Front.BMP") as Texture;
            Texture SkyBack = GlobalAssetPool.GetResource("skys_*TYPE*", "Sky_" + s + "_Back.BMP") as Texture;
            Texture SkyDown = GlobalAssetPool.GetResource("skys_*TYPE*", "Sky_" + s + "_Down.BMP") as Texture;
            Material m = new Material(Shader.Find("Skybox/6 Sided"));
            m.SetTexture("_FrontTex", SkyFront);
            m.SetTexture("_BackTex", SkyBack);
            m.SetTexture("_LeftTex", SkyRight);
            m.SetTexture("_RightTex", SkyLeft);
            m.SetTexture("_DownTex", SkyDown);
            return m;
        }
        /// <summary>
        /// 创建自定义天空盒
        /// </summary>
        /// <param name="SkyLeft">左边的图像</param>
        /// <param name="SkyRight">右边的图像</param>
        /// <param name="SkyFront">前边的图像</param>
        /// <param name="SkyBack">后边的图像</param>
        /// <param name="SkyDown">下边的图像</param>
        /// <returns>返回创建好的天空盒</returns>
        public static Material MakeCustomSkyBox(Texture SkyLeft, Texture SkyRight, Texture SkyFront, Texture SkyBack, Texture SkyDown)
        {
            Material m = new Material(Shader.Find("Skybox/6 Sided"));
            m.SetTexture("_FrontTex", SkyFront);
            m.SetTexture("_BackTex", SkyBack);
            m.SetTexture("_LeftTex", SkyRight);
            m.SetTexture("_RightTex", SkyLeft);
            m.SetTexture("_DownTex", SkyDown);
            return m;
        }
    }
}
