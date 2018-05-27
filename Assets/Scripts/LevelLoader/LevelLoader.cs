using Assets.Scripts.GameCore;
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
/// 关卡错误处理模式。
/// </summary>
public enum LevelErrorMode
{
    /// <summary>
    /// 不指定
    /// </summary>
    None,
    /// <summary>
    /// 不显示错误，任然加载
    /// </summary>
    NoErrorShow,
    /// <summary>
    /// 警告错误
    /// </summary>
    Warning,
    /// <summary>
    /// 警告错误并询问玩家是否继续加载
    /// </summary>
    WarnAndArsk,
    /// <summary>
    /// 出现错误马上停止加载
    /// </summary>
    StopLoad
}

/// <summary>
/// 关卡加载器
/// </summary>
public class LevelLoader : MonoBehaviour
{
    public LevelLoader()
    {
        GlobalMediator.SetSystemServices(GameServices.LevelLoader, this);
        ModulCreater m1;
        if (!IsModulCreaterRegistered("ModulCreaterDef", out m1))
            RegisterModulCreater(modulCreaterDef);
    }

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

    #region Loader UI

    public Text textCreater;
    public Text textErr;
    public Text textTip;
    public GameObject errPanel;
    public GameObject errInfo;
    public RectTransform progress;

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



    #endregion

    #region ModulInitnazier

    /// <summary>
    /// 机关初始化器。
    /// 你可以继承此类用于初始化自己的机关。
    /// </summary>
    public class ModulCreater
    {
        /// <summary>
        /// 初始化机关，由LevelLoader调用。
        /// 请继承重写此函数用于对自己的机关初始化。
        /// </summary>
        /// <param name="name">机关实例名字（在文件中定义的）</param>
        /// <param name="orginalObject">机关实例</param>
        /// <param name="gm">注册的机关实例</param>
        /// <returns></returns>
        public virtual bool InitModul(LevelLoader loader, string name, GameObject orginalObject, GlobalGameModul gm, out GameObject realModulObject)
        {
            realModulObject = null;
            return false;
        }

        /// <summary>
        /// 机关初始化器名字。
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 默认机关初始化器。
    /// </summary>
    private class ModulCreaterDef : ModulCreater
    {
        public ModulCreaterDef()
        {
            Name = "ModulCreaterDef";
        }

        public override bool InitModul(LevelLoader loader, string name, GameObject orginalObject, GlobalGameModul gm, out GameObject realModulObject)
        {
            if (gm != null && orginalObject != null)
            {
                if(gm.BasePerfab!=null)
                {
                    //初始化新机关实例
                    GameObject ro = Instantiate(gm.BasePerfab);
                    Modul m = ro.GetComponent<Modul>();
                    if (m != null)
                    {
                        m.ModulType = gm;
                        ro.name = gm.Name + "_" + name;
                        //同步位置和旋转
                        ro.transform.position = orginalObject.transform.position;
                        ro.transform.rotation = orginalObject.transform.rotation;
                        //加载事件
                        m.OnLoad();
                        //根据设置设置起始参数
                        if (gm.ICBackupType == ICBackType.BackupThisObject)
                            loader.icManager.BackupObjectIC(ro);
                        else if (gm.ICBackupType == ICBackType.BackupThisAndChild)
                            loader.icManager.BackupObjectAndChildIC(ro);
                        else if (gm.ICBackupType == ICBackType.Custom)
                        {

                            string[] objsforics = gm.ICBackupCustom.Split(';');
                            foreach(string objsforic in objsforics)
                            {
                                Transform tr = ro.transform.Find(objsforic);
                                if (tr != null)
                                {
                                    GameObject o = tr.gameObject;
                                    loader.icManager.BackupObjectIC(o);
                                }
                            }
                        }
                        if (gm.ICResetType == ICResetType.SendResetMsg)
                            m.OnBackupIC();
                        if (gm.StartActive || gm.ActiveType == GlobalGameModulActiveType.AlwaysActive)
                            ro.SetActive(true);
                        else ro.SetActive(false);
                        realModulObject = ro;
                        return true;
                    }
                }
            }

            realModulObject = null;
            return false;
        }
    }
    private static ModulCreaterDef modulCreaterDef = new ModulCreaterDef();
    private static List<ModulCreater> modulCreaters = new List<ModulCreater>();

