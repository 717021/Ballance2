using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using SLua;
using Assets.Scripts.Worker;

/*
 * 代码说明：动态加载dll
 * 
 * 为了实现功能：游戏mod的开发，扩展api
 * 需要动态载入代码
 * 
 * 动态载入代码/动态附加脚本模块
 * 
 */

/// <summary>
/// mod类型
/// </summary>
public enum DyamicModType
{
    /// <summary>
    /// LUA模块
    /// </summary>
    LUAModule,
    /// <summary>
    /// c#模块
    /// </summary>
    MonoModule,
}
/// <summary>
/// 代码MOD加载器，扩展Mod的必要部分
/// </summary>
public class GlobalDyamicModManager : MarshalByRefObject
{
    /// <summary>
    /// 已经加载的Dll数组
    /// </summary>
    public static List<GlobalDyamicModManager> LoadedDlls = new List<GlobalDyamicModManager>();
    /// <summary>
    /// 当前 程序集
    /// </summary>
    public Assembly assembly { get; private set; }
    /// <summary>
    /// 当前 LUA 虚拟机
    /// </summary>
    public LuaState luaState { get; private set; }
    /// <summary>
    /// 获取加载是否成功
    /// </summary>
    public bool succeed { get { return suc; } }
    /// <summary>
    /// 获取 DLL 路径
    /// </summary>
    public string path { get { return pat; } }
    /// <summary>
    /// mod类型
    /// </summary>
    public DyamicModType type { get; private set; }

    private string pat = "";
    private bool suc = false;

    /// <summary>
    /// 文件加载dll或lua脚本
    /// </summary>
    /// <param name="path">dll路径或lua脚本路径</param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName）或自动开始运行的lua脚本文件名</param>
    public GlobalDyamicModManager(string path, string initcode = "", DyamicModType type = DyamicModType.MonoModule)
    {
        this.type = type;
        pat = path;
        if (type == DyamicModType.MonoModule)
        {
            assembly = Assembly.LoadFile(path);
            CallDllInit(initcode);
        }
        else if (type == DyamicModType.MonoModule)
        {
            luaState = new LuaState();
            luaState.loaderDelegate = ((string fn) =>
            {
                if (path == "")
                    return System.IO.File.ReadAllBytes(fn);
                else return System.IO.File.ReadAllBytes(path + "\\" + fn);
            });
            if (initcode.Contains(":"))
            {
                string[] s = initcode.Split(':');
                luaState.doFile(s[0]);
                if (s.Length >= 2)
                {
                    LuaFunction startFun = luaState.getFunction(s[2]);
                    startFun.call();
                }
            }
            else luaState.doFile(initcode);
            suc = true;
        }
    }
    /// <summary>
    /// 在AssetBundle里加载dll或lua脚本
    /// </summary>
    /// <param name="path">AssetBundle路径</param>
    /// <param name="name">dll或lua脚本在资源中的名字</param>
    /// <param name="m">MonoBehaviour</param>
    /// <param name="initcode">初始化调DLL中用的函数（initcode格式：className:methodName）或 自动开始运行的lua脚本文件名:动开始运行的lua脚本函数</param>
    public GlobalDyamicModManager(string path, string name, MonoBehaviour m, string initcode = "", DyamicModType type = DyamicModType.MonoModule)
    {
        this.type = type;
        m.StartCoroutine(LoadDll(path, name, type, initcode));
    }
    private IEnumerator LoadDll(string path, string name, DyamicModType type, string initcode)
    {
        WWW www = new WWW(path);
        yield return www;
        pat = path;
        AssetBundle bundle = www.assetBundle;

        if (type == DyamicModType.MonoModule)
        {
            TextAsset asset = bundle.LoadAsset(name, typeof(TextAsset)) as TextAsset;
            assembly = Assembly.Load(asset.bytes);
            suc = (assembly != null);
            CallDllInit(initcode);
        }
        else if (type == DyamicModType.LUAModule)
        {
            luaState = new LuaState();
            luaState.loaderDelegate = ((string fn) =>
            {
                TextAsset asset = bundle.LoadAsset(fn, typeof(TextAsset)) as TextAsset;
                return asset.bytes;
            });
            if (initcode != "")
                if (initcode.Contains(":"))
                {
                    string[] s = initcode.Split(':');
                    luaState.doFile(s[0]);
                    if (s.Length >= 2)
                    {
                        LuaFunction startFun = luaState.getFunction(s[2]);
                        startFun.call();
                    }
                }
                else  luaState.doFile(initcode);
            suc = true;
        }

    }
    //
    //Dll 调用模块入口点（我的设计）
    //
    //用于Mod初始化用（）initcode格式：className:methodName 必须是静态类，静态方法
    private void CallDllInit(string initcode)
    {
        if (assembly != null)
        {
            if (initcode.Contains(":"))
            {
                string[] s = initcode.Split(':');
                if (s.Length >= 2)
                {
                    Type type = assembly.GetType(s[0]);
                    MethodInfo methodInfo = type.GetMethod(s[1], BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { }, null);
                    if (s.Length > 2)
                        methodInfo.Invoke(null, new object[] { s[2] });
                    else methodInfo.Invoke(null, null);
                }
            }
        }
    }

