using System;
using System.Runtime.CompilerServices;

namespace RedOnion.Attributes
{
	/// <summary>
	/// Markers for documentation builder.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class|AttributeTargets.Struct|
		AttributeTargets.Enum|AttributeTargets.Delegate,
		AllowMultiple = false, Inherited = false)]
	public class DocBuildAttribute : Attribute
	{
		/// <summary>
		/// Path for the output file (without file extension).
		/// Used e.g. in namespaces to strip off _Namespace suffix.
		/// </summary>
		public string Path { get; set; }
		/// <summary>
		/// Document the type as another type.
		/// Used e.g. in helpers like UI_Window to redirect to base class.
		/// </summary>
		public Type AsType { get; set; }
		/// <summary>
		/// Make the builder register/document other types when seeing this.
		/// Used e.g. in base class like PartBase to reference derived classes.
		/// </summary>
		public Type[] RegisterTypes { get; set; }

		public DocBuildAttribute() { }
		public DocBuildAttribute(string path) => Path = path;
		public DocBuildAttribute(params Type[] registerTypes) => RegisterTypes = registerTypes;
	}

	/// <summary>
	/// Ordering marker for documentation builder.
	/// Automatically provides line number if index not specified.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Field|AttributeTargets.Event|
		AttributeTargets.Property|AttributeTargets.Method,
		AllowMultiple = false, Inherited = false)]
	public class DocIndexAttribute : Attribute
	{
		public int Index { get; set; }

		public DocIndexAttribute(int index) => Index = index;
		public DocIndexAttribute(int baseIndex = 1000, [CallerLineNumber] int index = 0)
			=> Index = baseIndex + index;
	}
}
