using Assets.Scripts.Global;
using Assets.Scripts.Global.GlobalUI;
using Assets.Scripts.Global.GlobalUI.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 游戏 通用界面 管理器
 */

/// <summary>
/// 游戏 通用界面 管理器
/// </summary>
public class UIManager : MonoBehaviour
{
    public GameObject dbgCam;
    /// <summary>
    /// 当前 屏幕渐变 控制脚本
    /// </summary>
    public FadeHlper UIFadeHlper;

    //菜单模板
    const string main_start_menu =
        "$TextDef##0#显示设置" +
        "$BtnMainMenuDef#StartOrg#0#原版关卡" +
        "$BtnMainMenuDef##0#自定义关卡" +
        "$BtnMainMenuDef##0#选择关卡文件" +
        "$BtnMainBackDef#Back#20#返回";

    const string test_menu =
        "$TextDef##0#显示设置" +
        "$DropDownDef#dropdownScreen#20#屏幕分辨率设置" +
        "$ToggleDef#toggleFunnScreen#10#全屏" +
        "$DropDownDef#dropdownQuality#20#画质设置" +
        "$TextDef##20#在游戏启动时(双击exe时) 按住“Alt”键也可以设置" +
        "$BtnMainBackDef#Back#20#返回";


    private void test()
    {
        UIMaker.RegisterUIGroup("MainMenuUI");
        UIMenu menu = UIMaker.RegisterMenuPage("MainMenuUI", "Main", null);

        UIMaker.CreateMenuAuto(menu, test_menu);

        GoMenuPage(menu);
    }
    private void Btn_onClick()
    {
        UIMaker.ShowDialog(0, "测试对话框", "测试文字", "确定", "关闭");
    }

    #region 初始化参数

    public GameObject Canvas;
    public GameObject EmeptyTemplate;

    public GameObject MenuPageTemplate;
    public GameObject MenuPageCleanTemplate;
    public GameObject SliderPrefabTemplate;
    public GameObject TextPrefabTemplate;
    public GameObject TogglePrefabTemplate;
    public GameObject DropDownPrefabTemplate;
    public GameObject BtnMulitTemplate;
    public GameObject BtnMainMenuTemplate;
    public GameObject BtnMainBackTemplate;
    public GameObject BtnLevTemplate;
    public GameObject TextTitlePerfab;


    public DialogUI MenuDialog2;
    public DialogUI MenuDialog3;

    #endregion

    /// <summary>
    /// 屏幕从黑渐渐变透明
    /// </summary>
    public void FadeOut()
    {
        UIFadeHlper.FadeOut();
    }
    /// <summary>
    /// 屏幕渐渐变黑
    /// </summary>
    public void FadeIn()
    {
        UIFadeHlper.FadeIn();
    }

    /// <summary>
    /// 通用UI界面生成器实例
    /// </summary>
    public StandardUIMaker UIMaker
    {
        get;private set;
    }

    /// <summary>
    /// 通用UI界面生成器
    /// </summary>
    public class StandardUIMaker
    {
        public StandardUIMaker(UIManager parent)
        {
            this.parent = parent;
            RegisterUIEle("TextTitle", parent.TextTitlePerfab);
            RegisterUIEle("TextDef", parent.TextPrefabTemplate);
            RegisterUIEle("ToggleDef", parent.TogglePrefabTemplate);
            RegisterUIEle("SliderDef", parent.SliderPrefabTemplate);
            RegisterUIEle("DropDownDef", parent.DropDownPrefabTemplate);
            RegisterUIEle("BtnMulitDef", parent.BtnMulitTemplate);
            RegisterUIEle("BtnMainMenuDef", parent.BtnMainMenuTemplate);
            RegisterUIEle("BtnMainBackDef", parent.BtnMainBackTemplate);
            RegisterUIEle("BtnLevDef", parent.BtnLevTemplate);
        }

