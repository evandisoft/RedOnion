using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public static readonly Descriptor Null = new OfNull();

		/// <summary>
		/// Represents empty reference
		/// and will cause errors on attemt to use it in most operations.
		/// </summary>
		internal class OfNull : Descriptor
		{
			internal OfNull() : base("null", typeof(object), ExCode.Null, TypeCode.Empty) { }
		}
	}
}
