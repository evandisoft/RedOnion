using RedOnion.KSP.API;
using RedOnion.ROS;
using RedOnion.ROS.Objects;
using RedOnion.ROS.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RedOnion.KSP.ROS
{
	public class RosSuggest
	{
		protected RosCore core;
		public RosSuggest(RosCore core)
			=> this.core = core;

		protected bool Matches(string suggestion)
		{
			if (partial == null || partial.Length == 0)
				return true;
			if (suggestion.Length < partial.Length)
				return false;
			for (int i = 0, n = suggestion.Length - partial.Length; i <= n; i++)
				if (string.Compare(partial,
					suggestion.Substring(i, partial.Length),
					StringComparison.OrdinalIgnoreCase) == 0)
					return true;
			return false;
		}
		protected class Comparer : IComparer<string>
		{
			public string partial;
			public Comparer(string partial) => this.partial = partial;
			public int Compare(string x, string y)
			{
				bool a = x.StartsWith(partial, StringComparison.OrdinalIgnoreCase);
				bool b = y.StartsWith(partial, StringComparison.OrdinalIgnoreCase);
				if (a && !b) return -1;
				if (b && !a) return +1;
				return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
			}
		}

		/// <summary>
		/// Completion/replacement suggestions for point of interest
		/// </summary>
		public ArraySegment<string> Suggestions
			=> new ArraySegment<string>(_suggestions, 0, _suggestionsCount);
		protected string[] _suggestions = new string[64];
		protected int _suggestionsCount;
		protected void AddSuggestion(string name)
		{
			if (!Matches(name))
				return;
			if (_suggestionsCount == _suggestions.Length)
				Array.Resize(ref _suggestions, _suggestions.Length << 1);
			_suggestions[_suggestionsCount++] = name;
		}
		/// <summary>
		/// Get copy of the suggestions
		/// (unfortunately ArraySegment is not IList in .NET 3.5)
		/// </summary>
		/// <returns></returns>
		public string[] GetSuggestions()
		{
			string[] it = new string[_suggestionsCount];
			Array.Copy(_suggestions, it, it.Length);
			return it;
		}

		protected Lexer lexer = new Lexer();
		protected int interest, replaceAt, replaceTo;
		protected Value found;
		protected string partial;

		public IList<string> GetCompletions(
			string source, int at,
			out int replaceAt, out int replaceTo)
		{
			Reset();
			interest = at;
			this.replaceAt = at;
			this.replaceTo = at;
			lexer.Source = source;

			try
			{
				Execute();
			}
			catch (Exception ex)
			{
				core.DebugLog("{0} in completion: {1}", ex.GetType().Name, ex.Message);
				replaceAt = at;
				replaceTo = at;
				return new string[0];
			}

			replaceAt = this.replaceAt;
			replaceTo = this.replaceTo;
			if (!found.IsVoid)
			{
				partial = at == replaceAt ? null : source.Substring(replaceAt, at-replaceAt);
				foreach (var name in found.desc.EnumerateProperties(ref found))
					AddSuggestion(name);
				if (found.desc is Context)
				{
					foreach (var name in core.Globals.EnumerateProperties())
						AddSuggestion(name);
				}
				if (_suggestionsCount > 1)
				{
					if (partial == null || partial.Length == 0)
						Array.Sort(_suggestions, 0, _suggestionsCount, StringComparer.OrdinalIgnoreCase);
					else Array.Sort(_suggestions, 0, _suggestionsCount, new Comparer(partial));
					int i = 0;
					for (int j = 1; j < _suggestionsCount; j++)
					{
						if (string.Compare(_suggestions[j], _suggestions[i],
							StringComparison.OrdinalIgnoreCase) != 0)
							_suggestions[++i] = _suggestions[j];
					}
					_suggestionsCount = i + 1;
				}
			}
			return GetSuggestions();
		}

		public void Reset()
		{
			if (_suggestionsCount > 0)
			{
				Array.Clear(_suggestions, 0, _suggestionsCount);
				_suggestionsCount = 0;
			}
		}

		protected void Execute()
		{
			found = Value.Void;
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
					&& c != '=' && c != '{'
					&& c != '+' && c != '-'
					&& c != '*' && c != '/'
					&& c != '|' && c != '&'
					&& c != '<' && c != '>')
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
			var obj = (Descriptor)core.Context ?? core.Globals;
			if (dotAt < 0)
			{
				found = new Value(obj);
				return;
			}
			var value = new Value(obj);
			for (; ; i = j + 1)
			{
				string name = source.Substring(i, dotAt-i);
				int index = value.desc.Find(value.obj, name);
				if (index < 0)
					return;
				if (!value.desc.Get(ref value, index))
					return;
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
				if (dotAt == interest)
				{
					found = value;
					return;
				}
			}
		}
	}
}
