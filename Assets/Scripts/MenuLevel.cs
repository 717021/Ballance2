using Assets.Scripts.Global;
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

    private Toggle toggleFunnScreen;
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
    //创建菜单
    private void InitMenus()
    {
        #region PageMain
        GlobalMediator.UIManager.UIMaker.RegisterUIGroup("MainMenuUI");
        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "开始", "Menu", ButtonStart_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "分数", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "设置", "Menu", ButtonSet_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "关于", "Menu", ButtonAbout_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "退出", "Menu", ButtonQuit_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageStart", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageStart", "请选择关卡开始");
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart", "原版关卡", "Menu", ButtonStartOrg_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart", "自定义关卡", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart", "选择关卡文件", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart", "返回", "Back", ButtonBack_OnClicked, 10);
        #endregion

        #region PageStart.Orginal
        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageStart.Orginal", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level1", "Level", ButtonStartOrg1_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level2", "Level", ButtonStartOrg2_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level3", "Level", ButtonStartOrg3_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level4", "Level", ButtonStartOrg4_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level5", "Level", ButtonStartOrg5_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level6", "Level", ButtonStartOrg6_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level7", "Level", ButtonStartOrg7_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level8", "Level", ButtonStartOrg8_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level9", "Level", ButtonStartOrg9_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level10", "Level", ButtonStartOrg10_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level11", "Level", ButtonStartOrg11_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Level12", "Level", ButtonStartOrg12_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "Light Zone", "Level", ButtonStartOrgLZ_OnClicked, 5);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal", "返回", "Back", ButtonBack_OnClicked, 10);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageWithTemplate("MainMenuUI", "Main.PageStart.Orginal.LZ", GlobalMediator.UIManager.MenuPageCleanTemplate);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageStart.Orginal.LZ", "返回", "Back", ButtonBackLZ_OnClicked, 500);
        #endregion

        #region PageSettings
        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageSettings", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageSettings", "设置");
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageSettings", "显示设置", "Menu", ButtonSetDisplay_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageSettings", "控制设置", "Menu", ButtonSetControl_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageSettings", "音乐设置", "Menu", ButtonSetSound_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageSettings", "其他设置", "Menu", ButtonSetOther_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageSettings", "返回", "Back", ButtonBack_OnClicked, 10);
        #endregion

        #region PageAbout
        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageAbout", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageAbout", "关于");
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageAbout", "Ballance2", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageAbout", "Ballance Unity 重制版", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageAbout", "V.1.0\n" + StaticValues.GameVersion, "Menu", ButtonDebug_OnClicked);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageAbout", "Bulid"+ StaticValues.GameBulidVersion, "Menu", null);

        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageAbout", "返回", "Back", ButtonBack_OnClicked, 10);
        #endregion

        #region PageSettings.Display
        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageSettings.Display", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageSettings.Display", "显示设置");
#if UNITY_STANDALONE
        //屏幕分辨率设置按钮
        dropdownScreen = GlobalMediator.UIManager.UIMaker.RegisterMenuPageDropdown("MainMenuUI", "Main.PageSettings.Display", "屏幕分辨率设置", null);
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
        toggleFunnScreen = GlobalMediator.UIManager.UIMaker.RegisterMenuPageToggle("MainMenuUI", "Main.PageSettings.Display", "全屏");
        toggleFunnScreen.onValueChanged.AddListener(OnFullScreenValueChanged);
        toggleFunnScreen.isOn = GlobalSettings.IsFullScreen;

#elif UNITY_ANDROID
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageSettings.Display", "屏幕分辨率 : "+ GlobalSettings.GetCurrentScrrenResolution());
#endif
        dropdownQuality = GlobalMediator.UIManager.UIMaker.RegisterMenuPageDropdown("MainMenuUI", "Main.PageSettings.Display", "画质设置", null);
        foreach (string ss in QualitySettings.names)
            dropdownQuality.options.Add(new Dropdown.OptionData(ss));
        dropdownQuality.value = GlobalSettings.GetCurrentQualitySettings();
        dropdownQuality.onValueChanged.AddListener(OnQualityDropDownValueChanged);

