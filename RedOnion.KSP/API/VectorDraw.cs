using MunOS;
using RedOnion.Attributes;
using RedOnion.Debugging;
using RedOnion.ROS;
using System;
using System.ComponentModel;
using UnityEngine;

namespace RedOnion.KSP.API
{
	public partial class VectorCreator
	{
		[WorkInProgress, Description("Vector drawing. WIP: The origin is glitching a bit shortly after launch and during warp.")]
		public class Draw : IDisposable
		{
			[Description("Reference for coordinate system (origin at zero if null).")]
			public ISpaceObject reference { get; set; }

			[Description("Starting point of the vector (relative to reference).")]
			public Vector origin { get; set; }
			[Description("Direction of the vector (from starting point).")]
			public Vector direction { get; set; }

			[Description("Alias to `origin`.")]
			public Vector position { get => origin; set => origin = value; }
			[Description("Alias to `direction`.")]
			public Vector vector { get => direction; set => direction = value; }

			[Description("Alias to `origin`.")]
			public Vector from { get => origin; set => origin = value; }
			[Description("End point (`origin + direction`).")]
			public Vector to { get => origin + direction; set => direction = value - origin; }

			[Description("Color of the arrow.")]
			public Color color { get; set; } = Color.white;
			[Description("Width / thickness of the arrow.")]
			public double width { get; set; } = 1.0;
			[Description("Scale of the vector.")]
			public double scale { get; set; } = 1.0;

			protected GameObject _bodyObject;
			protected GameObject _headObject;
			protected LineRenderer _bodyRender;
			protected LineRenderer _headRender;

			/// <summary>
			/// Unique ID of the vector-draw.
			/// </summary>
			public ulong id { get; }
			protected static ulong id_counter;

			public Draw()
			{
				id = ++id_counter;
				MainLogger.DebugLog("Creating new VectorDraw #{0} in process #{1}", id, MunProcess.CurrentID);
				_hooks = new Hooks(this);
			}

			~Draw() => Dispose(false);
			void IDisposable.Dispose() => dispose();
			protected virtual void Dispose(bool disposing)
			{
				if (_hooks != null)
				{
					MainLogger.DebugLog("Disposing VectorDraw #{0} in process #{1} (original: #{2}, dispose: {3})",
						id, MunProcess.CurrentID, _hooks?.process.ID ?? MunID.Zero, disposing);
					var hooks = _hooks;
					_hooks = null;
					hooks.Dispose();
				}
				if (disposing)
					hide();
				// see UI.Window for another example of this
				else UI.Collector.Add(this);
			}
			// this is to avoid direct hard-link from processor to vector draw,
			// so that it can be garbage-collected when no direct link exists
			protected Hooks _hooks;
			protected class Hooks : MunProcess.ShutdownHook<Draw>
			{
				public Hooks(Draw draw) : base(draw) { }
				public void show() => process.PhysicsUpdate += Update;
				public void hide() => process.PhysicsUpdate -= Update;
				protected override void Dispose(bool disposing)
				{
					if (process == null)
						return;
					process.PhysicsUpdate -= Update;
					base.Dispose(disposing);
				}
				protected void Update()
				{
					Draw draw = Target;
					if (draw != null)
						draw.Update();
					else Dispose();
				}
			}

			[Description("Show the vector. It is created hidden so that you can subscribe to `system.update` first.")]
			public void show()
			{
				if (_hooks == null)
					throw new ObjectDisposedException("Vector.Draw");
				_hooks.show();
			}
			[Description("Hide the vector.")]
			public void hide()
			{
				_hooks?.hide();
				_bodyObject?.DestroyGameObject();
				_bodyObject = null;
				_headObject?.DestroyGameObject();
				_headObject = null;
			}
			[Description("Dispose the vector. (Cannot be shown again after that.)")]
			public void dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			// see https://wiki.kerbalspaceprogram.com/wiki/API:Layers
			protected const int MapLayer    = 10; // Scaled Scenery
			protected const int FlightLayer = 15; // Local Scenery
			protected virtual void Update()
			{
				if (_hooks == null)
					return;
				if (!HighLogic.LoadedSceneIsFlight)
				{
					hide();
					return;
				}

				var ship = Ship.Active;
				if (ship == null)
				{
					hide();
					return;
				}
				if (_bodyObject == null)
				{
					MainLogger.DebugLog("Showing VectorDraw #{0} in process #{1}", id, MunProcess.CurrentID);

					// see https://github.com/GER-Space/Kerbal-Konstructs/wiki/Shaders-in-KSP
					// + KSP 1.8: https://forum.kerbalspaceprogram.com/index.php?/topic/188933-180-modders-notes/
					var shader = Shader.Find("Legacy Shaders/Particles/Additive");

					_bodyObject = new GameObject("RedOnion.Vector.Draw.Body");
					_bodyRender = _bodyObject.AddComponent<LineRenderer>();
					_bodyRender.useWorldSpace = false;
					_bodyRender.material = new Material(shader);

					_headObject = new GameObject("RedOnion.Vector.Draw.Head");
					_headRender = _headObject.AddComponent<LineRenderer>();
					_headRender.useWorldSpace = false;
					_headRender.material = new Material(shader);
				}
				var origin = this.origin;
				var direction = this.direction;
				var width = this.width;
				var center = Vector3d.zero;
				if (MapView.MapIsEnabled)
				{
					_bodyObject.layer = MapLayer;
					_headObject.layer = MapLayer;
					var factor = ScaledSpace.InverseScaleFactor;
					origin *= factor;
					direction *= factor;
					center = ScaledSpace.LocalToScaledSpace(center);
					width *= Math.Max(0.2,
						(PlanetariumCamera.Camera.transform.localPosition
						- center).magnitude / 200);
				}
				else
				{
					_bodyObject.layer = FlightLayer;
					_headObject.layer = FlightLayer;
				}
				// TODO: fix the glitching origin in warp and shortly after launch
				if (reference != null)
					origin += reference.position;
				else
				{
					var vessel = FlightGlobals.ActiveVessel;
					if (vessel != null)
						origin += vessel.transform.position;
				}
				var head = origin + 0.9 * scale * direction;
				var end = origin + scale * direction;

				_bodyRender.positionCount = 2;
				_bodyRender.SetPosition(0, origin);
				_bodyRender.SetPosition(1, head);
				_bodyRender.startWidth = (float)width;
				_bodyRender.endWidth = (float)width;

				_headRender.positionCount = 2;
				_headRender.SetPosition(0, head);
				_headRender.SetPosition(1, end);
				_headRender.startWidth = (float)(width * 2);
				_headRender.endWidth = 0.0f;

				var endColor = color;
				var startColor = endColor;
				startColor.a *= 0.5f;
				_bodyRender.startColor = startColor;
				_bodyRender.endColor = endColor;
				_headRender.startColor = endColor;
				_headRender.endColor = endColor;

				_bodyRender.transform.localPosition = center;
				_headRender.transform.localPosition = center;
				_bodyRender.enabled = true;
				_headRender.enabled = true;
			}
		}
	}
}
