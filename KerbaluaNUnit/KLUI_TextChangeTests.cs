using System;
using NUnit.Framework;
using Kerbalui.EditingChanges;

namespace KerbaluaNUnit
{
	[TestFixture()]
	public class KLUI_TextChangeTests
	{
		public KLUI_TextChangeTests()
		{
		}
		public TextChange NoChanges=TextChange.NO_CHANGE;

		[Test()]
		public void KLUI_TextChange01_NoChanges()
		{
			//

			var input="asdf";
			var output="asdf";

			var expected=NoChanges;
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(NoChanges, result);
		}

		[Test()]
		public void KLUI_TextChange02_InsertAtBeginning()
		{
			var input="df";
			var output="asdf";

			var expected=new TextChange(0,"","as");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange021_InsertAtEnd()
		{
			var input="asdf";
			var output="asdfabcd";

			var expected=new TextChange(input.Length,"","abcd");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange03_DeleteFromBeginning()
		{
			var input="asdf";
			var output="df";

			var expected=new TextChange(0,"as","");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}


		[Test()]
		public void KLUI_TextChange031_DeleteFromEnd()
		{
			var input="asdf";
			var output="as";

			var expected=new TextChange(output.Length,"df","");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}



		[Test()]
		public void KLUI_TextChange04_InsertIntoEmpty()
		{
			var input="";
			var output="asdfabcd";

			var expected=new TextChange(input.Length,input,output);
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange05_DeleteAll()
		{
			var input="asdfabcd";
			var output="";

			var expected=new TextChange(output.Length,input,output);
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange06_ReplaceMiddleInputLarger()
		{
			var input="asdfg";
			var output="agzg";

			var expected=new TextChange(1,"sdf","gz");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange07_ReplaceMiddleOutputLarger()
		{
			var input="agzg";
			var output="asdfg";

			var expected=new TextChange(1,"gz","sdf");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange08_ReplaceBeginningInputLarger()
		{
			var input="zxcvdf";
			var output="asdf";

			var expected=new TextChange(0,"zxcv","as");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange09_ReplaceBeginningOutputLarger()
		{
			var input="asdf";
			var output="zxcvdf";

			var expected=new TextChange(0,"as","zxcv");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange10_ReplaceEndInputLarger()
		{
			var input="aszxcv";
			var output="asdf";

			var expected=new TextChange(2,"zxcv","df");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}

		[Test()]
		public void KLUI_TextChange11_ReplaceEndOutputLarger()
		{
			var input="asdf";
			var output="aszxcv";

			var expected=new TextChange(2,"df","zxcv");
			var result=TextChange.Calculate(input,output);

			Assert.AreEqual(expected, result);
		}
	}
}
