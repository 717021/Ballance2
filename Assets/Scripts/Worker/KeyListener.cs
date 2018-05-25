using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 键 事件侦听器
/// </summary>
public class KeyListener
{
    /// <summary>
    /// 事件回调
    /// </summary>
    /// <param name="downed">是否按下</param>
    public delegate void VoidDelegate(bool downed);

    private class KeyListenerItem
    {
        public KeyCode key;
        public bool downed = false;
        public KeyListener.VoidDelegate callBack;
    }

    private List<KeyListenerItem> items = new List<KeyListenerItem>();

    /// <summary>
    /// 添加侦听器侦听键。
    /// </summary>
    /// <param name="key">键值。</param>
    /// <param name="callBack">回调函数。</param>
    public void AddKeyListen(KeyCode key, KeyListener.VoidDelegate callBack)
    {
        KeyListener.KeyListenerItem item = new KeyListener.KeyListenerItem();
        item.callBack = callBack;
        item.key = key;
        items.Add(item);
    }
    /// <summary>
    /// 清空事件侦听器所有侦听键。
    /// </summary>
    public void ClearKeyListen()
    {
        items.Clear();
    }
    /// <summary>
    /// 侦听器侦听，请在Update中调用。
    /// </summary>
    public void ListenKey()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (Input.GetKeyDown(items[i].key) && !items[i].downed)
            {
                items[i].downed = true;
                items[i].callBack(true);
            }
            else if (Input.GetKeyUp(items[i].key) && items[i].downed)
            {
                items[i].callBack(false);
                items[i].downed = false;
            }
        }
    }
}
