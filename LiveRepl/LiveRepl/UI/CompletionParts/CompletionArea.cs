using System;
using Kerbalui.Controls;
using UnityEngine;

namespace LiveRepl.UI.CompletionParts
{
	public class CompletionArea:TextArea
	{
		public CompletionGroup completionGroup;

		public CompletionArea(CompletionGroup completionGroup)
		{
			this.completionGroup=completionGroup;
		}

		protected override void ControlUpdate()
		{
			base.ControlUpdate();
		}
	}
}
