using System;
using System.Collections.Generic;
using System.Linq;
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
			if (i > 0 && !char.IsWhiteSpace(source[i-1]))
				return;
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
				if (!obj.Get(name, out var value)
					&& (linked == null || obj != Root
					|| !linked.Get(name, out value)))
					return;
				obj = Box(value);
				if (dotAt == interest)
				{
					found = obj;
					return;
				}
			}
		}
	}
}
