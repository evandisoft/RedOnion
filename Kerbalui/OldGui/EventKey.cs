using System;
using UnityEngine;

namespace Kerbalui.Obsolete {
	public struct EventKey {
		public KeyCode keyCode;
		public bool control;
		public bool shift;
		public bool alt;

		public EventKey(KeyCode keyCode,bool control=false,bool shift=false,bool alt=false)
		{
			this.keyCode = keyCode;
			this.control = control;
			this.shift = shift;
			this.alt = alt;	
		}
	}
}
