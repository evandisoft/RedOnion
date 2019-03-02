using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee.Run.Ecma262
{
	/// <summary>
	/// string function (used to create new string objects)
	/// </summary>
	public class StringFun: Obj
	{
		private StringObj _prototype;
		/// <summary>
		/// prototype of all string objects
		/// </summary>
		public StringObj Prototype
		{
			get
			{
				return _prototype;
			}
		}//Prototype
		
		public StringFun( Engine engine, IObject baseClass, StringObj prototype )
			: base( engine, baseClass, new Props( "prototype", prototype ) )
		{
			_prototype = prototype;
		}//.ctor
		
		public override Value Call( IObject self, int argc )
		{
			return argc == 0 ? new Value( "" ) : new Value( Arg( argc ).String );
		}//Call
		
		public override IObject Create( int argc )
		{
			return new StringObj( Engine, Prototype, argc == 0 ? "" : Arg( argc ).String );
		}//Create
	}//StringFun
	
	/// <summary>
	/// string object (string box)
	/// </summary>
	[DebuggerDisplay("{GetType().Name}: {_string}")]
	public class StringObj: Obj
	{
		private String _string;
		/// <summary>
		/// boxed value
		/// </summary>
		public String String
		{
			get
			{
				return _string;
			}
			protected set
			{
				_string = value;
			}
		}//String
		
		public override Value Value
		{
			get
			{
				return new Value( this.String );
			}
		}//Value
		
		/// <summary>
		/// create string.prototype
		/// </summary>
		public StringObj( Engine engine, IObject baseClass )
			: base( engine, baseClass )
		{
			this.String = "";
		}//.ctor
		
		/// <summary>
		/// create new string object boxing the string
		/// </summary>
		public StringObj( Engine engine, StringObj baseClass, string @string )
			: base( engine, baseClass, Stdprops )
		{
			this.String = @string;
		}//.ctor
		
		private static Props _stdprops = new Props();
		public static Props Stdprops
		{
			get
			{
				return _stdprops;
			}
		}//Stdprops
		
		static StringObj(  )
		{
			Stdprops.Set( "length", new StringLength() );
		}//.ctor
		
		public class StringLength: IProp
		{
			public Value Get( IObject obj )
			{
				return new Value( ((StringObj)obj).String.Length );
			}//Get
			
			public bool Set( IObject obj, Value value )
			{
				return false;
			}//Set
		}//StringLength
	}//StringObj
}//Bee.Run.Ecma262
