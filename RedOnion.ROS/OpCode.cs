using System.Diagnostics;

namespace RedOnion.ROS
{
	public enum TypeFlags : ushort
	{
		None			= 0x0000,
		Access			= 0x000F,
		Public			= 0x0001,
		Private			= 0x0002,
		Protected		= 0x0003,
		Internal		= 0x0004,
		IntPrivate		= 0x0006,
		IntProtected	= 0x0007,
		Scope			= 0x00F0,
		Final			= 0x0010,
		Virtual			= 0x0020,
		Abstract		= 0x0030,
		Override		= 0x0040,
		Readonly		= 0x0050,
		Const			= 0x0060,
		Static			= 0x0070,
		StaticReadOnly	= 0x0080,

		Other			= 0x3F00,
		Hide			= 0x0100, // C# new (hide inherited)
		Partial			= 0x0200, // partial class/struct/enum/interface
		Unsafe			= 0x0400, // unsafe context (allow pointers)
		Implicit		= 0x0800, // operators only
		Inline			= 0x1000, // methods
	}
	public static class TypeFlagsExtensions
	{
		public static TypeFlags Access(this TypeFlags self)
			=> self & TypeFlags.Access;
		public static TypeFlags Scope(this TypeFlags self)
			=> self & TypeFlags.Scope;
		public static string AccessText(this TypeFlags self)
			=> _access[(int)self.Access()];
		public static string ScopeText(this TypeFlags self)
			=> _scope[(int)self.Scope() >> 4];

		private static readonly string[] _access = new string[] {
			null, "public", "private", "protected",
			"internal", null, "private internal", "protected internal",
			null, null, null, null, null, null, null, null
		};
		private static readonly string[] _scope = new string[] {
			null, "final", "virtual", "abstract", "override",
			"readonly", "const", "static", "static readonly",
			null, null, null, null, null, null, null
		};
	}

	public enum OpKind : byte
	{//	NOTE: 0xA0-0xC0 (Access, Scope and Mods) may be removed/replaced in the future
		Literal			= 0x00, // constants, (non-number) literals and references
		Number			= 0x10, // numbers (int, byte, double, ...)
		Special			= 0x20, // special operators (call, index, new, ternary, ...)
		Assign			= 0x30, // assign and compound-assignment operators
		Binary			= 0x40, // binary operators with compound-assignmnet counterpart
		Logic			= 0x50, // logic (boolean) binary operators
		Unary			= 0x60, // unary operators (except post/pre: + - ~ !)
		PreOrPost		= 0x70, // pre- and post-fix operators (++ --)
		Statement		= 0x80, // statements, loops, etc.
		Statement2		= 0x90, // statement group 2: switch, try..catch..finaly, using
		Access			= 0xA0, // access modifiers (public, private, protected, internal)
		Scope			= 0xB0, // scope modifiers (virtual, static, ...)
		Mods			= 0xC0, // other modifiers (new, partial)
		Model			= 0xD0, // object model (classes, methods, fields, properties, attributes)
		Reserved		= 0xE0, // unused kind
		Meta			= 0xF0, // comments and other metadata that can be ignored during execution
	}

	public enum BlockCode : byte
	{
		Exception	= OpCode.Exception,	// 08 the catch block (needs to clear the active error unless re-throw)
		Block		= OpCode.Block,     // 80
		Finally     = OpCode.Raise,		// 83 the finally part of try..catch..finally when there is pending exception (simple block otherwise)

		// these need to be together and equal to OpCode - see IsLoop() and ToOpCode() extensions
		For         = OpCode.For,       // 86
		ForEach		= OpCode.ForEach,   // 87
		While		= OpCode.While,		// 88
		DoWhile		= OpCode.DoWhile,	// 89
		Until		= OpCode.Until,		// 8A
		DoUntil		= OpCode.DoUntil,   // 8B

		TryCatch    = OpCode.Catch,		// 97 the try part of try..catch..finally

