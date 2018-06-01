using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Assets.Scripts.Global.GlobalUI.UIElements
{
    public class UIToggle : UIElement
    {
        /// <summary>
        /// 事件回调
        /// </summary>
        public delegate void VoidDelegate(bool b);

        public event VoidDelegate onCheckChanged;

        public void OnClick()
        {
            isChecked = !isChecked;
        }

        public override void StartSet(string initstr)
        {
            base.StartSet(initstr);
            string[] s = initstr.Split('@');
            if (s.Length >= 1)
                Text = s[0];
            if (s.Length >= 2)
            {
                if (s[1] == "1" || s[1] == "true" || s[1] == "True")
                {
                    isChecked = true;
                }
            }
        }

        Button buttonChecked, buttonUnCheck;
        Text text;

        public override void Oninit()
        {
            base.Oninit();
            text = transform.GetChild(0).gameObject.GetComponent<Text>();
            buttonChecked = transform.GetChild(2).gameObject.GetComponent<Button>();
            buttonUnCheck = transform.GetChild(1).gameObject.GetComponent<Button>();
        }
        public bool isChecked
        {
            get { return buttonChecked.gameObject.activeSelf; }
            set
            {
                if(value)
                {
                    buttonChecked.gameObject.SetActive(true);
                    buttonUnCheck.gameObject.SetActive(false);
                }
                else
                {
                    buttonChecked.gameObject.SetActive(false);
                    buttonUnCheck.gameObject.SetActive(true);
                }
                if (onCheckChanged != null) onCheckChanged.Invoke(value);
            }
        }
        /// <summary>
        /// 获取或设置按钮的文字
        /// </summary>
        public string Text
        {
            get { return text.text; }
            set
            {
                text.text = value;
            }
        }
    }
}
