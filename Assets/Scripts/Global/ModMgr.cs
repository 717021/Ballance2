using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModMgr : MonoBehaviour
{
    public static MenuLevel menuLevel;

    public void Back()
    {
        GlobalMediator.UIManager.UIMaker.ShowDialogThreeChoice(105, "返回主菜单？", "您所做的更改是否保存？", "是", "否", "取消");
    }

    private EventLinster onDialolgClosedLinister;

    void Start()
    {
        onDialolgClosedLinister = GlobalMediator.RegisterEventLinster(new OnDialolgClosedLinister(UIDialogOK));
    }
    void Update()
    {

    }    
    //dialog 返回事件
    private void UIDialogOK(int dlgid, bool clickedok, bool clicked3)
    {
        if (dlgid == 105)
        {
            if (!clicked3)
            {
                if (clickedok)
                {
                    menuLevel.ModMgrBack();
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("ModManager");
                }
                else
                {
                    menuLevel.ModMgrBack();
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("ModManager");
                }
            }
        }
    }
}
