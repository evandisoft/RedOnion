using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using RedOnion.KSP.API;
using RedOnion.ROS;

namespace RedOnion.KSP.Parts
{
	[Description("List of parts attached to parent part.")]
	public class PartChildren : IList<PartBase>
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

		public PartBase this[int index] => parent.ship.parts[parent.native.children[index]];

		int ICollection<PartBase>.Count => count;
		bool ICollection<PartBase>.IsReadOnly => true;

		PartBase IList<PartBase>.this[int index]
		{
			get => this[index];
			set => throw new InvalidOperationException();
		}

		bool ICollection<PartBase>.Contains(PartBase item)
			=> parent.native.children.Contains(item.native);
		void ICollection<PartBase>.CopyTo(PartBase[] array, int arrayIndex)
		{
			foreach (var part in this)
				array[arrayIndex++] = part;
		}
		int IList<PartBase>.IndexOf(PartBase item)
			=> parent.native.children.IndexOf(item.native);

		void ICollection<PartBase>.Add(PartBase item) => throw new InvalidOperationException();
		void ICollection<PartBase>.Clear() => throw new InvalidOperationException();
		bool ICollection<PartBase>.Remove(PartBase item) => throw new InvalidOperationException();
		void IList<PartBase>.Insert(int index, PartBase item) => throw new InvalidOperationException();
		void IList<PartBase>.RemoveAt(int index) => throw new InvalidOperationException();
	}
}
