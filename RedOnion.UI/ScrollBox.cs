using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public class ScrollBox : Simple
	{
		public ScrollBox(string name = null)
			: base(name)
		{
			Layout = Layout.Horizontal;
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing || GameObject == null)
				return;
			image = null;
			base.Dispose(disposing);
		}
	}
}
