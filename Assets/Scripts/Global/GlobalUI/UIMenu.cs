using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Global.GlobalUI
{
    /// <summary>
    /// ui管理 菜单
    /// </summary>
    public class UIMenu : UIPage
    {
        /// <summary>
        /// 子 集合
        /// </summary>
        protected List<UIElement> childEles = new List<UIElement>();

        /// <summary>
        /// 获取 子
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns></returns>
        public virtual UIElement GetEle(string name)
        {
            foreach(UIElement e in childEles)
            {
                if(e.Name== name)
                {
                    return e;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取 子
        /// </summary>
        /// <param name="index">位置</param>
        /// <returns></returns>
        public virtual UIElement GetEle(int index)
        {
            if (index >= 0 && index < childEles.Count)
                return childEles[index];
            return null;
        }
        /// <summary>
        /// 插入 子
        /// </summary>
        /// <param name="index">位置</param>
        /// <param name="ele">子</param>
        public virtual void InsertEle(int index, UIElement ele)
        {
            childEles.Insert(index, ele);
            UpdateMenu(index);
        }
        /// <summary>
        /// 添加 子
        /// </summary>
        /// <param name="ele">子</param>
        public virtual void AddEle(UIElement ele)
        {
            childEles.Add(ele);
            UpdateMenu(childEles.Count - 1);
        }
        /// <summary>
        /// 删除 子
        /// </summary>
        /// <param name="ele">子</param>
        public virtual void RemoveEle(UIElement ele)
        {
            int id = childEles.IndexOf(ele);
            childEles.Remove(ele);
            UpdateMenu(id);
        }
        /// <summary>
        /// 刷新菜单
        /// </summary>
        /// <param name="start">开始位置</param>
        public virtual void UpdateMenu(int start = 0)
        {

        }

        public virtual void Oninit() { }

        private void Start()
        {
            Oninit();
        }

        private void OnDestroy()
        {
            childEles.Clear();
            childEles = null;
        }
    }
}
