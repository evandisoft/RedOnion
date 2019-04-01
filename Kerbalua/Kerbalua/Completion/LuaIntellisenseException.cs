using System;
namespace Kerbalua.Completion
{
	public class LuaIntellisenseException : Exception
	{
		public LuaIntellisenseException()
		{
		}

		public LuaIntellisenseException(string message) : base(message)
		{
		}
	}

	public class TableHasMetatableException : LuaIntellisenseException
	{
		public TableHasMetatableException()
		{
		}

		public TableHasMetatableException(string message) : base(message)
		{
		}
	}

	public class TableCallException : LuaIntellisenseException
	{
		public TableCallException()
		{
		}

		public TableCallException(string message) : base(message)
		{
		}
	}

	public class TableArrayAccessException : LuaIntellisenseException
	{
		public TableArrayAccessException()
		{
		}

		public TableArrayAccessException(string message) : base(message)
		{
		}
	}

	public class KeyNotInTableException : LuaIntellisenseException
	{
		public KeyNotInTableException()
		{
		}

		public KeyNotInTableException(string message) : base(message)
		{
		}
	}

	public class InteropArrayAccessException : LuaIntellisenseException
	{
		public InteropArrayAccessException()
		{
		}

		public InteropArrayAccessException(string message) : base(message)
		{
		}
	}

	public class InteropCallException : LuaIntellisenseException
	{
		public InteropCallException()
		{
		}

		public InteropCallException(string message) : base(message)
		{
		}
	}

}
