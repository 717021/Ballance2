

using Assets.Scripts.Global.GlobalUI.LayoutUI;
using UnityEngine;

namespace Assets.Scripts.Global.GlobalUI
{
    public class UIBallanceClassicalMenu : UIVerticalLayoutMenu
    {
        public override void Oninit()
        {
            base.Oninit();
            innern = transform.GetChild(0).gameObject;
            transformHost = innern.transform;
            rectTransformInnern = innern.GetComponent<RectTransform>();
        }

        private RectTransform rectTransformInnern;
        private GameObject innern;

        public override float DoLayout()
        {
            float h= base.DoLayout(); 
            if(h>0)
            {
                if (rectTransformInnern == null)
                {
                    innern = transform.GetChild(0).gameObject;
                    transformHost = innern.transform;
                    rectTransformInnern = innern.GetComponent<RectTransform>();
                }
                if (h != rectTransformInnern.sizeDelta.y) rectTransformInnern = innern.GetComponent<RectTransform>();
                rectTransformInnern.sizeDelta = new Vector2(rectTransformInnern.sizeDelta.x, h);
            }
            return h;
        }
    }
}
