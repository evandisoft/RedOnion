using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee.Run
{
	/// <summary>
	/// property interface (single property with custom access methods)
	/// @note can only be hosted in read-only properties (not in dynamic)
	/// </summary>
	[Alias("iProperty")]
	public interface IProp
	{
		/// <summary>
		/// get value of this property
		/// </summary>
		Value Get( IObject iObject );
		
		/// <summary>
		/// set value of this property
		/// @return false if not set (e.g. read-only)
		/// </summary>
		bool Set( IObject iObject, Value value );
	}//IProp
	
	/// <summary>
	/// properties interface (collection of properties)
	/// </summary>
	[Alias("iProperties")]
	public interface IProps
	{
		/// <summary>
		/// test the existence of the property with provided name
		/// </summary>
		bool Has( string @string );
		
		/// <summary>
		/// get the value of specified property
		/// </summary>
		Value Get( string @string );
		
		/// <summary>
		/// test and get value (has&get joined in *tryGet*)
		/// </summary>
		bool Get( string @string, out Value value );
		
		/// <summary>
		/// set the vakye of specified property
		/// </summary>
		bool Set( string @string, Value value );
		
		/// <summary>
		/// delete the specified property
		/// </summary>
		bool Del( string @string );
		
		/// <summary>
		/// reset (clear) the properties
		/// </summary>
		void Reset(  );
	}//IProps
	
	/// <summary>
	/// default property collection implementation
	/// </summary>
	[Alias("properties")]
	public class Props: Dictionary<string, Value>, IProps
	{
		public bool Has( string @string )
		{
			return ContainsKey( @string.ToLower() );
		}//Has
		
		public Value Get( string @string )
		{
			Value value;
			TryGetValue( @string.ToLower(), out value );
			return value;
		}//Get
		
		public bool Get( string @string, out Value value )
		{
			return TryGetValue( @string.ToLower(), out value );
		}//Get
		
		public bool Set( string @string, Value value )
		{
			@string = @string.ToLower();
			this[@string] = value;
			return true;
		}//Set
		
		public bool Set( string @string, IProp prop )
		{
			@string = @string.ToLower();
			this[@string] = new Value( prop );
			return true;
		}//Set
		
		public bool Del( string @string )
		{
			return Remove( @string );
		}//Del
		
		public void Reset(  )
		{
			Clear();
		}//Reset
		
		/// <summary>
		/// create empty
		/// </summary>
		public Props(  )
		{
		}//.ctor
		
		/// <summary>
		/// create with one property
		/// </summary>
		public Props( string @string, Value value )
		{
			this.Set( @string, value );
		}//.ctor
		
		/// <summary>
		/// create with one object-reference property (usually "prototype")
		/// </summary>
		public Props( string @string, IObject obj )
		{
			this.Set( @string, new Value( obj ) );
		}//.ctor
	}//Props
	
	public interface IObject: IProps
	{
		/// <summary>
		/// engine this object belongs to
		/// </summary>
		Engine Engine { get;  }
		/// <summary>
		/// base class (to search properties in this object next)
		/// </summary>
		IObject BaseClass { get;  }
		/// <summary>
		/// basic properties - not enumerable, not writable unless iProp with set returning true
		/// </summary>
		IProps BaseProps { get;  }
		/// <summary>
		/// added properties - enumerable and writable (unless same exist in baseProps)
		/// </summary>
		IProps MoreProps { get;  }
		/// <summary>
		/// contained value (if any)
		/// </summary>
		Value Value { get;  }
		/// <summary>
		/// find the object containing the property
		/// </summary>
		IObject Which( string @string );
		
		/// <summary>
		/// execute regular function call
		/// @self the object to call it on (as method if not null)
		/// @argc number of arguments (pass to @arg method)
		/// @return the result
		/// </summary>
		Value Call( IObject self, int argc );
		
		/// <summary>
		/// execute constructor ('new' used)
		/// @argc number of arguments (pass to @arg method)
		/// @return the new object (or null if not supported)
		/// </summary>
		IObject Create( int argc );
		
		/// <summary>
		/// get propertu/value (reference) at the indexes
		/// @note default treats x[y, z] as x[y][z], but redirecting to @call may be valid as well
		/// </summary>
		Value Index( IObject self, int argc );
	}//IObject
	
	[DebuggerDisplay("{GetType().Name}")]
	public class Obj: IObject
	{
		private Engine _engine;
		/// <summary>
		/// engine this object belongs to
		/// </summary>
		public Engine Engine
		{
			get
			{
				return _engine;
			}
		}//Engine
		
		private IObject _baseClass;
		/// <summary>
		/// base class (to search properties in this object next)
		/// </summary>
		public IObject BaseClass
		{
			get
			{
				return _baseClass;
			}
		}//BaseClass
		
		private IProps _baseProps;
		/// <summary>
		/// basic properties - not enumerable, not writable unless iProp with set returning true
		/// </summary>
		public IProps BaseProps
		{
			get
			{
				return _baseProps;
			}
			protected set
			{
				_baseProps = value;
			}
		}//BaseProps
		
		private IProps _moreProps;
		/// <summary>
		/// added properties - enumerable and writable (unless same exist in baseProps)
		/// </summary>
		public IProps MoreProps
		{
			get
			{
				return _moreProps;
			}
			protected set
			{
				_moreProps = value;
			}
		}//MoreProps
		
		public virtual Value Value
		{
			get
			{
				return new Value( "[internal]" );
			}
		}//Value
		
		/// <summary>
		/// create empty object with no base class
		/// </summary>
		public Obj( Engine engine )
		{
			_engine = engine;
		}//.ctor
		
		/// <summary>
		/// create empty object with base class
		/// </summary>
		public Obj( Engine engine, IObject baseClass )
		{
			_engine = engine;
			_baseClass = baseClass;
		}//.ctor
		
		/// <summary>
		/// create object with prototype and some base properties
		/// </summary>
		public Obj( Engine engine, IObject baseClass, IProps baseProps )
		{
			_engine = engine;
			_baseClass = baseClass;
			_baseProps = baseProps;
		}//.ctor
		
		/// <summary>
		/// create object with prototype, some base properties and more properties
		/// </summary>
		public Obj( Engine engine, IObject baseClass, IProps baseProps, IProps moreProps )
		{
			_engine = engine;
			_baseClass = baseClass;
			_baseProps = baseProps;
			_moreProps = moreProps;
		}//.ctor
		
		public bool Has( string @string )
		{
			return null != Which( @string );
		}//Has
		
		public virtual IObject Which( string @string )
		{
			IProps props;
			for( IObject obj = this; ;  )
			{
				props = obj.BaseProps;
				if( (props != null) && props.Has( @string ) )
				{
					return obj;
				}
				props = obj.MoreProps;
				if( (props != null) && props.Has( @string ) )
				{
					return obj;
				}
				if( (obj = obj.BaseClass) == null )
				{
					return null;
				}
			}
		}//Which
		
		public Value Get( string @string )
		{
			Value value;
			Get( @string, out value );
			return value;
		}//Get
		
		public virtual bool Get( string @string, out Value value )
		{
			value = new Value();
			IProps props;
			IObject obj = this;
			for( ; ;  )
			{
				props = obj.BaseProps;
				if( (props != null) && props.Get( @string, out value ) )
				{
					break;
				}
				props = obj.MoreProps;
				if( (props != null) && props.Get( @string, out value ) )
				{
					break;
				}
				if( (obj = obj.BaseClass) == null )
				{
					return false;
				}
			}
			if( value.Type == Vtype.Create )
			{
				value = new Value( ((Create)value.Ptr)( Engine ) );
				props.Set( @string, value );
			}
			else if( value.Type == Vtype.Prop )
			{
				value = ((IProp)value.Ptr).Get( obj );
			}
			return true;
		}//Get
		
		public virtual bool Set( string @string, Value value )
		{
			IProps props;
			Value query;
			for( IObject obj = this; ;  )
			{
				props = obj.BaseProps;
				if( (props != null) && props.Get( @string, out query ) )
				{
					if( query.Type == Vtype.Prop )
					{
						((IProp)query.Ptr).Set( obj, value );
						return true;
					}
					if( obj == this )
					{
						return false;
					}
					break;
				}
				props = obj.MoreProps;
				if( (props != null) && props.Get( @string, out query ) )
				{
					break;
				}
				if( (obj = obj.BaseClass) == null )
				{
					break;
				}
			}
			if( MoreProps == null )
			{
				MoreProps = new Props();
			}
			return MoreProps.Set( @string, value );
		}//Set
		
		public bool Del( string @string )
		{
			return MoreProps == null ? false : MoreProps.Del( @string );
		}//Del
		
		public virtual void Reset(  )
		{
			MoreProps = null;
		}//Reset
		
		public virtual Value Call( IObject self, int argc )
		{
			return new Value();
		}//Call
		
		public virtual IObject Create( int argc )
		{
			return null;
		}//Create
		
		public virtual Value Index( IObject self, int argc )
		{
			switch( argc )
			{
			case 0:
				return new Value();
			case 1:
				return new Value( this, Arg( argc, 0 ).String );
			default:
				self = Engine.Box( new Value( this, Arg( argc, 0 ).String ) );
				return self.Index( this, argc - 1 );
			}
		}//Index
		
		/// <summary>
		/// get n-th argument (for call/create implementation)
		/// </summary>
		protected Value Arg( int argc, int @int = 0 )
		{
			return Engine.Args.Arg( argc, @int );
		}//Arg
		
		/// <summary>
		/// activation context
		/// </summary>
		protected Engine.Context Ctx
		{
			get
			{
				return Engine.Ctx;
			}
		}//Ctx
		
		/// <summary>
		/// create new activation context
		/// </summary>
		protected void CreateContext(  )
		{
			Engine.CreateContext( Ctx.Self );
		}//CreateContext
		
		/// <summary>
		/// create new activation context
		/// </summary>
		protected void CreateContext( IObject self )
		{
			Engine.CreateContext( self );
		}//CreateContext
		
		/// <summary>
		/// create new activation context
		/// </summary>
		protected void CreateContext( IObject self, IObject scope )
		{
			Engine.CreateContext( self, scope );
		}//CreateContext
		
		/// <summary>
		/// destroy activation context
		/// </summary>
		protected Value DestroyContext(  )
		{
			return Engine.DestroyContext();
		}//DestroyContext
	}//Obj
}//Bee.Run