        private UIManager parent;
        private List<UIGroup> uIGroups = new List<UIGroup>();
        private static List<UIEleCreate> uiEles = new List<UIEleCreate>();
        private class UIGroup
        {
            public string Name;
            public GameObject UI;
            public List<UIPage> Pages = new List<UIPage>();
        }
        private class UIEleCreate
        {
            public string Name;
            public GameObject Perfab;
        }

        /// <summary>
        /// 注册UI元素
        /// </summary>
        /// <param name="group">指定元素</param>
        /// <returns></returns>
        public bool RegisterUIEle(string name, GameObject perfab)
        {
            UIEleCreate uo;
            if (!IsUIEleRegistered(name, out uo))
            {
                if (perfab != null)
                {
                    UIEleCreate u = new UIEleCreate() { Name = name, Perfab = perfab };
                    uiEles.Add(u);
                }
            }
            else
            {
                uo.Perfab = perfab;
            }
            return false;
        }
        /// <summary>
        /// 获取UI元素是否已经注册
        /// </summary>
        /// <param name="group">指定元素</param>
        /// <returns>返回UI元素是否已经注册</returns>
        public bool IsUIEleRegistered(string name)
        {
            bool rs = false;
            foreach (UIEleCreate u in uiEles)
            {
                if (u.Name == name)
                {
                    rs = true;
                    break;
                }
            }
            return rs;
        }
        /// <summary>
        /// 取消注册UI组
        /// </summary>
        /// <param name="group">指定元素</param>
        /// <returns>返回是否成功</returns>
        public bool UnregisterUIEle(string name)
        {
            UIEleCreate g;
            if (IsUIEleRegistered(name, out g))
            {
                uiEles.Remove(g);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取UI组是否已经注册
        /// </summary>
        /// <param name="name">指定元素</param>
        /// <param name="ug">如果已经注册则返回元素到此变量</param>
        /// <returns>返回UI元素是否已经注册</returns>
        private bool IsUIEleRegistered(string name, out UIEleCreate ug)
        {
            foreach (UIEleCreate u in uiEles)
            {
                if (u.Name == name)
                {
                    ug = u;
                    return true;
                }
            }
            ug = null;
            return false;
        }


        /// <summary>
        /// 注册UI组
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="ui">附加UI物体，不填则使用默认</param>
        /// <returns></returns>
        public bool RegisterUIGroup(string group, GameObject ui = null)
        {
            if (!IsUIGroupRegistered(group))
            {
                if (ui == null)
                {
                    ui = GameObject.Instantiate(parent.EmeptyTemplate);
                    ui.transform.SetParent(parent.Canvas.transform);
                    ui.name = group;
                    RectTransform t = ui.GetComponent<RectTransform>();
                    t.anchoredPosition = Vector2.zero;
                    t.offsetMax = Vector2.zero;
                    t.offsetMin = Vector2.zero;
                }
                ui.transform.SetParent(parent.Canvas.transform);
                if (ui.activeSelf)
                    ui.SetActive(false);
                UIGroup u = new UIGroup() { Name = group, UI = ui };
                uIGroups.Add(u);
            }
            return false;
        }
        /// <summary>
        /// 获取UI组是否已经注册
        /// </summary>
        /// <param name="group">指定组</param>
        /// <returns>返回UI组是否已经注册</returns>
        public bool IsUIGroupRegistered(string group)
        {
            bool rs = false;
            foreach (UIGroup u in uIGroups)
            {
                if (u.Name == group)
                {
                    rs = true;
                    break;
                }
            }
            return rs;
        }
        /// <summary>
        /// 取消注册UI组
        /// </summary>
        /// <param name="group">指定组</param>
        /// <returns>返回是否成功</returns>
        public bool UnregisterUIGroup(string group)
        {
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                uIGroups.Remove(g);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取UI组是否已经注册
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="ug">如果已经注册则返回组到此变量</param>
        /// <returns>返回UI组是否已经注册</returns>
        private bool IsUIGroupRegistered(string group, out UIGroup ug)
        {
            bool rs = false;
            foreach (UIGroup u in uIGroups)
            {
                if (u.Name == group)
                {
                    ug = u;
                    return true;
                }
            }
            ug = default(UIGroup);
            return rs;
        }

        /// <summary>
        /// 注册 UI 页
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="pg">自定义UI页 UI物体（不填使用默认）</param>
        /// <param name="pginnern">自定义UI页 UI物体（内部）</param>
        /// <returns>返回是否成功</returns>
        public UIMenu RegisterMenuPage(string group, string address, UIMenu pg)
        {
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (!IsMenuPageRegistered(group, address))
                {
                    if (pg == null)
                    {
                        pg = Instantiate(parent.MenuPageTemplate).GetComponent<UIMenu>();
                        pg.transform.SetParent(g.UI.transform);
                        RectTransform t = pg.GetComponent<RectTransform>();
                        t.anchoredPosition = Vector2.zero;
                        t.offsetMax = Vector2.zero;
                        t.offsetMin = Vector2.zero;
                        t.sizeDelta = new Vector2(350, t.sizeDelta.y);
                        pg.Oninit();
                        if (pg.gameObject.activeSelf)
                            pg.gameObject.SetActive(false);
                        pg.Address = address;
                        pg.Group = group;
                        g.Pages.Add(pg);
                    }
                    else
                    {
                        pg.Oninit();
                        if (pg.gameObject.activeSelf)
                            pg.gameObject.SetActive(false);
                        g.Pages.Add(pg);
                    }
                    return pg;
                }
            }
            return null;
        }
        /// <summary>
        /// 使用 自定义 UI页模板 注册 UI 页 UI页模板 必须 附加 UIMenu 或  UIMenu 的继承类
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="template"> 自定义 UI页模板</param>
        /// <returns>返回是否成功</returns>
        public UIMenu RegisterMenuPageWithTemplate(string group, string address, GameObject template)
        {
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (!IsMenuPageRegistered(group, address))
                {
                    if (template != null)
                    {
                        GameObject pg = Instantiate(template);
                        pg.transform.SetParent(g.UI.transform);
                        RectTransform t = pg.GetComponent<RectTransform>();
                        t.anchoredPosition = Vector2.zero;
                        t.offsetMax = Vector2.zero;
                        t.offsetMin = Vector2.zero;
                        t.sizeDelta = new Vector2(350, t.sizeDelta.y);                       
                        UIMenu p = pg.GetComponent<UIMenu>();
                        p.Oninit();
                        if (pg.activeSelf)
                            pg.SetActive(false);
                        p.Address = address;
                        p.Group = group;
                        g.Pages.Add(p);
                        return p;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取 UI 页 是否已经注册
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <returns>返回是否注册</returns>
        public bool IsMenuPageRegistered(string group, string address)
        {
            bool rs = false;
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                foreach (UIPage u in g.Pages)
                {
                    if (u.Address == address)
                    {
                        rs = true;
                        break;
                    }
                }
            }
            return rs;
        }
        /// <summary>
        /// 获取 UI 页 是否已经注册
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="p">如果已经注册则返回指定页到变量 p 中</param>
        /// <returns>返回是否注册</returns>
        private bool IsMenuPageRegistered(string group, string address, out UIPage p)
        {
            bool rs = false;
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                foreach (UIPage u in g.Pages)
                {
                    if (u.Address == address)
                    {
                        p = u;
                        return true;
                    }
                }
            }
            p = null;
            return rs;
        }
        /// <summary>
        /// 取消注册 UI 页
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <returns>返回是否成功</returns>
        public bool UnegisterMenuPage(string group, string address)
        {
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                UIPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    g.Pages.Remove(p);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 跳转到指定 UI 页
        /// </summary>
        /// <param name="p">指定UI页</param>
        public void GoMenuPage(UIPage p)
        {
            if (p != null) GoMenuPage(p.Group, p.Address);
        }
        /// <summary>
        /// 隐藏 UI 页
        /// </summary>
        /// <param name="p">指定UI页</param>
        public void HideMenuPage(UIPage p)
        {
            if (p != null)
                HideMenuPage(p.Group);
        }
        /// <summary>
        /// 跳转到指定 UI 页
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        public void GoMenuPage(string group, string address)
        {
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (!g.UI.activeSelf)
                    g.UI.SetActive(true);
                if (parent.showedPage != null)
                    parent.showedPage.Hide();

                UIPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (!p.IsShowed())
                        p.Show();
                    parent.showedPage = p;
                    parent.lastPageAddress = p.Address;
                }
            }
        }
        /// <summary>
        /// 隐藏 UI 页
        /// </summary>
        /// <param name="group">指定组</param>
        public void HideMenuPage(string group)
        {
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (parent.showedPage != null)
                    parent.showedPage.Hide();
                parent.showedPage = null;
                if (g.UI.activeSelf)
                    g.UI.SetActive(false);
                parent.lastPageAddress = "";
            }
        }
        /// <summary>
        /// 返回上一页
        /// </summary>
        /// <param name="group">指定组</param>
        public void BackForntMenuPage(string group)
        {
            UIGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (parent.lastPageAddress != "")
                {
                    if (parent.showedPage != null)
                        parent.showedPage.Hide();

                    if (parent.lastPageAddress.Contains("."))
                    {
                        parent.lastPageAddress = parent.lastPageAddress.Substring(0, parent.lastPageAddress.LastIndexOf('.'));
                        GoMenuPage(group, parent.lastPageAddress);
                    }
                }
            }
        }

