using Assets.Scripts.GameCore;
using Assets.Scripts.Global;
using Assets.Scripts.Worker;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SLua;

/*
 * 代码说明：游戏动态模块（mod）加载器。
 *                    游戏加载器（GameInit）。
 *                    
 ********************************************
*/

/// <summary>
/// 游戏动态模块（mod）加载器。
/// </summary>
public static class GlobalModLoader
{    
    /// <summary>
    /// 初始化
    /// </summary>
    public static void GlobalModLoaderInitialization()
    {
        LoadedPacks = new List<GlobalPack>();
        RegisteredGameParts = new List<GlobalGamePart>();
        RegisteredGameModuls = new List<GlobalGameModul>();
        CommandManager.RegisterCommand("gpack", ViewPacksCommandReceiverHandler, "查看 已加载的包", "[all] / [detals/assets/assets2][GlobalPack name]");
        CommandManager.RegisterCommand("gpart", ViewGamePartsCommandReceiverHandler, "查看 已注册的部件", "[all] / [detals/assets/assets2][GlobalGamePart name]");
    }

    //查看 已加载的包 指令接收器
    private static bool ViewPacksCommandReceiverHandler(string[] pararms)
    {
        if(pararms.Length>0)
        {
            switch(pararms[0])
            {
                case "all":
                    GlobalMediator.CommandManager.OutPut("已加载的包 <size=9>共" + LoadedPacks.Count + "个</size>");
                    foreach (GlobalPack p in LoadedPacks)
                        GlobalMediator.CommandManager.OutPut(p.Name + " <color=#1177ffff>作者</color> <color=#66eeeeff>" + p.AuthorName + "</color> <color=#1177ffff>加载状态</color> " + p.GetLoadStateStr() + "  [" + p.Path + "]");
                    return true;
                case "detals":
                    if (pararms.Length > 1)
                    {
                        string pss = pararms[1];
                        GlobalPack p;
                        if (IsPackLoaded(pss, out p))
                        {
                            GlobalMediator.CommandManager.OutPut("<color=#66eeeeff>" + p.Name + "</color> 的信息");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>作者</color> <color=#66eeeeff>" + p.AuthorName + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>加载路径</color> <color=#66eeeeff>" + p.Path + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>加载状态</color> <color=#66eeeeff>" + p.GetLoadStateStr() + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>入口函数</color> <color=#66eeeeff>" + p.EntryFunction + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>类型</color> <color=#66eeeeff>" + p.Type + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>需要初始化</color> <color=#66eeeeff>" + p.NeedInitialize + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>依赖包</color> <color=#66eeeeff>" + p.DependsPack + "</color>");
                            return true;
                        }
                        else GlobalMediator.CommandManager.SetCurrentCommandResult("未找到 GlobalPack " + pss);
                    }
                    else GlobalMediator.CommandManager.SetCurrentCommandResult("错误的参数 detals ：需要第二个参数");
                    break;
                case "assets":
                    if (pararms.Length > 1)
                    {
                        string pss = pararms[1];
                        GlobalPack p;
                        if (IsPackLoaded(pss, out p))
                        {
                            GlobalMediator.CommandManager.OutPut("<color=#66eeeeff>" + p.Name + "</color> 的 AssetPool 信息");
                            foreach (var item in p.AssetsPool)
                            {
                                GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>名字 </color> <color=#66eeeeff>" + item.Key + "</color> <color=#1177ffff>名字 </color> <color=#66eeeeff>" + item.Value.name + "</color>");
                            }                         
                            return true;
                        }
                        else GlobalMediator.CommandManager.SetCurrentCommandResult("未找到 GlobalPack " + pss);
                    }
                    else GlobalMediator.CommandManager.SetCurrentCommandResult("错误的参数 detals ：需要第二个参数");
                    break;
                case "assets2":
                    if (pararms.Length > 1)
                    {
                        string pss = pararms[1];
                        GlobalPack p;
                        if (IsPackLoaded(pss, out p))
                        {
                            GlobalMediator.CommandManager.OutPut("<color=#66eeeeff>" + p.Name + "</color> 的 AssetBundle 信息");
                            if (p.Base != null)
                            {
                                string[] s = p.Base.GetAllAssetNames();
                                foreach (var item in s)
                                    GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>资源 </color> <color=#66eeeeff>" + item + "</color>");
                                string[] s2 = p.Base.GetAllScenePaths();
                                foreach (var item in s2)
                                    GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>场景 </color> <color=#66eeeeff>" + item + "</color>");
                                return true;
                            }
                            GlobalMediator.CommandManager.OutPut("<color=#66eeeeff>不存在 AssetBundle </color>");
                        }
                        else GlobalMediator.CommandManager.SetCurrentCommandResult("未找到 GlobalPack " + pss);
                    }
                    else GlobalMediator.CommandManager.SetCurrentCommandResult("错误的参数 detals ：需要第二个参数");
                    break;
                default:
                    GlobalMediator.CommandManager.SetCurrentCommandResult("错误的参数：" + pararms[0]);
                    return false;
            }
        }
        return false;
    }
    //查看 已注册的部件 指令接收器
    private static bool ViewGamePartsCommandReceiverHandler(string[] pararms)
    {
        if (pararms.Length > 0)
        {
            switch (pararms[0])
            {
                case "all":
                    GlobalMediator.CommandManager.OutPut("已注册的部件 <size=9>共" + LoadedPacks.Count + "个</size>");
                    foreach (GlobalGamePart p in RegisteredGameParts)
                        GlobalMediator.CommandManager.OutPut(p.Name +
                           " <color=#1177ffff>AutoAttachScript</color> <color=#66eeeeff>" + p.AutoAttachScript + "</color> <color=#1177ffff>AutoInitObject</color> <color=#66eeeeff>" + p.AutoInitObject +
                           "</color> <color=#1177ffff>类型</color> <color=#66eeeeff>" + p.Type +
                           "</color> <color=#1177ffff>Pack</color> <color=#66eeeeff>" + p.Pack.Name + "</color>"
                           );       
                    return true;
                case "detals":
                    if (pararms.Length > 1)
                    {
                        string pss = pararms[1];
                        GlobalGamePart p;
                        if (IsGamePartRegistered(pss, out p))
                        {
                            GlobalMediator.CommandManager.OutPut("<color=#66eeeeff>" + p.Name + "</color> 的信息");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>AutoAttachScript</color> <color=#66eeeeff>" + p.AutoAttachScript + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>AutoInitObject</color> <color=#66eeeeff>" + p.AutoInitObject + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>类型</color> <color=#66eeeeff>" + p.Type + "</color>");
                            GlobalMediator.CommandManager.OutPut(" <color=#1177ffff>Pack</color> <color=#66eeeeff>" + p.Pack.Name + "</color>");
                            return true;
                        }
                        else GlobalMediator.CommandManager.SetCurrentCommandResult("未找到 GlobalGamePart " + pss);
                    }
                    else GlobalMediator.CommandManager.SetCurrentCommandResult("错误的参数 detals ：需要第二个参数");
                    break;
                default:
                    GlobalMediator.CommandManager.SetCurrentCommandResult("错误的参数：" + pararms[0]);
                    return false;
            }
        }
        return false;
    }

