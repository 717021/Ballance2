

using UnityEngine.UI;

namespace Assets.Scripts.Global.GlobalUI.UIElements
{
    public class UISlider : UIElement
    {

        public override void Oninit()
        {
            base.Oninit();
            text = transform.GetChild(1).gameObject.GetComponent<Text>();
            slider = transform.GetChild(0).gameObject.GetComponent<Slider>();
        }
        Text text;
        Slider slider;

        public Slider Slider { get { return slider; } }
        public float value { get { return slider.value; } set { slider.value = value; } }

        public override void StartSet(string initstr)
        {
            base.StartSet(initstr);
            string[] s = initstr.Split('@');
            if (s.Length >= 1)
                Text = s[0];
            if (s.Length >= 2)
                slider.value = float.Parse(s[1]);
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
