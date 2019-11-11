using System.Collections.Generic;
using System.Linq;
using Kerbalui.Types;
using UnityEngine;
using Kerbalui.Controls.Abstract;

namespace Kerbalui.Groups
{
	/// <summary>
	/// Accepts Elements with associated weights. A weight of 0 will reserve, for that element, space
	/// equal to its content size, but only works for elements that are ContentControls.
	/// 
	/// The space not taken up by weight 0 content controls is divided up among the rest of the elements based on their
	/// weights relative to the total weight sum of all elements.
	/// </summary>
	public class HorizontalSpacer:Group
	{
		List<SpacerEntry> spacerEntries=new List<SpacerEntry>();

		public override Vector2 MinSize
		{
			get
			{
				Vector2 minSize=new Vector2();
				foreach(var spacerEntry in spacerEntries)
				{
					if(spacerEntry.element is ContentControl contentControl)
					{
						Vector2 contentMinSize=contentControl.MinSize;
						minSize.y=Mathf.Max(minSize.y, contentMinSize.y);
						minSize.x+=contentMinSize.x;
					}
				}
				return minSize;
			}
		}

		/// <summary>
		/// Adds a weight and associates it with an element to be rendered.
		/// A weight of 0 makes the element be rendered by it's minimum content size.
		/// </summary>
		/// <param name="weight">Weight.</param>
		/// <param name="element">Element.</param>
		public void Add(float weight,Element element)
		{
			spacerEntries.Add(new SpacerEntry(weight, element));
			RegisterForUpdates(element);
			needsRecalculation=true;
		}

		protected override void SetChildRects()
		{
			float totalWeight=0;

			foreach (var spacerEntry in spacerEntries)
			{
				totalWeight+=spacerEntry.weight;
			}

			float totalMinContentWidth=spacerEntries.Select(MinContentWidth).Sum();
			float minfract=totalMinContentWidth/rect.width;

			float totalWeightFract=1-minfract;

			// Multiplying weightMultiplier to spacerEntry.weight, gives us the width for that entry
			// spacerEntryWidth=spacerEntry.weight*weightMultiplier
			// weightMultiplier=1/totalWeight*totalWeightFract*rect.width
			float weightMultiplier=totalWeightFract/totalWeight*rect.width;

			float minContentSize=0;
			float startPoint=0;
			float endPoint=0;
			foreach (var spacerEntry in spacerEntries)
			{
				minContentSize=MinContentWidth(spacerEntry);
				if (minContentSize>0)
				{
					endPoint=startPoint+minContentSize;
				}
				else
				{
					endPoint=startPoint+spacerEntry.weight*weightMultiplier;
				}

				spacerEntry.element.SetRect(new Rect(startPoint, 0, endPoint-startPoint, rect.height));
				startPoint=endPoint;
			}
		}

		/// <summary>
		/// Returns the minimum content size for elements that are contentControls and have style initialized.
		/// Otherwise returns zero.
		/// </summary>
		/// <returns>The content size.</returns>
		/// <param name="spacerEntry">Spacer entry.</param>
		float MinContentWidth(SpacerEntry spacerEntry)
		{
			float weight=spacerEntry.weight;
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			if (weight==0 && spacerEntry.element is ContentControl contentControl)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
			{
				return contentControl.MinSize.x;
			}
			return 0;
		}

		struct SpacerEntry
		{
			public float weight;
			public Element element;

			public SpacerEntry(float weight, Element element)
			{
				this.weight=weight;
				this.element=element;
			}
		}
	}
}
