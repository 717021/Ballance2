using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Global.GlobalUI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIElement : MonoBehaviour
    {
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            Oninit();
        }


        public virtual void Oninit() { }


        private RectTransform rectTransform;

        /// <summary>
        /// 
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                if(rectTransform==null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsShowed()
        {
            return gameObject.activeSelf;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SpaceStart { get; set; }

        public string Name { get; set; }

        public virtual void StartSet(string initstr)
        {

        }
    }
}
