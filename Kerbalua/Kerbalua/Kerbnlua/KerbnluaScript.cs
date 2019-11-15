using System;
using NLua;

namespace Kerbalua.Kerbnlua
{
	public class KerbnluaScript
	{
		public Lua state = new Lua();

		public KerbnluaScript()
		{
			state.State.SetHook(AutoyieldHook, KeraLua.LuaHookMask.Count, 1000);

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


			//Console.WriteLine("top is "+state.State.GetTop());
			//int prevtop = state.State.GetTop();
			//var what = state.Pop();
			//Console.WriteLine("what is "+what.GetType());
			//state.Push(what);
			//Console.WriteLine(state.State)
			KeraLua.LuaStatus status=state.State.Resume(null, 0);
			//Console.WriteLine("top is " + state.State.GetTop());
			//Console.WriteLine("Status: "+status);
			//state.State.Pop(2);

			retvals = new object[state.State.GetTop()];
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
			state.State.LoadString(source);
		}

		public void Terminate()
		{
			state.State.DoString("return 1");
		}
	}
}
