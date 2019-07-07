using System;
using System.Collections.Generic;
using RedOnion.ROS;
using RedOnion.KSP.API;
using RedOnion.KSP.API.Namespaces;
using RedOnion.KSP.Autopilot;
using UE = UnityEngine;

namespace RedOnion.KSP
{
	[IgnoreForDocs]
	public class RosGlobals : RedOnion.ROS.Objects.Globals, IType
	{
		protected static MemberList Members => GlobalMembers.MemberList;
		MemberList IType.Members => Members;

		public override void Fill()
		{
			base.Fill();
			System.Add(typeof(UE.Debug));
			System.Add(typeof(UE.Color));
			System.Add(typeof(UE.Rect));
			System.Add(typeof(PID));
			System.Add("PIDloop", typeof(PID));

			System.Add("UI", typeof(UI_Namespace));
			Add(typeof(API_UI.Window));
			Add(typeof(UI.Anchors));
			Add(typeof(UI.Padding));
			Add(typeof(UI.Layout));
			Add(typeof(UI.Panel));
			Add(typeof(UI.Label));
			Add(typeof(UI.Button));
			Add(typeof(UI.TextBox));

			Add("KSP", typeof(KSP_Namespace));
			Add("Unity", typeof(Unity_Namespace));

			/*
			Add("assembly", new Value(new ReflectionUtil.GetMappings()));
			
			sys.Set("Vector2", AddType(typeof(UE.Vector2)));
			var vector =        AddType(typeof(UE.Vector3));
			soft.Set("Vector", vector);
			sys.Set("Vector", vector);
			sys.Set("Vector3", vector);
			sys.Set("Vector4", AddType(typeof(UE.Vector4)));

			var hard = BaseProps; // will resist overwrite and shadowing in global scope
			var soft = MoreProps; // can be easily overwritten or shadowed

			AddType(typeof(VesselType));
			Add("assembly", Value.FromObject(new ReflectionUtil.GetMappings()));
			*/
		}

		const int GlobalMark = 0x7F000000;
		public override int Find(string name)
		{
			int at = Members.Find(name);
			if (at >= 0) return at + GlobalMark;
			return base.Find(name);
		}
		public override bool Get(ref Value self, int at)
		{
			if (at >= GlobalMark)
			{
				var member = Members[at - GlobalMark];
				if (!member.CanRead) return false;
				self = member.RosGet(self.obj);
				return true;
			}
			return base.Get(ref self, at);
		}
		public override bool Set(ref Value self, int at, OpCode op, ref Value value)
		{
			if (at >= GlobalMark)
			{
				var member = Members[at - GlobalMark];
				if (!member.CanWrite) return false;
				if (op != OpCode.Assign) return false;
				member.RosSet(self.obj, value);
				return true;
			}
			return base.Set(ref self, at, op, ref value);
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			var seen = new HashSet<string>();
			foreach (var member in Members)
			{
				seen.Add(member.Name);
				yield return member.Name;
			}
			;
			foreach (var name in EnumerateProperties(self, seen))
				yield return name;
		}
	}
}
