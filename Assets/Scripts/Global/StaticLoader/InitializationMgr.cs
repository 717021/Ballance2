using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 元件初始化器
/// </summary>
public class InitializationMgr : MonoBehaviour
{
    /// <summary>
    /// 要加载的元件类型
    /// </summary>
    public GlobalGamePartType InitType = GlobalGamePartType.None;

    /// <summary>
    /// 加载器是否正在加载
    /// </summary>
    public static bool IsLoading
    {
        get; private set;
    }
    /// <summary>
    /// 加载器是否加载完成
    /// </summary>
    public static bool IsInitializationMgrInitializefinished()
    {
        return !IsLoading;
    }

    // Use this for initialization
    void Start()
    {
        IsLoading = true;
        if (InitType != GlobalGamePartType.GamePart && InitType != GlobalGamePartType.StaticPart)
            StartCoroutine(Loader());
    }
    IEnumerator Loader()
    {
        if (GlobalMediator.UIManager == null)
            //初加载 UI
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        GlobalMediator.SetUILoadFinished();

        //等待 StaticLoader 加载完成
        yield return new WaitUntil(StaticLoader.IsStaticLoaderLoadfinished);

        if (InitType == GlobalGamePartType.None)
            yield break;
        //加载游戏 部分 (mod GamePart)
        foreach (GlobalGamePart gp in GlobalModLoader.RegisteredGameParts)
        {
            if (gp.Type == GlobalGamePartType.GamePart)
                Load(gp);
        }
        //加载游戏 部分 (mod)
        foreach (GlobalGamePart gp in GlobalModLoader.RegisteredGameParts)
        {
            if (gp.Type == InitType)
                Load(gp);
        }

        IsLoading = false;
    }

    private void Load(GlobalGamePart gp)
    {
        //自动加载模型
        if (!string.IsNullOrEmpty(gp.AutoInitObject))
        {
            GlobalMediator.Log("[InitializationMgr] Initializing object " + gp.AutoInitObject);
            StringSpliter stringSpliter = new StringSpliter(gp.AutoInitObject, ';');
            if (stringSpliter.Success)
            {
                foreach (string ss in stringSpliter.Result)
                {
                    if (ss.Contains(">"))
                    {
                        string[] ssx = ss.Split('>');
                        GameObject perfab = gp.Pack.GetPerfab(ssx[0]);
                        if (perfab != null)
                        {
                            GameObject gameObject = Object.Instantiate(perfab);
                            gameObject.name = ssx[1];
                        }
                        else GlobalMediator.LogErr("[InitializationMgr] Auto Initialize object failure : " + ss + "  Not  found perfab.");
                    }
                    else
                    {
                        GameObject perfab = gp.Pack.GetPerfab(ss);
                        if (perfab != null) Object.Instantiate(perfab);
                        else GlobalMediator.LogErr("[InitializationMgr] Auto Initialize object failure : " + ss + "  Not  found perfab.");
                    }
                }
            }
            else
            {
                if (gp.AutoInitObject.Contains(">"))
                {
                    string[] ssx = gp.AutoInitObject.Split('>');
                    GameObject perfab = gp.Pack.GetPerfab(ssx[0]);
                    if (perfab != null)
                    {
                        GameObject gameObject = Object.Instantiate(perfab);
                        gameObject.name = ssx[1];
                    }
                    else GlobalMediator.LogErr("[InitializationMgr] Auto Initialize object failure : " + gp.AutoInitObject + "  Not  found perfab.");
                }
                else
                {
                    GameObject perfab = gp.Pack.GetPerfab(gp.AutoInitObject);
                    if (perfab != null) Object.Instantiate(perfab);
                    else GlobalMediator.LogErr("[InitializationMgr] Auto Initialize object failure : " + gp.AutoInitObject + "  Not  found perfab.");
                }
            }
        }
        //自动附加脚本
        if (!string.IsNullOrEmpty(gp.AutoAttachScript))
        {
            StringSpliter stringSpliter = new StringSpliter(gp.AutoAttachScript, ';');
            if (stringSpliter.Success)
            {
                foreach (string ss in stringSpliter.Result)
                {
                    if (ss.Contains(">"))
                    {
                        string[] ssx = ss.Split('>');
                        GameObject gameObject = GameObject.Find(ssx[1]);
                        if (gameObject != null)
                        {
                            string[] ssxe = ssx[0].Split(':');
                            if (ssxe.Length >= 2)
                            {
                                GlobalMediator.Log("[InitializationMgr] Attaching script " + ssxe[1] + "to object" + ssx[1]);
                                GlobalDyamicModManager d;
                                if (GlobalModLoader.IsCodeModLoaded(ssxe[0], out d))
                                    d.AddCompotent(gameObject, ssxe[1]);
                            }
                            else GlobalMediator.LogErr("[InitializationMgr] Attach script failure : " + ssx[0] + "  Error format.");
                        }
                        else GlobalMediator.LogErr("[InitializationMgr] Attach script failure : " + ssx[0] + "  Not found object :" + ssx[1]);
                    }
                    else GlobalMediator.LogErr("[InitializationMgr] Attach script failure : " + gp.AutoAttachScript + "  Error format.");
                }
            }
            else GlobalMediator.LogErr("[InitializationMgr] Attach script failure : " + gp.AutoAttachScript + "  Error format.");
        }
    }
}
