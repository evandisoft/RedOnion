using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee.Run.Ecma262
{
	/// <summary>
	/// number function (used to create new number objects)
	/// </summary>
	public class NumberFun: Obj
	{
		private NumberObj _prototype;
		/// <summary>
		/// prototype of all number objects
		/// </summary>
		public NumberObj Prototype
		{
			get
			{
				return _prototype;
			}
		}//Prototype
		
		public NumberFun( Engine engine, Obj baseClass, NumberObj prototype )
			: base( engine, baseClass, new Props( "prototype", prototype ) )
		{
			_prototype = prototype;
		}//.ctor
		
		public override Value Call( IObject self, int argc )
		{
			return argc == 0 ? new Value() : Arg( argc ).Number;
		}//Call
		
		public override IObject Create( int argc )
		{
			return new NumberObj( Engine, Prototype, argc == 0 ? new Value() : Arg( argc ).Number );
		}//Create
	}//NumberFun
	
	/// <summary>
	/// number object (value box)
	/// </summary>
	[DebuggerDisplay("{GetType().Name}: {_number}")]
	public class NumberObj: Obj
	{
		private Value _number;
		/// <summary>
		/// boxed value
		/// </summary>
		public Value Number
		{
			get
			{
				return _number;
			}
			protected set
			{
				_number = value;
			}
		}//Number
		
		public override Value Value
		{
			get
			{
				return this.Number;
			}
		}//Value
		
		/// <summary>
		/// create number.prototype
		/// </summary>
		public NumberObj( Engine engine, IObject baseClass )
			: base( engine, baseClass )
		{
		}//.ctor
		
		/// <summary>
		/// create new number object boxing the value
		/// </summary>
		public NumberObj( Engine engine, NumberObj baseClass, Value value )
			: base( engine, baseClass, Stdprops )
		{
			this.Number = value;
		}//.ctor
		
		private static Props _stdprops = new Props();
		public static Props Stdprops
		{
			get
			{
				return _stdprops;
			}
		}//Stdprops
	}//NumberObj
}//Bee.Run.Ecma262
