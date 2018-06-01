using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Global.GlobalUI.UIElements
{
    public class UIImage : UIElement
    {
        public override void Oninit()
        {
            base.Oninit();
            image = gameObject.GetComponent<Image>();
        }
        private Image image;

        /// <summary>
        /// 
        /// </summary>
        public Image Image { get { return image; } }

        /// <summary>
        /// 获取或设置按钮的文字
        /// </summary>
        public Sprite Sprite
        {
            get { return image.sprite; }
            set
            {
                image.sprite = value;
            }
        }
    }
}
