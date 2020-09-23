using RedOnion.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RedOnion.KSP.Parts
{
	[Unsafe, Description("List of modules of a part.")]
	public class PartModules : IList<PartModule>, IReadOnlyList<PartModule>
	{
		[Description("The part this list belongs to.")]
		public PartBase part { get; }
		protected PartModuleList native { get; }

		[Description("Number of modules.")]
		public int count => native.Count;

		internal PartModules(PartBase part)
			=> native = (this.part = part).native.Modules;

		IEnumerator IEnumerable.GetEnumerator() => native.GetEnumerator();
		public IEnumerator<PartModule> GetEnumerator() => native.GetEnumerator();

		[Description("Get module at specified index.")]
		public PartModule this[int index] => native[index];
		[Description("Get module by persistent ID.")]
		public PartModule this[uint persistentId] => native[persistentId];
		[Description("Get module by class name (first or null).")]
		public PartModule this[string className] => native[className];
		[Description("Get module of specified type (first or null, includes sub-classes).")]
		public PartModule this[Type type]
		{
			get
			{
				foreach (var module in native)
				{
					if (type.IsAssignableFrom(module.GetType()))
						return module;
				}
				return null;
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("[");
			for (int i = 0; i < native.Count; i++)
			{
				if (i > 0)
					sb.Append(", ");
				sb.Append(native[i].ClassName);
			}
			sb.Append("]");
			return sb.ToString();
		}

		int IReadOnlyCollection<PartModule>.Count => count;
		int ICollection<PartModule>.Count => count;
		bool ICollection<PartModule>.IsReadOnly => true;

		PartModule IList<PartModule>.this[int index]
		{
			get => this[index];
			set => throw new InvalidOperationException();
		}

		bool ICollection<PartModule>.Contains(PartModule item)
		{
			foreach (var module in native)
				if (item == module)
					return true;
			return false;
		}
		void ICollection<PartModule>.CopyTo(PartModule[] array, int arrayIndex)
		{
			foreach (var module in this)
				array[arrayIndex++] = module;
		}
		int IList<PartModule>.IndexOf(PartModule item)
			=> native.IndexOf(item);

		void ICollection<PartModule>.Add(PartModule item) => throw new InvalidOperationException();
		void ICollection<PartModule>.Clear() => throw new InvalidOperationException();
		bool ICollection<PartModule>.Remove(PartModule item) => throw new InvalidOperationException();
		void IList<PartModule>.Insert(int index, PartModule item) => throw new InvalidOperationException();
		void IList<PartModule>.RemoveAt(int index) => throw new InvalidOperationException();
	}
}
