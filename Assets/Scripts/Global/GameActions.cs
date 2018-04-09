using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 代码说明：游戏 Actions 定义
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// 播放声音
    /// </summary>
    public class PlaySoundAction : Action
    {
        private static ActionHandler staticActionHandler = null;

        public PlaySoundAction() { }
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public PlaySoundAction(string pkg, string name, int type) : base(pkg, name, type)
        {

        }

        public override void Register(ActionHandler handler)
        {
            staticActionHandler = handler;
        }
        public override ActionHandler GetStaticHandler()
        {
            return staticActionHandler;
        }
    }
    /// <summary>
    /// 停止声音
    /// </summary>
    public class StopSoundAction : Action
    {
        private static ActionHandler staticActionHandler = null;

        public StopSoundAction() { }
        /// <summary>
        /// 停止声音
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        public StopSoundAction(string pkg, string name) : base(pkg, name)
        {

        }

        public override void Register(ActionHandler handler)
        {
            staticActionHandler = handler;
        }
        public override ActionHandler GetStaticHandler()
        {
            return staticActionHandler;
        }
    }

    /// <summary>
    /// 游戏已经注册的 Actions
    /// </summary>
    public class GameActions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Action PlaySound(string pkg, string name, int type)
        {
            return new PlaySoundAction(pkg, name, type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Action StopSound(string pkg, string name)
        {
            return new StopSoundAction(pkg, name);
        }
    }
}
