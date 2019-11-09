using System;
using UnityEngine;

namespace LiveRepl.UI.Elements
{
	public abstract class TextElement:Element
	{
		public GUIContent content=new GUIContent("");
		public GUIStyle style;

		public override void Update()
		{
			throw new NotImplementedException();
		}
	}
}
