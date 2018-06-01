using UnityEngine;
using SLua;

namespace Assets.Scripts.Worker
{
    public class LUAScriptCom : MonoBehaviour
    {
        public void SetRunScript(LuaState luaState, string startlua)
        {
            this.luaState = luaState;
            luaStartFile = startlua;
        }

        protected LuaState luaState;
        protected string luaStartFile = "";

        private void Start()
        {
            if (!string.IsNullOrEmpty(luaStartFile))
            {
                if (luaStartFile.Contains(":"))
                {
                    string[] s = luaStartFile.Split(':');
                    luaState.doFile(s[0]);
                    if (s.Length >= 2)
                    {
                        LuaFunction startFun = luaState.getFunction(s[2]);
                        startFun.call();
                    }
                }
                else luaState.doFile(luaStartFile);
            }
        }
    }
}
