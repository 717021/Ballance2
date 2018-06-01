using Assets.Scripts.Global;
using Assets.Scripts.Global.GlobalUI;
using Assets.Scripts.Global.GlobalUI.UIElements;
using Assets.Scripts.Hlper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 游戏 进入时的界面 （第一个看到的界面） 的控制脚本
 */

public class MenuLevel : MonoBehaviour
{
    private Material skyboxMaterialDay;
    private Material skyboxMaterialLz;
    private GameObject LZSuDu;
    private GameObject LZLiLiang;
    private GameObject LZNengLi;
    private Skybox SkyboxMain;
    private Light SkyLightA;
    private Light SkyLightB;
    private bool isInLZ = false;
    private EventLinster onDialolgClosedLinister;
    private Camera MainCamera;

    private UIToggle toggleFunnScreen;
    private Dropdown dropdownScreen;
    private Dropdown dropdownQuality;
    private Slider sliderMusicVol;
    private Slider sliderSoundVol;
    private int clkCount = 0;
    private int ssx = 0;

    void Start()
    {
        Application.runInBackground = false;

        onDialolgClosedLinister = GlobalMediator.RegisterEventLinster(new OnDialolgClosedLinister(UIDialogOK));

        SkyboxMain = gameObject.transform.Find("I_Camera").gameObject.GetComponent<Skybox>();
        MainCamera = gameObject.transform.Find("I_Camera").gameObject.GetComponent<Camera>();
        if (SkyboxMain != null)
        {
            skyboxMaterialDay = SkyMaker.MakeSkyBox("C");
            skyboxMaterialLz = SkyMaker.MakeSkyBox("F");
            SkyboxMain.material = skyboxMaterialDay;
        }

        GlobalMediator.CallAction(GameActions.PlaySound("sounds_*TYPE*", "assets/audios/menu_atmo.wav", 3));

        SkyLightA = gameObject.transform.Find("I_Light_A").gameObject.GetComponent<Light>();
        SkyLightB = gameObject.transform.Find("I_Light_B").gameObject.GetComponent<Light>();
        GameObject objs = gameObject.transform.Find("I_Objects").gameObject;
        LZSuDu = objs.transform.Find("I_Zone_SuDu_Innen").gameObject;
        LZLiLiang = objs.transform.Find("I_Zone_LiLiang_Innen").gameObject;
        LZNengLi = objs.transform.Find("I_Zone_NenLi_Innen").gameObject;
        LZSuDu.SetActive(false);
        LZLiLiang.SetActive(false);
        LZNengLi.SetActive(false);
        InitMenus();
    }
    void Update()
    {
        if (isInLZ)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitLZ();
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject == LZSuDu)
                        {
                            StartLoadLevel("13");
                        }
                        else if (hit.collider.gameObject == LZLiLiang)
                        {
                            StartLoadLevel("14");
                        }
                        else if (hit.collider.gameObject == LZNengLi)
                        {
                            StartLoadLevel("15");
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject == LZSuDu)
                        {
                            StartLoadLevel("13");
                        }
                        else if (hit.collider.gameObject == LZLiLiang)
                        {
                            StartLoadLevel("14");
                        }
                        else if (hit.collider.gameObject == LZNengLi)
                        {
                            StartLoadLevel("15");
                        }
                    }
                }
            }

            LZSuDu.transform.transform.eulerAngles = new Vector3(LZSuDu.transform.transform.eulerAngles.x, MainCamera.transform.eulerAngles.y - 180, LZSuDu.transform.transform.eulerAngles.z);
            LZLiLiang.transform.transform.eulerAngles = new Vector3(LZLiLiang.transform.transform.eulerAngles.x, MainCamera.transform.eulerAngles.y - 180, LZLiLiang.transform.transform.eulerAngles.z);
            LZNengLi.transform.transform.eulerAngles = new Vector3(LZNengLi.transform.transform.eulerAngles.x, MainCamera.transform.eulerAngles.y - 180, LZNengLi.transform.transform.eulerAngles.z);
        }
        else
        {
            if (ssx < 60) ssx++;
            else
            {
                ssx = 0;
                if (clkCount > 0)
                    clkCount--;
            }
        }
    }
    private void OnDestroy()
    {
        GlobalMediator.UnRegisterEventLinster(onDialolgClosedLinister);
    }

    //菜单模板
    const string main_menu =
        "$BtnMainMenuDef#Start#10#开始" +
        "$BtnMainMenuDef#Score#10#分数" +
        "$BtnMainMenuDef#Set#10#设置" +
        "$BtnMainMenuDef#About#10#关于" +
        "$BtnMainMenuDef#Quit#10#退出";
    const string main_start_menu =
        "$TextTitle##0#请选择关卡开始" +
        "$BtnMainMenuDef#StartOrg#40#原版关卡" +
        "$BtnMainMenuDef##20#自定义关卡" +
        "$BtnMainMenuDef##20#选择关卡文件" +
        "$BtnMainMenuDef#StartModMgr#20#Mod 管理器" +
        "$BtnMainBackDef#Back#30#返回";
    const string main_start_org =
        "$BtnLevDef#StartOrg1#0#Level1" +
        "$BtnLevDef#StartOrg2#0#Level2" +
        "$BtnLevDef#StartOrg3#0#Level3" +
        "$BtnLevDef#StartOrg4#0#Level4" +
        "$BtnLevDef#StartOrg5#0#Level5" +
        "$BtnLevDef#StartOrg6#0#Level6" +
        "$BtnLevDef#StartOrg7#0#Level7" +
        "$BtnLevDef#StartOrg8#0#Level8" +
        "$BtnLevDef#StartOrg9#0#Level9" +
        "$BtnLevDef#StartOrg10#0#Level10" +
        "$BtnLevDef#StartOrg11#0#Level11" +
        "$BtnLevDef#StartOrg12#0#Level12" +
        "$BtnLevDef#StartOrgLZ#0#Light Zone" +
        "$BtnMainBackDef#Back#20#返回";
    const string main_settings_desplay_menu =
        "$TextTitle##0#显示设置" +
        "$DropDownDef#dropdownScreen#20#屏幕分辨率设置" +
        "$ToggleDef#toggleFunnScreen#10#全屏" +
        "$DropDownDef#dropdownQuality#20#画质设置" +
        "$TextDef##0#在游戏启动时(双击exe时) 按住“Alt”键也可以设置" +
        "$BtnMainBackDef#Back#20#返回";
    const string main_start_lz = 
       "$BtnMainBackDef#Back#350#返回";
    const string main_settings_volume_menu =
        "$TextTitle##0#音量设置" +
        "$SliderDef#sliderMusicVol#20#音乐音量" +
        "$SliderDef#sliderSoundVol#10#音效音量" +
        "$BtnMainBackDef#Back#20#返回";
    const string main_settings_menu =
        "$TextTitle##0#设置" +
        "$BtnMainMenuDef#SetDisplay#20#显示设置" +
        "$BtnMainMenuDef#SetControl#20#控制设置" +
        "$BtnMainMenuDef#SetSound#20#音乐设置" +
        "$BtnMainMenuDef#SetOther#20#其他设置" +
        "$BtnMainBackDef#Back#50#返回";
    const string main_about_menu =  
        "$TextDef##0#关于" +   
        "$BtnMainMenuDef##20#Ballance2" +   
        "$BtnMainMenuDef##20#Ballance Unity 重制版" +
        "$BtnMainMenuDef#BtnVer#20#音乐设置" +
        "$BtnMainMenuDef#BtnVerBulid#20#其他设置" +
        "$BtnMainBackDef#Back#50#返回";

    //创建菜单
    private void InitMenus()
    {
        #region PageMain
        GlobalMediator.UIManager.UIMaker.RegisterUIGroup("MainMenuUI");
        UIMenu menu_main = GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main", null);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_main, main_menu);

        ((UIButton)menu_main.GetEle("Start")).onClick += ButtonStart_OnClicked;
        ((UIButton)menu_main.GetEle("About")).onClick += ButtonAbout_OnClicked;
        ((UIButton)menu_main.GetEle("Set")).onClick += ButtonSet_OnClicked;
        ((UIButton)menu_main.GetEle("Quit")).onClick += ButtonQuit_OnClicked;

        UIMenu menu_start = GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageStart", null);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_start, main_start_menu);

        ((UIButton)menu_start.GetEle("StartOrg")).onClick += ButtonStartOrg_OnClicked;
        ((UIButton)menu_start.GetEle("StartModMgr")).onClick += ButtonStartModMgr_OnClicked;
        
        ((UIButton)menu_start.GetEle("Back")).onClick += ButtonBack_OnClicked;

        #endregion

        #region PageStart

        UIMenu menu_start_levs = GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageStart.Orginal", null);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_start_levs, main_start_org);

        ((UIButton)menu_start_levs.GetEle("StartOrg1")).onClick += ButtonStartOrg1_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg2")).onClick += ButtonStartOrg2_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg3")).onClick += ButtonStartOrg3_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg4")).onClick += ButtonStartOrg4_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg5")).onClick += ButtonStartOrg5_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg6")).onClick += ButtonStartOrg6_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg7")).onClick += ButtonStartOrg7_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg8")).onClick += ButtonStartOrg8_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg9")).onClick += ButtonStartOrg9_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg10")).onClick += ButtonStartOrg10_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg11")).onClick += ButtonStartOrg11_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrg12")).onClick += ButtonStartOrg12_OnClicked;
        ((UIButton)menu_start_levs.GetEle("StartOrgLZ")).onClick += ButtonStartOrgLZ_OnClicked;
        ((UIButton)menu_start_levs.GetEle("Back")).onClick += ButtonBack_OnClicked;

        UIMenu menu_start_lz = GlobalMediator.UIManager.UIMaker.RegisterMenuPageWithTemplate("MainMenuUI", "Main.PageStart.Orginal.LZ", GlobalMediator.UIManager.MenuPageCleanTemplate);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_start_lz, main_start_lz);
        ((UIButton)menu_start_lz.GetEle("Back")).onClick += ButtonBackLZ_OnClicked;

        #endregion

        #region PageSettings
        UIMenu menu_settings = GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageSettings", null);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_settings, main_settings_menu);

        ((UIButton)menu_settings.GetEle("SetDisplay")).onClick += ButtonSetDisplay_OnClicked;
        ((UIButton)menu_settings.GetEle("SetControl")).onClick += ButtonSetControl_OnClicked;
        ((UIButton)menu_settings.GetEle("SetSound")).onClick += ButtonSetSound_OnClicked;
        ((UIButton)menu_settings.GetEle("SetOther")).onClick += ButtonSetOther_OnClicked;
        ((UIButton)menu_settings.GetEle("Back")).onClick += ButtonBack_OnClicked;

        #endregion

        #region PageAbout
        UIMenu menu_about = GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageAbout", null);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_about, main_about_menu);
        ((UIButton)menu_about.GetEle("BtnVer")).Text = "V.1.0\n" + StaticValues.GameVersion;
        ((UIButton)menu_about.GetEle("BtnVer")).onClick += ButtonDebug_OnClicked;
        ((UIButton)menu_about.GetEle("BtnVerBulid")).Text = "V.1.0\n" + "Bulid" + StaticValues.GameBulidVersion;
        ((UIButton)menu_about.GetEle("Back")).onClick += ButtonBack_OnClicked;
        #endregion

        #region PageSettings.Display
        UIMenu menu_settings_desplay = GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageSettings.Display", null);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_settings_desplay, main_settings_desplay_menu);