    /// <summary>
    /// 已加载的包。
    /// </summary>
    public static List<GlobalPack> LoadedPacks
    {
        get;
        private set;
    }
    /// <summary>
    /// 已注册的部件。
    /// </summary>
    public static List<GlobalGamePart> RegisteredGameParts
    {
        get;
        private set;
    }
    /// <summary>
    /// 已注册的机关。
    /// </summary>
    public static List<GlobalGameModul> RegisteredGameModuls
    {
        get;
        private set;
    }

    /// <summary>
    /// 获取已加载的GamePart，如果没有加载，则返回false。
    /// </summary>
    /// <param name="path">名字。</param>
    /// <param name="p">接收包的变量。</param>
    /// <returns>是否成功。</returns>
    public static bool IsGamePartRegistered(string name, out GlobalGamePart p)
    {
        bool b = false;
        foreach (GlobalGamePart g in RegisteredGameParts)
        {
            if (g.Name == name)
            {
                p = g;
                return b;
            }
        }
        p = null;
        return b;
    }
    /// <summary>
    /// 获取已加载的 GameModul，如果没有加载，则返回false。
    /// </summary>
    /// <param name="path">名字。(机关名:包名 或 机关名)</param>
    /// <param name="p">接收包的变量。</param>
    /// <returns>是否成功。</returns>
    public static bool IsGameModulRegistered(string name, out GlobalGameModul p)
    {
        if (string.IsNullOrEmpty(name)) { p = null; return false; }
        bool b = false;
        string rname, pname = "";
        if (name.Contains(":"))
        {
            string[] ss = name.Split(':');
            rname = ss[0];
            pname = ss[1];
        }
        else rname = name;
        string name2 = "P_" + rname;
        if (pname == "")
        {
            foreach (GlobalGameModul g in RegisteredGameModuls)
            {
                if (g.Name == rname || g.Name == name2)
                {
                    p = g;
                    return b;
                }
            }
        }
        else
        {
            GlobalPack g;
            if (IsPackLoaded(pname, out g))
            {
                foreach (GlobalGameModul gm in RegisteredGameModuls)
                {
                    if (g == gm.ParentPack && (gm.Name == rname || gm.Name == name2))
                    {
                        p = gm;
                        return b;
                    }
                }
            }
        }
        p = null;
        return b;
    }
    /// <summary>
    /// 尝试注册 Modul
    /// </summary>
    /// <param name="name">名字(机关名:包名 或 机关名)</param>
    /// <returns></returns>
    public static bool TryRegisterGameModul(string name, GlobalPack inpack = null)
    {
        if (string.IsNullOrEmpty(name)) return false;
        string rname, pname = "";
        if (name.Contains(":"))
        {
            string[] ss = name.Split(':');
            rname = ss[0];
            pname = ss[1];
        }
        else rname = name;
        if (inpack != null)
        {
            if (inpack.HasPreRegisterModul(rname))
                return TryRegisterGameModulInnern(rname, inpack);
        }
        else
        {
            if (pname == "")
            {
                foreach (GlobalPack g in LoadedPacks)
                {
                    if (g.HasPreRegisterModul(rname))
                    {
                        if (TryRegisterGameModulInnern(rname, g))
                            return true;
                    }
                }
            }
            else
            {
                GlobalPack g;
                if (IsPackLoaded(pname, out g))
                {
                    if (g.HasPreRegisterModul(rname))
                    {
                        if (TryRegisterGameModulInnern(rname, g))
                            return true;
                    }
                }
            }
        }
        return false;
    }
    private static bool TryRegisterGameModulInnern(string name, GlobalPack inpack)
    {
        if (inpack != null && inpack.DescribeFile != null)
        {
            GlobalGameModul gm = new GlobalGameModul(name, inpack);
            BFSReader reader = inpack.DescribeFile;
            string at = reader.GetPropertyValue(name + ".ActiveType");
            if(!string.IsNullOrEmpty(at))
                gm.ActiveType = (GlobalGameModulActiveType)System.Enum.Parse(typeof(GlobalGameModulActiveType), at);

            at = reader.GetPropertyValue(name + ".StartActive");
            if (!string.IsNullOrEmpty(at))
                gm.StartActive = ((at == "1") ||( at.ToLower() == "true"));

            at = reader.GetPropertyValue(name + ".ICBackupType");
            if (!string.IsNullOrEmpty(at))
                gm.ICBackupType = (ICBackType)System.Enum.Parse(typeof(ICBackType), at);

            at = reader.GetPropertyValue(name + ".ICResetType");
            if (!string.IsNullOrEmpty(at))
                gm.ICResetType = (ICResetType)System.Enum.Parse(typeof(ICResetType), at);

            at = reader.GetPropertyValue(name + ".ICBackupCustom");
            if (!string.IsNullOrEmpty(at))
                gm.ICBackupCustom = at;

            at = reader.GetPropertyValue(name + ".ModulCreater");
            if (!string.IsNullOrEmpty(at))
                gm.ModulCreater = at;

            at = reader.GetPropertyValue(name + ".ModulPerfab");
            if (!string.IsNullOrEmpty(at))
                gm.BasePerfab = GlobalAssetPool.GetAsset(at) as GameObject;
        }
        return false;
    }