        public UIElement CreateElementAuto(UIMenu m, string eleTypeName, string name = "", int sp = 0, string initstr="")
        {
            if (m != null)
            {
                UIEleCreate g;
                if (IsUIEleRegistered(eleTypeName, out g))
                {
                    UIElement btn = Instantiate(g.Perfab).GetComponent<UIElement>();
                    if (btn == null)
                    {
                        Destroy(btn);
                        return null;
                    }
                    btn.Oninit();
                    if (initstr != "") btn.StartSet(initstr);
                    btn.Name = name;
                    btn.SpaceStart = sp;
                    if (name != "")
                        btn.gameObject.name = eleTypeName + "_" + name;
                    m.AddEle(btn);
                    return btn;
                }
            }
            return null;
        }
        public bool CreateMenuAuto(UIMenu m, string table)
        {
            UIAutoLayoutMenu mx = null;
            if (m is UIAutoLayoutMenu)
            {
                mx = m as UIAutoLayoutMenu;
                mx.LockLayout();
            }
            string[] d = table.Split('$');
            string li = "";
            for (int i = 0; i < d.Length; i++)
            {
                li = d[i];
                if (li.Contains("#"))
                {
                    string[] s = li.Split('#');
                    if (s.Length == 4)
                        CreateElementAuto(m, s[0], s[1], int.Parse(s[2]), s[3]);
                    else if (s.Length == 3)
                        CreateElementAuto(m, s[0], s[1], int.Parse(s[2]));
                    else if (s.Length == 2)
                        CreateElementAuto(m, s[0], s[1]);
                    else CreateElementAuto(m, s[0]);
                }
            }
            m.UpdateMenu();
            if (m is UIAutoLayoutMenu)
            {
                mx.UnLockLayout();

                mx.DoLayout();
            }
            return false;
        }

