using System;
using System.Collections.Generic;
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

			float startFraction=0;
			float endFraction=0;
			float cumulativeWeight=0;
			foreach (var spacerEntry in spacerEntries)
			{
				cumulativeWeight+=spacerEntry.weight;
				endFraction=cumulativeWeight/totalWeight;
				spacerEntry.renderable.SetRect(new Rect(startFraction*rect.width, 0, (endFraction-startFraction)*rect.width, rect.height));
				startFraction=endFraction;
			}
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
