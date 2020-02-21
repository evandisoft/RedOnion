using RedOnion.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedOnion.KSP.Parts
{
	public class RedOnionPartValues : PartModule
	{
		[KSPField(guiName = "Tags", groupName = "RedOnion",
			groupDisplayName = "Red Onion", groupStartCollapsed = true,
			isPersistant = true, guiActive = true, guiActiveEditor = true)]
		public string values = "";

		[KSPEvent(guiName = "Change Script Tags/Values",
			groupName = "RedOnion", guiActive = true, guiActiveEditor = true)]
		public void EditTags()
		{
			editor?.Close();
			editor = gameObject.AddComponent<Editor>();
			editor.Show(this);
		}

		Editor editor;
		class Editor : MonoBehaviour
		{
			RedOnionPartValues module;
			UI.Window window;
			UI.TextBox box;
			public void Show(RedOnionPartValues module)
			{
				this.module = module;
				window = new UI.Window(false, "Script Tags and Values");
				box = window.Add(new UI.TextBox(module.values)
				{
					MultiLine = true,
					FlexWidth = 1,
					FlexHeight = 1
				});
				var buttons = window.AddHorizontal();
				buttons.AddButton("Accept", Accept);
				buttons.AddButton("Cancel", Cancel);
				window.Closed += Closed;
				window.Show();
				GameEvents.onPartDestroyed.Add(CheckPart);
				GameEvents.onPartDie.Add(CheckPart);
			}
			public void SetText(string text)
				=> box.Text = text;
			void Closed(UI.Window window)
			{
				if (window != this.window)
					return;
				this.window = null;
				GameEvents.onPartDestroyed.Remove(CheckPart);
				GameEvents.onPartDie.Remove(CheckPart);
				window.Dispose();
				enabled = false;
				Destroy(this);
				module.editor = null;
			}
			public void Close()
				=> window?.Close();
			public void OnDestroy()
				=> Close();
			void Cancel(UI.Button button)
				=> Close();
			void CheckPart(Part dead)
			{
				if (dead == module.part)
					Close();
			}
			void Accept(UI.Button button)
			{
				module.SetValues(box.Text);
				Close();
			}
		}

		internal ListCore<KeyValuePair<string,string>> list;
		internal bool parsed;
		internal void SetValues(string values)
		{
			if (values == null)
				values = "";
			if (values == this.values)
				return;
			this.values = values;
			list.Clear();
			ParseValues();
			UpdateList();
		}
		//NOTE: don't even think about renaming this to Update! Unity!!
		internal void UpdateList()
		{
			var sb = new StringBuilder();
			foreach (var pair in list)
			{
				if (sb.Length > 0)
					sb.Append(' ');
				if (pair.Value == null)
					sb.Append(pair.Key);
				else sb.Append(pair.Key).Append('=').Append(pair.Value);
			}
			values = sb.ToString();
			editor?.SetText(values);
		}
		internal void ParseValues()
		{
			parsed = true;
			var full = values;
			if (full == null || full.Length == 0)
				return;
			for (int at = 0, start = 0; at < full.Length;)
			{
				char c = full[at];
				if (c == '=')
				{
					var name = full.Substring(start, at-start);
					start = ++at;
					while (at < full.Length)
					{
						c = full[at];
						if (char.IsWhiteSpace(c))
							break;
						at++;
						//TODO: quotes
					}
					list.Add(new KeyValuePair<string, string>(name, full.Substring(start, at-start)));
					start = ++at;
					continue;
				}
				if (char.IsWhiteSpace(c))
				{
					if (at != start)
					{
						var name = full.Substring(start, at-start);
						list.Add(new KeyValuePair<string, string>(name, ""));
					}
					start = ++at;
					continue;
				}
				at++;
			}
		}
		public int count
		{
			get
			{
				if (!parsed)
					ParseValues();
				return list.size;
			}
		}
		public int indexOf(string tag)
		{
			if (!parsed)
				ParseValues();
			for (int i = 0, n = list.size; i < n; i++)
				if (list.items[i].Key.Equals(tag, StringComparison.OrdinalIgnoreCase))
					return i;
			return -1;
		}
		public bool has(string tag)
			=> indexOf(tag) >= 0;
		public string get(string tag)
		{
			var at = indexOf(tag);
			return at < 0 ? null : list.items[at].Value;
		}
		public void set(string tag, string value = "")
		{
			var at = indexOf(tag);
			if (at >= 0)
			{
				if (value == null)
				{
					list.RemoveAt(at);
					UpdateList();
					return;
				}
				list.items[at] = new KeyValuePair<string, string>(tag, value);
				UpdateList();
				return;
			}
			list.Add(new KeyValuePair<string, string>(tag, value));
			UpdateList();
		}
		public bool add(string tag, string value = "")
		{
			if (value == null)
				return false;
			var at = indexOf(tag);
			if (at >= 0)
				return false;
			list.Add(new KeyValuePair<string, string>(tag, value));
			UpdateList();
			return true;
		}
		public string at(int at)
		{
			if (!parsed)
				ParseValues();
			return list[at].Value;
		}
		public void at(int at, string value)
		{
			if (!parsed)
				ParseValues();
			ref var it = ref list.GetRef(at);
			it = new KeyValuePair<string, string>(it.Key, value);
		}
		public bool remove(string tag)
		{
			var at = indexOf(tag);
			if (at < 0)
				return false;
			list.RemoveAt(at);
			UpdateList();
			return true;
		}
		public void removeAt(int at)
		{
			if (!parsed)
				ParseValues();
			list.RemoveAt(at);
			UpdateList();
		}
	}

	[Description("Tags and values that can be attached to a part (in editor or flight).")]
	public class PartValues : IDictionary<string, string>, IReadOnlyDictionary<string, string>
	{
		//NOTE: this can be null only if ModuleManager is not installed
		internal readonly RedOnionPartValues values;

		internal PartValues(PartBase part)
			=> values = part.native.GetComponent<RedOnionPartValues>();

		public override string ToString()
			=> values?.values ?? "";

		[Description("Get or set the value associated with a key (empty string for tags and 'null' for non-existing / removing).")]
		public string this[string tag]
		{
			get => values?.get(tag) ?? null;
			set => values?.set(tag, value);
		}

		[Description("Test for existence of a tag or key-value pair.")]
		public bool has(string tag)
			=> values?.has(tag) ?? false;
		[Description("Test for existence of a tag or key-value pair.")]
		public bool contains(string tag)
			=> values?.has(tag) ?? false;
		[Description("Get the value associated with a key (empty string for tags, 'null' for non-existing).")]
		public string get(string tag)
			=> values?.get(tag) ?? null;
		[Description("Set the value associated with a key (default empty string for tags, 'null' for removing).")]
		public void set(string tag, string value = "")
			=> values?.set(tag, value);
		[Description("Add new tag or key-value pair (default empty string, `null` will return false immediately).")]
		public bool add(string tag, string value = "")
			=> values?.add(tag, value) ?? false;

		[Description("Number of tags and key-value pairs.")]
		public int count
			=> values?.count ?? 0;
		int IReadOnlyCollection<KeyValuePair<string, string>>.Count
			=> values?.count ?? 0;
		int ICollection<KeyValuePair<string, string>>.Count
			=> values?.count ?? 0;

		[Description("Get or set the value associated with a key (empty string for tags and 'null' for non-existing / removing).")]
		public string this[int at]
		{
			get => values?.at(at) ?? null;
			set => values?.at(at, value);
		}

		string IReadOnlyDictionary<string, string>.this[string key]
			=> this[key];
		string IDictionary<string, string>.this[string key]
		{
			get => this[key];
			set => this[key] = value;
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			var values = this.values;
			if (values == null)
				yield break;
			if (!values.parsed)
				values.ParseValues();
			foreach (var pair in values.list)
				yield return pair;
		}
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		IEnumerable<string> IReadOnlyDictionary<string, string>.Keys
		{
			get
			{
				var values = this.values;
				if (values == null)
					yield break;
				if (!values.parsed)
					values.ParseValues();
				foreach (var pair in values.list)
					yield return pair.Key;
			}
		}
		IEnumerable<string> IReadOnlyDictionary<string, string>.Values
		{
			get
			{
				var values = this.values;
				if (values == null)
					yield break;
				if (!values.parsed)
					values.ParseValues();
				foreach (var pair in values.list)
					yield return pair.Value;
			}
		}

		bool IReadOnlyDictionary<string, string>.ContainsKey(string key)
			=> has(key);
		bool IReadOnlyDictionary<string, string>.TryGetValue(string key, out string value)

		{
			var values = this.values;
			if (values != null)
			{
				value = values.get(key);
				return value != null;
			}
			value = null;
			return false;
		}
		bool IDictionary<string, string>.ContainsKey(string key)
			=> values?.has(key) ?? false;
		void IDictionary<string, string>.Add(string key, string value)
			=> values?.add(key, value);
		bool IDictionary<string, string>.Remove(string key)
			=> values?.remove(key) ?? false;
		bool IDictionary<string, string>.TryGetValue(string key, out string value)
		{
			var values = this.values;
			if (values != null)
			{
				value = values.get(key);
				return value != null;
			}
			value = null;
			return false;
		}
		void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
			=> values?.add(item.Key, item.Value);
		void ICollection<KeyValuePair<string, string>>.Clear()
			=> values?.SetValues("");
		bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
			=> values?.get(item.Key) == item.Value;
		void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			var values = this.values;
			if (values == null)
				return;
			if (!values.parsed)
				values.ParseValues();
			values.list.CopyTo(array, arrayIndex);
		}
		bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
		{
			var values = this.values;
			if (values == null)
				return false;
			if (!values.parsed)
				values.ParseValues();
			int at = values.indexOf(item.Key);
			if (at < 0)
				return false;
			if (values.list.items[at].Value != item.Value)
				return false;
			values.list.RemoveAt(at);
			return true;
		}

		bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;
		ICollection<string> IDictionary<string, string>.Keys => throw new NotImplementedException();
		ICollection<string> IDictionary<string, string>.Values => throw new NotImplementedException();
	}
}
