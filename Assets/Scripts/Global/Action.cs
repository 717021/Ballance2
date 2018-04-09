using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 代码说明：动作action处理器方法
 * 
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// Action 处理器基类
    /// </summary>
    public class ActionHandler
    {
        public ActionHandler(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        public delegate bool ActionHandlerDelegate(params object[] datas);

        public Dictionary<string, ActionHandlerDelegate> handlers = new Dictionary<string, ActionHandlerDelegate>();

        /// <summary>
        /// 清除 Action 处理器的所有处理方法。
        /// </summary>
        public void ClearAllHandler()
        {
            handlers.Clear();
        }
        /// <summary>
        /// 添加一个指定名字的处理方法到此 Action 处理器。
        /// </summary>
        /// <param name="name">指定处理方法的名字</param>
        /// <param name="d">处理方法，函数格式为 bool functionName(params object[] datas) </param>
        public void AddHandler(string name, ActionHandlerDelegate d)
        {
            handlers.Add(name, d);
        }
        /// <summary>
        /// 清除 Action 处理器的指定名字的处理方法。
        /// </summary>
        /// <param name="name">指定处理方法的名字</param>
        public void RemoveHandler(string name)
        {
            handlers.Remove(name);
        }
    }

    /// <summary>
    /// Action 基类
    /// </summary>
    public class Action
    {
        private static ActionHandler staticActionHandler = null;

        public object[] pararms
        {
            get;
            set;
        }

        public Action()
        {
        }
        public Action(params object[] pararms)
        {
            this.pararms = pararms;
        }

        /// <summary>
        /// 获取Action名字
        /// </summary>
        /// <returns></returns>
        public virtual string GetActionName()
        {
            return staticActionHandler.Name;
        }
        /// <summary>
        /// 获取Action静态处理器。此方法在派生的Action中必须被重写，负责无法正常运行。
        /// </summary>
        /// <returns></returns>
        public virtual ActionHandler GetStaticHandler()
        {
            return staticActionHandler;
        }
        /// <summary>
        /// 注册Action静态处理器。此方法在派生的Action中必须被重写，负责无法正常运行。
        /// </summary>
        /// <param name="handler"></param>
        public virtual void Register(ActionHandler handler)
        {
            if(staticActionHandler == null) staticActionHandler = handler;
        }
    }
}
