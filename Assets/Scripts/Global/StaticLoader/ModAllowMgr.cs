using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Global.StaticLoader
{
    internal static class ModAllowMgr
    {
        public static bool IsGameCore(string pkg)
        {
            return false;
        }
        public static bool IsAllowLoad(string pkg)
        {
            return false;
        }
    }
}
