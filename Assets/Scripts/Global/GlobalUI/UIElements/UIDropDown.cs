using UnityEngine.UI;

namespace Assets.Scripts.Global.GlobalUI.UIElements
{
    public class UIDropDown : UIElement
    {
        public override void Oninit()
        {
            text = transform.GetChild(0).gameObject.GetComponent<Text>();
            dropdown = transform.GetChild(1).gameObject.GetComponent<Dropdown>();
            base.Oninit();
        }

        public override void StartSet(string initstr)
        {
            base.StartSet(initstr);
            string[] s = initstr.Split('@');
            if (s.Length >= 1)
                Text = s[0];
            if (s.Length >= 2)
                dropdown.value = int.Parse(s[1]);
        }

        Text text;
        Dropdown dropdown;

        /// <summary>
        /// 获取下拉控件主体
        /// </summary>
        public Dropdown Dropdown { get { return dropdown; } }
        /// <summary>
        /// 获取或设置控件的文字
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
