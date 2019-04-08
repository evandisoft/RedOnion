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
		String			= 0x0B, // string			+[index to string table]
		Char			= 0x0C, // short char		+[byte]
		WideChar		= 0x0D, // wide char		+[2B char]
		LongChar		= 0x0E, // long char		+[4B char]
		Number			= 0x0F, // generic number (useful for conversions)

		Byte			= 0x10, // u8
		UShort			= 0x11, // u16
		UInt			= 0x12, // u32
		ULong			= 0x13, // u64
		SByte			= 0x14, // s8
		Short			= 0x15, // s16
		Int				= 0x16, // s32
		Long			= 0x17, // s64
		Float			= 0x18, // f32
		Double			= 0x19, // f64
		LongDouble		= 0x1A, // f80
		Bool			= 0x1B, // used only as type specifier (true/false used in expressions)
		Complex			= 0x1C, // reserved for complex numbers
		Decimal			= 0x1D, // f128
		Quad			= 0x1E, // u128
		Hyper			= 0x1F, // s128

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
		NullCol			= 0x2B, // ??		(one ?? two)
		NullCall		= 0x2C, // ?.()
		NullDot			= 0x2D, // ?.it
		Ternary			= 0x2E, // ?:		(or: val1 if cond else val2)
		Lambda			= 0x2F, // =>		(reserved)

	//	assign
		Assign			= 0x30, // =
		OrAssign		= 0x31, // |=
		XorAssign		= 0x32, // ^=
		AndAssign		= 0x33, // &=
		LshAssign		= 0x34, // <<=
		RshAssign		= 0x35, // >>=
		AddAssign		= 0x36, // +=
		SubAssign		= 0x37, // -=
		MulAssign		= 0x38, // *=
		DivAssign		= 0x39, // /=
		ModAssign		= 0x3A, // %=

	//	binary
		BitOr			= 0x41, // |
		BitXor			= 0x42, // ^
		BitAnd			= 0x43, // &
		ShiftLeft		= 0x44, // <<
		ShiftRight		= 0x45, // >>
		Add				= 0x46, // +
		Sub				= 0x47, // -
		Mul				= 0x48, // *
		Div				= 0x49, // /
		Mod				= 0x4A, // %
		Power			= 0x4B, // **		(reserved)

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
		Return			= 0x82, // return
		Raise			= 0x83, // throw/raise (Python uses raise)
		Break			= 0x84, // break
		Continue		= 0x85, // continue
		For				= 0x86, // for init; test; iter: stts
		ForEach			= 0x87, // for var in list: stts
		While			= 0x88, // while cond do stts
		Do				= 0x89, // do stts; while cond
		Until			= 0x8A, // until cond do stts  
		DoUntil			= 0x8B, // do stts; until cond
		If				= 0x8C, // if cond then truestts
		Unless			= 0x8D, // unless cond do falsestts
		Else			= 0x8E, // if cond then truestts else falsestts

		Goto			= 0x90, // goto label
		GotoCase		= 0x91, // goto case
		Switch			= 0x92, // switch x: case 1: break; case 2: break; default: continue
		SwitchCond		= 0x93, // switch x: case < 0: ...; case 0: ...; case > 0: ...
		Using			= 0x94, // using macro (try..finally dispose)
		Catch			= 0x95, // try..catch..finally
		With			= 0x96, // with var do stts
		From			= 0x97, // LINQ

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
		SByte			= 0x4114, // s8
		Short			= 0x4215, // s16
		Int				= 0x4416, // s32
		Long			= 0x4817, // s64
		Float			= 0xC418, // f32
		Double			= 0xC819, // f64
		LongDouble		= 0xCA1A, // f80
		Bool			= 0x011B, // used only as type specifier (true/false used in expressions)
		Complex			= 0x101C, // reserved for complex numbers
		Decimal			= 0x901D, // f128
		Quad			= 0x101E, // u128
		Hyper			= 0x501F, // s128

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
		NullCol			= 0x002B, // ??		(one ?? two)
		NullCall		= 0x002C, // ?.()
		NullDot			= 0x002D, // ?.it
		Ternary			= 0x202E, // ?:		(or: val1 if cond else val2)
		Lambda			= 0x802F, // =>		(reserved)

	//	assign
		Assign			= 0x0130, // =
		OrAssign		= 0x0131, // |=
		XorAssign		= 0x0132, // ^=
		AndAssign		= 0x0133, // &=
		LshAssign		= 0x0134, // <<=
		RshAssign		= 0x0135, // >>=
		AddAssign		= 0x0136, // +=
		SubAssign		= 0x0137, // -=
		MulAssign		= 0x0138, // *=
		DivAssign		= 0x0139, // /=
		ModAssign		= 0x013A, // %=

	//	binary
		BitOr			= 0x0941, // |		!! changed priority !! 4 in C#
		BitXor			= 0x0A42, // ^		!! changed priority !! 5 in C#
		BitAnd			= 0x0B43, // &		!! changed priority !! 6 in C#
		ShiftLeft		= 0x0C44, // <<
		ShiftRight		= 0x0C45, // >>
		Add				= 0x0D46, // +
		Sub				= 0x0D47, // -
		Mul				= 0x0E48, // *
		Div				= 0x0E49, // /
		Mod				= 0x0E4A, // %
		Power			= 0x8F4B, // **		(reserved)

	//	logic + casts
		LogicOr			= 0x0250, // ||
		LogicAnd		= 0x0351, // &&
	//	priority 4..6 reserved for C# binary or, xor, and
		Equals			= 0x0752, // ==
		Differ			= 0x0753, // !=
		Less			= 0x0854, // <
		More			= 0x0855, // >
		LessEq			= 0x0856, // <=
		MoreEq			= 0x0857, // >=
		Identity		= 0x0858, // ===
		NotIdentity		= 0x0859, // !==

		As				= 0x085A, // as
		AsCast			= 0x085B, // as!
		Cast			= 0x085C, // !		(type! value)
		Is				= 0x085D, // is
		IsNot			= 0x085E, // is!
	
	//	unary
		Plus			= 0x4E60, // +x
		Neg				= 0x4E61, // -x
		Flip			= 0x4E62, // ~x
		Not				= 0x4E63, // !x
		AddrOf			= 0xCE64, // &x		(reserved)
		Deref			= 0xCE65, // *x		(reserved)

		TypeOf			= 0xCE68, // typeof x
		NameOf			= 0xCE69, // nameof x
		Await			= 0x406A, // await task
		Delete			= 0x406D, // delete
		Ref				= 0x406E, // ref
		Out				= 0x406F, // out

	//	unary post/pre
		PostInc			= 0x5070, // x++
		PostDec			= 0x5071, // x--
		Inc				= 0x5178, // ++x
		Dec				= 0x5179, // --x

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
		In				= 0x0187, // (for parsing only)
		While			= 0x0088, // while cond do stts
		Do				= 0x0089, // do stts; while cond
		Until			= 0x008A, // until cond do stts  
		DoUntil			= 0x008B, // do stts; until cond
		If				= 0x008C, // if cond then truestts
		Then			= 0x018C, // (for parsing only)
		Unless			= 0x008D, // unless cond do falsestts
		Else			= 0x008E, // if cond then truestts else falsestts

		Goto			= 0x0090, // goto label
		Label			= 0x0190, // label:   (for 1st phase of parsing/compilation)
		GotoCase		= 0x0091, // goto case
		Case			= 0x0191, // case 0:  (for parsing only)
		Switch			= 0x0092, // switch x: case 1: break; case 2: break; default: continue
		SwitchCond		= 0x0093, // switch x: case < 0: ...; case 0: ...; case > 0: ...
		Using			= 0x0094, // using macro (try..finally dispose)
		Catch			= 0x0095, // try..catch..finally
		Try				= 0x0195, // (for parsing only)
		Finally			= 0x0295, // (for parsing only)
		Except			= 0x0395, // catch from Python
		With			= 0x0096, // with var do stts
		From			= 0x0097, // LINQ
		Select			= 0x0197, // (for parsing only)
		OrderBy			= 0x0297, // (for parsing only)

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
			&& self.Code() <= ExCode.BitAnd.Code()
			? self.Priority() - 0x0500
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

		private static readonly byte[] _info = new byte[] {
			// 0    1    2    3    4    5    6    7     8    9    A    B    C    D    E    F
			0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x01,0x02,0x04,0x00, //0 constants
			0x01,0x02,0x04,0x08,0x41,0x42,0x44,0x48, 0xC4,0xC8,0xCA,0x01,0x10,0x90,0x10,0x50, //1 numbers
			0x40,0x40,0x00,0x20,0x60,0x00,0x60,0x00, 0x20,0x60,0x60,0x00,0x00,0x00,0x20,0x80, //2 special
			0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01, 0x01,0x01,0x01,0x81,0x81,0x81,0x81,0x81, //3 assign
			0x81,0x09,0x0A,0x0B,0x0C,0x0C,0x0D,0x0D, 0x0E,0x0E,0x0E,0x8F,0x8F,0x8F,0x8F,0x8F, //4 binary
			0x02,0x03,0x07,0x07,0x08,0x08,0x08,0x08, 0x08,0x08,0x08,0x08,0x08,0x08,0x08,0x88, //5 logic + casts
			0x4E,0x4E,0x4E,0x4E,0xCE,0xCE,0xCE,0xCE, 0xCE,0xCE,0x40,0xC0,0xC0,0x40,0x40,0x40, //6 unary
			0x50,0x50,0xD0,0xD0,0xD0,0xD0,0xD0,0xD0, 0x51,0x51,0xD1,0xD1,0xD1,0xD1,0xD1,0xD1, //7 post/pre
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
			"byte", "ushort", "uint", "ulong", "sbyte", "short", "int", "long",
			"float", "double", "long double", "bool", "complex", "decimal", "quad", "hyper",
		//	special
			"new",  "()",   "()",   "()",   "()",   "[]",   "[]",   ".",
			"var",  ".[]",  "[]",   "??",   "?.",   "?.()", "?:",   "=>",
		//	assign
			"=",    "|=",   "^=",   "&=",   "<<=",  ">>=",  "+=",   "-=",
			"*=",   "/=",   "%=",   null,   null,   null,   null,   "var",
		//	binary
			null,   "|",    "^",    "&",    "<<",   ">>",   "+",    "-",
			"*",    "/",    "%",    "**",   null,   null,   "as",   "as!",
		//	logic + casts
			"||",   "&&",   "==",   "!=",   "<",    ">",    "<=",   ">=",
			"===",  "!==",  "as",   "as!",  "!",    "is",   "is!",  null,
		//	unary
			"+",    "-",    "~",    "!",    "&",    "*",    "[]",   null,
			"typeof", "nameof", "await", null,  "new", "delete", "ref", "out",
		//	post/pre
			"++",   "--",   "++",   "--",   null,   null,   null,   null,
			null,   null,   null,   null,   null,   null,   null,   null,
		//	statements
			"{}", null, "return", "throw", "break", "continue", "for", "foreach",
			"while", "do", "until", "do-until", "if", "unless", "else", "with",
			"switch", "case", "default", "label", "goto", "using", "catch", "from",
			null,   null,   null,   null,   null,   null,   null,   null,
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
