using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class Icon : Element
	{
#if DEBUG
		protected UUI.RawImage Core { get; private set; }
#else
		protected UUI.RawImage Core { get; private set; }
#endif

		public Icon(string name = null)
			: base(name)
		{
			Core = GameObject.AddComponent<UUI.RawImage>();
		}

		public Texture Texture
		{
			get => Core.texture;
			set
			{
				Core.texture = value;
				Core.SetNativeSize();
			}
		}
	}
}