		Library     = OpCode.Import,    // D0 see Core.CallScript(..., include: true)
		Function    = OpCode.Function,	// DB root block of function call (where arguments live)
	}

	public enum OpCode : byte
	{
	//	constants and literals
		Void			= 0x00, // no value
		Null			= 0x01, // null/nullptr (object if used as type)
		False			= 0x02, // boolean false
		True			= 0x03, // boolean true
		This			= 0x04, // this (self)
		Base			= 0x05, // base (super)
		Value			= 0x06, // value in setter
		Implicit		= 0x07, // implicit value (in case of conditional switch)
		Exception		= 0x08, // exception in catch block
		Default			= 0x09, // default value	+[index to string table]
		Identifier		= 0x0A, // identifier		+[index to string table]

		//note: [String, Create) is used for type identification
		String			= 0x0B, // string			+[index to string table]
		Char			= 0x0C, // short char		+[byte]
		WideChar		= 0x0D, // wide char		+[2B char]
		LongChar		= 0x0E, // long char		+[4B char]
		Number			= 0x0F, // generic number (useful for conversions)

		Byte			= 0x10, // u8
		UShort			= 0x11, // u16
		UInt			= 0x12, // u32
		ULong			= 0x13, // u64
		Quad			= 0x14, // u128
		SByte			= 0x15, // s8
		Short			= 0x16, // s16
		Int				= 0x17, // s32
		Long			= 0x18, // s64
		Hyper			= 0x19, // s128
		Bool			= 0x1A, // used only as type specifier (true/false used in expressions)
		Complex			= 0x1B, // reserved for complex numbers
		Float			= 0x1C, // f32
		Double			= 0x1D, // f64
		LongDouble		= 0x1E, // f80
		Decimal			= 0x1F, // x128 (fixed point)

	//	special
		Create			= 0x20, // new
		Call0			= 0x21, // f()	empty (no-argument) call
		Call1			= 0x22, // f(arg)	call with single argument
		Call2			= 0x23, // f(x,y)	call with two arguments
		CallN			= 0x24, // f(,,)	call with more than two arguments
		Index			= 0x25, // a[i]	indexing with single argument
		IndexN			= 0x26, // a[i,j]	indexing with more than one argument
		Dot				= 0x27, // .		(can work as left["right"] in scripting mode)
		Var				= 0x28, // var x[[:]type][=val] - moved to expressions that we can do if var... and for var...
		Type			= 0x29, // 			special marker for built-in types in postfix notation
		Array			= 0x2A, // t[n]	array declaration or creation
		BigArray		= 0x2B, // t[n]	array declaration or creation
		NullCol			= 0x2C, // ??		(one ?? two)
		NullCall		= 0x2D, // ?.()
		NullDot			= 0x2E, // ?.it
		Ternary			= 0x2F, // ?:		(or: val1 if cond else val2)

	//	assign
		Assign			= 0x30, // =
		AddAssign		= 0x31, // +=
		SubAssign		= 0x32, // -=
		MulAssign		= 0x33, // *=
		DivAssign		= 0x34, // /=
		ModAssign		= 0x35, // %=
		PwrAssign		= 0x36, // **=
		OrAssign		= 0x37, // |=
		XorAssign		= 0x38, // ^=
		AndAssign		= 0x39, // &=
		LshAssign		= 0x3A, // <<=
		RshAssign		= 0x3B, // >>=

	//	binary
		Add				= 0x41, // +
		Sub				= 0x42, // -
		Mul				= 0x43, // *
		Div				= 0x44, // /
		Mod				= 0x45, // %
		Power			= 0x46, // **
		BitOr			= 0x47, // |		!! changed priority from C#/C++ !!
		BitXor			= 0x48, // ^		!! changed priority from C#/C++ !!
		BitAnd			= 0x49, // &		!! changed priority from C#/C++ !!
		ShiftLeft		= 0x4A, // <<		!! changed priority from C#/C++ !!
		ShiftRight		= 0x4B, // >>		!! changed priority from C#/C++ !!

