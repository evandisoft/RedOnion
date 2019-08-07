using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RedOnion.KSP.Completion;
using RedOnion.ROS;
using RedOnion.ROS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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
					Value.DebugLog("Properties conflict: {0} vs. {1}, overwrite: {2}", name, prev, overwrite);
					if (overwrite)
						dict[name] = list.size;
				}
				list.Add(new KeyValuePair<string, T>(name, value));
				return true;
			}
		}

		// ROS requires every property to have some integer index
		// therefore every property (or string-keyed value) gets added to this list
		protected readonly ListCore<KeyValuePair<string, T>> list;
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
				Value.DebugLog("Properties conflict: {0} vs. {1}, overwrite: {2}", name, prev, overwrite);
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
			if (strict.TryGetValue(completionName, out var at))
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

		Descriptor ISelfDescribing.Descriptor => SelfDescriptor.Instance;
		protected class SelfDescriptor : Descriptor
		{
			public static SelfDescriptor Instance { get; } = new SelfDescriptor();
			protected SelfDescriptor() { }
			public override int Find(object self, string name, bool add = false)
			{
				var it = (Properties<T>)self;
				return it.strict.TryGetValue(name, out var at) || it.dict.TryGetValue(name, out at)
					? at : -1;
			}
			public override string NameOf(object self, int at)
			{
				var it = (Properties<T>)self;
				return at < 0 || at >= it.list.size ? "[?]" : it.list[at].Key;
			}
			public override bool Get(ref Value self, int at)
			{
				self = new Value(((Properties<T>)self.obj).list[at].Value);
				return true;
			}
			public override int IndexFind(ref Value self, Arguments args)
			{
				if (args.Length != 1)
					return -1;
				var index = args[0];
				if (!index.desc.Convert(ref index, String))
					return -1;
				return Find(self.obj, index.obj.ToString());
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
