using Assets.Scripts.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * 代码说明：Ballance专用Mod文件解析类
 * 
 * 
 * 
 */

namespace Assets.Scripts.Worker
{
    /// <summary>
    /// Ballance Mod文件解析类
    /// </summary>
    public class BFModReader : BFSReader
    {
        public BFModReader()
        {

        }
        /// <summary>
        /// 预解析mod文件
        /// </summary>
        /// <param name="file">文件路径</param>
        public BFModReader(string file)
        {
            tryLoadFilePath = file;
        }
        /// <summary>
        /// 预解析mod AssetBundle
        /// </summary>
        /// <param name="file"> mod AssetBundle</param>
        public BFModReader(AssetBundle file)
        {
            tryLoadAssetBundle = file;
        }

        private AssetBundle tryLoadAssetBundle = null;
        private string tryLoadFilePath = "";
        private string lastLoadErr = "";
        private GlobalPack currentLoadPack = null;

        /// <summary>
        /// 此加载器正在加载的包。
        /// </summary>
        public GlobalPack CurrentLoadPack { get { return currentLoadPack; } }
        /// <summary>
        /// 此加载器上一个错误信息。
        /// </summary>
        public string LastLoaderError { get { return lastLoadErr; } }

        /// <summary>
        /// 读取。
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public IEnumerator Read(MonoBehaviour m)
        {
            string errMsg = "未知错误";
            GlobalPack p = null; ;

            if (tryLoadAssetBundle == null)
            {
                if (!string.IsNullOrEmpty(tryLoadFilePath))
                {
                    WWW www = new WWW(tryLoadFilePath);
                    yield return www;
                    if (string.IsNullOrEmpty(www.error))
                    {

                        tryLoadAssetBundle = www.assetBundle;
                        if (tryLoadAssetBundle == null)
                        {
                            errMsg = "模组包 " + p.Path + " 加载失败：这不是一个有效的模组包";
                            goto LoadError;
                        }

                        p = new GlobalPack(tryLoadFilePath);
                        p.Name = Path.GetFileNameWithoutExtension(tryLoadFilePath);
                    }
                    else
                    {
                        errMsg = "模组包 " + p.Path + " 加载失败：\n" + www.error;
                        goto LoadError;
                    }
                }
                else
                {
                    errMsg = "模组包 " + p.Path + " 加载失败：未指定";
                    goto LoadError;
                }
            }
            else
            {
                p = new GlobalPack(tryLoadAssetBundle.name);
                p.Name = tryLoadAssetBundle.name;
            }

            currentLoadPack = p;

            string[] a = p.Base.GetAllAssetNames();
            string t = "";
            foreach (string ss in a)
                if (ss.EndsWith(".ballance.txt"))
                {
                    t = ss;
                    break;
                }
            if (t == null)
            {
                errMsg = "模组包 " + p.Path + " 加载失败：找不到描述文件";
                goto LoadError;
            }
            TextAsset txt = p.Base.LoadAsset<TextAsset>(t);

            yield return m.StartCoroutine(Analysis(p, txt, m));


            LoadError:
            lastLoadErr = errMsg;
            //GlobalMediator.LogErr(callerName, errMsg);
            p.LoadState = GlobalPackLoadState.LoadFailed;
            yield break;
        }

        /// <summary>
        /// 分析。
        /// </summary>
        /// <param name="p">指定包，不指定则使用默认。</param>
        /// <param name="txt">文本。</param>
        /// <param name="m"></param>
        /// <returns></returns>
        public IEnumerator Analysis(GlobalPack p, TextAsset txt ,MonoBehaviour m)
        {
            string errMsg = "未知错误";

            if (p == null)
                p = currentLoadPack;
            else if (currentLoadPack == null)
                currentLoadPack = p;
            if (txt == null)
            {
                errMsg = "模组包 " + p.Path + " 加载失败：找不到描述文件";
                goto AnalysisError;
            }

            AnalysisString(txt.text);

            string aname = GetPropertyValue("ModAuthor");
            if (aname != null) p.AuthorName = aname;
            string name = GetPropertyValue("ModName");
            if (name != null) p.Name = name;
            string type = GetPropertyValue("ModType");
            switch (type)
            {
                case "Resource":
                    p.Type = GlobalPackType.Resource;
                    break;
                case "Level":
                    p.Type = GlobalPackType.Level;
                    break;
                case "Mod":
                    p.Type = GlobalPackType.Mod;
                    break;
            }
            string dps = GetPropertyValue("ModDepends");
            if (dps != null)
            {
                p.DependsPack = dps;
                yield return m.StartCoroutine(GlobalModLoader.LoadModDepends(p, m));
                if (p.LoadState == GlobalPackLoadState.LoadFailed)
                {
                    errMsg = "模组包 " + p.Path + " 加载失败：因为必要的一个依赖包无法加载。详情请查看控制台输出。";
                    goto AnalysisError;
                }
            }

            p.DescribeFile = this;

            string entry = GetPropertyValue("ModEntry");
            if (!string.IsNullOrEmpty(entry))
            {
                p.EntryFunction = entry;
                string dps1 = GetPropertyValue("NeedInitialize");
                if (dps1 != null)
                {
                    if (dps1 == "true") p.NeedInitialize = true;
                    else if (dps1 == "false") p.NeedInitialize = false;
                }
                if (!GlobalModLoader.RunModEntry(p) && GlobalSettings.Debug && !GlobalModLoader.ScenseIniting)
                {
                    GlobalMediator.Log("GrobalModLoader", "Mod :" + p.Path + " 初始化失败。");
                }
            }

            string dllname = GetPropertyValue("RegisterCodeModul");
            if (!string.IsNullOrEmpty(dllname))
            {
                string[] dllnames = GetPropertyValueChildValue(dllname);
                for (int i = 0; i < dllnames.Length; i++)
                {
                    string[] dllname2z = GetPropertyValueChildValue2(dllnames[i]);
                    GlobalDyamicModManager p2;
                    if (!GlobalModLoader.IsCodeModLoaded(StoragePathManager.GetCodeModPathWithName(dllname2z[0]), out p2))
                        GlobalModLoader.LoadCodeMod(StoragePathManager.GetCodeModPathWithName(dllname2z[0]), m, dllname2z.Length >= 2 ? dllname2z[1] : "");
                }
            }

            string partname = GetPropertyValue("RegisterGamePart");
            if (!string.IsNullOrEmpty(partname))
            {
                string[] dllnames = GetPropertyValueChildValue(partname);
                for (int i = 0; i < dllnames.Length; i++)
                {
                    string partname2 = dllnames[i];
                    if (partname2 != "")
                    {
                        GlobalGamePart g;
                        if (!GlobalModLoader.IsGamePartRegistered(partname2, out g))
                        {
                            g = new GlobalGamePart(p);
                            g.AutoAttachScript = GetPropertyValue(partname2 + ".AutoAttachScript");
                            g.AutoInitObject = GetPropertyValue(partname2 + ".AutoInitObject");
                            string s = GetPropertyValue(partname2 + ".PartType");
                            if (!string.IsNullOrEmpty(s)) g.Type = (GlobalGamePartType)System.Enum.Parse(typeof(GlobalGamePartType), s);
                            GlobalModLoader.RegisteredGameParts.Add(g);
                        }
                    }
                }
            }

            p.LoadState = GlobalPackLoadState.Loaded;
            yield break;

            AnalysisError:
            lastLoadErr = errMsg;
            //GlobalMediator.LogErr(callerName, errMsg);
            p.LoadState = GlobalPackLoadState.LoadFailed;
            yield break;
        }
    }
}
