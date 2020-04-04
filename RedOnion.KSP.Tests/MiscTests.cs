using System;
using System.Globalization;
using NUnit.Framework;
using RedOnion.KSP.Utilities;
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

		[Test]
		public void XXX_AutoRemoveList()
		{
			var list = new AutoRemoveList<int>();
			list.add(1);
			list.add(2);
			list.add(3);
			list.add(4);
			list.add(5);
			foreach (var e in list)
				if ((e & 1) != 0)
					list.remove(e);
			Assert.AreEqual(2, list.count);
			int v = 2;
			foreach (var e in list)
			{
				Assert.AreEqual(v, e);
				v += 2;
			}
		}
	}
}
