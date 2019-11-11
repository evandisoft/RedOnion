using System.Collections.Generic;
using System.Linq;
using Kerbalui.Types;
using UnityEngine;
using Kerbalui.Controls.Abstract;
using Kerbalui.Layout.Abstract;

namespace Kerbalui.Layout
{
	/// <summary>
	/// Accepts Elements with associated weights. A weight of 0 will reserve, for that element, space
	/// equal to its content size, but only works for elements that are ContentControls.
	/// 
	/// The space not taken up by weight 0 content controls is divided up among the rest of the elements based on their
	/// weights relative to the total weight sum of all elements.
	/// </summary>
	public class HorizontalSpacer:Spacer
	{
		protected override void SetChildRects()
		{
			var spacingPoints=CalculateSpacingPoints(MinContentWidth,rect.width);

			for (int i = 0; i<spacerEntries.Count; i++)
			{
				spacerEntries[i].element.SetRect(new Rect(spacingPoints[i],0, spacingPoints[i+1]-spacingPoints[i], rect.height));
			}
		}

		public override Vector2 MinSize
		{
			get
			{
				Vector2 minSize=new Vector2();
				foreach (var spacerEntry in spacerEntries)
				{
					if (spacerEntry.element is ContentControl contentControl)
					{
						Vector2 contentMinSize=contentControl.MinSize;
						minSize.y=Mathf.Max(minSize.y, contentMinSize.y);
						minSize.x+=contentMinSize.x;
					}
				}
				return minSize;
			}
		}
	}
}
