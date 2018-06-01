
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Global.GlobalUI.UIElements
{
    /// <summary>
    /// UI 按钮
    /// </summary>
    public class UIButton : UIElement
    {
        /// <summary>
        /// 事件回调
        /// </summary>
        public delegate void VoidDelegate();

        public event VoidDelegate onClick;

        public void OnClick()
        {
            if (onClick != null) onClick.Invoke();
        }

        public override void Oninit()
        {
            base.Oninit();
            text = transform.GetChild(0).gameObject.GetComponent<Text>();
        }
        public override void StartSet(string initstr)
        {
            base.StartSet(initstr);
            if(initstr.Contains("@"))
            {
                string[] s = initstr.Split('@');
                if (s.Length >= 2)
                    Text = s[0];
                else
                {
                    RectTransform r = GetComponent<RectTransform>();
                    r.sizeDelta = new Vector2(float.Parse(s[1]), r.sizeDelta.y);
                }
            }
            else Text = initstr;
        }

        Text text;

        /// <summary>
        /// 获取或设置按钮的文字
        /// </summary>
        public string Text
        {
            get { return text.text; }
            set
            {
                if(text!=null)
                text.text = value;
            }
        }
    }
}
