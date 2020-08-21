using MunSharp.Interpreter;
using MunSharp.Interpreter.Interop;
using RedOnion.ROS;
using RedOnion.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RedOnion.Debugging;
using RedOnion.Common.Completion;
using RedOnion.ROS.Utilities;

namespace RedOnion.KSP.Utilities
{
	public abstract class Properties : Properties<object>
	{
	}
	[Description(
@"String-keyed read-only dictionary that exposes its values as properties.
The dictionary is filled by the scripting engine.
Both the properties and indexed values will first try exact case match (even in ROS),
then (if exact match not found) try insensitive match where UPPER is preferred
(`SomeThing` will match `Something` if there is `Something` and `something`).
Can be used as base class for list of celestial bodies,
discovered assemblies, namespaces, classes etc.")]
	public abstract class Properties<T> : IEnumerable<T>, ISelfDescribing, IUserDataType, ICompletable
	{
		/// <summary>
		/// Properties with map of native objects to wrapped ones.
		/// Designed to return `null` for null-key or non-existent key,
		/// but returns `default` for struct / value types.
		/// </summary>
		public class WithMap<R> : Properties<T>
		{
			protected readonly Dictionary<R, T> map = new Dictionary<R, T>();
			[Browsable(false), MoonSharpHidden]
			public T this[R key]
				=> key == null ? default : map.TryGetValue(key, out var it) ? it : default;
			protected bool Add(string name, T value, R native)
			{
				if (strict.ContainsKey(name))
					return false;
				strict[name] = list.size;
				map[native] = value;
				if (dict.TryGetValue(name, out int at))
				{
					var prev = list[at].Key;
					var overwrite = string.CompareOrdinal(name, prev) > 0;
					MainLogger.DebugLog("Properties conflict: {0} vs. {1}, overwrite: {2}", name, prev, overwrite);
					if (overwrite)
						dict[name] = list.size;
				}
				list.Add(new KeyValuePair<string, T>(name, value));
				return true;
			}
		}

		// ROS requires every property to have some integer index
		// therefore every property (or string-keyed value) gets added to this list
		protected ListCore<KeyValuePair<string, T>> list;
		protected readonly Dictionary<string, int> dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		protected readonly Dictionary<string, int> strict = new Dictionary<string, int>();

		protected bool Add(string name, T value)
		{
			if (strict.ContainsKey(name))
				return false;
			strict[name] = list.size;
			if (dict.TryGetValue(name, out int at))
			{
				var prev = list[at].Key;
				var overwrite = string.CompareOrdinal(name, prev) > 0;
				MainLogger.DebugLog("Properties conflict: {0} vs. {1}, overwrite: {2}", name, prev, overwrite);
				if (overwrite)
					dict[name] = list.size;
			}
			list.Add(new KeyValuePair<string, T>(name, value));
			return true;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		[Browsable(false), MoonSharpHidden]
		public IEnumerator<T> GetEnumerator()
		{
			foreach (var pair in list)
				yield return pair.Value;
		}

		public T this[string name]
			=> strict.TryGetValue(name, out var at) || dict.TryGetValue(name, out at)
			? list[at].Value : default(T);

		IList<string> ICompletable.PossibleCompletions => strict.Keys.ToList();
		bool ICompletable.TryGetCompletion(string completionName, out object completion)
		{
			if (strict.TryGetValue(completionName, out var at) || dict.TryGetValue(completionName, out at))
			{
				completion = list[at].Value;
				return true;
			}
			completion = null;
			return false;
		}
		DynValue IUserDataType.Index(Script script, DynValue index, bool isDirectIndexing)
		{
			var name = index.String;
			if (strict.TryGetValue(name, out var at) || dict.TryGetValue(name, out at))
				return DynValue.FromObject(script, list[at].Value);
			return DynValue.Nil;
		}
		bool IUserDataType.SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
			=> false;
		DynValue IUserDataType.MetaIndex(Script script, string metaname)
			=> null;


		int Find(string name) => strict.TryGetValue(name, out var at) || dict.TryGetValue(name, out at) ? at : -1;
		Descriptor ISelfDescribing.Descriptor => SelfDescriptor.Instance;
		protected class SelfDescriptor : Descriptor
		{
			public static SelfDescriptor Instance { get; } = new SelfDescriptor();
			protected SelfDescriptor() { }
			public override void Get(ref Value self)
			{
				var it = (Properties<T>)self.obj;
				var name = self.idx as string;
				if (name == null && self.idx is ValueBox box
					&& box.Value.desc.Convert(ref box.Value, String))
				{
					name = (string)box.Value.obj;
					self.idx = name;
					ValueBox.Return(box);
				}
				if (name != null)
				{
					int at = it.Find(name);
					if (at >= 0)
					{
						self = new Value(it.list[at].Value);
						return;
					}
				}
				GetError(ref self);
			}
			public override IEnumerable<string> EnumerateProperties(object self)
			{
				foreach (var name in ((Properties<T>)self).dict.Keys)
					yield return name;
			}
			public override IEnumerable<Value> Enumerate(object self)
			{
				foreach (var pair in ((Properties<T>)self).list)
					yield return new Value(pair.Value);
			}
		}
	}
}
