using System;
namespace Kerbalua.Completion
{
	public struct Instance
	{
		public readonly Type Type;

		public Instance(Type t)
		{
			Type = t;
		}
	}
}
