using System;
using System.Reflection;

namespace RedOnion.Attributes
{
	/// <summary>Marks potentially dangerous API.</summary>
	public class UnsafeAttribute : Attribute { }

	/// <summary>Marks unfinished API.</summary>
	public class WorkInProgressAttribute : Attribute { }

	/// <summary>
	/// Marks classes with properies without side-effects (usually script-namespace).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
	public class SafePropsAttribute : Attribute { }

	/// <summary>
	/// To make class (even static) callable by naming a member
	/// (can be method, property or field - converted to return if not method).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
	public class CallableAttribute : Attribute
	{
		/// <summary>
		/// Name of the member to use for the call
		/// </summary>
		public string Name { get; }
		public CallableAttribute(string name) => Name = name;
	}

	/// <summary>
	/// Link from instance to creator (ROS object to function).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
	public class CreatorAttribute : Attribute
	{
		public Type Creator { get; }
		public CreatorAttribute(Type creator)
			=> Creator = creator;
	}

	/// <summary>
	/// Convert values to specified type (when presenting to script)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Method
		|AttributeTargets.ReturnValue|AttributeTargets.Parameter)]
	public class ConvertAttribute : Attribute
	{
		public Type Type { get; }
		public ConvertAttribute(Type type) => Type = type;

		// this is also workaround for some bugs in mono
		// - seen IndexOutOfRangeException thrown from .GetCustomAttributes(typeof... on Linux
		public static Type Get(MethodInfo m)
		{
			try
			{
				var it = m.GetCustomAttribute<ConvertAttribute>();
				if (it != null)
					return it.Type;
				var convertAttrs = m.ReturnTypeCustomAttributes
					.GetCustomAttributes(typeof(ConvertAttribute), true);
				if (convertAttrs.Length == 1)
					return ((ConvertAttribute)convertAttrs[0]).Type;
			}
			catch
			{
			}
			return null;
		}
	}

	/// <summary>
	/// Hint for documentation builder that set of types can be used
	/// (where the signature usually accepts object but requires one of the types)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.Method
		|AttributeTargets.ReturnValue|AttributeTargets.Parameter)]
	public class TypesAttribute : Attribute
	{
		public Type[] Types { get; }
		public TypesAttribute(params Type[] types) => Types = types;
		// this is also workaround for some bugs in mono
		// - seen IndexOutOfRangeException thrown from .GetCustomAttributes(typeof... on Linux
		public static Type[] Get(MethodInfo m)
		{
			try
			{
				var it = m.GetCustomAttribute<TypesAttribute>();
				if (it != null)
					return it.Types;
				var convertAttrs = m.ReturnTypeCustomAttributes
					.GetCustomAttributes(typeof(TypesAttribute), true);
				if (convertAttrs.Length == 1)
					return ((TypesAttribute)convertAttrs[0]).Types;
			}
			catch
			{
			}
			return null;
		}
	}

	/* TODO: use this in reflected descriptors to disable modification

	/// <summary>
	/// Make the value read-only for the script (disable writes and most methods)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.ReturnValue)]
	public class ReadOnlyContent : Attribute
	{
		public bool ContentIsReadOnly { get; }
		public ReadOnlyContent() => ContentIsReadOnly = true;
		public ReadOnlyContent(bool contentIsReadOnly) => ContentIsReadOnly = contentIsReadOnly;
	}
	/// <summary>
	/// Make items of collection read-only for the script (disable writes and most methods)
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field|AttributeTargets.ReturnValue)]
	public class ReadOnlyItems : Attribute
	{
		public bool ItemsAreReadOnly { get; }
		public ReadOnlyItems() => ItemsAreReadOnly = true;
		public ReadOnlyItems(bool itemsAreReadOnly) => ItemsAreReadOnly = itemsAreReadOnly;
	}
	*/
}
