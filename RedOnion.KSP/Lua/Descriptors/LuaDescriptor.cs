using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Compatibility;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Interop.BasicDescriptors;

namespace RedOnion.KSP.Lua.Descriptors
{
	public class LuaDescriptor : DispatchingUserDataDescriptor
	{
		/// <summary>
		/// Gets the interop access mode this descriptor uses for members access
		/// </summary>
		public InteropAccessMode AccessMode { get; private set; }

		public LuaDescriptor(Type type, InteropAccessMode accessMode = InteropAccessMode.Default, string friendlyName = null)
			: base(type, friendlyName)
		{
			if (accessMode == InteropAccessMode.NoReflectionAllowed)
				throw new ArgumentException("Can't create a LuaDescriptor under a NoReflectionAllowed access mode");
			if (Script.GlobalOptions.Platform.IsRunningOnAOT())
				accessMode = InteropAccessMode.Reflection;
			if (accessMode == InteropAccessMode.Default)
				accessMode = UserData.DefaultAccessMode;
			AccessMode = accessMode;

			var membersToIgnore = new HashSet<string>(
				Framework.Do.GetCustomAttributes(type, typeof(MoonSharpHideMemberAttribute), true)
					.OfType<MoonSharpHideMemberAttribute>()
					.Select(a => a.MemberName)
				);

			if (!type.IsDelegateType())
			{
				// add declared constructors
				foreach (var ci in Framework.Do.GetConstructors(type))
				{
					if (membersToIgnore.Contains("__new"))
						continue;

					AddMember("__new", MethodMemberDescriptor.TryCreateIfVisible(ci, accessMode));
				}

				// valuetypes don't reflect their empty ctor.. actually empty ctors are a perversion, we don't care and implement ours
				if (Framework.Do.IsValueType(type) && !membersToIgnore.Contains("__new"))
					AddMember("__new", new ValueTypeDefaultCtorMemberDescriptor(type));
			}


			// add methods to method list and metamethods
			foreach (var mi in Framework.Do.GetMethods(type))
			{
				if (membersToIgnore.Contains(mi.Name)) continue;

				var md = MethodMemberDescriptor.TryCreateIfVisible(mi, accessMode);

				if (md != null)
				{
					if (!MethodMemberDescriptor.CheckMethodIsCompatible(mi, false))
						continue;

					// transform explicit/implicit conversions to a friendlier name.
					string name = mi.Name;
					if (mi.IsSpecialName && (mi.Name == SPECIALNAME_CAST_EXPLICIT || mi.Name == SPECIALNAME_CAST_IMPLICIT))
					{
						name = mi.ReturnType.GetConversionMethodName();
					}

					AddMember(name, md);

					foreach (string metaname in mi.GetMetaNamesFromAttributes())
					{
						AddMetaMember(metaname, md);
					}
				}
			}

			// get properties
			foreach (var pi in Framework.Do.GetProperties(type))
			{
				if (pi.IsSpecialName || pi.GetIndexParameters().Any() || membersToIgnore.Contains(pi.Name))
					continue;

				AddMember(pi.Name, PropertyMemberDescriptor.TryCreateIfVisible(pi, accessMode));
			}

			// get fields
			foreach (var fi in Framework.Do.GetFields(type))
			{
				if (fi.IsSpecialName || membersToIgnore.Contains(fi.Name))
					continue;

				AddMember(fi.Name, FieldMemberDescriptor.TryCreateIfVisible(fi, accessMode));
			}

			// get events
			foreach (var ei in Framework.Do.GetEvents(type))
			{
				if (ei.IsSpecialName || membersToIgnore.Contains(ei.Name))
					continue;

				AddMember(ei.Name, EventMemberDescriptor.TryCreateIfVisible(ei, accessMode));
			}

			// get nested types and create statics
			foreach (var nestedType in Framework.Do.GetNestedTypes(type))
			{
				if (membersToIgnore.Contains(nestedType.Name))
					continue;

				if (!Framework.Do.IsGenericTypeDefinition(nestedType))
				{
					if (Framework.Do.IsNestedPublic(nestedType) || Framework.Do.GetCustomAttributes(nestedType, typeof(MoonSharpUserDataAttribute), true).Length > 0)
					{
						var descr = UserData.RegisterType(nestedType, accessMode);

						if (descr != null)
							AddDynValue(nestedType.Name, UserData.CreateStatic(nestedType));
					}
				}
			}

			if (!membersToIgnore.Contains("[this]"))
			{
				if (Type.IsArray)
				{
					int rank = Type.GetArrayRank();

					var get_pars = new ParameterDescriptor[rank];
					var set_pars = new ParameterDescriptor[rank + 1];

					for (int i = 0; i < rank; i++)
						get_pars[i] = set_pars[i] = new ParameterDescriptor("idx" + i.ToString(), typeof(int));

					set_pars[rank] = new ParameterDescriptor("value", Type.GetElementType());

					AddMember(SPECIALNAME_INDEXER_SET, new ArrayMemberDescriptor(SPECIALNAME_INDEXER_SET, true, set_pars));
					AddMember(SPECIALNAME_INDEXER_GET, new ArrayMemberDescriptor(SPECIALNAME_INDEXER_GET, false, get_pars));
				}
				else if (Type == typeof(Array))
				{
					AddMember(SPECIALNAME_INDEXER_SET, new ArrayMemberDescriptor(SPECIALNAME_INDEXER_SET, true));
					AddMember(SPECIALNAME_INDEXER_GET, new ArrayMemberDescriptor(SPECIALNAME_INDEXER_GET, false));
				}
			}
		}
	}
}
