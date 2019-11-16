using System;
using NLua;
using UnityEngine;

namespace Kerbalua.Kerbnlua
{
	public class KerbnluaScript
	{
		public Lua state = new Lua();
		public KeraLua.Lua runningThread;

		public KerbnluaScript()
		{
			state.LoadCLRPackage();
		}

		private void AutoyieldHook(IntPtr luaState, IntPtr ar)
		{
			var keraluaState=KeraLua.Lua.FromIntPtr(luaState);

			if (keraluaState.IsYieldable)
			{
				keraluaState.Yield(0);
			}
		}

		public KeraLua.LuaStatus Evaluate(out object[] retvals)
		{
			//Debug.Log("top is "+state.State.GetTop());
			//int prevtop = state.State.GetTop();
			//var what = state.Pop();
			//Debug.Log("what is "+what.GetType());
			//state.Push(what);
			//Debug.Log(state.State);
			//Debug.Log("State size is "+state.State.GetTop());
			KeraLua.LuaStatus status=runningThread.Resume(null, 0);
			//Debug.Log("State size is "+state.State.GetTop());
			//Debug.Log("top is " + state.State.GetTop());
			//Debug.Log("Status: "+status);
			//state.State.Pop(2);
			var nretvals=runningThread.GetTop();
			runningThread.XMove(state.State, nretvals);

			retvals = new object[nretvals];
			for (int i = 0; i < retvals.Length; i++)
			{
				retvals[retvals.Length-i-1] = state.Pop();
			}
			//Console.WriteLine(kstate)
			//kstate.Type
			return status;
		}
		public void SetSource(string source)
		{
			state.State.SetHook(AutoyieldHook, KeraLua.LuaHookMask.Count, 1000);
			runningThread=state.State.NewThread();
			runningThread.LoadString(source);
		}

		public void Terminate()
		{
			state.State.DoString("return 1");
		}
	}
}