	//	logic + casts
		LogicOr			= 0x50, // ||
		LogicAnd		= 0x51, // &&
		Equals			= 0x52, // ==
		Differ			= 0x53, // !=
		Less			= 0x54, // <
		More			= 0x55, // >
		LessEq			= 0x56, // <=
		MoreEq			= 0x57, // >=
		Identity		= 0x58, // ===
		NotIdentity		= 0x59, // !==

		As				= 0x5A, // as
		AsCast			= 0x5B, // as!
		Cast			= 0x5C, // !		(type! value)
		Is				= 0x5D, // is
		IsNot			= 0x5E, // is!
		In				= 0x5F, // in		(contains or for/foreach in)
	
	//	unary
		Plus			= 0x60, // +x
		Neg				= 0x61, // -x
		Flip			= 0x62, // ~x
		Not				= 0x63, // !x
		AddrOf			= 0x64, // &x		(reserved)
		Deref			= 0x65, // *x		(reserved)

		TypeOf			= 0x68, // typeof x
		NameOf			= 0x69, // nameof x
		Await			= 0x6A, // await task
		Delete			= 0x6D, // delete
		Ref				= 0x6E, // ref
		Out				= 0x6F, // out

	//	unary post/pre
		PostInc			= 0x70, // x++
		PostDec			= 0x71, // x--
		Inc				= 0x78, // ++x
		Dec				= 0x79, // --x

	//	statements
		Block			= 0x80, // block with its own scope
		Autocall		= 0x81, // expression that should be called (like Call0) if it is a function
		Return			= 0x82, // return (also used to mark scripts run as library)
		Raise			= 0x83, // throw/raise (Python uses raise)
		Break			= 0x84, // break
		Continue		= 0x85, // continue
		For				= 0x86, // for init; test; iter: stts
		ForEach			= 0x87, // for var in list: stts
		While			= 0x88, // while cond do stts
		DoWhile			= 0x89, // do stts; while cond
		Until			= 0x8A, // until cond do stts  
		DoUntil			= 0x8B, // do stts; until cond
		If				= 0x8C, // if cond then truestts
		Unless			= 0x8D, // unless cond do falsestts
		Else			= 0x8E, // if cond then truestts else falsestts
		Cond			= 0x8F, // condition of loops (for/while/unless in postfix)

		Pop				= 0x90,	// discard top value
		Yield			= 0x91,	// wait/yield (cooperative multitasking)

		Goto			= 0x92, // goto label
		GotoCase		= 0x93, // goto case
		Switch			= 0x94, // switch x: case 1: break; case 2: break; default: continue
		SwitchCond		= 0x95, // switch x: case < 0: ...; case 0: ...; case > 0: ...
		Using			= 0x96, // using macro (try..finally dispose)
		Catch			= 0x97, // try..catch..finally
		With			= 0x98, // with var do stts
		From			= 0x99, // LINQ

	//	model
		Import			= 0xD0, // using/import/include
		UseAll			= 0xD1, // using static (C#; Java: import SomeClass.*)
		UseAs			= 0xD2, // using name = ns.whatever
		Namespace		= 0xD3, // namespace (C#), package (Java), Module in Visual Basic is a bit different

		Class			= 0xD4, // ref-class
		Struct			= 0xD5, // value-type
		Enum			= 0xD6, // integer with named values

		Interface		= 0xD7, // interface
		Mixin			= 0xD8, // mixin class/interface (to be implemented as composed-in-struct with redirections)
		Delegate		= 0xD9, // delegate (function prototype)
		Attribute		= 0xDA, // attribute

		Function		= 0xDB, // method (function, procedure)
		Operator		= 0xDC, // operator (type conversion or +-*/% ...)

		Field			= 0xDD, // var in class
		Event			= 0xDE, // event field