    /// <summary>
    /// 获取已加载的包，如果没有加载，则返回false。
    /// </summary>
    /// <param name="path">名字。</param>
    /// <param name="p">接收包的变量。</param>
    /// <returns>是否成功。</returns>
    public static bool IsPackLoaded(string path, out GlobalPack p)
    {
        if (path.Contains("*TYPE*")) path = path.Replace("*TYPE*", type);
        bool b = false;
        foreach (GlobalPack g in LoadedPacks)
        {
            if (g.Path == path || g.Name == path)
            {
                p = g;
                b = g.LoadState == GlobalPackLoadState.Loaded;
                return b;
            }
        }
        p = null;
        return b;
    }
    /// <summary>
    /// 获取包 是否正在加载。
    /// </summary>
    /// <param name="path">名字。</param>
    /// <returns>是否成功。</returns>
    public static bool IsPackLoading(string path)
    {
        if (path.Contains("*TYPE*")) path = path.Replace("*TYPE*", type);
        bool b = false;
        foreach (GlobalPack g in LoadedPacks)
        {
            if ((g.Path == path || g.Name == path) && g.LoadState == GlobalPackLoadState.Loading)
            {
                b = true;
                return b;
            }
            else return false;
        }
        return b;
    }


    /// <summary>
    /// 文件加载dll
    /// </summary>
    /// <param name="path">dll路径</param>
    /// <param name="m"></param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName 必须是静态类，静态方法）</param>
    public static void LoadCodeMod(string path, MonoBehaviour m, string initcode = "", DyamicModType type = DyamicModType.MonoModule)
    {
        try
        {
            GlobalDyamicModManager.LoadAssembly(path, initcode, type);
        }
        catch (System.Exception e)
        {
            ModLoadFinishEventCall(path, e.ToString(), m, false);
            return;
        }
        ModLoadFinishEventCall(path, null, m, true);
    }
    /// <summary>
    /// 资源包加载dll
    /// </summary>
    /// <param name="path">资源包路径</param>
    /// <param name="name">dll在资源中的名字</param>
    /// <param name="m"></param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName 必须是静态类，静态方法）</param>
    public static void LoadCodeModInRes(string path, string name, MonoBehaviour m, string initcode = "", DyamicModType type = DyamicModType.MonoModule)
    {
        try
        {
            GlobalDyamicModManager.LoadAssemblyInAssetBundle(path, name, m, initcode, type);

        }
        catch (System.Exception e)
        {
            ModLoadFinishEventCall(path, e.ToString(), m, false); return;
        }
        ModLoadFinishEventCall(path, null, m, true);
    }
    /// <summary>
    /// 获取已加载的包，如果没有加载，则返回false。
    /// </summary>
    /// <param name="path">名字。</param>
    /// <param name="p">接收包的变量。</param>
    /// <returns>是否成功。</returns>
    public static bool IsCodeModLoaded(string path, out GlobalDyamicModManager p)
    {
        if (path.Contains("*TYPE*")) path = path.Replace("*TYPE*", type);
        bool b = false;
        foreach (GlobalDyamicModManager g in GlobalDyamicModManager.LoadedDlls)
        {
            if (g.path == path || g.succeed)
            {
                p = g;
                b = true;
                return b;
            }
        }
        p = null;
        return b;
    }

