using System;
using System.Collections.Generic;
using System.Linq;
using LiveRepl.UI.Elements;
using UnityEngine;

namespace LiveRepl.UI.Panes
{
	public class HorizontalSpacer:Pane
	{
		List<SpacerEntry> spacerEntries=new List<SpacerEntry>();

		public void Add(float weight,IRectRenderable renderable)
		{
			spacerEntries.Add(new SpacerEntry(weight, renderable));
			RegisterForUpdate(renderable);
		}

		public override void SetRect(Rect rect)
		{
			this.rect=rect;

			float totalWeight=0;

			foreach(var spacerEntry in spacerEntries)
			{
				totalWeight+=spacerEntry.weight;
			}

			float totalMinContentSize=spacerEntries.Select(MinContentSize).Sum();
			float minfract=totalMinContentSize/rect.width;
			//Debug.Log("minfract:"+minfract);
			float totalWeightFract=1-minfract;
			// spacerEntryWidth=spacerEntry.weight/totalWeight*totalWeightFract*rect.width
			float weightMultiplier=totalWeightFract/totalWeight*rect.width;
			//Debug.Log("weightMul "+weightMultiplier);
			float minContentSize=0;
			float startPoint=0;
			float endPoint=0;
			foreach (var spacerEntry in spacerEntries)
			{
				minContentSize=MinContentSize(spacerEntry);
				if (minContentSize>0)
				{
					endPoint=startPoint+minContentSize;
				}
				else
				{
					endPoint=startPoint+spacerEntry.weight*weightMultiplier;
				}
				//Debug.Log(spacerEntry);
				//Debug.Log("startPoint "+startPoint+", endPoint "+endPoint);

				spacerEntry.renderable.SetRect(new Rect(startPoint, 0, endPoint-startPoint, rect.height));
				startPoint=endPoint;
			}
		}

		float MinContentSize(SpacerEntry spacerEntry)
		{
			float weight=spacerEntry.weight;
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
			if (weight==0 && spacerEntry.renderable is TextElement textElement && textElement.style!=null)
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
			{
				return textElement.style.CalcSize(textElement.content).x;
			}
			return 0;
		}

		public struct SpacerEntry
		{
			public float weight;
			public IRectRenderable renderable;

			public SpacerEntry(float weight, IRectRenderable renderable)
			{
				this.weight=weight;
				this.renderable=renderable;
			}
		}
	}
}
