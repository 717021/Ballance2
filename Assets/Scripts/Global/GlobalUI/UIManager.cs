using Assets.Scripts.Global;
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

    private void test()
    {
        GlobalMediator.UIManager.UIMaker.RegisterUIGroup("MainMenuUI");
        GlobalMediator.UIManager.UIMaker.RegisterMenuPage("MainMenuUI", "Main", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "开始", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "分数", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "设置", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "关于", "Menu", null);
        GlobalMediator.UIManager.UIMaker.RegisterMenuPageButton("MainMenuUI", "Main", "退出", "Menu", null);
    }

    #region 初始化参数

    public GameObject Canvas;
    public GameObject EmeptyTemplate;

    public GameObject MenuPageTemplate;
    public GameObject MenuPageCleanTemplate;

    public GameObject MenuTextTemplate;
    public GameObject MenuBtnHugeTemplate;
    public GameObject MenuBtnNornalTemplate;
    public GameObject MenuBtnSmallTemplate;
    public GameObject MenuBtnMultiTemplate;
    public GameObject MenuImageTemplate;
    public GameObject MenuSwitchTemplate;
    public GameObject MenuScrollTemplate;
    public GameObject MenuDropDownTemplate;

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
            RegisterMenuPageButtonStyle("Menu", parent.MenuBtnHugeTemplate);
            RegisterMenuPageButtonStyle("Back", parent.MenuBtnNornalTemplate);
            RegisterMenuPageButtonStyle("Level", parent.MenuBtnSmallTemplate);
            RegisterMenuPageButtonStyle("Key", parent.MenuBtnMultiTemplate);

        }

        private UIManager parent;
        private List<uiGroup> uIGroups = new List<uiGroup>();
        private class uiGroup
        {
            public string Name;
            public GameObject UI;
            public List<uiPage> Pages = new List<uiPage>();
        }
        private class uiPage
        {
            public bool IsMenuPage;
            public string Address;
            public GameObject Page;
            public GameObject PageInnern;
            public int BtnCount = 0;
            public float Height = 0;
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
                uiGroup u = new uiGroup() { Name = group, UI = ui };
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
            foreach (uiGroup u in uIGroups)
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
            uiGroup g;
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
        private bool IsUIGroupRegistered(string group, out uiGroup ug)
        {
            bool rs = false;
            foreach (uiGroup u in uIGroups)
            {
                if (u.Name == group)
                {
                    ug = u;
                    return true;
                }
            }
            ug = default(uiGroup);
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
        public bool RegisterMenuPage(string group, string address, GameObject pg, GameObject pginnern = null)
        {
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (!IsMenuPageRegistered(group, address))
                {
                    if (pg == null)
                    {
                        pg = Instantiate(parent.MenuPageTemplate);
                        pg.transform.SetParent(g.UI.transform);
                        RectTransform t = pg.GetComponent<RectTransform>();
                        t.anchoredPosition = Vector2.zero;
                        t.offsetMax = Vector2.zero;
                        t.offsetMin = Vector2.zero;
                        t.sizeDelta = new Vector2(350, t.sizeDelta.y);
                        if (pg.activeSelf)
                            pg.SetActive(false);
                        uiPage p = new uiPage() { Address = address, Page = pg, PageInnern = pg.transform.GetChild(0).gameObject, IsMenuPage = true };
                        g.Pages.Add(p);
                    }
                    else
                    {
                        if (pg.activeSelf)
                            pg.SetActive(false);
                        uiPage p = new uiPage() { Address = address, Page = pg, PageInnern = (pginnern == null ? pg : pginnern) };
                        g.Pages.Add(p);
                    }
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 使用 自定义 UI页模板 注册 UI 页
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="template"> 自定义 UI页模板</param>
        /// <returns>返回是否成功</returns>
        public bool RegisterMenuPageWithTemplate(string group, string address, GameObject template)
        {
            uiGroup g;
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
                        if (pg.activeSelf)
                            pg.SetActive(false);
                        uiPage p = new uiPage() { Address = address, Page = pg, PageInnern = pg.transform.childCount == 0 ? null : pg.transform.GetChild(0).gameObject, IsMenuPage = true };
                        g.Pages.Add(p);
                        return true;
                    }
                }
            }
            return false;
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
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                foreach (uiPage u in g.Pages)
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
        private bool IsMenuPageRegistered(string group, string address, out uiPage p)
        {
            bool rs = false;
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                foreach (uiPage u in g.Pages)
                {
                    if (u.Address == address)
                    {
                        p = u;
                        return true;
                    }
                }
            }
            p = default(uiPage);
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
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    g.Pages.Remove(p);
                    return true;
                }
            }
            return false;
        }

        private Dictionary<string, GameObject> pageButtonStyles = new Dictionary<string, GameObject>();

        /// <summary>
        /// 注册按钮样式
        /// </summary>
        /// <param name="btnStyle">按钮样式名称</param>
        /// <param name="perfab">按钮样式perfab</param>
        public void RegisterMenuPageButtonStyle(string btnStyle, GameObject perfab)
        {
            pageButtonStyles.Add(btnStyle, perfab);
        }
        /// <summary>
        /// 向指定页里添加一个按钮
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="text">按钮文字</param>
        /// <param name="btnStyle">这个按钮使用的样式</param>
        /// <param name="callback">点击事件</param>
        /// <param name="offist">pos Y偏移</param>
        /// <returns></returns>
        public Button RegisterMenuPageButton(string group, string address, string text, string btnStyle, EventTriggerListener.VoidDelegate callback, int offist = 20)
        {
            Button rs = null;
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (p.IsMenuPage)
                    {
                        GameObject perfab = null;
                        if (pageButtonStyles.TryGetValue(btnStyle, out perfab))
                        {
                            p.BtnCount = p.BtnCount + 1;
                            GameObject btn = Instantiate(perfab);
                            btn.transform.SetParent(p.PageInnern.transform);
                            btn.transform.GetChild(0).gameObject.GetComponent<Text>().text = text;
                            RectTransform pi = p.PageInnern.GetComponent<RectTransform>();
                            RectTransform rgg = btn.GetComponent<RectTransform>();
                            p.Height += (rgg.sizeDelta.y + offist);
                            pi.sizeDelta = new Vector2(pi.sizeDelta.x, p.Height);
                            rgg.anchoredPosition = new Vector2(0, -p.Height + rgg.sizeDelta.y / 2);
                            if (callback != null) EventTriggerListener.Get(btn).onClick = callback;
                            rs = btn.GetComponent<Button>();
                        }
                    }
                }
            }
            return rs;
        }
        /// <summary>
        /// 向指定页里添加一个复选框
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="text">复选框文字</param>
        /// <param name="offist">pos Y偏移</param>
        /// <returns></returns>
        public Toggle RegisterMenuPageToggle(string group, string address, string text, int offist = 20)
        {
            Toggle rs = null;
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (p.IsMenuPage)
                    {
                        GameObject perfab = parent.MenuSwitchTemplate;
                        p.BtnCount = p.BtnCount + 1;
                        GameObject btn = Instantiate(perfab);
                        btn.transform.SetParent(p.PageInnern.transform);
                        btn.transform.GetChild(1).gameObject.GetComponent<Text>().text = text;
                        RectTransform pi = p.PageInnern.GetComponent<RectTransform>();
                        RectTransform rgg = btn.GetComponent<RectTransform>();
                        p.Height += (rgg.sizeDelta.y + offist);
                        pi.sizeDelta = new Vector2(pi.sizeDelta.x, p.Height);
                        rgg.anchoredPosition = new Vector2(0, -p.Height + rgg.sizeDelta.y / 2);
                        rs = btn.GetComponent<Toggle>();

                    }
                }
            }
            return rs;
        }
        /// <summary>
        /// 向指定页里添加一个文字控件
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="text">文本</param>
        /// <param name="offist">pos Y偏移</param>
        /// <returns></returns>
        public Text RegisterMenuPageText(string group, string address, string text, int offist = 20)
        {
            Text rs = null;
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (p.IsMenuPage)
                    {
                        p.BtnCount = p.BtnCount + 1;
                        GameObject btn = Instantiate(parent.MenuTextTemplate);
                        btn.transform.SetParent(p.PageInnern.transform);
                        Text txt = btn.GetComponent<Text>();
                        txt.text = text;
                        rs = txt;
                        RectTransform pi = p.PageInnern.GetComponent<RectTransform>();
                        RectTransform rgg = btn.GetComponent<RectTransform>();
                        p.Height += (rgg.sizeDelta.y + offist);
                        pi.sizeDelta = new Vector2(pi.sizeDelta.x, p.Height);
                        rgg.anchoredPosition = new Vector2(0, -p.Height + rgg.sizeDelta.y / 2);
                    }
                }
            }
            return rs;
        }
        /// <summary>
        /// 向指定页里添加一个图像控件
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="image">图片</param>
        /// <param name="offist">pos Y偏移</param>
        /// <returns></returns>
        public Image RegisterMenuPageImage(string group, string address, Sprite image, int offist = 20)
        {
            uiGroup g;
            Image rs = null;
            if (IsUIGroupRegistered(group, out g))
            {
                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (p.IsMenuPage)
                    {
                        p.BtnCount = p.BtnCount + 1;
                        GameObject btn = Instantiate(parent.MenuImageTemplate);
                        btn.transform.SetParent(p.PageInnern.transform);
                        Image img = btn.GetComponent<Image>();
                        img.sprite = image;
                        rs = img;
                        RectTransform pi = p.PageInnern.GetComponent<RectTransform>();
                        RectTransform rgg = btn.GetComponent<RectTransform>();
                        p.Height += (rgg.sizeDelta.y + offist);
                        pi.sizeDelta = new Vector2(pi.sizeDelta.x, p.Height);
                        rgg.anchoredPosition = new Vector2(0, -p.Height + rgg.sizeDelta.y / 2);
                    }
                }
            }
            return rs;
        }
        /// <summary>
        /// 向指定页里添加一个滑动条控件
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="text">滑动条上的文字</param>
        /// <param name="startvalue">滑动条起始数值</param>
        /// <param name="callback">滑动条点击事件</param>
        /// <param name="offist">pos Y偏移</param>
        /// <returns></returns>
        public Slider RegisterMenuPageSlider(string group, string address, string text, float startvalue, EventTriggerListener.VoidDelegate callback, int offist = 20)
        {
            uiGroup g;
            Slider rs = null;
            if (IsUIGroupRegistered(group, out g))
            {
                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (p.IsMenuPage)
                    {
                        p.BtnCount = p.BtnCount + 1;
                        GameObject btn = Instantiate(parent.MenuScrollTemplate);
                        btn.transform.SetParent(p.PageInnern.transform);
                        btn.transform.GetChild(1).gameObject.GetComponent<Text>().text = text;
                        GameObject sl = btn.transform.GetChild(0).gameObject;
                        Slider slider = sl.GetComponent<Slider>();
                        slider.value = startvalue;
                        rs = slider;
                        RectTransform pi = p.PageInnern.GetComponent<RectTransform>();
                        RectTransform rgg = btn.GetComponent<RectTransform>();
                        p.Height += (rgg.sizeDelta.y + offist);
                        pi.sizeDelta = new Vector2(pi.sizeDelta.x, p.Height);
                        rgg.anchoredPosition = new Vector2(0, -p.Height + rgg.sizeDelta.y / 2);
                        if (callback != null) EventTriggerListener.Get(sl).onClick = callback;
                    }
                }
            }
            return rs;
        }
        /// <summary>
        /// 向指定页里添加一个下拉列表
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        /// <param name="text">下拉列表上的文字</param>
        /// <param name="callback">下拉列表点击事件</param>
        /// <param name="offist">pos Y偏移</param>
        /// <returns></returns>
        public Dropdown RegisterMenuPageDropdown(string group, string address, string text, EventTriggerListener.VoidDelegate callback, int offist = 20)
        {
            uiGroup g;
            Dropdown rs = null;
            if (IsUIGroupRegistered(group, out g))
            {
                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (p.IsMenuPage)
                    {
                        p.BtnCount = p.BtnCount + 1;
                        GameObject btn = Instantiate(parent.MenuDropDownTemplate);
                        btn.transform.SetParent(p.PageInnern.transform);
                        btn.transform.Find("DropdownText").gameObject.GetComponent<Text>().text = text;
                        GameObject sl = btn.transform.Find("DropdownBase").gameObject;
                        Dropdown dropdown = sl.GetComponent<Dropdown>();
                        rs = dropdown;
                        RectTransform pi = p.PageInnern.GetComponent<RectTransform>();
                        RectTransform rgg = btn.GetComponent<RectTransform>();
                        p.Height += (rgg.sizeDelta.y + offist);
                        pi.sizeDelta = new Vector2(pi.sizeDelta.x, p.Height);
                        rgg.anchoredPosition = new Vector2(0, -p.Height + rgg.sizeDelta.y / 2);
                        if (callback != null) EventTriggerListener.Get(sl).onClick = callback;
                    }
                }
            }
            return rs;
        }

        /// <summary>
        /// 跳转到指定 UI 页
        /// </summary>
        /// <param name="group">指定组</param>
        /// <param name="address">指定地址</param>
        public void GoMenuPage(string group, string address)
        {
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (!g.UI.activeSelf)
                    g.UI.SetActive(true);
                if (parent.showedPage != null)
                    parent.showedPage.SetActive(false);

                uiPage p;
                if (IsMenuPageRegistered(group, address, out p))
                {
                    if (!p.Page.activeSelf)
                        p.Page.SetActive(true);
                    parent.showedPage = p.Page;
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
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (parent.showedPage != null)
                    parent.showedPage.SetActive(false);
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
            uiGroup g;
            if (IsUIGroupRegistered(group, out g))
            {
                if (parent.lastPageAddress != "")
                {
                    if (parent.showedPage != null)
                        parent.showedPage.SetActive(false);

                    if (parent.lastPageAddress.Contains("."))
                    {
                        parent.lastPageAddress = parent.lastPageAddress.Substring(0, parent.lastPageAddress.IndexOf('.'));
                        GoMenuPage(group, parent.lastPageAddress);
                    }
                }
            }
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

    private GameObject showedPage;
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
            if (showedPage.activeSelf) showedPage.SetActive(false);
    }
    /// <summary>
    /// 强制显示 UI
    /// </summary>
    public void ReshowUI()
    {
        if (showedPage != null)
            if (!showedPage.activeSelf) showedPage.SetActive(true);
    }
    /// <summary>
    /// 屏幕不黑
    /// </summary>
    public static void SetStartNoBlack()
    {
        isBlack = false;
    }
    /// <summary>
    /// 屏幕变黑
    /// </summary>
    public static void SetStartBlack()
    {
        isBlack = true;
    }

    private void Start()
    {
        UIMaker = new StandardUIMaker(this);
        GlobalMediator.SetUIManager(this);
        if(GlobalSettings.StartInIntro) dbgCam.SetActive(false);
        //test();
        if(isBlack)
            UIFadeHlper.Alpha = 1;
        else UIFadeHlper.Alpha = 0;
    }
    private void Update()
    {
    }
    private void OnDestroy()
    {
        UIMaker = null;
        GlobalMediator.SetUIManagerUnavailable(this);

    }
}
