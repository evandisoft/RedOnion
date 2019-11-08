using RedOnion.UI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Panel : Simple, IEnumerable<Element>
	{
		public Panel() : base() { }
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
		public Element this[int index]
			=> children != null ? children[index] : throw new IndexOutOfRangeException();
		public int Count => children?.Count ?? 0;
		public int IndexOf(Element item) => children != null ? children.IndexOf(item) : -1;
		public bool Contains(Element item) => children != null && children.Contains(item);
	}
}
