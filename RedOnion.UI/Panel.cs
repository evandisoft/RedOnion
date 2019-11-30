using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace RedOnion.UI
{
	[Description(
@"`UI.Panel` is the basis for more complex layout. You will usually nest few panels with alternating
`Layout.Horizontal` and `Layout.Vertical` (do not forget to assign its `Layout` property,
it is set to `Layout.None` when the panel is created).")]
	public class Panel : Simple, IEnumerable<Element>
	{
		[Description("Create new panel with layout set to `Layout.None`.")]
		public Panel() : base() { }
		[Description("Create new panel with specified layout.")]
		public Panel(Layout layout) : base(layout) { }

		protected List<Element> children;
		protected override void AddElement(Element element)
		{
			base.AddElement(element);
			if (children == null)
				children = new List<Element>();
			children.Add(element);
		}
		protected override void RemoveElement(Element element)
		{
			base.RemoveElement(element);
			children.Remove(element);
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<Element> GetEnumerator()
		{
			if (children == null)
				yield break;
			foreach (var e in children)
				yield return e;
		}
		[Description("Get contained element by name. (Direct children only.)")]
		public Element this[string name]
		{
			get
			{
				foreach (var e in this)
					if (e.Name == name)
						return e;
				return null;
			}
		}
		[Description("Get child element by index.")]
		public Element this[int index]
			=> children != null ? children[index] : throw new IndexOutOfRangeException();
		[Description("Get number of child elements.")]
		public int Count => children?.Count ?? 0;
		[Description("Get index of child element (-1 if not found).")]
		public int IndexOf(Element item) => children != null ? children.IndexOf(item) : -1;
		[Description("Test if element is contained in this panel.")]
		public bool Contains(Element item) => children != null && children.Contains(item);
	}
}
