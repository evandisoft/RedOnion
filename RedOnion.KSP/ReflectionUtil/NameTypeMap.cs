using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RedOnion.KSP.ReflectionUtil
{
	public class NameTypeMap
	{
		Dictionary<string, TypeAlternatives> nameTypeDict = new Dictionary<string, TypeAlternatives>();

		public Dictionary<string, TypeAlternatives>.KeyCollection BaseTypeNames => nameTypeDict.Keys;
		public List<string> RawTypeNames
		{
			get
			{
				List<string> rawTypeNames = new List<string>();
				foreach(var entry in nameTypeDict)
				{
					rawTypeNames.AddRange(entry.Value.Values.Select(t => t.Name));
				}
				return rawTypeNames;
			}
		}

		public void Add(Type rawType)
		{
			string basename = GetBasename(rawType.Name);
			int index = GetNumTypeParameters(rawType.Name);

			if (!nameTypeDict.ContainsKey(basename))
			{
				nameTypeDict[basename] = new TypeAlternatives(basename);
			}

			nameTypeDict[basename][index] = rawType;
		}

		public bool ContainsRawTypeName(string typeName)
		{
			string basename = GetBasename(typeName);
			int numTypeParameters = GetNumTypeParameters(typeName);

			return Contains(basename, numTypeParameters);
		}

		public bool Contains(string basename, int numTypeParameters)
		{
			return nameTypeDict.ContainsKey(basename)
				&& nameTypeDict[basename].ContainsKey(numTypeParameters);
		}

		public bool Contains(string basename)
		{
			return nameTypeDict.ContainsKey(basename);
		}

		public bool TryBasename(string basename, out Type type)
		{
			type = null;

			if (nameTypeDict.TryGetValue(basename, out TypeAlternatives typeAlternatives))
			{
				if (typeAlternatives.Count == 0)
				{
					return false;
				}

				int firstKey = typeAlternatives.Keys[0];
				type=typeAlternatives[firstKey];

				return true;
			}

			return false;
		}

		public bool TryBasename(string basename,int numTypeParameters, out Type type)
		{
			type = null;

			if (nameTypeDict.TryGetValue(basename, out TypeAlternatives typeAlternatives))
			{
				if(typeAlternatives.TryGetValue(numTypeParameters,out type))
				{
					return true;
				}
			}

			return false;
		}

		public bool TryRawTypeName(string rawTypeName, out Type type)
		{
			type = null;

			string basename = GetBasename(rawTypeName);
			int numTypeParameters = GetNumTypeParameters(rawTypeName);

			return TryBasename(basename, numTypeParameters, out type);
		}

		int GetNumTypeParameters(string rawName)
		{
			if (rawName.Contains("`"))
			{
				return int.Parse(rawName.Substring(rawName.LastIndexOf('`') + 1));
			}

			return 0;
		}

		string GetBasename(string rawName)
		{
			if (rawName.Contains("`"))
			{
				return rawName.Substring(0, rawName.LastIndexOf('`'));
			}

			return rawName;
		}
	}
}