		Property		= 0xDF, // property (get-set or add-remove method pair)

	//	metadata
		Where			= 0xFB, // where rules
		Comment			= 0xFC, // line comment
		Documentation	= 0xFD, // documenting comment
		MultiLineComment= 0xFE, // multi-line comment
		Whitespace		= 0xFF, // white space
	}

	public enum ExCode : ushort
	{
	//	flags and masks
		fUnused			= 0x8000, // unused operator
		fUnary			= 0x4000, // unary operator
		fTernary		= 0x2000, // ternary operator
		fMulti			= 0x6000, // operator accepting multiple arguments (call, index)
		mPriority		= 0x1F00, // operator priority
		mCode			= 0x00FF, // code mask
		mKind			= 0x00F0, // kind mask

		fFp				= 0x8000, // floating point (number section)
		fSig			= 0x4000, // signed
		mSz				= 0x3F00, // number size mask

	//	constants and literals
		Void			= 0x0000, // undefined value
		Null			= 0x0001, // null/nullptr
		False			= 0x0002, // boolean false
		True			= 0x0003, // boolean true
		This			= 0x0004, // this (self) - preferred
		Self			= 0x0104, // self (this) - alternative (used in extension methods)
		Base			= 0x0005, // base (super) - preferred
		Super			= 0x0105, // super (base) - alternative (Java)
		Value			= 0x0006, // value in setter
		Implicit		= 0x0007, // implicit value (in case of conditional switch)
		Exception		= 0x0008, // exception in catch block
		Default			= 0x0009, // default value	+[index to string table]
		Identifier		= 0x000A, // identifier		+[index to string table]
		String			= 0x000B, // string			+[index to string table]
		Char			= 0x010C, // short char		+[byte]
		WideChar		= 0x020D, // wide char		+[2B char]
		LongChar		= 0x040E, // long char		+[4B char]
		Number			= 0x000F, // generic number (useful for conversions)

		Byte			= 0x0110, // u8
		UShort			= 0x0211, // u16
		UInt			= 0x0412, // u32
		ULong			= 0x0813, // u64
		Quad			= 0x1014, // u128
		SByte			= 0x4115, // s8
		Short			= 0x4216, // s16
		Int				= 0x4417, // s32
		Long			= 0x4818, // s64
		Hyper			= 0x5019, // s128
		Bool			= 0x011A, // used only as type specifier (true/false used in expressions)
		Complex			= 0x101B, // reserved for complex numbers
		Float			= 0xC41C, // f32
		Double			= 0xC81D, // f64
		LongDouble		= 0xCA1E, // f80
		Decimal			= 0x901F, // f128

	//	special
		Create			= 0x4020, // new
		Call0			= 0x4021, // f()	empty (no-argument) call
		Call1			= 0x0022, // f(arg)	call with single argument
		Call2			= 0x2023, // f(x,y)	call with two arguments
		CallN			= 0x6024, // f(,,)	call with more than two arguments
		Index			= 0x0025, // a[i]	indexing with single argument
		IndexN			= 0x6026, // a[i,j]	indexing with more than one argument
		Dot				= 0x0027, // .		(can work as left["right"] in scripting mode)
		Var				= 0x2028, // var x[[:]type][=val] - moved to expressions that we can do if var... and for var...
		Type			= 0x6029, //		special marker for built-in types in postfix notation
		Array			= 0x602A, // t[n]	array declaration or creation
		BigArray		= 0x602B, // t[n]	array declaration or creation
		NullCol			= 0x022C, // ??		(one ?? two)
		NullCall		= 0x002D, // ?.()
		NullDot			= 0x002E, // ?.it
		Ternary			= 0x202F, // ?:		(or: val1 if cond else val2)

