using System;

namespace RedOnion.ROS
{
	public partial class Descriptor
	{
		public class Reflected : Descriptor
		{
			public Reflected(Type type) : this(type.Name, type) { }
			public Reflected(string name, Type type) : base(name, type)
			{
			}
		}
	}
}
