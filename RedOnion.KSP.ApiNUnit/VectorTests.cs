using System;
using NUnit.Framework;
using RedOnion.KSP.API;
using RedOnion.Script;
using RedOnion.Script.BasicObjects;

namespace RedOnion.KSP.ApiNUnit
{
	public class ApiTestsBase : Engine
	{
		public ApiTestsBase()
		{
			Options |= EngineOption.Repl;
			FillRoot();
		}
		public override void Reset()
		{
			base.Reset();
			FillRoot();
		}
		public virtual void FillRoot()
		{
		}
		public void Test(string script, int countdown = 100)
		{
			try
			{
				ExecutionCountdown = countdown;
				Execute(script);
			}
			catch (Exception e)
			{
				throw new Exception(String.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Test(object value, string script, int countdown = 100)
		{
			Test(script);
			Assert.AreEqual(value, Result.Native, "Test: <{0}>", script);
		}
		public void Expect<Ex>(string script, int countdown = 100) where Ex : Exception
		{
			try
			{
				ExecutionCountdown = countdown;
				Execute(script);
				Assert.Fail("Should throw " + typeof(Ex).Name);
			}
			catch (Ex)
			{
			}
			catch (Exception e)
			{
				if (e is RuntimeError re && re.InnerException is Ex)
					return;
				throw new Exception(String.Format("{0} in Eval: {1}; IN: <{2}>",
					e.GetType().ToString(), e.Message, script), e);
			}
		}
		public void Lines(params string[] lines)
			=> Test(string.Join(Environment.NewLine, lines));
		public void Lines(object value, params string[] lines)
			=> Test(value, string.Join(Environment.NewLine, lines));
		public void Expect<Ex>(params string[] lines) where Ex : Exception
			=> Expect<Ex>(string.Join(Environment.NewLine, lines));
	}
	[TestFixture]
	public class API_VectorTests : ApiTestsBase
	{
		public override void FillRoot()
		{
			Root.Set("v", new Value(VectorCreator.Instance));
		}
		[Test]
		public void API_Vec01_Create()
		{
			Test("v(1)");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(1.0, v.Y);
			Assert.AreEqual(1.0, v.Z);

			Test("v 1,2u");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(2.0, v.Y);
			Assert.AreEqual(0.0, v.Z);

			Test("v 1,2f,3.0");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(2.0, v.Y);
			Assert.AreEqual(3.0, v.Z);
		}
		[Test]
		public void API_Vec02_Unary()
		{
			Test("-v(1)");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(-1.0, v.X);
			Assert.AreEqual(-1.0, v.Y);
			Assert.AreEqual(-1.0, v.Z);

			Test("+v(1)");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0, v.X);
			Assert.AreEqual(1.0, v.Y);
			Assert.AreEqual(1.0, v.Z);
		}
		[Test]
		public void API_Vec03_Binary()
		{
			Test("var a = v 1,2,3");
			Test("var b = v 4,5,6");
			Test("var c = a + b");
			var v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0+4.0, v.X);
			Assert.AreEqual(2.0+5.0, v.Y);
			Assert.AreEqual(3.0+6.0, v.Z);
			Test("var d = a - b");
			v = Result.Object as Vector;
			Assert.NotNull(v);
			Assert.AreEqual(1.0-4.0, v.X);
			Assert.AreEqual(2.0-5.0, v.Y);
			Assert.AreEqual(3.0-6.0, v.Z);
		}
	}
}