#if UNITY_STANDALONE
        //屏幕分辨率设置按钮
        dropdownScreen = (menu_settings_desplay.GetEle("dropdownScreen") as UIDropDown).Dropdown;
        Resolution[] s = GlobalSettings.GetSupportScrrenResolutions();
        string ccc = GlobalSettings.GetCurrentScrrenResolution();
        int i = 0, i2 = -1;
        foreach (Resolution ss in s)
        {
            string sss = ss.width + "x" + ss.height + " (" + ss.refreshRate + "Hz)";
            dropdownScreen.options.Add(new Dropdown.OptionData(sss));
            if (sss == ccc)
                i2 = i;
            i++;
        }
        if (i2 != -1) dropdownScreen.value = i2;
        dropdownScreen.onValueChanged.AddListener(OnScreenDropDownValueChanged);
        toggleFunnScreen = (menu_settings_desplay.GetEle("toggleFunnScreen") as UIToggle);
        toggleFunnScreen.onCheckChanged += OnFullScreenValueChanged;
        toggleFunnScreen.isChecked = GlobalSettings.IsFullScreen;

#elif UNITY_ANDROID
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageSettings.Display", "屏幕分辨率 : "+ GlobalSettings.GetCurrentScrrenResolution());
#endif
        dropdownQuality = (menu_settings_desplay.GetEle("dropdownQuality") as UIDropDown).Dropdown;
        foreach (string ss in QualitySettings.names)
            dropdownQuality.options.Add(new Dropdown.OptionData(ss));
        dropdownQuality.value = GlobalSettings.GetCurrentQualitySettings();
        dropdownQuality.onValueChanged.AddListener(OnQualityDropDownValueChanged);
        ((UIButton)menu_settings_desplay.GetEle("Back")).onClick += ButtonBack_OnClicked;
        #endregion

        #region PageSettings.Volume

        UIMenu menu_settings_volume = GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageSettings.Volume", null);
        GlobalMediator.UIManager.UIMaker.CreateMenuAuto(menu_settings_volume, main_settings_volume_menu);

        sliderMusicVol = ((UISlider)menu_settings_volume.GetEle("sliderMusicVol")).Slider;
        sliderSoundVol = ((UISlider)menu_settings_volume.GetEle("sliderSoundVol")).Slider;
        sliderMusicVol.onValueChanged.AddListener(OnMusicVolSliderValueChanged);
        sliderSoundVol.onValueChanged.AddListener(OnSoundVolSliderValueChanged);
        ((UIButton)menu_settings_volume.GetEle("Back")).onClick += ButtonBack_OnClicked;
        #endregion

        GlobalMediator.UIManager.GoMenuPage(menu_main);
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 1;
        GlobalMediator.UIManager.FadeOut();

       
    }
    //退出
    private void Exit()
    {
        StartCoroutine(ExitC());
    }
    IEnumerator ExitC()
    {
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 3f;
        GlobalMediator.UIManager.FadeIn();
        yield return new WaitForSeconds(1f);

        GlobalMediator.UIManager.HideUIForAWhile();
        GlobalMediator.ExitGame();
    }

    //dialog 返回事件
    private void UIDialogOK(int dlgid, bool clickedok, bool clicked3)
    {
        if (dlgid == 100)
        {
            if (clickedok) Exit();
        }
        else if (dlgid == 101)
        {
            if (!clickedok)
            {
                if (oldScrrenResolution != -1)
                    GlobalSettings.SetCurrentScrrenResolution(oldScrrenResolution);
            }
            else oldScrrenResolution = -1;
        }
        else if (dlgid == 102)
        {
            if (!clickedok)
            {
                if (oldScrrenQuality != -1)
                    GlobalSettings.SetCurrentQualitySettings(oldScrrenQuality);
            }
            else oldScrrenQuality = -1;
        }
    }

    private int oldScrrenResolution = -1;
    private int oldScrrenQuality = -1;

    void OnScreenDropDownValueChanged(int dd)
    {
        oldScrrenResolution = dropdownScreen.value;
        GlobalSettings.SetCurrentScrrenResolution(dd);
        GlobalMediator.UIManager.UIMaker.ShowDialog(101, "您希望保存更改吗？", "屏幕分辨率已经改为：" + GlobalSettings.GetCurrentScrrenResolution(), "保存", "取消");

    }
    void OnFullScreenValueChanged(bool b)
    {
        GlobalSettings.IsFullScreen = b;
    }
    void OnQualityDropDownValueChanged(int dd)
    {
        oldScrrenQuality = GlobalSettings.GetCurrentQualitySettings();
        GlobalSettings.SetCurrentQualitySettings(dd);
        GlobalMediator.UIManager.UIMaker.ShowDialog(102, "您希望保存更改吗？", "画质已经改为：" + QualitySettings.GetQualityLevel().ToString(), "保存", "取消");
    }
    void OnMusicVolSliderValueChanged(float v)
    {
        GlobalSettings.MusicVolume = v;
    }
    void OnSoundVolSliderValueChanged(float v)
    {
        GlobalSettings.SoundVolume = v;
    }

    void ButtonBack_OnClicked()
    {
        GlobalMediator.UIManager.BackForntMenuPage("MainMenuUI");
    }
    void ButtonStartOrg_OnClicked()
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageStart.Orginal");
    }
    void ButtonStartModMgr_OnClicked()
    {
        //if(GlobalSettings.Debug)
            StartCoroutine(StartLoadModMgr());
        //else
        //    GlobalMediator.UIManager.UIMaker.ShowDialog(13, "Mod 管理器只能在调试模式中使用", "请先开启调试模式", "确定", null);
    }
    void ButtonStart_OnClicked()
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageStart");
    }
    void ButtonQuit_OnClicked()
    {
        GlobalMediator.UIManager.UIMaker.ShowDialog(100, "真的要退出？", "", "退出", "取消");
    }
    void ButtonAbout_OnClicked()
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageAbout");
    }
    void ButtonSet_OnClicked()
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageSettings");
    }

    void ButtonStartOrg1_OnClicked()
    {
        StartLoadLevel("1");
    }
    void ButtonStartOrg2_OnClicked()
    {
        StartLoadLevel("2");
    }
    void ButtonStartOrg3_OnClicked()
    {
        StartLoadLevel("3");
    }
    void ButtonStartOrg4_OnClicked()
    {
        StartLoadLevel("4");
    }
    void ButtonStartOrg5_OnClicked()
    {
        StartLoadLevel("5");
    }
    void ButtonStartOrg6_OnClicked()
    {
        StartLoadLevel("6");
    }
    void ButtonStartOrg7_OnClicked()
    {
        StartLoadLevel("7");
    }
    void ButtonStartOrg8_OnClicked()
    {
        StartLoadLevel("8");
    }
    void ButtonStartOrg9_OnClicked()
    {
        StartLoadLevel("9");
    }
    void ButtonStartOrg10_OnClicked()
    {
        StartLoadLevel("10");
    }
    void ButtonStartOrg11_OnClicked()
    {
        StartLoadLevel("11");
    }
    void ButtonStartOrg12_OnClicked()
    {
        StartLoadLevel("12");
    }
    void ButtonStartOrgLZ_OnClicked()
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageStart.Orginal.LZ");
        isInLZ = true;
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 3f;
        GlobalMediator.UIManager.UIFadeHlper.FadeOut();
        SkyboxMain.material = skyboxMaterialLz;
        SkyLightA.color = new Color(0.4f, 0.4f, 0.5f);
        SkyLightB.color = new Color(0.4f, 0.4f, 0.5f);
        GlobalMediator.CallAction(GameActions.PlaySound("sounds_*TYPE*", "assets/audios/music_thunder.wav", 2));
        LZSuDu.SetActive(true);
        LZLiLiang.SetActive(true);
        LZNengLi.SetActive(true);
    }
    void ButtonBackLZ_OnClicked()
    {
        ExitLZ();
    }
    void ExitLZ()
    {
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 3f;
        GlobalMediator.UIManager.UIFadeHlper.FadeOut();
        SkyLightA.color = new Color(1, 0.9686f, 0.9137f);
        SkyLightB.color = new Color(1, 0.9686f, 0.9137f);
        isInLZ = false;
        GlobalMediator.UIManager.BackForntMenuPage("MainMenuUI");
        SkyboxMain.material = skyboxMaterialDay;
        LZSuDu.SetActive(false);
        LZLiLiang.SetActive(false);
        LZNengLi.SetActive(false);
    }

    void ButtonSetDisplay_OnClicked()
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageSettings.Display");
    }
    void ButtonSetSound_OnClicked()
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageSettings.Volume");
    }
    void ButtonSetControl_OnClicked()
    {

    }
    void ButtonSetOther_OnClicked()
    {

    }
    void ButtonDebug_OnClicked()
    {
        clkCount++;
        if(clkCount>8)
        {
            if (!GlobalSettings.Debug)
            {
                GlobalMediator.UIManager.UIMaker.ShowDialog(103, "您现在开启了调试模式。", "可以通过指令“/nodebug”关闭调试模式\n按“F12”开启控制台窗口", "好", "");
                GlobalSettings.Debug = true;
                GlobalMediator.CommandManager.CanDebug();
            }
        }
    }
    void ButtonBackSetDisplay_OnClicked()
    {
        GlobalMediator.UIManager.BackForntMenuPage("MainMenuUI");
    }


    private void StartLoadLevel(string level)
    {
        StartCoroutine(StartLoadLevelC(level));
    }
    IEnumerator StartLoadLevelC(string level)
    {
        GlobalMediator.CallAction(GameActions.StopSound("sounds_*TYPE*", "assets/audios/menu_atmo.wav"));
        GlobalMediator.CallAction(GameActions.PlaySound("sounds_*TYPE*", "assets/audios/menu_load.wav", 2));
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 3f;
        GlobalMediator.UIManager.FadeIn();

        yield return new WaitForSeconds(2f);

        LevelLoader.SetLoaderShouldLoad(level);
        LevelLoader.StartLoadLevel(this);
    }
    public void ModMgrBack()
    {
        gameObject.SetActive(true);
        GlobalMediator.UIManager.ReshowUI();
        GlobalMediator.CallAction(GameActions.PlaySound("sounds_*TYPE*", "assets/audios/menu_atmo.wav", 3));
    }
    IEnumerator StartLoadModMgr()
    {
        GlobalMediator.CallAction(GameActions.StopSound("sounds_*TYPE*", "assets/audios/menu_atmo.wav"));
        GlobalMediator.CallAction(GameActions.PlaySound("sounds_*TYPE*", "assets/audios/menu_load.wav", 2));
        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 3f;
        GlobalMediator.UIManager.FadeIn();

        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("ModManager", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        gameObject.SetActive(false);
        GlobalMediator.UIManager.HideUIForAWhile();

        ModMgr.menuLevel = this;

        GlobalMediator.UIManager.UIFadeHlper.FadeSpeed = 3f;
        GlobalMediator.UIManager.FadeOut();
    }
}
