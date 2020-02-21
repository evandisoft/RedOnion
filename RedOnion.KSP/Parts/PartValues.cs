using RedOnion.Collections;
using System;
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
			public void Update(string text)
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

		ListCore<KeyValuePair<string,string>> list;
		bool parsed;
		void SetValues(string values)
		{
			if (values == null)
				values = "";
			if (values == this.values)
				return;
			this.values = values;
			list.Clear();
			Parse();
			Update();
		}
		void Update()
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
			editor?.Update(values);
		}
		void Parse()
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
						list.Add(new KeyValuePair<string, string>(name, null));
					}
					start = ++at;
					continue;
				}
				at++;
			}
		}
		public bool has(string tag)
		{
			if (!parsed)
				Parse();
			foreach (var pair in list)
				if (pair.Key.Equals(tag, StringComparison.OrdinalIgnoreCase))
					return true;
			return false;
		}
		public string get(string tag)
		{
			if (!parsed)
				Parse();
			foreach (var pair in list)
				if (pair.Key.Equals(tag, StringComparison.OrdinalIgnoreCase))
					return pair.Value;
			return null;
		}
		public void set(string tag, string value = null)
		{
			if (!parsed)
				Parse();
			for (int i = 0; i < list.size; i++)
			{
				ref var pair = ref list.items[i];
				if (!pair.Key.Equals(tag, StringComparison.OrdinalIgnoreCase))
					continue;
				pair = new KeyValuePair<string, string>(tag, value);
				Update();
				return;
			}
			list.Add(new KeyValuePair<string, string>(tag, value));
			Update();
		}
		public bool add(string tag, string value = null)
		{
			if (!parsed)
				Parse();
			for (int i = 0; i < list.size; i++)
			{
				if (list.items[i].Key.Equals(tag, StringComparison.OrdinalIgnoreCase))
					return false;
			}
			list.Add(new KeyValuePair<string, string>(tag, value));
			Update();
			return true;
		}
	}

	[Description("Tags and values that can be attached to a part (in editor or flight).")]
	public class PartValues
	{
		internal readonly RedOnionPartValues values;
		internal PartValues(PartBase part)
			=> values = part.native.GetComponent<RedOnionPartValues>();

		[Description("Test for existence of a tag or key-value pair.")]
		public bool has(string tag)
			=> values?.has(tag) ?? false;
		[Description("Test for existence of a tag or key-value pair.")]
		public bool contains(string tag)
			=> values?.has(tag) ?? false;
		[Description("Get the value associated with a key ('null' for tags and non-existing).")]
		public string get(string tag)
			=> values?.get(tag) ?? null;
		[Description("Set the value associated with a key (default 'null' for tags).")]
		public void set(string tag, string value = null)
			=> values?.set(tag, value);
		[Description("Add new tag or key-value pair (default 'null' for tags).")]
		public bool add(string tag, string value = null)
			=> values?.add(tag, value) ?? false;
		[Description("Get or set the value associated with a key ('null' for tags and non-existing).")]
		public string this[string tag]
		{
			get => values?.get(tag) ?? null;
			set => values?.set(tag, value);
		}

		public override string ToString()
			=> values?.values ?? "";
	}
}
