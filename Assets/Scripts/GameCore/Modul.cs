using Assets.Scripts.Global;
using Assets.Scripts.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 机关基类
 * 
 */

/// <summary>
/// 机关基类
/// </summary>
public class Modul : MonoBehaviour
{
    public GlobalGameModul ModulType { get; set; }

    /// <summary>
    /// 激活事件，机关的 ActiveType 必须设置为 <see cref="GlobalGameModulActiveType.CustomAndSend"/> 时此事件才有效。
    /// </summary>
    public virtual void OnActive()
    {

    }
    /// <summary>
    /// 取消激活事件，机关的 ActiveType 必须设置为 <see cref="GlobalGameModulActiveType.CustomAndSend"/> 时此事件才有效。
    /// </summary>
    public virtual void OnDeactive()
    {

    }
    /// <summary>
    /// 恢复IC事件，机关的 ICResetType 必须设置为 <see cref="ICResetType.SendResetMsg"/> 时此事件才有效。
    /// </summary>
    public virtual void OnResetIC()
    {

    }
    /// <summary>
    /// 恢复IC事件，机关的 ICResetType 必须设置为 <see cref="ICResetType.SendResetMsg"/> 时此事件才有效。
    /// </summary>
    public virtual void OnBackupIC()
    {

    }
    /// <summary>
    /// 小节改变事件，机关的 ActiveType 必须设置为 <see cref="GlobalGameModulActiveType.CustomAndSend"/> 时此事件才有效。
    /// </summary>
    public virtual void OnSectorChanged()
    {

    }
    /// <summary>
    /// 机关加载事件
    /// </summary>
    public virtual void OnLoad()
    {

    }
    /// <summary>
    /// 机关卸载事件
    /// </summary>
    public virtual void OnUnLoad()
    {

    }
}
