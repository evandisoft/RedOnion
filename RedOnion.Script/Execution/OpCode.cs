using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedOnion.Script
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

	public enum OpCode : ushort
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
		Undefined		= 0x0000, // undefined value
		Null			= 0x0001, // null/nullptr
		Object			= 0x0101, // object type
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
		Number			= 0x000F, // number as string (for code converters)

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
		Generic			= 0x6029, // t.[i]	generic type or call (any number of arguments)
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
		Def				= 0x01D4, // define class or method/lambda - short
		Define			= 0x02D4, // define class or method/lambda - long
		Struct			= 0x00D5, // value-type
		Enum			= 0x00D6, // integer with named values

		Interface		= 0x00D7, // interface
		Mixin			= 0x00D8, // mixin class/interface (to be implemented as composed-in-struct with redirections)
		Delegate		= 0x00D9, // delegate (function prototype)
		Attribute		= 0x00DA, // attribute

		Function		= 0x00DB, // method (function, procedure)
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
		public static bool Unused(this OpCode self)
			=> (self & OpCode.fUnused) != 0;
		public static bool Unary(this OpCode self)
			=> (self & OpCode.fMulti) == OpCode.fUnary;
		public static bool Binary(this OpCode self)
			=> (self & OpCode.fMulti) == 0;
		public static bool Ternary(this OpCode self)
			=> (self & OpCode.fMulti) == OpCode.fTernary;
		public static bool Multi(this OpCode self)
			=> (self & OpCode.fMulti) == OpCode.fMulti;
		public static bool Postfix(this OpCode self)
			=> (self & OpCode.mPriority) == (OpCode.PostInc & OpCode.mPriority);

		/// <summary>
		/// Operator priority (binary 'or', 'xor' and 'and' have higher priority than in C#)
		/// </summary>
		public static OpCode Priority(this OpCode self)
			=> self & OpCode.mPriority;
		/// <summary>
		/// Operator priority in C# (C/C++/Java and derivates - binary or/xor/and with same priority as logic)
		/// </summary>
		public static OpCode StdPriority(this OpCode self)
			=> self.Code() >= OpCode.BitOr.Code()
			&& self.Code() <= OpCode.BitAnd.Code()
			? self.Priority() - 0x0500
			: self.Code() == OpCode.Cast.Code()
			? 0 : self.Priority();

		public static byte Code(this OpCode self)
			=> (byte)self;
		public static OpKind Kind(this OpCode self)
			=> (OpKind)(self & OpCode.mKind);
		public static byte NumberSize(this OpCode self)
			=> (byte)(((ushort)(self & OpCode.mSz)) >> 8);
		public static string Text(this OpCode self)
			=> _text[(byte)self];
		public static OpCode Extend(this OpCode self)
			=> (OpCode)((_info[(byte)self] << 8) | (byte)self);

		public static TypeFlags TypeFlags(this OpCode self)
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
			"undefined", "null", "false", "true", "this", "base", "value", null,
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