    /// <summary>
    /// 阻塞加载静态包。
    /// </summary>
    /// <param name="staticMod"></param>
    /// <param name="m">this</param>
    /// <returns></returns>
    public static IEnumerator LoadPackInStatic(StaticLoader.StaticMod staticMod, MonoBehaviour m)
    {
        if (staticMod!=null)
        {
            GlobalPack p = new GlobalPack(staticMod.Name);
            p.LoadState = GlobalPackLoadState.Loading;
            if (staticMod.DescriptionFile != null)
            {
                p.Name = staticMod.Name;
                if (staticMod.Prefabs.Count > 0)
                    foreach (GameObject gg in staticMod.Prefabs)
                        p.AssetsPool.Add(gg.name + ".prefab", gg);

                BFModReader modReader = new BFModReader(p.Path);
                TextAsset txt = staticMod.DescriptionFile;
                yield return m.StartCoroutine(modReader.Analysis(p, txt, m));
                ModLoadFinishEventCall(p.Path, modReader.LastLoaderError, m, modReader.CurrentLoadPack.LoadState == GlobalPackLoadState.Loaded, p.Path);
            }
            else ModLoadFinishEventCall(staticMod.ToString(), "无效包", m, false, staticMod.ToString());
        }
    }
    /// <summary>
    /// 阻塞加载包。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="m">this</param>
    /// <returns></returns>
    public static IEnumerator LoadPack(string path, MonoBehaviour m)
    {
        BFModReader modReader = new BFModReader(path);
        yield return m.StartCoroutine(modReader.Read(m));
        ModLoadFinishEventCall(path, modReader.LastLoaderError, m, modReader.CurrentLoadPack.LoadState == GlobalPackLoadState.Loaded, path);
    }
    /// <summary>
    /// 阻塞加载资源包（unity3d或assetbundle文件）。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="m">this</param>
    public static IEnumerator LoadResourcePack(string path, MonoBehaviour m)
    {
        GlobalMediator.Log("[GlobalModLoader] Loading Resource Pack " + path);
        WWW www = new WWW(path);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            GlobalPack p = new GlobalPack(path);
            p.Base = www.assetBundle;
            p.Name = Path.GetFileNameWithoutExtension(path);
            LoadedPacks.Add(p);
            if (p.Base != null)
            {
                p.Type = GlobalPackType.Resource;
                ModLoadFinishEventCall(path, null, m, true);
                p.LoadState = GlobalPackLoadState.Loaded;
                yield break;
            }
            else
            {
                ModLoadFinishEventCall(path, "Failed to load base assetBundle!", m, false, path);
                p.LoadState = GlobalPackLoadState.LoadFailed;
            }
        }
        else  ModLoadFinishEventCall(path, www.error, m);
    }
    /// <summary>
    /// 加载包的依赖。
    /// </summary>
    /// <param name="p">包</param>
    /// <param name="m">this</param>
    /// <returns></returns>
    public static IEnumerator LoadModDepends(GlobalPack p, MonoBehaviour m)
    {
        if(p.LoadState!= GlobalPackLoadState.Loading)
            yield break;
        StringSpliter s = new StringSpliter(p.DependsPack, ':');
        if (s.Success)
        {
            foreach (string ss in s.Result)
            {
                if (ss != "" && ss != p.Name && ss != p.Path)
                {
                    if (!IsPackLoading(ss))
                    {
                        yield return m.StartCoroutine(LoadPack(ss, m));
                        GlobalPack p2;
                        if (!IsPackLoaded(ss, out p2))
                        {
                            GlobalMediator.LogErr("Mod :" + p.Name + " 加载失败。因为依赖包 " + p2.Name + " 无法加载", "GlobalModLoader");
                            //
                            p2.LoadState =  GlobalPackLoadState.LoadFailed;
                            yield break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 运行包的入口函数。
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static bool RunModEntry(GlobalPack p)
    {
        bool erred = false;
        if (p.LoadState == GlobalPackLoadState.Loaded && !p.NeedInitialize)
        {
            StringSpliter s = new StringSpliter(p.DependsPack, ':');
            if (s.Success)
            {
                foreach (string ss in s.Result)
                {
                    if (ss != "" && ss != p.Name && ss != p.Path)
                    {
                        GlobalPack p1;
                        if (IsPackLoaded(ss, out p1))
                        {
                            if (p.LoadState == GlobalPackLoadState.Loaded && !p1.NeedInitialize)
                            {
                                if (!RunModEntry(p1))
                                {
                                    GlobalMediator.Log("GrobalModLoader", "Mod :" + p.Name + " 的依赖包 " + ss + " 无法初始化。");
                                    erred = true;
                                }

                                //Run Entry
                            }
                        }
                        else
                        {
                            GlobalMediator.Log("GrobalModLoader", "Mod :" + p.Name + " 的依赖包 " + ss + " 无法初始化。(找不到包)");
                            erred = true;
                        }
                    }
                }
            }
            p.NeedInitialize = false;
        }
        return !erred;
    }

    private static void ModLoadFinishEventCall(string e, string err, MonoBehaviour m, bool f = false, string path = "")
    {
        LaetPackLoadSuccess = f;
        if (!f) GlobalMediator.CommandManager.OutPutError("Mod :" + e + " 加载失败。" + err, "GlobalModLoader");
        if (f) GlobalMediator.DispatchEvent("ModLoadFinish", null, true, e);
        else GlobalMediator.DispatchEvent("ModLoadFinish", null, false, e, err);

    }

    /// <summary>
    /// 上一个包是否加载
    /// </summary>
    public static bool LaetPackLoadSuccess
    {
        get;private set;
    }

    #region GameInit

    public delegate void GameInitFeedBack(bool finished, string status, float progress, bool error = false);
    static GameInitFeedBack gameInitFeedBack;

    public static void GameInit(GameInitFeedBack df, MonoBehaviour m)
    {
        if (!gameInited)
        {
            gameInitFeedBack = df;
            gameInited = false;
            m.StartCoroutine(GameInit1(m));
        }
    }

    public static string GameTypeString { get { return type; } }
    public static bool ScenseIniting { get { return scenseIniting; } }

    static float allcount = 0;
    static float finishcount = 0;
    static bool nextmustload = false;
    private static bool gameInited = false;
    private static bool scenseIniting = false;
    static string type = "";
    static IEnumerator GameInit1(MonoBehaviour m)
    {
        string path = StoragePathManager.CoreFolder + "GameInit.txt";
        WWW www = new WWW(path);
        yield return www;
        if (www.error == null)
        {
            BFSReader br = new BFSReader(www.text);
            string type = br.GetPropertyValue("TYPE");
            GlobalModLoader.type = type;
            string version = br.GetPropertyValue("COREVER");
            if (version != StaticValues.GameVersion)
                gameInitFeedBack(false, "GameInit 目标版本版本不符。\n请尝试重新安装Ballance2。\n内核版本：" + StaticValues.GameVersion + "\n目标版本：" + version, 0.3f, true);
            else
            {
                Application.runInBackground = true;
                string[] strs = br.GetLineAllItems();
                allcount = strs.Length;
                foreach (string s in strs)
                {
                    yield return m.StartCoroutine(GameInit2(s, m));
                    if (!LaetPackLoadSuccess && nextmustload)
                        gameInitFeedBack(false, "无法继续加载游戏，因为必要的一个模块\nPath: " + s + "\n不能成功加载", 0.3f, true);
                }

                gameInitFeedBack(true, null, 0);
                gameInited = true;
            }
        }
        else gameInitFeedBack(false, "GameInit 加载失败。\n请尝试重新安装Ballannce2。\nError: " + www.error + "\nPath: " + path, 0.3f, true);
    }
    static IEnumerator GameInit2(string s, MonoBehaviour m)
    {
        string[] strs = s.Split(';');
        string path = StoragePathManager.CoreFolder + strs[0];
        if (path.Contains("*TYPE*"))
            path = path.Replace("*TYPE*", type);
        if (strs[1] == "MustLoad")
            nextmustload = true;
        else nextmustload = false;

        switch (strs[2].Trim())
        {
            case "Resource":
                yield return m.StartCoroutine(LoadResourcePack(path, m));
                break;
            case "Code":
                //path = strs[0];
                if (strs.Length >= 4)
                    LoadCodeMod(path, m, strs[3]);
                else LoadCodeMod(path, m);
                break;
            case "CodeRes":
                if (strs.Length >= 4)
                {
                    //path = strs[0];
                    if (strs.Length >= 5)
                        LoadCodeModInRes(path, strs[3], m, strs[4]);
                    else LoadCodeModInRes(path, strs[3], m);
                }
                break;
            case "Function":
            default:
                yield return m.StartCoroutine(LoadPack(path, m));
                break;
        }
        finishcount++;
        gameInitFeedBack(false, "Loading " + path, finishcount / allcount);
    }

    #endregion

    /// <summary>
    /// 退出游戏之前的清理。【不可调用】
    /// </summary>
    public static void ExitGameClear()
    {
        if (GlobalMediator.GameExiting)
        {
            foreach (GlobalPack p in LoadedPacks)
                p.Dispose();
            LoadedPacks.Clear();
            LoadedPacks = null;
            RegisteredGameModuls.Clear();
            RegisteredGameModuls = null;
            RegisteredGameParts.Clear();
            RegisteredGameParts = null;
        }
    }
}