using Assets.Scripts.Global;
using Assets.Scripts.Worker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 关卡加载器
 */

/// <summary>
/// 关卡加载器
/// </summary>
public class LevelLoader : MonoBehaviour
{
    /// <summary>
    /// 设置加载任务
    /// </summary>
    /// <param name="levelname">关卡名字或路径</param>
    public static void SetLoaderShouldLoad(string levelname)
    {
        nextShouldLoad = true;
        nextShouldLoadLevel = levelname;
    }

    //是否需要加载
    private static bool nextShouldLoad = false;
    private static string nextShouldLoadLevel = "";

    /// <summary>
    /// 开始加载
    /// </summary>
    /// <param name="m"></param>
    public static void StartLoadLevel(MonoBehaviour m)
    {
        m.StartCoroutine(StartLoadLevelWait(m));
    }
    private static IEnumerator StartLoadLevelWait(MonoBehaviour m)
    {
        //Level Loader
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(3, UnityEngine.SceneManagement.LoadSceneMode.Single);
        //Level 主场景
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(4, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        //Ui
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 2f;
        GlobalMediator.UIManager.FadeOut();
    }

    //开始加载
    private void Start()
    {
        if (nextShouldLoad)
        {
            //开始加载
            StartCoroutine(LoadLevel());
        }
        else SetErrorAndStopLoad("", false, "", "没有指定加载路径");
    }

    public Text textCreater;
    public Text textErr;
    public Text textTip;
    public GameObject errPanel;
    public GameObject errInfo;
    public RectTransform progress;

    [System.NonSerialized]
    public LevelManager levelManager;
    [System.NonSerialized]
    public GameObject loadedLevelBaseObject;


    /// <summary>
    /// 强制返回主菜单
    /// </summary>
    public void BackToMenu()
    {
        StartCoroutine(BackToMenuW());
    }
    private IEnumerator BackToMenuW()
    {
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 2f;
        GlobalMediator.UIManager.FadeIn();
        yield return new WaitForSeconds(1f);
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
    }
    private void ButtonBackToMenu_OnClicked()
    {
        BackToMenu();
    }
    private bool IsLevelLoadFinished()
    {
        return levelManager != null;
    }

    //进度条长度设置
    private void UpdateProgress(float f)
    {
        progress.sizeDelta = new Vector2(Screen.width * f, 20f);
    }
    //显示错误并停止加载
    private void SetErrorAndStopLoad(string errstr, bool iscreater = true, string cr = "", string es = "")
    {
        GlobalMediator.LogErr("[LevelLoader] " + errstr);
        StopAllCoroutines();
        errPanel.SetActive(true);
        if (iscreater && !string.IsNullOrEmpty(cr))
            textCreater.text = "如果您有疑问，可以联系此地图的制作者 " + cr + " 了解解决方案";
        else textCreater.text = es;
        if (GlobalSettings.Debug)
        {
            textTip.text = "请输入指令\n/levelloader last err\n查看详细的错误信息和控制台输出";
            errInfo.SetActive(true);
            textErr.text = errstr;
        }
    }
    //获取关卡在磁盘中的绝对路径
    private string LoadLevelGetPath()
    {
        string path = "";
        if (!string.IsNullOrEmpty(nextShouldLoadLevel))
        {
            int i;
            if (int.TryParse(nextShouldLoadLevel, out i))
            {
                path = StoragePathManager.CoreLevelsFolder + "level" + i + ".ballance";
            }
            else
            {
                if (System.IO.File.Exists(nextShouldLoadLevel))
                    path = nextShouldLoadLevel;
                else path = StoragePathManager.LevelsFolder + nextShouldLoadLevel;
            }
        }
        else SetErrorAndStopLoad("Error Unspecified.", false, null, "[Loader] Error Unspecified.");
        return path;
    }
    //控制加载的主函数
    private IEnumerator LoadLevel()
    { 
        GlobalMediator.Log("[LevelLoader] Wait for InitializationMgr");
        //等待 初始化器 完成
        yield return new WaitUntil(InitializationMgr.IsInitializationMgrInitializefinished);
        GlobalMediator.Log("[LevelLoader] Wait for LevelManager");
        //等待 核心模块 完成
        yield return new WaitUntil(IsLevelLoadFinished);

        string path = LoadLevelGetPath();
        GlobalMediator.Log("[LevelLoader] Start load : " + path);
        //加载主 包（包含关卡文件）
        yield return StartCoroutine(GlobalModLoader.LoadPack(path, this));
        GlobalPack p;
        if (GlobalModLoader.IsPackLoaded(path, out p))
        {
            //10%
            UpdateProgress(0.1f);
            if (p.Type != GlobalPackType.Level)
                GlobalMediator.LogWarn("[LevelLoader] The pack : " + p.Name + " is not a GlobalPackType.Level pack.\nThis may cause some error.Please make sure that it 's Type is GlobalPackType.Level.");

            BFSReader b = p.DescribeFile;
            if (p.DescribeFile == null)
                SetErrorAndStopLoad("Error LoadPack.Describe File not found.\nPath : " + path, false, null, "包描述文件丢失");
            else
            {
                //解析 描述文件
                yield return StartCoroutine(LoadLevelDescribeFileBase(p, b));
                //15%
                UpdateProgress(0.15f);
                //加载关键元件
                yield return StartCoroutine(LoadLevelBase(p, b));
                //20%
                UpdateProgress(0.2f);

            }
        }
        else SetErrorAndStopLoad("Error LoadPack.(You can view the other information in command output)\nPath : " + path, false, null, "加载包失败");
        yield break;
    }
    private IEnumerator LoadLevelDescribeFileBase(GlobalPack p, BFSReader b)
    {
        string baseobj = b.GetPropertyValue("LevelObject");
        if (!string.IsNullOrEmpty(baseobj))
        {
            GlobalMediator.Log("[LevelLoader] Start load the LevelObject : " + baseobj);
            //
            GameObject perfab = p.GetPerfab(baseobj);
            if (perfab == null)
                SetErrorAndStopLoad("Error LoadPack.\nLevelObject " + baseobj + " not find.\nPath : " + p.Name, false, null, "加载包失败");
            loadedLevelBaseObject = Object.Instantiate(perfab);
            //
            GlobalMediator.Log("[LevelLoader] LevelObject : " + baseobj + " Instantiateed.");
        }
        else SetErrorAndStopLoad("Error LoadPack.\nLevelObject not defined.\nPath : " + p.Name, true, p.AuthorName, "加载包失败");

        yield break;
    }
    private IEnumerator LoadLevelBase(GlobalPack p, BFSReader b)
    {
        GlobalMediator.Log("[LevelLoader] Start load the LevelBase objects. ");
        //PS_LevelStart
        string baseobj = b.GetPropertyValue("PS_LevelStart");
        if (!string.IsNullOrEmpty(baseobj))
        {
            GameObject obj = GameObject.Find(baseobj);
            if(obj==null)
                SetErrorAndStopLoad("Error LoadLevel. PS_LevelStart not find.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
            levelManager.psLevelStart = obj;
        }
        else SetErrorAndStopLoad("Error LoadLevel. PS_LevelStart not defined.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
        //PE_LevelEnd
        baseobj = b.GetPropertyValue("PE_LevelEnd");
        if (!string.IsNullOrEmpty(baseobj))
        {
            GameObject obj = GameObject.Find(baseobj);
            if (obj == null)
                SetErrorAndStopLoad("Error LoadLevel. PE_LevelEnd not find.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
            levelManager.peLevelEnd = obj;
        }
        else SetErrorAndStopLoad("Error LoadLevel. PE_LevelEnd not defined.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
        //PR_ResetPoints
        baseobj = b.GetPropertyValue("PR_ResetPoints");
        if (!string.IsNullOrEmpty(baseobj))
        {
            string[] strs = b.GetPropertyValueChildValue(baseobj);
            for (int i = 0; i < strs.Length; i++)
            {
                GameObject obj = GameObject.Find(strs[i]);
                if (obj == null)
                    SetErrorAndStopLoad("Error LoadLevel. PR_ResetPoints [" + i + "] " + strs[i] + " not find.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
                levelManager.prResetPoints.Add(obj);
            }
            if (levelManager.prResetPoints.Count > 0)
                levelManager.prResetPoints0 = levelManager.prResetPoints[0];
            else SetErrorAndStopLoad("Error LoadLevel. There is none of ResetPoints were defined.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
        }
        else SetErrorAndStopLoad("Error LoadLevel. PR_ResetPoints not defined.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
        //PC_CheckPoints
        baseobj = b.GetPropertyValue("PC_CheckPoints");
        if (!string.IsNullOrEmpty(baseobj))
        {
            string[] strs = b.GetPropertyValueChildValue(baseobj);
            for (int i = 0; i < strs.Length; i++)
            {
                GameObject obj = GameObject.Find(strs[i]);
                if (obj == null)
                    SetErrorAndStopLoad("Error LoadLevel. PC_CheckPoints [" + i + "] " + strs[i] + " not find.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
                levelManager.pcCheckPoints.Add(obj);
            }
        }



        yield break;
    }
}