	//	assign
		Assign			= 0x0130, // =
		AddAssign		= 0x0131, // +=
		SubAssign		= 0x0132, // -=
		MulAssign		= 0x0133, // *=
		DivAssign		= 0x0134, // /=
		ModAssign		= 0x0135, // %=
		PwrAssign		= 0x0136, // **=
		OrAssign		= 0x0137, // |=
		XorAssign		= 0x0138, // ^=
		AndAssign		= 0x0139, // &=
		LshAssign		= 0x013A, // <<=
		RshAssign		= 0x013B, // >>=

	//	binary
		Add				= 0x0B41, // +
		Sub				= 0x0B42, // -
		Mul				= 0x0C43, // *
		Div				= 0x0C44, // /
		Mod				= 0x0C45, // %
		Power			= 0x0D46, // **
		BitOr			= 0x0E47, // |		!! changed priority from C#/C++ !!
		BitXor			= 0x0F48, // ^		!! changed priority from C#/C++ !!
		BitAnd			= 0x1049, // &		!! changed priority from C#/C++ !!
		ShiftLeft		= 0x114A, // <<		!! changed priority from C#/C++ !!
		ShiftRight		= 0x114B, // >>		!! changed priority from C#/C++ !!

	//	logic + casts
		LogicOr			= 0x0350, // ||		or
		LogicAnd		= 0x0451, // &&		and
	//	priority 5..8 reserved for C# binary or, xor, and, <<, >>
		Equals			= 0x0952, // ==
		Differ			= 0x0953, // !=
		Less			= 0x0A54, // <
		More			= 0x0A55, // >
		LessEq			= 0x0A56, // <=
		MoreEq			= 0x0A57, // >=
		Identity		= 0x0A58, // ===
		NotIdentity		= 0x0A59, // !==

		As				= 0x0A5A, // as
		AsCast			= 0x0A5B, // as!
		Cast			= 0x0A5C, // !		(type! value)
		Is				= 0x0A5D, // is
		IsNot			= 0x0A5E, // is!
		In				= 0x0A5F, // in		(contains + for/foreach in)
	
	//	unary
		Plus			= 0x5260, // +x
		Neg				= 0x5261, // -x
		Flip			= 0x5262, // ~x
		Not				= 0x5263, // !x		not x
		AddrOf			= 0xD264, // &x		(reserved)
		Deref			= 0xD265, // *x		(reserved)

		TypeOf			= 0xD268, // typeof x
		NameOf			= 0xD269, // nameof x
		Await			= 0x526A, // await task
		Delete			= 0x526D, // delete
		Ref				= 0x526E, // ref
		Out				= 0x526F, // out

	//	unary post/pre
		PostInc			= 0x5370, // x++
		PostDec			= 0x5371, // x--
		Inc				= 0x5478, // ++x
		Dec				= 0x5479, // --x

	//	statements
		Block			= 0x0080, // block with its own scope
		Autocall		= 0x0081, // expression that should be called (like Call0) if it is a function
		Return			= 0x0082, // return
		Raise			= 0x0083, // throw/raise (Python uses raise)
		Throw			= 0x0183, // throw (alternative to raise from Python)
		Break			= 0x0084, // break
		Continue		= 0x0085, // continue
		For				= 0x0086, // for init; test; iter: stts
		ForEach			= 0x0087, // for var in list: stts
		While			= 0x0088, // while cond do stts
		DoWhile			= 0x0089, // do stts; while cond
		Until			= 0x008A, // until cond do stts  
		DoUntil			= 0x008B, // do stts; until cond
		If				= 0x008C, // if cond then truestts
		Then			= 0x018C, // (for parsing only)
		Unless			= 0x008D, // unless cond do falsestts
		Else			= 0x008E, // if cond then truestts else falsestts
		Cond			= 0x008F, // condition of loops (for/while/unless in postfix)

