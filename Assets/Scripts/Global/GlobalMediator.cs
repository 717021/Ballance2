﻿using Assets.Scripts.GameCore;
using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 代码说明：游戏核心模块，游戏主管理模块、中介者模式（Mediator）执行器。
 * 不应该更改。
*/

/// <summary>
/// 中介者。游戏主管理模块
/// </summary>
public static class GlobalMediator
{
    private static bool exitingStarted = false;
    private static bool isExiting = false;
    private static bool isUILoadFinished = false;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void GlobalMediatorInitialization()
    {
        Assets.Scripts.Worker.StoragePathManager.StoragePathInit();
        CommandManager.RegisterCommand("exit", ExitCommandReceiverHandler, "退出游戏");
        CommandManager.RegisterCommand("nodebug", NoDebugCommandReceiverHandler, "退出调试模式");
    }
    public static void SetCommandManager(CommandManager commandManager)
    {
        if (commandManager != null && CommandManager == null)
            CommandManager = commandManager;
    }
    public static void SetCommandManagerUnavailable(CommandManager commandManager)
    {
        if (commandManager != null && CommandManager == commandManager)
            CommandManager = null;
    }
    public static void SetUIManager(UIManager uIManager)
    {
        if (uIManager != null && UIManager == null)
            UIManager = uIManager;
    }
    public static void SetUIManagerUnavailable(UIManager uIManager)
    {
        if (uIManager != null && UIManager == uIManager)
        {
            UIManager = null;
            isUILoadFinished = false;
        }
    }
    public static void SetUILoadFinished()
    {
        isUILoadFinished = true;
    }

    private static bool ExitCommandReceiverHandler(string[] pararms)
    {
        ExitGame();
        return true;
    }
    private static bool NoDebugCommandReceiverHandler(string[] pararms)
    {
        GlobalSettings.Debug = false;
        if (CommandManager != null) CommandManager.NoDebug();
        return true;
    }

    #region Logs

    /// <summary>
    /// 输出信息到控制台
    /// </summary>
    /// <param name="f">发送者名字</param>
    /// <param name="s">信息</param>
    public static void Log(string f, string s)
    {
        if (CommandManager != null)
            CommandManager.Log("[" + f + "] " + s);
        else Debug.Log("[" + f + "] " + s);
    }
    /// <summary>
    /// 输出错误信息到控制台
    /// </summary>
    /// <param name="f">发送者名字</param>
    /// <param name="s">信息</param>
    public static void LogErr(string f, string s)
    {
        if (CommandManager != null)
            CommandManager.LogErr("[" + f + "] " + s);
        else Debug.LogError("[" + f + "] " + s);
    }
    /// <summary>
    /// 输出警告信息到控制台
    /// </summary>
    /// <param name="f">发送者名字</param>
    /// <param name="s">信息</param>
    public static void LogWarn(string f, string s)
    {
        if (CommandManager != null)
            CommandManager.LogWarn("[" + f + "] " + s);
        else Debug.LogWarning("[" + f + "] " + s);
    }
    /// <summary>
    /// 输出信息到控制台
    /// </summary>
    /// <param name="f">发送者名字</param>
    /// <param name="s">信息</param>
    public static void LogInfo(string f, string s)
    {
        if (CommandManager != null)
            CommandManager.LogInfo("[" + f + "] " + s);
        else Debug.Log("["+f+"] "+s);
    }

    /// <summary>
    /// 输出信息到控制台
    /// </summary>
    /// <param name="s">信息</param>
    public static void Log(string s)
    {
        if (CommandManager != null)
            CommandManager.Log(s);
        else Debug.Log(s);
    }
    /// <summary>
    /// 输出错误信息到控制台
    /// </summary>
    /// <param name="s">信息</param>
    public static void LogErr(string s)
    {
        if (CommandManager != null)
            CommandManager.LogErr(s);
        else Debug.LogError(s);
    }
    /// <summary>
    /// 输出警告信息到控制台
    /// </summary>
    /// <param name="s">信息</param>
    public static void LogWarn(string s)
    {
        if (CommandManager != null)
            CommandManager.LogWarn(s);
        else Debug.LogWarning(s);
    }
    /// <summary>
    /// 输出信息到控制台
    /// </summary>
    /// <param name="s">信息</param>
    public static void LogInfo(string s)
    {
        if (CommandManager != null)
            CommandManager.LogInfo(s);
        else Debug.Log(s);
    }
    #endregion

