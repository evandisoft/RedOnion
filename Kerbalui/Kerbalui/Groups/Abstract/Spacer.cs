using System;
using System.Collections.Generic;
using System.Linq;
using Kerbalui.Controls.Abstract;
using Kerbalui.Types;
using UnityEngine;

namespace Kerbalui.Groups.Abstract
{
	public abstract class Spacer:Group
	{
		protected List<SpacerEntry> spacerEntries=new List<SpacerEntry>();

		/// <summary>
		/// Adds a weight and associates it with an element to be rendered.
		/// A weight of 0 makes the element be rendered by it's minimum content size.
		/// </summary>
		/// <param name="weight">Weight.</param>
		/// <param name="element">Element.</param>
		public void Add(float weight, Element element)
		{
			spacerEntries.Add(new SpacerEntry(weight, element));
			RegisterForUpdates(element);
			needsRecalculation=true;
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

		/// <summary>
		/// Returns the minimum content size for elements that are contentControls and have style initialized.
		/// Otherwise returns zero.
		/// </summary>
		/// <returns>The content size.</returns>
		/// <param name="spacerEntry">Spacer entry.</param>
		protected float MinContentWidth(SpacerEntry spacerEntry)
		{
			float weight=spacerEntry.weight;
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			if (weight==0)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
			{
				return spacerEntry.element.MinSize.x;
			}
			return 0;
		}
		protected float MinContentHeight(SpacerEntry spacerEntry)
		{
			float weight=spacerEntry.weight;
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			if (weight==0)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
			{
				return spacerEntry.element.MinSize.y;
			}
			return 0;
		}

		protected List<float> CalculateSpacingPoints(Func<SpacerEntry,float> getMinSize,float totalSize)
		{
			var points=new List<float>();
			points.Add(0);
			float totalWeight=0;

			foreach (var spacerEntry in spacerEntries)
			{
				totalWeight+=spacerEntry.weight;
			}

			float totalMinContentSize=spacerEntries.Select(getMinSize).Sum();
			float minfract=totalMinContentSize/totalSize;

			float totalWeightFract=1-minfract;

			// Multiplying weightMultiplier to spacerEntry.weight, gives us the width for that entry
			// spacerEntryWidth=spacerEntry.weight*weightMultiplier
			// weightMultiplier=1/totalWeight*totalWeightFract*rect.width
			float weightMultiplier=totalWeightFract/totalWeight*totalSize;

			float minContentSize=0;
			float startPoint=0;
			float endPoint=0;
			foreach (var spacerEntry in spacerEntries)
			{
				minContentSize=getMinSize(spacerEntry);
				if (minContentSize>0)
				{
					endPoint=startPoint+minContentSize;
				}
				else
				{
					endPoint=startPoint+spacerEntry.weight*weightMultiplier;
				}

				points.Add(endPoint);
				startPoint=endPoint;
			}

			return points;
		}

		protected struct SpacerEntry
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
