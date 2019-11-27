using System;
using Kerbalui.EditingChanges;
using NUnit.Framework;

namespace KerbaluaNUnit
{
	/// <summary>
	/// Tests for the EditChangesManager
	/// </summary>
	[TestFixture()]
	public class KLUI_EditChangesManagerTests
	{
		public EditingChangesManager changesManager;

		void Setup()
		{
			changesManager=new EditingChangesManager();
		}

		[Test()]
		public void KLUI_EditChangesM01_NoChanges()
		{
			Setup();

			var state1=new EditingState("asdf",0,0);
			var state2=new EditingState("asdf",0,0);

			changesManager.AddChange(state1, state2);

			Assert.AreEqual(0, changesManager.ChangesLength);
		}

		[Test()]
		public void KLUI_EditChangesM02_InsertAtEnd()
		{
			Setup();

			var state1=new EditingState("asdf",0,4);
			var state2=new EditingState("asdfabc",10,0);

			changesManager.AddChange(state1, state2);

			Assert.AreEqual(state1, changesManager.Undo(state2));
			Assert.AreEqual(state2, changesManager.Redo(state1));
		}

		[Test()]
		public void KLUI_EditChangesM03_DeleteFromEnd()
		{
			Setup();

			var state1=new EditingState("asdfabc",1,0);
			var state2=new EditingState("asdf",0,7);

			changesManager.AddChange(state1, state2);

			Assert.AreEqual(state1, changesManager.Undo(state2));
			Assert.AreEqual(state2, changesManager.Redo(state1));
			Assert.AreEqual(state1, changesManager.Undo(state2));
			Assert.AreEqual(state1, changesManager.Undo(state1));
		}

		[Test()]
		public void KLUI_EditChangesM04_InsertIntoMiddle()
		{
			Setup();

			var state1=new EditingState("asdf",3,1);
			var state2=new EditingState("asabcdf",34,8);

			changesManager.AddChange(state1, state2);

			Assert.AreEqual(state1, changesManager.Undo(state2));
			Assert.AreEqual(state2, changesManager.Redo(state1));
			Assert.AreEqual(state2, changesManager.Redo(state2));
			Assert.AreEqual(state1, changesManager.Undo(state2));
			Assert.AreEqual(state1, changesManager.Undo(state1));
		}

		[Test()]
		public void KLUI_EditChangesM05_DeleteFromMiddle()
		{
			Setup();

			var state1=new EditingState("asabcdf",3,1);
			var state2=new EditingState("asdf",34,8);

			changesManager.AddChange(state1, state2);

			Assert.AreEqual(state1, changesManager.Undo(state2));
			Assert.AreEqual(state1, changesManager.Undo(state1));
			Assert.AreEqual(state1, changesManager.Undo(state1));
			Assert.AreEqual(state1, changesManager.Undo(state1));
		}

		[Test()]
		public void KLUI_EditChangesM06_AddChangeAfterUndo()
		{
			Setup();

			var state1=new EditingState("asdf",3,1);
			var state2=new EditingState("asabcdf",34,8);
			var state3=new EditingState("aaasabcdf",34,8);
			changesManager.AddChange(state1, state2);
			changesManager.AddChange(state2, state3);

			Assert.AreEqual(state2, changesManager.Undo(state3));
			Assert.AreEqual(state1, changesManager.Undo(state2));
			Assert.AreEqual(state1, changesManager.Undo(state1));
			changesManager.AddChange(state1, state3);
			Assert.AreEqual(1, changesManager.ChangesLength);
		}
	}
}
