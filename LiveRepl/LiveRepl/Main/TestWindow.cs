using System;
using Kerbalui.Gui;
using UnityEngine;

namespace LiveRepl.Main
{
	public class TestWindow
	{
		static Rect windowRect=new Rect(100,100,500,500);
		static Rect inWindowRect=new Rect(0,0,500,500);
		static int scrollPos=0;
		static public void TestGUI()
		{
			GUI.Window(0, windowRect, MainWindow, "Blah");
		}

		static string text1="";
		static string text2="";
		static Editor editor=new Editor();
		static TextArea textArea=new TextArea();
		static EditingArea editingArea=new EditingArea();
		static ScrollableTextArea scrollableTextArea=new ScrollableTextArea();
		static public void MainWindow(int id)
		{
			//BeginGroupTest();
			//scrollableTextArea.Update(new Rect(0, 0, 500, 500));
			//editor.Update(new Rect(0, 0, 500, 500));
			//if (Event.current.control)
			//{
			//	GUI.FocusControl("text1");
			//}
			//else if (Event.current.alt)
			//{
			//	GUI.FocusControl("text2");
			//}

			//GUI.SetNextControlName("text1");
			//text1=GUI.TextArea(new Rect(0, 0, 250, 500), text1);
			//GUI.SetNextControlName("text2");
			//text2=GUI.TextArea(new Rect(250, 0, 250, 500), text2);
		}

		static public void BeginGroupTest()
		{
			//GUI.SetNextControlName(textArea.ControlName);
			GUI.BeginGroup(inWindowRect);
			GUI.SetNextControlName(textArea.ControlName);
			textArea.Update(inWindowRect);
			GUI.EndGroup();
		}
	}
}
