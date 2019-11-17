using System;
using System.Collections.Generic;

namespace Kerbalua.Completion.CompletionTypes
{
    internal class RuntimeTypeCompletion : InstanceStaticCompletion
    {
#pragma warning disable RECS0035 // Possible mistaken call to 'object.GetType()'
		public RuntimeTypeCompletion(Type type) : base(type.GetType())
#pragma warning restore RECS0035 // Possible mistaken call to 'object.GetType()'
		{
        }
	}
}