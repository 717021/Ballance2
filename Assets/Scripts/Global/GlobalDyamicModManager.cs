using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

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
/// 动态dll加载器，扩展Mod的必要部分
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
    public Assembly assembly{ get; private set; }
    /// <summary>
    /// 获取加载是否成功
    /// </summary>
    public bool succeed { get { return suc; } }
    /// <summary>
    /// 获取 DLL 路径
    /// </summary>
    public string path { get { return pat; } }

    private string pat = "";
    private bool suc = false;

    /// <summary>
    /// 文件加载dll
    /// </summary>
    /// <param name="path">dll路径</param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName）</param>
    public GlobalDyamicModManager(string path, string initcode = "")
    {
        pat = path;
        assembly = Assembly.LoadFile(path);
        CallDllInit(initcode);
    }
    /// <summary>
    /// 在AssetBundle里加载dll
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="name">dll在资源中的名字</param>
    /// <param name="m">MonoBehaviour</param>
    /// <param name="initcode">初始化调DLL中用的函数（initcode格式：className:methodName）</param>
    public GlobalDyamicModManager(string path, string name, MonoBehaviour m, string initcode = "")
    {
        m.StartCoroutine(LoadDll(path, name));
    }
    private IEnumerator LoadDll(string path, string name, string initcode = "")
    {
        WWW www = new WWW(path);
        yield return www;
        pat = path;
        AssetBundle bundle = www.assetBundle;
        TextAsset asset = bundle.LoadAsset(name, typeof(TextAsset)) as TextAsset;
        assembly = Assembly.Load(asset.bytes);
        suc = assembly == null;
        CallDllInit(initcode);
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
    /// 文件加载dll
    /// </summary>
    /// <param name="path">dll路径</param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName 必须是静态类，静态方法）</param>
    public static void LoadAssembly(string path, string initcode = "")
    {
        LoadedDlls.Add(new GlobalDyamicModManager(path));
    }
    /// <summary>
    /// 在AssetBundle里加载dll
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="name">dll在资源中的名字</param>
    /// <param name="m">MonoBehaviour</param>
    /// <param name="initcode">初始化调用DLL中的函数（initcode格式：className:methodName 必须是静态类，静态方法）</param>
    public static void LoadAssemblyInAssetBundle(string path, string name, MonoBehaviour m, string initcode = "")
    {
        LoadedDlls.Add(new GlobalDyamicModManager(path, name,m));
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
            if(r.assembly.GetName().Name == name)
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
    /// 不要用
    /// </summary>
    /// <param name="assembly"></param>
    public void InitAssembly(System.Reflection.Assembly assembly)
    {
        this.assembly = assembly;
        LoadedDlls.Add(this);
    }

    /// <summary>
    /// 从此dll里取出脚本附加到GameObject上。脚本必须是MonoBehaviour的派生类。
    /// </summary>
    /// <param name="prefab">需要附加的物体。</param>
    /// <param name="compotentName">需要附加的脚本类名。</param>
    /// <returns></returns>
    public UnityEngine.Component AddCompotent(GameObject prefab, string compotentName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (assembly != null)
            {
                Type type = assembly.GetType(compotentName);
                if (prefab != null)
                {
                    prefab.AddComponent(type);
                }
                return null;
            }
        }
        else
        {
            UnityEngine.Component com = prefab.GetComponent(compotentName);
            if (com == null)
            {
                if (assembly != null)
                {
                    Type type = assembly.GetType(compotentName);
                    prefab.AddComponent(type);
                }
            }
            return com;
        }
        return null;
    }
}

