using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Global.GlobalUI
{
    /// <summary>
    /// ui管理 页
    /// </summary>
    public class UIPage : MonoBehaviour
    {
        public string Address { get; set; }
        public string Group { get; set; }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        public virtual bool IsShowed()
        {
            return gameObject.activeSelf;
        }


    }
}
