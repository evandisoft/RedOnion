using System;
using System.Collections.Generic;
using RedOnion.ROS;

namespace RedOnion.KSP.API
{
	public class InteropDescriptor : Descriptor
	{
		public InteropDescriptor()
			: base() { }
		protected InteropDescriptor(Type type)
			: base(type) { }
		protected InteropDescriptor(string name)
			: base(name) { }
		protected InteropDescriptor(string name, Type type)
			: base(name, type) { }

		public override int Find(object self, string name, bool add)
			=> ((IType)self).Members.Find(name);
		public override string NameOf(object self, int at)
			=> ((IType)self).Members[at].Name;
		public override bool Get(ref Value self, int at)
		{
			var member = ((IType)self.obj).Members[at];
			if (!member.CanRead)
				return false;
			self = member.RosGet(self.obj);
			return true;
		}
		public override bool Set(ref Value self, int at, OpCode op, ref Value value)
		{
			if (op != OpCode.Assign)
				return false;
			var member = ((IType)self.obj).Members[at];
			if (!member.CanWrite)
				return false;
			member.RosSet(self.obj, value);
			return true;
		}
		public override IEnumerable<string> EnumerateProperties(object self)
		{
			foreach (var member in ((IType)self).Members)
				yield return member.Name;
		}
	}
}