        void DialogCallBack(bool b, bool b2)
        {
            parent.ReshowUI();
            GlobalMediator.DispatchEvent("UIDialogClosed", null, parent.showedDialogid, b, b2);
        }

        /// <summary>
        /// 显示两个按钮的对话框
        /// </summary>
        /// <param name="id">对话框id 接收用户按下某个按钮消息时识别是不是自己的对话框</param>
        /// <param name="title">标题</param>
        /// <param name="text">文字</param>
        /// <param name="btnOKText">ok按钮的文字</param>
        /// <param name="btnCancelText">cancel按钮的文字</param>
        public void ShowDialog(int id, string title, string text, string btnOKText, string btnCancelText)
        {
            parent.HideUIForAWhile();
            parent.showedDialogid = id;
            parent.MenuDialog2.Set(title, text, btnOKText, btnCancelText,"");
            parent.MenuDialog2.Show(DialogCallBack);
        }
        /// <summary>
        /// 显示三个按钮的对话框
        /// </summary>
        /// <param name="id">对话框id 接收用户按下某个按钮消息时识别是不是自己的对话框</param>
        /// <param name="title">标题</param>
        /// <param name="text">文字</param>
        /// <param name="btnOKText"ok按钮的文字></param>
        /// <param name="btnCancelText">cancel按钮的文字</param>
        /// <param name="btn3Text">第三个按钮的文字</param>
        public void ShowDialogThreeChoice(int id, string title, string text, string btnOKText, string btnCancelText, string btn3Text)
        {
            parent.HideUIForAWhile();
            parent.showedDialogid = id;
            parent.MenuDialog3.Set(title, text, btnOKText, btnCancelText, btn3Text);
            parent.MenuDialog3.Show(DialogCallBack);
        }
    }