#if UNITY_STANDALONE
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageSettings.Display", "在游戏启动时(双击exe时) 按住“Alt”键也可以设置");
#endif
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageSettings.Display", "返回", "Back", ButtonBackSetDisplay_OnClicked, 10);
        #endregion

        #region PageSettings.Volume

        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main.PageSettings.Volume", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageText("MainMenuUI", "Main.PageSettings.Volume", "音量设置");
        sliderMusicVol = GlobalMediator.UIManager.UIMaker.RegisterMenuPageSlider("MainMenuUI", "Main.PageSettings.Volume", "音乐音量", GlobalSettings.MusicVolume, null);
        sliderSoundVol = GlobalMediator.UIManager.UIMaker.RegisterMenuPageSlider("MainMenuUI", "Main.PageSettings.Volume", "音效音量", GlobalSettings.SoundVolume, null);
        sliderMusicVol.onValueChanged.AddListener(OnMusicVolSliderValueChanged);
        sliderSoundVol.onValueChanged.AddListener(OnSoundVolSliderValueChanged);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main.PageSettings.Volume", "返回", "Back", ButtonBack_OnClicked, 30);
        #endregion

        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main");
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

    void ButtonBack_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.BackForntMenuPage("MainMenuUI");
    }
    void ButtonStartOrg_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageStart.Orginal");
    }
    void ButtonStart_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageStart");
    }
    void ButtonQuit_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.UIMaker.ShowDialog(100, "真的要退出？", "", "退出", "取消");
    }
    void ButtonAbout_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageAbout");
    }
    void ButtonSet_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageSettings");
    }

    void ButtonStartOrg1_OnClicked(GameObject g)
    {
        StartLoadLevel("1");
    }
    void ButtonStartOrg2_OnClicked(GameObject g)
    {
        StartLoadLevel("2");
    }
    void ButtonStartOrg3_OnClicked(GameObject g)
    {
        StartLoadLevel("3");
    }
    void ButtonStartOrg4_OnClicked(GameObject g)
    {
        StartLoadLevel("4");
    }
    void ButtonStartOrg5_OnClicked(GameObject g)
    {
        StartLoadLevel("5");
    }
    void ButtonStartOrg6_OnClicked(GameObject g)
    {
        StartLoadLevel("6");
    }
    void ButtonStartOrg7_OnClicked(GameObject g)
    {
        StartLoadLevel("7");
    }
    void ButtonStartOrg8_OnClicked(GameObject g)
    {
        StartLoadLevel("8");
    }
    void ButtonStartOrg9_OnClicked(GameObject g)
    {
        StartLoadLevel("9");
    }
    void ButtonStartOrg10_OnClicked(GameObject g)
    {
        StartLoadLevel("10");
    }
    void ButtonStartOrg11_OnClicked(GameObject g)
    {
        StartLoadLevel("11");
    }
    void ButtonStartOrg12_OnClicked(GameObject g)
    {
        StartLoadLevel("12");
    }
    void ButtonStartOrgLZ_OnClicked(GameObject g)
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
    void ButtonBackLZ_OnClicked(GameObject g)
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

    void ButtonSetDisplay_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageSettings.Display");
    }
    void ButtonSetSound_OnClicked(GameObject g)
    {
        GlobalMediator.UIManager.GoMenuPage("MainMenuUI", "Main.PageSettings.Volume");
    }
    void ButtonSetControl_OnClicked(GameObject g)
    {

    }
    void ButtonSetOther_OnClicked(GameObject g)
    {

    }
    void ButtonDebug_OnClicked(GameObject g)
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
    void ButtonBackSetDisplay_OnClicked(GameObject g)
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
}
