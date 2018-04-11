using Assets.Scripts.Global;
using Assets.Scripts.Worker;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        CommandManager.RegisterCommand("gpack", ViewPacksCommandReceiverHandler, "查看 已加载的包", "[simple] / [detals][GlobalPack name]");
        CommandManager.RegisterCommand("gpart", ViewGamePartsCommandReceiverHandler, "查看 已注册的部件", "[simple] / [detals][GlobalGamePart name]");
    }

    //查看 已加载的包 指令接收器
    private static bool ViewPacksCommandReceiverHandler(string[] pararms)
    {
        if(pararms.Length>0)
        {
            switch(pararms[0])
            {
                case "simple":
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
                case "simple":
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
    /// 获取包中的资源。
    /// </summary>
    /// <param name="packName">包名。</param>
    /// <param name="resourceName">需要的资源名字。</param>
    /// <returns></returns>
    public static Object GetResource(string packName, string resourceName)
    {
        GlobalPack p;
        if (packName.Contains("*TYPE*")) packName = packName.Replace("*TYPE*", type);
        if (IsPackLoaded(packName, out p))
        {
            if (p.Base != null)
            {
                return p.Base.LoadAsset(resourceName);
            }
        }
        return null;
    }
    /// <summary>
    /// 获取包中的资源。
    /// </summary>
    /// <typeparam name="T">需要的资源类型。</typeparam>
    /// <param name="packName">包名。</param>
    /// <param name="resourceName">需要的资源名字。</param>
    /// <returns></returns>
    public static T GetResource<T>(string packName, string resourceName) where T : Object
    {
        GlobalPack p;
        if (packName.Contains("*TYPE*")) packName = packName.Replace("*TYPE*", type);
        if (IsPackLoaded(packName, out p))
        {
            if (p.Base != null)
                return p.Base.LoadAsset(resourceName, typeof(T)) as T;
        }
        return default(T);
    }

    /// <summary>
    /// 加载资源包（unity3d或assetbundle文件）。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="m">this</param>
    /// <returns>是否成功。</returns>
    public static bool LoadResourcePack(string path, MonoBehaviour m)
    {
        GlobalPack p;
        if (!IsPackLoaded(path, out p))
        {
            m.StartCoroutine(LoadResourcePackWait(path, m));
            return true;
        }
        return false;
    }
    /// <summary>
    /// 默认加载包。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="m">this</param>
    /// <returns>是否成功。</returns>
    public static bool LoadPack(string path, MonoBehaviour m)
    {
        GlobalPack p;
        if (!IsPackLoaded(path, out p))
        {
            m.StartCoroutine(LoadPackWait(path, m));
            return true;
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
    /// 获取在Ballance2_Data/Core文件夹里的dll路径
    /// </summary>
    /// <param name="name">dll名字</param>
    /// <returns></returns>
    public static string GetCodeModPathWithName(string name)
    {
        if (name.EndsWith(".dll"))
            return StaticValues.CoreFolder + name;
        else return StaticValues.CoreFolder + name + ".dll";
    }
    /// <summary>
    /// 文件加载dll
    /// </summary>
    /// <param name="path">dll路径</param>
    /// <param name="m"></param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName 必须是静态类，静态方法）</param>
    public static void LoadCodeMod(string path, MonoBehaviour m, string initcode = "")
    {
        try
        {
            GlobalDyamicModManager.LoadAssembly(path, initcode);
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
    public static void LoadCodeModInRes(string path, string name, MonoBehaviour m, string initcode = "")
    {
        try
        {
            GlobalDyamicModManager.LoadAssemblyInAssetBundle(path, name, m, initcode);

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

    public static IEnumerator LoadPackWaitInStatic(StaticLoader.StaticMod staticMod, MonoBehaviour m)
    {
        if (staticMod!=null)
        {
            GlobalPack p = new GlobalPack();
            p.LoadState = GlobalPackLoadState.Loading;
            if (staticMod.DescriptionFile != null)
            {
                p.Name = staticMod.Name;
                if (staticMod.Prefabs.Count > 0)
                    foreach (GameObject gg in staticMod.Prefabs)
                        p.AssetsPool.Add(gg.name + ".prefab", gg);

                TextAsset txt = staticMod.DescriptionFile;
                yield return m.StartCoroutine(LoadPackWaitKK(p.Name, txt, p, m));
            }
            else ModLoadFinishEventCall(staticMod.ToString(), "无效包", m, false, staticMod.ToString());
        }
    }
    public static IEnumerator LoadPackWait(string path, MonoBehaviour m)
    {
        WWW www = new WWW(path);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            GlobalPack p = new GlobalPack();
            p.LoadState = GlobalPackLoadState.Loading;
            p.Base = www.assetBundle;
            if (www.assetBundle != null)
            {
                p.Path = path;
                p.Name = Path.GetFileNameWithoutExtension(path);

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
                    ModLoadFinishEventCall(path, "找不到描述文件!", m, false, path);
                    p.LoadState = GlobalPackLoadState.LoadFailed;
                    yield break;
                }
                TextAsset txt = p.Base.LoadAsset<TextAsset>(t);

                yield return m.StartCoroutine(LoadPackWaitKK(path, txt, p, m));
            }
            else ModLoadFinishEventCall(path, "无效包", m, false, path);
        }
        else
        {
            ModLoadFinishEventCall(path, www.error, m, false, path);
        }
    }
    public static IEnumerator LoadPackWaitKK(string path, TextAsset txt, GlobalPack p, MonoBehaviour m)
    {
        GlobalMediator.CommandManager.Log("[GlobalModLoader] Loading " + path);
        if (txt == null)
        {
            ModLoadFinishEventCall(path, "找不到描述文件", m, false, path);
            p.LoadState = GlobalPackLoadState.LoadFailed;
            yield break;
        }
        BFSReader b = new BFSReader(txt);
        string aname = b.GetPropertyValue("ModAuthor");
        if (aname != null) p.AuthorName = aname;
        string name = b.GetPropertyValue("ModName");
        if (name != null) p.Name = name;
        string type = b.GetPropertyValue("ModType");
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
        string dps = b.GetPropertyValue("ModDepends");
        if (dps != null)
        {
            p.DependsPack = dps;
            yield return m.StartCoroutine(LoadModDepends(p, m));
            if (p.LoadState == GlobalPackLoadState.LoadFailed)
            {
                ModLoadFinishEventCall(path, "无法加载包，因为必要的一个依赖包无法加载。详情请查看控制台输出。", m, false, path);
                p.LoadState = GlobalPackLoadState.LoadFailed;
                yield break;
            }
        }
        LoadedPacks.Add(p);
        string entry = b.GetPropertyValue("ModEntry");
        if (!string.IsNullOrEmpty(entry))
        {
            p.EntryFunction = entry;
            string dps1 = b.GetPropertyValue("NeedInitialize");
            if (dps1 != null)
            {
                if (dps1 == "true") p.NeedInitialize = true;
                else if (dps1 == "false") p.NeedInitialize = false;
            }
            if (!RunModEntry(p) && GlobalSettings.Debug && !scenseIniting)
            {
                //GlobalMediator.CommandManager.OutPutDebug("GrobalModLoader", "Mod :" + path + " 初始化失败。");
            }
        }

        string dllname = b.GetPropertyValue("RegisterCodeModul");
        if (!string.IsNullOrEmpty(dllname))
        {
            string[] dllnames = b.GetPropertyValueChildValue(dllname);
            for (int i = 0; i < dllnames.Length; i++)
            {
                string[] dllname2z = b.GetPropertyValueChildValue2(dllnames[i]);
                GlobalDyamicModManager p2;
                if (!IsCodeModLoaded(GetCodeModPathWithName(dllname2z[0]), out p2))
                    LoadCodeMod(GetCodeModPathWithName(dllname2z[0]), m, dllname2z.Length >= 2 ? dllname2z[1] : "");
            }
        }

        string partname = b.GetPropertyValue("RegisterGamePart");
        if (!string.IsNullOrEmpty(partname))
        {
            string[] dllnames = b.GetPropertyValueChildValue(partname);
            for (int i = 0; i < dllnames.Length; i++)
            {
                string partname2 = dllnames[i];
                if(partname2!="")
                {
                    GlobalGamePart g;
                    if (!IsGamePartRegistered(partname2, out g))
                    {
                        g = new GlobalGamePart(p);
                        g.AutoAttachScript = b.GetPropertyValue(partname2 + ".AutoAttachScript");
                        g.AutoInitObject = b.GetPropertyValue(partname2 + ".AutoInitObject");
                        string s = b.GetPropertyValue(partname2 + ".PartType");
                        if(!string.IsNullOrEmpty(s)) g.Type =  (GlobalGamePartType)System.Enum.Parse(typeof(GlobalGamePartType), s);
                        RegisteredGameParts.Add(g);
                    }
                }
            }
        }

        ModLoadFinishEventCall(path, null, m, true);
        p.LoadState = GlobalPackLoadState.Loaded;
    }
    public static IEnumerator LoadResourcePackWait(string path, MonoBehaviour m)
    {
        GlobalMediator.CommandManager.Log("[GlobalModLoader] Loading Resource Pack " + path);
        WWW www = new WWW(path);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            GlobalPack p = new GlobalPack();
            p.Base = www.assetBundle;
            p.Path = path;
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
    public static IEnumerator LoadModDepends(GlobalPack p, MonoBehaviour m)
    {
        StringSpliter s = new StringSpliter(p.DependsPack, ':');
        if (s.Success)
        {
            foreach (string ss in s.Result)
            {
                if (ss != "" && ss != p.Name && ss != p.Path)
                {
                    if (!IsPackLoading(ss))
                    {
                        yield return m.StartCoroutine(LoadPackWait(ss, m));
                        GlobalPack p2;
                        if (!IsPackLoaded(ss, out p2))
                        {
                            GlobalMediator.CommandManager.OutPutError("Mod :" + p.Name + " 加载失败。因为依赖包 " + p2.Name + " 无法加载", "GlobalModLoader");
                            //
                            p2.LoadState =  GlobalPackLoadState.LoadFailed;
                            yield break;
                        }
                    }
                }
            }
        }
    }

    private static bool RunModEntry(GlobalPack p)
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
                                if (!RunModEntry(p1))
                                {
                                    //GlobalMediator.CommandManager.OutPutDebug("GrobalModLoader", "Mod :" + p.Name + " 的依赖包 " + ss + " 无法初始化。");
                                    erred = true;
                                }
                        }
                        else
                        {
                            //GlobalMediator.CommandManager.OutPutDebug("GrobalModLoader", "Mod :" + p.Name + " 的依赖包 " + ss + " 无法初始化。(找不到包)");
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

    static float allcount = 0;
    static float finishcount = 0;
    static bool nextmustload = false;
    private static bool gameInited = false;
    private static bool scenseIniting = false;
    static string type = "";
    static IEnumerator GameInit1(MonoBehaviour m)
    {
        string path = StaticValues.CoreFolder + "GameInit.txt";
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
        string path = StaticValues.CoreFolder + strs[0];
        if (path.Contains("*TYPE*"))
            path = path.Replace("*TYPE*", type);
        if (strs[1] == "MustLoad")
            nextmustload = true;
        else nextmustload = false;

        switch (strs[2].Trim())
        {
            case "Resource":
                yield return m.StartCoroutine(LoadResourcePackWait(path, m));
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
                yield return m.StartCoroutine(LoadPackWait(path, m));
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
            if (LoadedPacks != null)
            {
                foreach (GlobalPack p in LoadedPacks)
                    p.Dispose();
                LoadedPacks.Clear();
            }
            LoadedPacks = null;
            RegisteredGameParts.Clear();
            RegisteredGameParts = null;
        }
    }
}