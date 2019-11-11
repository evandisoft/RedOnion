using System;
using Kerbalui.Controls;
using Kerbalui.Decorators;
using UnityEngine;

namespace LiveRepl.UI.CompletionParts
{
	public class CompletionArea:EditingArea
	{
		public CompletionGroup completionGroup;

		public CompletionArea(CompletionGroup completionGroup) : base(new TextArea())
		{
			this.completionGroup=completionGroup;
		}

		protected override void DecoratorUpdate()
		{
			base.DecoratorUpdate();
		}
	}
}
