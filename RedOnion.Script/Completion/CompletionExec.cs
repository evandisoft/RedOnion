using RedOnion.Script.ReflectedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RedOnion.Script.Completion
{
	partial class CompletionEngine
	{
		// EXPERIMENTAL IMPLEMENTATION
		protected virtual void Execute()
		{
			var source = lexer.Source;
			int i = interest;
			int dotAt = -1;
			int firstAt = -1;
			while (i > 0)
			{
				char c = source[--i];
				if (char.IsLetterOrDigit(c))
					continue;
				if (c == '_')
					continue;
				if (c == '.')
				{
					dotAt = i;
					if (firstAt < 0)
						firstAt = i;
					continue;
				}
				i++;
				break;
			}
			if (i > 0)
			{
				char c = source[i-1];
				if (!char.IsWhiteSpace(c)
					&& c != '(' && c != '['
					&& c != '=')
					return;
			}
			replaceAt = firstAt < 0 ? i : firstAt + 1;
			int j = interest;
			while (j < source.Length)
			{
				char c = source[j++];
				if (char.IsLetterOrDigit(c))
					continue;
				if (c == '_')
					continue;
				j--;
				break;
			}
			replaceTo = j;
			if (dotAt < 0)
			{
				found = Root;
				return;
			}
			IObject obj = Root;
			Type type = null;
			bool instance = false;
			for (; ; i = j + 1)
			{
				string name = source.Substring(i, dotAt-i);
				j = dotAt;
				while (++dotAt < interest)
				{
					char c = source[dotAt];
					if (char.IsLetterOrDigit(c))
						continue;
					if (c == '_')
						continue;
					if (c != '.')
						return;
					break;
				}
				if (obj != null)
				{
					if (obj.HasFeature(ObjectFeatures.Proxy|ObjectFeatures.TypeReference))
					{
						type = obj.Type;
						instance = obj.HasFeature(ObjectFeatures.Proxy);
						obj = null;
					}
					else
					{
						if (!obj.Get(name, out var value)
						&& (linked == null || obj != Root
						|| !linked.Get(name, out value)))
							return;
						obj = Box(value);
					}
				}
				if (type != null)
				{
					bool matched = false;
					foreach (var member in ReflectedType.GetMembers(type, name, instance))
					{
						if (member is FieldInfo field)
						{
							matched = true;
							type = field.FieldType;
							break;
						}
						if (member is PropertyInfo property)
						{
							matched = true;
							type = property.PropertyType;
							break;
						}
					}
					if (!matched || type == typeof(void))
						return;
				}
				if (dotAt == interest)
				{
					found = (object)obj ?? type;
					return;
				}
			}
		}
	}
}
