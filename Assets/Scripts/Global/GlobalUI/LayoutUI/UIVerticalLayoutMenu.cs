
using UnityEngine;

namespace Assets.Scripts.Global.GlobalUI.LayoutUI
{
    public class UIVerticalLayoutMenu : UIAutoLayoutMenu
    {
        private int lastStartLayoutPos = 0;
        private bool layoutLocked = false;

        protected Transform transformHost = null;


        public override void Oninit()
        {
            base.Oninit();
            if (transformHost == null)
                transformHost = transform;
        }

        public override float DoLayout()
        {
            base.DoLayout();
            if (lastStartLayoutPos >= childEles.Count)
                lastStartLayoutPos = childEles.Count - 1;
            if (!layoutLocked)
            {
                float hstart = 0;
                if (lastStartLayoutPos != 0)
                    hstart = childEles[lastStartLayoutPos].RectTransform.anchoredPosition.y - childEles[lastStartLayoutPos].RectTransform.sizeDelta.y / 2;
                for (int i = lastStartLayoutPos; i < childEles.Count; i++)
                {
                    hstart -= childEles[i].SpaceStart;
                    if (childEles[i].RectTransform != null)
                    {
                        childEles[i].RectTransform.anchoredPosition = new Vector2(0, hstart - childEles[i].RectTransform.sizeDelta.y / 2);
                        hstart -= childEles[i].RectTransform.sizeDelta.y;
                    }
                }

                return -hstart;
            }
            return -1;
        }
        public override void UpdateMenu(int start = 0)
        {
            if (start >= 0)
                lastStartLayoutPos = start;
            base.UpdateMenu(start);
            DoLayout();
        }
        public override void LockLayout()
        {
            layoutLocked = true;
            base.LockLayout();
        }
        public override void UnLockLayout()
        {
            layoutLocked = false;
            base.UnLockLayout();
        }

        public override void AddEle(UIElement ele)
        {
            ele.gameObject.transform.SetParent(transformHost);
            ele.gameObject.transform.position = Vector3.zero;
            base.AddEle(ele);
        }
        public override void InsertEle(int index, UIElement ele)
        {
            ele.gameObject.transform.SetParent(transformHost);
            ele.gameObject.transform.position = Vector3.zero;
            base.InsertEle(index, ele);
        }
        public override void RemoveEle(UIElement ele)
        {
            ele.gameObject.transform.SetParent(null);
            base.RemoveEle(ele);
        }
    }
}
