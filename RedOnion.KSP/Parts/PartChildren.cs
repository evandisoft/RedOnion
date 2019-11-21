using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.KSP.API;
using RedOnion.KSP.Attributes;
using RedOnion.ROS;

namespace RedOnion.KSP.Parts
{
	[Description("List of parts attached to parent part.")]
	public class PartChildren : ICollection<PartBase>
	{
		[Description("The parent part owning this list.")]
		public PartBase parent { get; }

		[Description("Number of attached parts.")]
		public int count => parent.native.children.Count;

		internal PartChildren(PartBase parent)
			=> this.parent = parent;

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<PartBase> GetEnumerator()
		{
			var parts = parent.ship.parts;
			foreach (var part in parent.native.children)
				yield return parts[part];
		}

		int ICollection<PartBase>.Count => count;
		bool ICollection<PartBase>.IsReadOnly => true;

		bool ICollection<PartBase>.Contains(PartBase item)
			=> parent.native.children.Contains(item.native);
		void ICollection<PartBase>.CopyTo(PartBase[] array, int arrayIndex)
		{
			foreach (var part in this)
				array[arrayIndex++] = part;
		}

		void ICollection<PartBase>.Add(PartBase item) => throw new NotImplementedException();
		void ICollection<PartBase>.Clear() => throw new NotImplementedException();
		bool ICollection<PartBase>.Remove(PartBase item) => throw new NotImplementedException();
	}
}
