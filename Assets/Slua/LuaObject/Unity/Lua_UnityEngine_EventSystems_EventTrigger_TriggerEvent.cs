using System;
using SLua;
using System.Collections.Generic;
[UnityEngine.Scripting.Preserve]
public class Lua_UnityEngine_EventSystems_EventTrigger_TriggerEvent : LuaObject {
	[SLua.MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	[UnityEngine.Scripting.Preserve]
	static public int constructor(IntPtr l) {
		try {
			#if DEBUG
			var method = System.Reflection.MethodBase.GetCurrentMethod();
			string methodName = GetMethodName(method);
			UnityEngine.Profiling.Profiler.BeginSample(methodName);
			#endif
			UnityEngine.EventSystems.EventTrigger.TriggerEvent o;
			o=new UnityEngine.EventSystems.EventTrigger.TriggerEvent();
			pushValue(l,true);
			pushValue(l,o);
			return 2;
		}
		catch(Exception e) {
			return error(l,e);
		}
		#if DEBUG
		finally {
			UnityEngine.Profiling.Profiler.EndSample();
		}
		#endif
	}
	[UnityEngine.Scripting.Preserve]
	static public void reg(IntPtr l) {
		LuaUnityEvent_UnityEngine_EventSystems_BaseEventData.reg(l);
		getTypeTable(l,"UnityEngine.EventSystems.EventTrigger.TriggerEvent");
		createTypeMetatable(l,constructor, typeof(UnityEngine.EventSystems.EventTrigger.TriggerEvent),typeof(LuaUnityEvent_UnityEngine_EventSystems_BaseEventData));
	}
}
