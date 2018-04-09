using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * 代码说明：游戏动态模块（mod）加载器。
 *                    游戏加载器（GameInit）。
 *                    
 ********************************************
*/


public static class GlobalModLoader
{    
    /// <summary>
    /// 初始化
    /// </summary>
    public static void GlobalModLoaderInitialization()
    {
    }




    #region GameInit


    #endregion

    /// <summary>
    /// 退出游戏之前的清理。【不可调用】
    /// </summary>
    public static void ExitGameClear()
    {
        if (GlobalMediator.GameExiting)
        { 

        }
    }
}