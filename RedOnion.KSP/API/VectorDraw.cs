using MoonSharp.Interpreter;
using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RedOnion.KSP.API
{
	public partial class VectorCreator
	{
		[Description("Vector drawing")]
		public class Draw : IDisposable
		{
			protected IProcessor _processor;

			[Description("Reference for coordinate system (active ship if null).")]
			public ISpaceObject reference { get; set; }
			[Convert(typeof(Vector)), Description("Starting point of the vector (relative to reference).")]
			public Vector3d origin { get; set; }
			[Convert(typeof(Vector)), Description("Direction of the vector (from starting point).")]
			public Vector3d direction { get; set; }

			[Convert(typeof(Vector)), Description("Alias to `origin`.")]
			public Vector3d from { get; set; }
			[Convert(typeof(Vector)), Description("End point (relative to reference, not starting point).")]
			public Vector3d to { get => origin + direction; set => direction = value - origin; }

			protected GameObject _bodyObject;
			//protected GameObject headObject;
			protected LineRenderer _bodyRender;
			//protected LineRenderer headRender;

			public Draw(IProcessor processor)
				=> _processor = processor;

			~Draw() => Dispose(false);
			[Browsable(false), MoonSharpHidden]
			public void Dispose()
			{
				GC.SuppressFinalize(this);
				Dispose(true);
			}
			protected virtual void Dispose(bool disposing)
			{
				if (_processor != null)
					Hide();
				if (disposing)
					_processor = null;
			}
			// this is to avoid direct hard-link from processor to vector draw,
			// so that it can be garbage-collected when no direct link exists
			protected Hooks hooks;
			protected class Hooks : IDisposable
			{
				WeakReference draw;
				public Hooks(Draw draw)
				{
					this.draw = new WeakReference(draw);
					draw._processor.Update += Update;
				}
				~Hooks() => Dispose(false);
				public void Dispose()
				{
					GC.SuppressFinalize(this);
					Dispose(true);
				}
				protected virtual void Dispose(bool disposing)
				{
					var draw = this.draw?.Target as Draw;
					if (draw != null)
						return;
					this.draw = null;
					draw._processor.Update -= Update;
				}
				protected void Update()
					=> (draw.Target as Draw)?.Update();
			}

			public void Show()
			{
				if (_bodyObject != null)
					return;
				_bodyObject = new GameObject("RedOnion.Vector.Draw.Body");
				_bodyRender = _bodyObject.AddComponent<LineRenderer>();
				_bodyRender.useWorldSpace = false;
				// see https://github.com/GER-Space/Kerbal-Konstructs/wiki/Shaders-in-KSP
				_bodyRender.material = new Material(Shader.Find("Particles/Additive"));

				hooks = new Hooks(this);
			}
			public void Hide()
			{
				if (_bodyObject == null)
					return;
				_bodyObject.DestroyGameObject();
				_bodyObject = null;

				hooks.Dispose();
				hooks = null;
			}

			// see https://wiki.kerbalspaceprogram.com/wiki/API:Layers
			protected const int MapLayer    = 10; // Scaled Scenery
			protected const int FlightLayer = 15; // Local Scenery
			protected virtual void Update()
			{
				if (_bodyObject == null)
					return;
				if (!HighLogic.LoadedSceneIsFlight)
				{
					Hide();
					return;
				}
				if (MapView.MapIsEnabled)
				{
					_bodyObject.layer = MapLayer;
				}
				else
				{
					_bodyObject.layer = FlightLayer;
				}
			}
		}
	}
}
