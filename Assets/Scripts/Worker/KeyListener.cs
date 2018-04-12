using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener
{
    public delegate void VoidDelegate(bool downed);

    private class KeyListenerItem
    {
        public KeyCode key;
        public bool downed = false;
        public KeyListener.VoidDelegate callBack;
    }

    private List<KeyListener.KeyListenerItem> items = new List<KeyListener.KeyListenerItem>();

    public void AddKeyListen(KeyCode key, KeyListener.VoidDelegate callBack)
    {
        KeyListener.KeyListenerItem item = new KeyListener.KeyListenerItem();
        item.callBack = callBack;
        item.key = key;
        items.Add(item);
    }

    public void ClearKeyListen()
    {
        items.Clear();
    }

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