    #region Action

    /// <summary>
    /// 注册 Action。
    /// </summary>
    /// <param name="act">需要注册的 Action 。</param>
    /// <param name="name">Action 名字</param>
    /// <returns>返回一个Action处理器，可以用来接受Action。</returns>
    public static ActionHandler RegisterAction(Action act, string name = "")
    {
        if (act != null)
        {
            ActionHandler h = act.GetStaticHandler();
            if (h == null)
            {
                h = new ActionHandler(name);
                act.Register(h);
                return h;
            }
        }
        return null;
    }

    /// <summary>
    /// 调用 Action。
    /// </summary>
    /// <param name="act">需要调用的Action。</param>
    /// <returns>返回调用是否成功。</returns>
    public static bool CallAction(Action act)
    {
        if (act != null)
        {
            ActionHandler h = act.GetStaticHandler();
            if (h != null)
            {
                Dictionary<string, ActionHandler.ActionHandlerDelegate>.ValueCollection valueCol = h.handlers.Values;
                foreach (ActionHandler.ActionHandlerDelegate d in valueCol)
                    if (d != null) d.Invoke(act.pararms);
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 调用Action到指定接收者。
    /// </summary>
    /// <param name="act">需要调用的Action。</param>
    /// <param name="target">Action的接收者。</param>
    /// <returns>Action的接收者返回数据。</returns>
    public static object CallAction(Action act, string target)
    {
        if (act != null)
        {
            ActionHandler h = act.GetStaticHandler();
            if (h != null)
            {
                if (h.handlers.ContainsKey(target)) {
                    ActionHandler.ActionHandlerDelegate d = h.handlers[target];
                    if (d == null) return h.handlers.Remove(target);
                    else d.Invoke(act.pararms);
                }
            }
        }
        return null;
    }

    #endregion

    #region Event

    //事件侦听器储存Dictionary
    private static Dictionary<string, List<EventLinster>> events = new Dictionary<string, List<EventLinster>>();

    /// <summary>
    /// 注册事件侦听器 注意：事件使用完毕（比如object已经释放，请使用UnRegisterEventLinster取消注册事件侦听器，否则会发生错误）
    /// </summary>
    /// <param name="eventLinster">要注册的事件侦听器</param>
    /// <returns>返回要注册的事件侦听器</returns>
    public static EventLinster RegisterEventLinster(EventLinster eventLinster)
    {
        string evtName = eventLinster.eventName;
        if (evtName == "" || eventLinster == null) return null;
        List<EventLinster> list = null;
        if (!events.ContainsKey(evtName))
        {
            list = new List<EventLinster>();
            events.Add(evtName, list);
        }
        else list = events[evtName];
        if (!list.Contains(eventLinster))
            list.Add(eventLinster);
        return eventLinster;
    }
    /// <summary>
    /// 取消注册事件侦听器
    /// </summary>
    /// <param name="eventLinster">要取消注册的事件侦听器</param>
    /// <returns>返回是否成功</returns>
    public static bool UnRegisterEventLinster(EventLinster eventLinster)
    {
        string evtName = eventLinster.eventName;
        if (evtName == "" || eventLinster == null) return false;
        List<EventLinster> list = null;
        if (events.ContainsKey(evtName))
        {
            list = events[evtName];
            if (list.Contains(eventLinster))
            {
                list.Remove(eventLinster);
                return false;
            }
        }
        return false;
    }
    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="evtName">事件名称</param>
    /// <param name="sender">发送者</param>
    /// <param name="par">附加参数</param>
    /// <returns>返回是否成功</returns>
    public static bool DispatchEvent(string evtName, object sender, params object[] par)
    {
        if (string.IsNullOrEmpty(evtName)) return false;
        List<EventLinster> list = null;
        if (events.ContainsKey(evtName))
        {
            list = events[evtName];
            foreach (EventLinster l in list)
            {
                l.OnEvent(sender, par);
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 发送事件给指定接收者
    /// </summary>
    /// <param name="evtName">事件名称</param>
    /// <param name="targetName">指定接收者</param>
    /// <param name="sender">发送者</param>
    /// <param name="par">附加参数</param>
    /// <returns>返回是否成功</returns>
    public static bool DispatchEventToTarget(string evtName, string targetName, object sender, params object[] par)
    {
        if (string.IsNullOrEmpty(evtName)) return false;
        if (string.IsNullOrEmpty(targetName)) return DispatchEvent(evtName, sender, par);
        List<EventLinster> list = null;
        if (events.ContainsKey(evtName))
        {
            list = events[evtName];
            foreach (EventLinster l in list)
            {
                if (l.receiverName == targetName)
                    l.OnEvent(sender, par);
            }
            return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// 获取 指令管理器
    /// </summary>
    public static CommandManager CommandManager
    {
        get; private set;
    }
    /// <summary>
    /// 获取 我的界面管理器
    /// </summary>
    public static UIManager UIManager
    {
        get; private set;
    }
    /// <summary>
    /// 获取 UI 是否加载完成
    /// </summary>
    public static bool IsUILoadFinished()
    {
        return isUILoadFinished;
    }

    #region GameServices

    /// <summary>
    /// 获取游戏是否正在退出
    /// </summary>
    public static bool GameExiting { get { return isExiting; } }


    /// <summary>
    /// 退出游戏之前的清理。【不可调用】
    /// </summary>
    public static void ExitGameClear()
    {
        if (GameExiting)
        {
            foreach (List<EventLinster> l in events.Values)
                l.Clear();
            events.Clear();
        }
    }
    /// <summary>
    /// 退出游戏。
    /// </summary>
    public static void ExitGame()
    {
        if (!exitingStarted)
        {
            exitingStarted = true;
            isExiting = true;
            Time.timeScale = 0;

            Application.runInBackground = true;

            GlobalMediator.DispatchEvent("GameExiting", null);

            CommandManager.ExitGameClear();

            GlobalModLoader.ExitGameClear();
            GlobalDyamicModManager.ExitClear();
            GlobalMediator.ExitGameClear();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }

    public static bool AnimTranfoMgrLoaded { get { return animTranfoMgr == null; } }
    public static bool BallsManagerLoaded { get { return ballsManager == null; } }

    private static LevelManager levelManager;
    private static LevelLoader levelLoader;
    private static BallsManager ballsManager;
    private static AnimTranfoMgr animTranfoMgr;
    private static ICManager iCManager;
    private static SectorManager sectorManager;

    /// <summary>
    /// 【你不能调用这个方法】
    /// </summary>
    /// <param name="g"></param>
    /// <param name="o"></param>
    public static void SetSystemServices(GameServices g, object o)
    {
        switch (g)
        {
            case GameServices.LevelLoader:
                if (o is LevelLoader)
                    levelLoader = o as LevelLoader;
                break;
            case GameServices.LevelManager:
                if (o is LevelManager)
                    levelManager = o as LevelManager;
                break;
            case GameServices.SectorManager:
                if (o is SectorManager)
                    sectorManager = o as SectorManager;
                break;
            case GameServices.ICManager:
                if (o is ICManager)
                    iCManager = o as ICManager;
                break;
            case GameServices.BallsManager:
                if (o is BallsManager)
                    ballsManager = o as BallsManager;
                break;
            case GameServices.AnimTranfo:
                if (o is AnimTranfoMgr)
                    animTranfoMgr = o as AnimTranfoMgr;
                break;
        }
    }
    /// <summary>
    /// 获取游戏核心部件
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    public static object GetSystemServices(GameServices g)
    {
        switch (g)
        {
            case GameServices.LevelLoader:
                return levelLoader;
            case GameServices.LevelManager:
                return levelManager;
            case GameServices.SectorManager:
                return sectorManager;
            case GameServices.ICManager:
                return iCManager;
            case GameServices.BallsManager:
                return ballsManager;
            case GameServices.AnimTranfo:
                return animTranfoMgr;
        }
        return null;
    }

    #endregion
}

/// <summary>
/// 游戏 核心模块 枚举
/// </summary>
public enum GameServices
{
    LevelLoader,
    LevelManager,
    SectorManager,
    ICManager,
    BallsManager,
    AnimTranfo,
}