    /// <summary>
    /// 文件加载dll或lua脚本
    /// </summary>
    /// <param name="path">dll路径或lua脚本路径</param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName）或自动开始运行的lua脚本文件名</param>
    public static void LoadAssembly(string path, string initcode = "", DyamicModType type = DyamicModType.MonoModule)
    {
        LoadedDlls.Add(new GlobalDyamicModManager(path, initcode, type));
    }
    /// <summary>
    /// 在AssetBundle里加载dll或lua脚本
    /// </summary>
    /// <param name="path">AssetBundle路径</param>
    /// <param name="name">dll或lua脚本在资源中的名字</param>
    /// <param name="m">MonoBehaviour</param>
    /// <param name="initcode">初始化调DLL中用的函数（initcode格式：className:methodName）或 自动开始运行的lua脚本文件名:动开始运行的lua脚本函数</param>
    public static void LoadAssemblyInAssetBundle(string path, string name, MonoBehaviour m, string initcode = "", DyamicModType type = DyamicModType.MonoModule)
    {
        LoadedDlls.Add(new GlobalDyamicModManager(path, name, m, initcode, type));
    }

    /// <summary>
    /// 获取已经加载的DLL
    /// </summary>
    /// <param name="name">名字</param>
    /// <returns>返回GlobalDyamicModManager</returns>
    public static GlobalDyamicModManager GetAssembly(string name)
    {
        GlobalDyamicModManager rs = null;
        foreach (GlobalDyamicModManager r in LoadedDlls)
            if (r.assembly.GetName().Name == name)
            {
                rs = r;
                break;
            }
        return rs;
    }
    /// <summary>
    /// 退出前的清理
    /// </summary>
    public static void ExitClear()
    {
        if (GlobalMediator.GameExiting)
            LoadedDlls.Clear();
    }

    /// <summary>
    /// 从此dll里取出脚本附加到GameObject上。<br/>
    /// 脚本必须是 MonoBehaviou 的派生类或 UnityEngine.Component 的派生类<br/>
    /// 如果当前代码mod是lua mod，则会使用LUAScriptCom进行运行，compotentName属性则填写您要运行的lua文件名字。
    /// </summary>
    /// <param name="prefab">需要附加的物体。</param>
    /// <param name="compotentName">需要附加的脚本类名。如果当前代码mod是lua mod，则该属性是lua文件名字</param>
    /// <returns></returns>
    public UnityEngine.Component AddCompotent(GameObject prefab, string compotentName)
    {
        if (type == DyamicModType.MonoModule)
        {
            UnityEngine.Component com = prefab.GetComponent(compotentName);
            if (com == null)
            {
                if (assembly != null)
                {
                    Type type = assembly.GetType(compotentName);
                    prefab.AddComponent(type);
                    com = prefab.GetComponent(compotentName);
                }
            }
            return com;
        }
        else if (type == DyamicModType.LUAModule)
        {
            Type type = typeof(LUAScriptCom);
            if (prefab != null)
            {
                prefab.AddComponent(type);
                LUAScriptCom c = prefab.GetComponent<LUAScriptCom>();
                c.SetRunScript(luaState, compotentName);
                return c;
            }
        }
        return null;
    }
}

