using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Void = new OfVoid();

		/// <summary>
		/// Represents no value (action does not return anything)
		/// and will cause errors on any attemt to use it in any operation.
		/// </summary>
		internal class OfVoid : Descriptor
		{
			internal OfVoid() : base("void", typeof(void), ExCode.Void, TypeCode.DBNull) { }

			public override string ToString(ref Value self, string format, IFormatProvider provider, bool debug)
				=> debug ? "void" : throw new InvalidOperationException("void");
		}
	}
}
