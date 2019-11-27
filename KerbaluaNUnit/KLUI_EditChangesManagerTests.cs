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
			var input="asdf";
			var output="asdf";

			var expected=NoChanges;
			var result=EditingChangesManager.GetTextChange(input,output);

			Assert.AreEqual(NoChanges, result);
		}
	}
}
