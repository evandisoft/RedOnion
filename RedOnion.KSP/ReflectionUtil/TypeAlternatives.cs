using System;
using System.Collections.Generic;

namespace RedOnion.KSP.ReflectionUtil
{
	/// <summary>
	/// Represents all the type alternatives for a given base type name.
	/// 
	/// Example: Func`1, Func`2, Func`3, are all alternatives for
	/// base type name 'Func'. A name that has no '`' gets an index of 0.
	/// </summary>
	public class TypeAlternatives: SortedList<int, Type>
	{
		public readonly string BaseTypeName;

		public TypeAlternatives(string baseTypeName)
		{
			BaseTypeName = baseTypeName;
		}
	}
}
