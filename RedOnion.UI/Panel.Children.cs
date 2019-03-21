using System;
using System.Collections;
using System.Collections.Generic;

namespace RedOnion.UI
{
	partial class Panel : IEnumerable<Element>
	{
		protected List<Element> children;
		protected struct ChildrenList : IList<Element>
		{
			public Panel Panel { get; }
			private List<Element> Children
			{
				get => Panel.children;
				set => Panel.children = value;
			}
			public ChildrenList(Panel panel) => Panel = panel;

			public bool IsReadOnly => true;
			public int Count => Children?.Count ?? 0;

			public Element this[int index]
			{
				get => Children != null ? Children[index] : throw new IndexOutOfRangeException();
				set => throw new System.NotImplementedException();
			}

			public IEnumerator<Element> GetEnumerator() => Panel.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => Panel.GetEnumerator();
			public int IndexOf(Element item) => Children != null ? Children.IndexOf(item) : -1;
			public bool Contains(Element item) => Children != null && Children.Contains(item);
			public void CopyTo(Element[] array, int arrayIndex)
			{
				if (Children != null)
					Children.CopyTo(array, arrayIndex);
			}

			public void Add(Element item) => throw new NotImplementedException();
			public void Clear() => throw new NotImplementedException();
			public void Insert(int index, Element item) => throw new NotImplementedException();
			public bool Remove(Element item) => throw new NotImplementedException();
			public void RemoveAt(int index) => throw new NotImplementedException();
		}
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

		public new E Add<E>(E element) where E : Element
			=> base.Add(element);
		public new E Remove<E>(E element) where E : Element
			=> base.Remove(element);
		public new Element Add(Element element)
			=> base.Add(element);
		public new Element Remove(Element element)
			=> base.Remove(element);
		public new void Add(params Element[] elements)
			=> base.Add(elements);
		public new void Remove(params Element[] elements)
			=> base.Remove(elements);


	}
}