    private UIPage showedPage;
    private string lastPageAddress = "";
    private int showedDialogid = 0;
    private static bool isBlack;

    /// <summary>
    /// 上一次显示的 UI 页
    /// </summary>
    public string LastShowMenuPage
    {
        get { return lastPageAddress; }
    }

    /// <summary>
    /// 跳转到指定 UI 页
    /// </summary>
    /// <param name="p">指定UI页</param>
    public void GoMenuPage(UIPage p)
    {
        if (p != null) GoMenuPage(p.Group, p.Address);
    }
    /// <summary>
    /// 隐藏 UI 页
    /// </summary>
    /// <param name="p">指定UI页</param>
    public void HideMenuPage(UIPage p)
    {
        if (p != null)
            HideMenuPage(p.Group);
    }
    /// <summary>
    /// 跳转到指定 UI 页
    /// </summary>
    /// <param name="group">指定组</param>
    /// <param name="address">指定地址</param>
    public void GoMenuPage(string group, string address)
    {
        UIMaker.GoMenuPage(group, address);
    }
    /// <summary>
    /// 隐藏 UI 页
    /// </summary>
    /// <param name="group">指定组</param>
    public void HideMenuPage(string group)
    {
        UIMaker.HideMenuPage(group);
    }
    /// <summary>
    /// 返回上一页
    /// </summary>
    /// <param name="group">指定组</param>
    public void BackForntMenuPage(string group)
    {
        UIMaker.BackForntMenuPage(group);
    }
    /// <summary>
    /// 强制隐藏UI
    /// </summary>
    public void HideUIForAWhile()
    {
        if (showedPage != null)
            if (showedPage.IsShowed()) showedPage.Hide();
    }
    /// <summary>
    /// 强制显示 UI
    /// </summary>
    public void ReshowUI()
    {
        if (showedPage != null)
            if (!showedPage.IsShowed()) showedPage.Show();
    }
    /// <summary>
    /// 设置开始时候屏幕不黑
    /// </summary>
    public static void SetStartNoBlack()
    {
        isBlack = false;
    }
    /// <summary>
    /// 设置开始的时候屏幕变黑
    /// </summary>
    public static void SetStartBlack()
    {
        isBlack = true;
    }

    private void Start()
    {
        UIMaker = new StandardUIMaker(this);
        GlobalMediator.SetUIManager(this);
        if(!GlobalSettings.StartInIntro) dbgCam.SetActive(true);

        if(isBlack) UIFadeHlper.Alpha = 1;
        else UIFadeHlper.Alpha = 0;
    }
    private void OnGUI()
    {
      /*  if(GUI.Button(new Rect(0,0,50,30), "in();"))
        {
            FadeIn();
        }
        if (GUI.Button(new Rect(0, 35, 50, 30), "out();"))
        {
            FadeOut();
        }*/
    }
    private void OnDestroy()
    {
        UIMaker = null;
        GlobalMediator.SetUIManagerUnavailable(this);

    }
}
