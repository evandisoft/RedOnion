using System;
using System.Globalization;
using NUnit.Framework;
using RedOnion.Utilities;

namespace RedOnion.KSP.Tests
{
	[TestFixture]
	public class XXX_MiscTests
	{
		public string Formattable(StringWrapper str) => str.String;
		public string Formattable(FormattableString str) => str.ToString(CultureInfo.InvariantCulture);
		public string FormattableCz(StringWrapper str) => str.String;
		public string FormattableCz(FormattableString str) => str.ToString(CultureInfo.GetCultureInfo(1029));


		[Test]
		public void XXX_FormattedString()
		{
			Assert.AreEqual("1234.56", Formattable($"{1234.56}"));
			Assert.AreEqual("1234,56", FormattableCz($"{1234.56}"));
		}
	}
}