		Pop				= 0x0090, // discard top value
		Yield			= 0x0091, // yield (cooperative multitasking)
		Wait			= 0x0191, // wait (cooperative multitasking)
		Goto			= 0x0092, // goto label
		Label			= 0x0192, // label:   (for 1st phase of parsing/compilation)
		GotoCase		= 0x0093, // goto case
		Case			= 0x0193, // case 0:  (for parsing only)
		Switch			= 0x0094, // switch x: case 1: break; case 2: break; default: continue
		SwitchCond		= 0x0095, // switch x: case < 0: ...; case 0: ...; case > 0: ...
		Using			= 0x0096, // using macro (try..finally dispose)
		Catch			= 0x0097, // try..catch..finally
		Try				= 0x0197, // (for parsing only)
		Finally			= 0x0297, // (for parsing only)
		Except			= 0x0397, // catch from Python
		With			= 0x0098, // with var do stts
		From			= 0x0099, // LINQ
		Select			= 0x0199, // (for parsing only)
		OrderBy			= 0x0299, // (for parsing only)

	//NOTE: acces, scope and other modifiers were moved to TypeFlags
	//..... could thus be changed to parsing-only codes

	//	access
		Public			= 0x00A1,
		Private			= 0x00A2,
		Protected		= 0x00A3,
		Internal		= 0x00A4,
		IntPrivate		= 0x00A6,
		IntProtected	= 0x00A7,
	//	scope
		Final			= 0x00B1, // final (Java), sealed (C#), NotInheritable/NotOverridable (Basic)
		Sealed			= 0x01B1, // final - alternative (C#)
		Virtual			= 0x00B2,
		Abstract		= 0x00B3, // abstract (C#/Java), MustInherit/MustOverride (Basic)
		Override		= 0x00B4,
		ReadOnly		= 0x00B5,
		Const			= 0x00B6,
		Static			= 0x00B7,
		StaticReadOnly	= 0x00B8,

	//	other modifiers ...maybe move to Fx-meta class
		Hide			= 0x00C0, // new
		Partial			= 0x00C1,
		Unsafe			= 0x00C2,
		Async			= 0x00C3,

	//	model
		Import			= 0x00D0, // import (Java/Python), use (B#), using (C#), include, ...
		Include			= 0x01D0, // import - alternative (C++ Preprocessor)
		Use				= 0x02D0, // import - alternative (B#)
		UseAll			= 0x00D1, // using static (C#; Java: import SomeClass.*)
		UseAs			= 0x00D2, // using name = ns.whatever

		Namespace		= 0x00D3, // namespace (C#), package (Java), Module in Visual Basic is a bit different
		Package			= 0x01D3, // namespace - alias (Java)
		Pkg				= 0x02D3, // namespace - alias (short)

		Class			= 0x00D4, // ref-class
		Struct			= 0x00D5, // value-type
		Enum			= 0x00D6, // integer with named values

		Interface		= 0x00D7, // interface
		Mixin			= 0x00D8, // mixin class/interface (to be implemented as composed-in-struct with redirections)
		Delegate		= 0x00D9, // delegate (function prototype)
		Attribute		= 0x00DA, // attribute

		Function		= 0x00DB, // method (function, procedure)
		Def				= 0x01DB, // define method/lambda (Python)
		Lambda			= 0x02DB, // => operator
		Operator		= 0x00DC, // operator (type conversion or +-*/% ...)

		Field			= 0x00DD, // var in class
		Event			= 0x00DE, // event field

		Property		= 0x00DF, // property (get-set or add-remove method pair)
		Get				= 0x01DF, // get (property)
		Set				= 0x02DF, // set (property)
		Combine			= 0x03DF, // add/combine (event/delegate)
		Remove			= 0x04DF, // remove (event/delegate)

	//	metadata
		Where			= 0x00FB, // where rules
		Comment			= 0x00FC, // line comment
		Documentation	= 0x00FD, // documenting comment
		MultiLineComment= 0x00FE, // multi-line comment
		Whitespace		= 0x00FF, // white space

	//	parsing only
		Unknown			= 0xFFFF,
		Comma			= 0xFEFF,
	}

