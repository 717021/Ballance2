using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 代码说明：事件侦听器基类
 * 
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// 事件侦听器基类
    /// </summary>
    public class EventLinster
    {
        public EventLinster(string eventName)
        {
            this.eventName = eventName;
        }
        public EventLinster(string eventName, string receiverName)
        {
            this.eventName = eventName;
            this.receiverName = receiverName;
        }

        public string receiverName { get; set; }
        public string eventName { get; set; }

        public virtual void OnEvent(object sender, params object[] par)
        {

        }
    }
}
