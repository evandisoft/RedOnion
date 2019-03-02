using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Bee.Run.Ecma262
{
	/// <summary>
	/// object function (used to create new objects)
	/// </summary>
	public class ObjectFun: Obj
	{
		private IObject _prototype;
		/// <summary>
		/// prototype of all objects
		/// </summary>
		public IObject Prototype
		{
			get
			{
				return _prototype;
			}
		}//Prototype
		
		private Engine.IRoot _root;
		public Engine.IRoot Root
		{
			get
			{
				return _root;
			}
		}//Root
		
		public ObjectFun( Engine engine, IObject baseClass, IObject prototype, Engine.IRoot root )
			: base( engine, baseClass, new Props( "prototype", prototype ) )
		{
			_prototype = prototype;
			_root = root;
		}//.ctor
		
		public override Value Call( IObject self, int argc )
		{
			return new Value( Create( argc ) );
		}//Call
		
		public override IObject Create( int argc )
		{
			if( argc == 0 )
			{
				return new Obj( Engine, Prototype );
			}
			return Root.Box( Arg( argc ) );
		}//Create
	}//ObjectFun
}//Bee.Run.Ecma262
