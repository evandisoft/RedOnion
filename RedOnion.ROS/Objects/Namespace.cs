using RedOnion.ROS.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RedOnion.ROS.Objects
{
	//TODO: This may work for classes and constants, but not for writable properties
	//TODO: support generic Value for import (not just UserObjects)
	public class Namespace : UserObject
	{
		protected List<UserObject> imports;
		public void Import(UserObject import)
		{
			if (imports == null)
				imports = new List<UserObject>();
			imports.Add(import);
		}
		public void RemoveImports(int count = int.MaxValue)
		{
			int max = imports.Count;
			if (count > max)
				count = max;
			imports.RemoveRange(count, max-count);
		}
		public Namespace(string name, Type type) : base(name, type) { }

		public override int Find(string name)
		{
			if (dict.TryGetValue(name, out var idx))
				return idx;
			if (parent != null)
			{
				idx = parent.Find(name);
				if (idx >= 0)
					return ImportFrom(parent, name, idx);
			}
			if (imports != null)
			{
				foreach (var space in imports)
				{
					idx = space.Find(name);
					if (idx < 0)
						return idx;
					return ImportFrom(parent, name, idx);
				}
			}
			return -1;
		}
		public override IEnumerable<string> EnumerateProperties(object self)
			=> imports == null ? base.EnumerateProperties(self) : EnumerateProperties(self, null);
		protected IEnumerable<string> EnumerateProperties(object self, HashSet<string> seen)
		{
			if (seen == null)
				seen = new HashSet<string>();
			foreach (var p in prop)
			{
				var name = p.name;
				if (name != null)
				{
					seen.Add(name);
					yield return name;
				}
			}
			if (parent != null)
			{
				foreach (var name in parent.EnumerateProperties(self))
				{
					if (seen.Contains(name))
						continue;
					seen.Add(name);
					yield return name;
				}
			}
			if (imports == null)
				yield break;
			foreach (var space in imports)
			{
				foreach (var name in space.EnumerateProperties(space))
				{
					if (seen.Contains(name))
						continue;
					seen.Add(name);
					yield return name;
				}
			}
		}
	}
}
