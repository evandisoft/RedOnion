using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedOnion.KSP.Utilities
{
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
	public class DocBuildAttribute : Attribute
	{
		public Type AsType { get; }
		public string Path { get; }
		public DocBuildAttribute(Type asType) => AsType = asType;
		public DocBuildAttribute(string path) => Path = path;
	}
}
