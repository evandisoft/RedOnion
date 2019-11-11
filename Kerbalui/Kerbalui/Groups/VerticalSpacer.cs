using System.Collections.Generic;
using System.Linq;
using Kerbalui.Types;
using UnityEngine;
using Kerbalui.Controls.Abstract;
using Kerbalui.Groups.Abstract;

namespace Kerbalui.Groups
{
	/// <summary>
	/// Accepts Elements with associated weights. A weight of 0 will reserve, for that element, space
	/// equal to its content size, but only works for elements that are ContentControls.
	/// 
	/// The space not taken up by weight 0 content controls is divided up among the rest of the elements based on their
	/// weights relative to the total weight sum of all elements.
	/// </summary>
	public class VerticalSpacer:Spacer
	{
		protected override void SetChildRects()
		{
			var spacingPoints=CalculateSpacingPoints(MinContentHeight,rect.height);

			for(int i = 0; i<spacerEntries.Count; i++)
			{
				spacerEntries[i].element.SetRect(new Rect(0, spacingPoints[i], rect.width, spacingPoints[i+1]-spacingPoints[i]));
			}
		}
	}
}
