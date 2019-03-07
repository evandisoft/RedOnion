using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Icon : Element
	{
		protected UUI.RawImage Core { get; private set; }

		public Icon(Element parent = null, string name = null)
			: base(parent, name)
		{
			Core = GameObject.AddComponent<UUI.RawImage>();
		}
	}
}