    /// <summary>
    /// 注册机关初始化器。
    /// </summary>
    /// <param name="m">机关初始化器</param>
    /// <returns></returns>
    public bool RegisterModulCreater(ModulCreater m)
    {
        ModulCreater m1;
        if (!IsModulCreaterRegistered(m.Name, out m1))
        {
            modulCreaters.Add(m);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 查看机关初始化器是否注册。
    /// </summary>
    /// <param name="name">机关初始化器名字</param>
    /// <param name="m">机关初始化器</param>
    /// <returns></returns>
    public bool IsModulCreaterRegistered(string name, out ModulCreater m)
    {
        bool rs = false;
        foreach(ModulCreater mc in modulCreaters)
        {
            if (mc.Name == name)
            {
                m = mc;
                rs = true;
                break;
            }
        }
        m = null;
        return rs; 
    }
    /// <summary>
    /// 取消注册机关初始化器。
    /// </summary>
    /// <param name="m">机关初始化器</param>
    /// <returns></returns>
    public bool UnRegisterModulCreater(ModulCreater m)
    {
        return modulCreaters.Remove(m); ;
    }


    #endregion

    #region 一些物体

    [System.NonSerialized]
    public LevelManager levelManager;
    [System.NonSerialized]
    public SectorManager sectorManager;
    [System.NonSerialized]
    public ICManager icManager;
    [System.NonSerialized]
    public GameObject loadedLevelBaseObject;

    private bool IsLevelLoadFinished()
    {
        return levelManager != null;
    }

    #endregion

    private LevelErrorMode levelErrorMode = LevelErrorMode.Warning;
    private bool lastWarn = false;
    private int warnCount = 0;

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
    //加载 主控函数
    private IEnumerator LoadLevel()
    { 
        GlobalMediator.Log("[LevelLoader] Wait for InitializationMgr");
        //等待 初始化器 完成
        yield return new WaitUntil(InitializationMgr.IsInitializationMgrInitializefinished);
        GlobalMediator.Log("[LevelLoader] Wait for LevelManager");
        //等待 核心模块 完成
        yield return new WaitUntil(IsLevelLoadFinished);

        sectorManager = GlobalMediator.GetSystemServices(GameServices.SectorManager) as SectorManager;
        icManager = GlobalMediator.GetSystemServices(GameServices.ICManager) as ICManager;

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
                string[] props = new string[b.Props.Keys.Count];
                b.Props.Keys.CopyTo(props, 0);

                //20% 解析 描述文件
                yield return StartCoroutine(LoadLevelDescribeFileBase(p, b));
                UpdateProgress(0.2f);
                //30% 加载关键元件
                yield return StartCoroutine(LoadLevelBase(p, b));
                UpdateProgress(04f);
                //50% 预加载机关
                yield return StartCoroutine(LoadLevelModulPrepare(props, p, b));
                UpdateProgress(0.5f);
                //60% 加载机关 
                yield return StartCoroutine(LoadLevelModul(props, p, b));
                UpdateProgress(0.6f);
                //70% 加载游戏主体
                yield return StartCoroutine(LoadLevelGameKernel());
                UpdateProgress(0.7f);



                props = null;
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
        List<GameObject> pcCheckPoints = new List<GameObject>();
        List<GameObject> prResetPoints = new List<GameObject>();
        
        GlobalMediator.Log("[LevelLoader] Start load the LevelBase objects. ");
        //==================
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
        //==================
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
        //==================
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
                
                prResetPoints.Add(obj);
            }
            if (prResetPoints.Count > 0)
                levelManager.prResetPoints0 = prResetPoints[0];
            else SetErrorAndStopLoad("Error LoadLevel. There is none of ResetPoints were defined.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
        }
        else SetErrorAndStopLoad("Error LoadLevel. PR_ResetPoints not defined.\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
        //==================
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
                pcCheckPoints.Add(obj);
            }
        }

        GlobalMediator.Log("[LevelLoader] Pre load Sectors. ");
        //==================
        //Pre load Sectors

        int expectSectorCount = prResetPoints.Count, ix = 1;
        if (expectSectorCount != pcCheckPoints.Count + 1)
            GlobalMediator.LogWarn("[LevelLoader] ResetPoint 个数和 CheckPoint 个数不匹配，这可能会造成错误的游戏行为。");

        for (; ix <= expectSectorCount; ix++)
        {
            baseobj = b.GetPropertyValue("Sector_" + ix.ToString("00"));
            if (baseobj == null) baseobj = b.GetPropertyValue("Sector_" + ix.ToString());
            if (baseobj != null)
            {
                string[] objs = b.GetPropertyValueChildValue(baseobj);
                sectorManager.AddSector(prResetPoints[ix], ix == 1 ? null : pcCheckPoints[ix - 1], objs);
                if (ix > SectorManager.MAX_SECTOR)
                {
                    SetErrorAndStopLoad("The sector " + expectSectorCount + " is greater than MAX_SECTOR (" + SectorManager.MAX_SECTOR + ").\nPath : " + p.Name, true, p.AuthorName, "加载关卡失败");
                    pcCheckPoints.Clear();
                    prResetPoints.Clear();
                    pcCheckPoints = null;
                    prResetPoints = null;
                }
            }
            else GlobalMediator.LogWarn("[LevelLoader] Sector_" + ix.ToString() + " not defined.");
        }
        
        pcCheckPoints.Clear();
        prResetPoints.Clear();
        pcCheckPoints = null;
        prResetPoints = null;
        yield break;
    }
    private IEnumerator LoadLevelModulPrepare(string[] props, GlobalPack p, BFSReader b)
    {
        foreach(string s in props)
        {
            if(s.StartsWith("P_"))
            {
                GlobalGameModul gm;
                if (!GlobalModLoader.IsGameModulRegistered(s, out gm))
                {
                    if (!GlobalModLoader.TryRegisterGameModul(s))
                        GlobalMediator.LogWarn("[LevelLoader] Failed register modul : " + 
                            s + " .The moduls of this type can not be loaded.");
                }
            }
        }   
        yield break;
    }
    private IEnumerator LoadLevelModul(string[] props, GlobalPack p, BFSReader b)
    {
        foreach (string s in props)
        {
            if (s.StartsWith("P_"))
            {
                GlobalGameModul gm;
                if (GlobalModLoader.IsGameModulRegistered(s, out gm))
                {
                    //加载自定义初始化器
                    ModulCreater modulCreater = null;
                    if (string.IsNullOrEmpty(gm.ModulCreater))
                        modulCreater = modulCreaterDef;
                    else
                    {
                        if (!IsModulCreaterRegistered(gm.ModulCreater, out modulCreater))
                        {
                            modulCreater = modulCreaterDef;
                            GlobalMediator.LogWarn("[LevelLoader] The GlobalGameModul : " +
                                gm.Name +
                                " was failed in load ModulCreater" + gm.ModulCreater + ".Use ModulCreaterDef");
                        }
                    }
                    //加载
                    string[] objs = b.GetPropertyValueChildValue(s);
                    for (int i = 0; i < objs.Length; i++)
                    {
                        GameObject go = GameObject.Find(objs[i]);
                        GameObject real;
                        if (!modulCreater.InitModul(this, objs[i], go, gm, out real))
                            GlobalMediator.LogWarn("[LevelLoader] The Modul : " + objs[i] + " (" +
                               gm.Name + ") load failed!");
                        else
                        {
                            if (real != null)
                            {
                                int isc = sectorManager.ObjInSector(objs[i]);
                                if (isc != -1)
                                    if (!sectorManager.AddObjToSector(real, isc))
                                        GlobalMediator.LogWarn("[LevelLoader] The Failed to add Modul " + objs[i] + " to sector " + isc + " .");
                            }
                        }
                    }
                }
            }
        }
        yield break;
    }
    private IEnumerator LoadLevelGameKernel()
    {
        yield return levelManager.StartCoroutine(levelManager.LoadGameCore());

        if(!GlobalMediator.AnimTranfoMgrLoaded)
            SetErrorAndStopLoad("AnimTranfoMgr 未加载。", false, null, "加载游戏内核失败");
        if (!GlobalMediator.BallsManagerLoaded)
            SetErrorAndStopLoad("BallsManager 未加载。", false, null, "加载游戏内核失败");
    }

}
