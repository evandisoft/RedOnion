using System;
using UnityEngine;

namespace Kerbalui.Obsolete {
	/// <summary>
	/// Subclasses of this class are expected to have no more or less than one control.
	/// This class calls GUI.SetNextControl(ControlName) immediately prior
	/// to the ProtectedUpdate when Visible is set to true. Thus, when Visible is set
	/// to true, the ProtectedUpdate must call a control. When Visible is set
	/// to false, ProtectedUpdate must not call a control.
	/// 
	/// Allowing the ControlName to be set properly allows HasFocus and GrabFocus to work
	/// properly
	/// </summary>
	public abstract class UIElement:IFocusable,IRenderer {
		static long NextID = 0;
		/// <summary>
		/// Auto-generated unique name for this control, to be used as the name
		/// in GUI.SetNextControlName(name), so that GUI.GetNameOfFocusedControl 
		/// and GUI.FocusControl can be used for managing focus of that control.
		/// </summary>
		public readonly string ControlName = "Control-"+NextID++;
		public bool ReceivedInput { get; set; }

		protected void RunBaseControl(Action action)
		{
			GUI.SetNextControlName(ControlName);
			action();
		}

		string IFocusable.ControlName => ControlName;
		/// <summary>
		/// Will be assigned on each Update.
		/// </summary>
		/// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
		protected bool Visible { get; private set; }
		protected GUIStyle style=new GUIStyle();

		/// <summary>
		/// Calls "CommonUpdateProcessing" to provide some default processing 
		/// on each update. For example, checking for input. Subclasses override 
		/// "ProtectedUpdate" to run their per-update code.
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="visible">If set to <c>true</c> visible.</param>
		/// <param name="style">Style.</param>
		public void Update(Rect rect, bool visible = true, GUIStyle style = null)
		{
			//bool lastEventWasMouseDown = Event.current.type == EventType.MouseDown && GUILibUtil.MouseInRect(rect);
			//if (lastEventWasMouseDown)
			//{
			//	GrabFocus();
			//	Event.current.Use();
			//}
			CommonUpdateProcessing(visible, style, () => ProtectedUpdate(rect));
		}
		/// <summary>
		/// Subclasses override this instead of "Update" to run their per-update code.
		/// </summary>
		/// <param name="rect">Rect.</param>
		protected abstract void ProtectedUpdate(Rect rect);

		/// <summary>
		/// Calls "CommonUpdateProcessing" to provide some default processing 
		/// on each update. For example, checking for input. Subclasses override 
		/// "ProtectedUpdate" to run their per-update code.
		/// </summary>
		public void Update(bool visible = true, GUIStyle style = null)
		{
			CommonUpdateProcessing(visible,style,ProtectedUpdate);
		}
		/// <summary>
		/// Subclasses override this instead of "Update" to run their per-update code.
		/// </summary>
		protected abstract void ProtectedUpdate();

		protected bool hadKeyDownThisUpdate = false;
		/// <summary>
		/// Any processing that is to occur before/after any ProtectedUpdate
		/// from any subclass code should go here.
		/// </summary>
		/// <param name="protectedUpdate">Protected update.</param>
		void CommonUpdateProcessing(bool visible,GUIStyle param_style,Action protectedUpdate)
		{
			Visible = visible;
			style = param_style;

			ClearMarkedCharEvent();
			hadKeyDownThisUpdate = Event.current.type == EventType.KeyDown;


			//if (Visible) GUI.SetNextControlName(ControlName);
			//else Debug.Log("not visible");
			protectedUpdate.Invoke();
			
			ReceivedInput = hadKeyDownThisUpdate && Event.current.type == EventType.Used;
		}

		/// <summary>
		/// Returns true if this control has focus.
		/// Must be called within a Render.
		/// </summary>
		public bool HasFocus()
		{
			//if(GUI.GetNameOfFocusedControl() == ControlName)
			//{
			//	Debug.Log("hasFocus ("+GUI.GetNameOfFocusedControl()+") ("+ControlName);
			//}


			return GUI.GetNameOfFocusedControl() == ControlName;
		}

		/// <summary>
		/// Make the main control for this element grab the focus.
		/// Must be called within a Render.
		/// </summary>
		public void GrabFocus()
		{
			Debug.Log("Grabbing Focus in "+ControlName);
			GUI.FocusControl(ControlName);

		}

		protected void ClearMarkedCharEvent()
		{
			if (Event.current.type == EventType.KeyDown) {
				GUILibUtil.ConsumeMarkedCharEvent(Event.current);
			}
		}
	}
}