	public static class OpCodeExtensions
	{
		public static bool Unused(this ExCode self)
			=> (self & ExCode.fUnused) != 0;
		public static bool Unary(this ExCode self)
			=> (self & ExCode.fMulti) == ExCode.fUnary;
		public static bool Binary(this ExCode self)
			=> (self & ExCode.fMulti) == 0;
		public static bool Ternary(this ExCode self)
			=> (self & ExCode.fMulti) == ExCode.fTernary;
		public static bool Multi(this ExCode self)
			=> (self & ExCode.fMulti) == ExCode.fMulti;
		public static bool Postfix(this ExCode self)
			=> (self & ExCode.mPriority) == (ExCode.PostInc & ExCode.mPriority);

		/// <summary>
		/// Operator priority (binary 'or', 'xor' and 'and' have higher priority than in C#)
		/// </summary>
		public static ExCode Priority(this ExCode self)
			=> self & ExCode.mPriority;
		/// <summary>
		/// Operator priority in C# (C/C++/Java and derivates - binary or/xor/and with same priority as logic)
		/// </summary>
		public static ExCode StdPriority(this ExCode self)
			=> self.Code() >= ExCode.BitOr.Code()
			&& self.Code() <= ExCode.ShiftRight.Code()
			? self.Priority() - 0x0900
			: self.Code() == ExCode.Cast.Code()
			? 0 : self.Priority();

		public static byte Code(this OpCode self)
			=> (byte)self;
		public static byte Code(this ExCode self)
			=> (byte)self;
		public static OpKind Kind(this OpCode self)
			=> (OpKind)(self & (OpCode)ExCode.mKind);
		public static OpKind Kind(this ExCode self)
			=> (OpKind)(self & ExCode.mKind);
		public static bool IsNumberOrChar(this ExCode self)
			=> (OpCode)self > OpCode.String && (OpCode)self < OpCode.Create;
		public static bool IsNumberOrChar(this OpCode self)
			=> self > OpCode.String && self < OpCode.Create;
		public static bool IsSigned(this ExCode self)
			=> (self & ExCode.fSig) != 0;
		public static bool IsFloatPoint(this ExCode self)
			=> (self & ExCode.fFp) != 0;
		public static byte NumberSize(this ExCode self)
			=> (byte)(((ushort)(self & ExCode.mSz)) >> 8);
		public static string Text(this OpCode self)
			=> _text[(byte)self];
		public static string Text(this ExCode self)
			=> _text[(byte)self];
		public static ExCode Extend(this OpCode self)
			=> (ExCode)((_info[(byte)self] << 8) | (byte)self);

		public static TypeFlags TypeFlags(this ExCode self)
		{
			switch (self.Kind())
			{
			case OpKind.Access:
				return (TypeFlags)(self.Code() & 15);
			case OpKind.Scope:
				return (TypeFlags)((self.Code() & 15) << 4);
			case OpKind.Mods:
				return (TypeFlags)(0x100 << (self.Code() & 15));
			default:
				return 0;
			}
		}

		public static bool IsLoop(this BlockCode self)
			=> self >= BlockCode.For && self <= BlockCode.DoUntil;
		public static BlockCode ToBlockCode(this OpCode self)
		{
			Debug.Assert(self >= OpCode.For && self <= OpCode.DoUntil);
			return (BlockCode)self;
		}

