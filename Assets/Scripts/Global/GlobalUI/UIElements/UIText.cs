
using UnityEngine.UI;

namespace Assets.Scripts.Global.GlobalUI.UIElements
{
    public class UIText : UIElement
    {
        public override void Oninit()
        {
            base.Oninit();
            text = GetComponent<Text>();
        }

        Text text;

        public override void StartSet(string initstr)
        {
            base.StartSet(initstr);


            Text = initstr;

        }
        /// <summary>
        /// 获取或设置文字
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