		private static readonly byte[] _info = new byte[] {
			// 0    1    2    3    4    5    6    7     8    9    A    B    C    D    E    F
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x01,0x02,0x04,0x00, //0 constants
			0x01,0x02,0x04,0x08,0x10,0x41,0x42,0x44, 0x48,0x50,0x01,0x10,0xC4,0xC8,0xCA,0x90, //1 numbers
			0x40,0x40,0x00,0x20,0x60,0x00,0x60,0x00, 0x20,0x60,0x60,0x60,0x02,0x00,0x00,0x20, //2 special
			0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01, 0x01,0x01,0x01,0x01,0x81,0x81,0x81,0x81, //3 assign
			0x81,0x0B,0x0B,0x0C,0x0C,0x0C,0x0D,0x0E, 0x0F,0x10,0x11,0x11,0x91,0x91,0x91,0x91, //4 binary
			0x03,0x04,0x09,0x09,0x0A,0x0A,0x0A,0x0A, 0x0A,0x0A,0x0A,0x0A,0x0A,0x0A,0x0A,0x0A, //5 logic + casts
			0x52,0x52,0x52,0x52,0xD2,0xD2,0xD2,0xD2, 0xD2,0xD2,0x52,0x52,0x52,0x52,0x52,0x52, //6 unary
			0x53,0x53,0xD3,0xD3,0xD3,0xD3,0xD3,0xD3, 0x54,0x54,0xD4,0xD4,0xD4,0xD4,0xD4,0xD4, //7 post/pre
			// the rest of this table must be zeros
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, //8 statements
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, //9 statements
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, //A access
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, //B scope
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, //C mods
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, //D model
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, //E unused
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00  //F meta
		};
		private static readonly string[] _text = new string[] {
		//	literals
			"void", "null", "false", "true", "this", "base", "value", null,
			"exception", "default", null, "string", "char", "wchar", "lchar", "number",
		//	numbers
			"byte", "ushort", "uint", "ulong", "quad", "sbyte", "short", "int",
			"long", "hyper", "bool", "complex", "float", "double", "long double", "decimal",
		//	special
			"new",  "()",   "()",   "()",   "()",   "[]",   "[]",   ".",
			"var",  ".[]",  "[]",   "??",   "?.",   "?.()", "?:",   "=>",
		//	assign
			"=",    "+=",   "-=",   "*=",   "/=",   "%=",   "**=",  "|=",
			"^=",   "&=",   "<<=",  ">>=",  null,   null,   null,   null,
		//	binary
			null,   "+",    "-",    "*",    "/",    "%",    "**",   "|",
			"^",	"&",    "<<",   ">>",   null,   null,   null,   null,
		//	logic + casts
			"||",   "&&",   "==",   "!=",   "<",    ">",    "<=",   ">=",
			"===",  "!==",  "as",   "as!",  "!",    "is",   "is!",  "in",
		//	unary
			"+",    "-",    "~",    "!",    "&",    "*",    "[]",   null,
			"typeof", "nameof", "await", null,  "new", "delete", "ref", "out",
		//	post/pre
			"++",   "--",   "++",   "--",   null,   null,   null,   null,
			null,   null,   null,   null,   null,   null,   null,   null,
		//	statements
			"{}", null, "return", "throw", "break", "continue", "for", "foreach",
			"while", "do", "until", "do-until", "if", "unless", "else", null,
			"pop", "yield", "goto", "case", "switch", null, "using", "catch",
			"with", "from", null,   null,   null,   null,   null,	null,
		//	access
			null, "public", "private", "protected",
			"internal", null, "private internal", "protected internal",
			null,   null,   null,   null,   null,   null,   null,   null,
		//	scope
			null, "final", "virtual", "abstract", "override",
			"readonly", "const", "static",
			"static readonly", null, null, null, null, null, null, null,
		//	mods
			"new", "partial", "get", "set", "async", null,  null,   null,
			null,   null,   null,   null,   null,   null,   null,   null,
		//	object model
			"import", "use static", "using", "namespace", "class", "struct", "enum", "interface",
			"mixin", "delegate", "@", "function", "operator", "field", "event", "property",
		//	tail
			null,   null,   null,   null,   null,   null,   null,   null,
			null,   null,   null,   null,   null,   null,   null,   null,
		//	meta (comments)
			null,   null,   null,   null,   null,   null,   null,   null,
			null,   null,   null,   null,   null,   null,   null,   null
		};
	}
}
