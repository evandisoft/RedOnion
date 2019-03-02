namespace Bee.Run
{
	public enum Tflag: ushort
	{
		None = 0x0000,
		Access = 0x000F,
		Public = 0x0001,
		Private = 0x0002,
		Protected = 0x0003,
		Internal = 0x0004,
		Iprivate = 0x0006,
		Iprotected = 0x0007,
		Scope = 0x00F0,
		Final = 0x0010,
		Virtual = 0x0020,
		Abstract = 0x0030,
		Override = 0x0040,
		Readonly = 0x0050,
		Const = 0x0060,
		Static = 0x0070,
		/// <summary>
		/// static readonly
		/// </summary>
		Rostatic = 0x0080,
		Other = 0x3F00,
		/// <summary>
		/// C#:new (hide inherited)
		/// </summary>
		Hide = 0x0100,
		/// <summary>
		/// partial class/struct/enum/interface
		/// </summary>
		Partial = 0x0200,
		/// <summary>
		/// unsafe context (allow pointers)
		/// </summary>
		Unsafe = 0x0400,
		/// <summary>
		/// operators only
		/// </summary>
		Implicit = 0x0800,
		/// <summary>
		/// methods (+def name inline = method group)
		/// </summary>
		Inline = 0x1000,
	}
	
	public static class TflagExtensions
	{
		public static Tflag Access( this Tflag self )
		{
			return self & Tflag.Access;
		}
		
		public static Tflag Scope( this Tflag self )
		{
			return self & Tflag.Scope;
		}
		
		public static string AccessText( this Tflag self )
		{
			return _access[((int)self.Access())];
		}
		
		public static string ScopeText( this Tflag self )
		{
			return _scope[((int)self.Scope()) >> 4];
		}
		
		private static string[] _access = new string[] {
			null,
			"public",
			"private",
			"protected",
			"internal",
			null,
			"private internal",
			"protected internal",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		private static string[] _scope = new string[] {
			null,
			"final",
			"virtual",
			"abstract",
			"override",
			"readonly",
			"const",
			"static",
			"static readonly",
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
	}
	
	public enum Opkind: byte
	{
		/// <summary>
		/// constants, (non-number) literals and references
		/// </summary>
		Literal = 0x00,
		/// <summary>
		/// numbers (int, byte, double, ...)
		/// </summary>
		Number = 0x10,
		/// <summary>
		/// special operators (call, index, new, ternary, ...)
		/// </summary>
		Special = 0x20,
		/// <summary>
		/// assign and compound-assignment operators
		/// </summary>
		Assign = 0x30,
		/// <summary>
		/// binary operators with compound-assignmnet counterpart
		/// </summary>
		Binary = 0x40,
		/// <summary>
		/// logic (boolean) binary operators
		/// </summary>
		Logic = 0x50,
		/// <summary>
		/// unary operators (except post/pre: + - ~ !)
		/// </summary>
		Unary = 0x60,
		/// <summary>
		/// pre- and post-fix operators (++ --)
		/// </summary>
		Prepost = 0x70,
		/// <summary>
		/// statements, loops, etc.
		/// </summary>
		Statement = 0x80,
		/// <summary>
		/// statement group 2: switch, try..catch..finaly, using
		/// </summary>
		Statement2 = 0x90,
		/// <summary>
		/// access modifiers (public, private, protected, internal)
		/// </summary>
		Access = 0xA0,
		/// <summary>
		/// scope modifiers (virtual, static, ...)
		/// </summary>
		Scope = 0xB0,
		/// <summary>
		/// other modifiers (new, partial)
		/// </summary>
		Mods = 0xC0,
		/// <summary>
		/// object model (classes, methods, fields, properties, attributes)
		/// </summary>
		Model = 0xD0,
		/// <summary>
		/// unused kind
		/// </summary>
		Reserved = 0xE0,
		/// <summary>
		/// comments and other metadata that can be ignored during execution
		/// </summary>
		Meta = 0xF0,
	}
	
	public enum Opcode: ushort
	{
		/// <summary>
		/// unused operator
		/// </summary>
		FUnused = 0x8000,
		/// <summary>
		/// unary operator
		/// </summary>
		FUnary = 0x4000,
		/// <summary>
		/// ternary operator
		/// </summary>
		FTernary = 0x2000,
		/// <summary>
		/// operator accepting multiple arguments (call, index)
		/// </summary>
		FMulti = 0x6000,
		/// <summary>
		/// operator priority
		/// </summary>
		MPriority = 0x1F00,
		/// <summary>
		/// code mask
		/// </summary>
		MCode = 0x00FF,
		/// <summary>
		/// kind mask
		/// </summary>
		MKind = 0x00F0,
		/// <summary>
		/// floating point (number section)
		/// </summary>
		FFp = 0x8000,
		/// <summary>
		/// signed
		/// </summary>
		FSig = 0x4000,
		/// <summary>
		/// number size mask
		/// </summary>
		MSz = 0x3F00,
		/// <summary>
		/// undefined value
		/// </summary>
		Undef = 0x0000,
		/// <summary>
		/// null/nullptr
		/// </summary>
		Null = 0x0001,
		/// <summary>
		/// object type
		/// </summary>
		Object = 0x0101,
		/// <summary>
		/// boolean false
		/// </summary>
		False = 0x0002,
		/// <summary>
		/// boolean true
		/// </summary>
		True = 0x0003,
		/// <summary>
		/// this (self) - preferred
		/// </summary>
		This = 0x0004,
		/// <summary>
		/// self (this) - alternative (used in extension methods)
		/// </summary>
		Self = 0x0104,
		/// <summary>
		/// base (super) - preferred
		/// </summary>
		Base = 0x0005,
		/// <summary>
		/// super (base) - alternative (Java)
		/// </summary>
		Super = 0x0105,
		/// <summary>
		/// value in setter
		/// </summary>
		Value = 0x0006,
		/// <summary>
		/// implicit value (in case of conditional switch)
		/// </summary>
		Ivalue = 0x0007,
		/// <summary>
		/// exception in catch block
		/// </summary>
		Exception = 0x0008,
		/// <summary>
		/// default value		+[len byte, type chars as UTF8 bytes]
		/// </summary>
		Default = 0x0009,
		/// <summary>
		/// identifier			+[len byte, chars as UTF8 bytes]
		/// </summary>
		Ident = 0x000A,
		/// <summary>
		/// string				+[len int/i7e, chars as UTF8 bytes]
		/// </summary>
		String = 0x000B,
		/// <summary>
		/// short char			+[byte]
		/// </summary>
		Char = 0x010C,
		/// <summary>
		/// wide char			+[2B char]
		/// </summary>
		Wchar = 0x020D,
		/// <summary>
		/// long char			+[4B char]
		/// </summary>
		Lchar = 0x040E,
		/// <summary>
		/// number as string (for converters)
		/// </summary>
		Number = 0x000F,
		/// <summary>
		/// u8
		/// </summary>
		Byte = 0x0110,
		/// <summary>
		/// u16
		/// </summary>
		Ushort = 0x0211,
		/// <summary>
		/// u32
		/// </summary>
		Uint = 0x0412,
		/// <summary>
		/// u64
		/// </summary>
		Ulong = 0x0813,
		/// <summary>
		/// s8
		/// </summary>
		Sbyte = 0x4114,
		/// <summary>
		/// s16
		/// </summary>
		Short = 0x4215,
		/// <summary>
		/// s32
		/// </summary>
		Int = 0x4416,
		/// <summary>
		/// s64
		/// </summary>
		Long = 0x4817,
		/// <summary>
		/// f32
		/// </summary>
		Float = 0xC418,
		/// <summary>
		/// f64
		/// </summary>
		Double = 0xC819,
		/// <summary>
		/// f80
		/// </summary>
		Ldouble = 0xCA1A,
		/// <summary>
		/// used only as type specifier (true/false used in expressions)
		/// </summary>
		Bool = 0x011B,
		/// <summary>
		/// reserved prefix for complex numbers
		/// </summary>
		Complex = 0x101C,
		/// <summary>
		/// f128
		/// </summary>
		Decimal = 0x901D,
		/// <summary>
		/// u128
		/// </summary>
		Quad = 0x101E,
		/// <summary>
		/// s128
		/// </summary>
		Hyper = 0x501F,
		/// <summary>
		/// new
		/// </summary>
		Create = 0x4020,
		/// <summary>
		/// f()		empty (no-argument) call
		/// </summary>
		Ecall = 0x4021,
		/// <summary>
		/// f(arg)	call with single argument
		/// </summary>
		Call = 0x0022,
		/// <summary>
		/// f(x,y)	call with two arguments
		/// </summary>
		Call2 = 0x2023,
		/// <summary>
		/// f(,,)	call with more than two arguments
		/// </summary>
		Mcall = 0x6024,
		/// <summary>
		/// a[i]	indexing with single argument
		/// </summary>
		Index = 0x0025,
		/// <summary>
		/// a[i,j]	indexing with more than one argument
		/// </summary>
		Mindex = 0x6026,
		/// <summary>
		/// .		(can work as left["right"] in scripting mode)
		/// </summary>
		Dot = 0x0027,
		/// <summary>
		/// var x[[:]type][=val] - moved to expressions that we can do if var... and for var...
		/// </summary>
		Var = 0x2028,
		/// <summary>
		/// t.[i]	generic type or call (any number of arguments)
		/// </summary>
		Generic = 0x6029,
		/// <summary>
		/// t[n]	array declaration or creation
		/// </summary>
		Array = 0x602A,
		/// <summary>
		/// ??		(one ?? two)
		/// </summary>
		Nullcol = 0x002B,
		/// <summary>
		/// ?.()
		/// </summary>
		Nullcall = 0x002C,
		/// <summary>
		/// ?.it
		/// </summary>
		Nulldot = 0x002D,
		/// <summary>
		/// ?:		(or: val1 if cond else val2)
		/// </summary>
		Ternary = 0x202E,
		/// <summary>
		/// =>		(reserved)
		/// </summary>
		Lambda = 0x802F,
		/// <summary>
		/// =
		/// </summary>
		Assign = 0x0130,
		/// <summary>
		/// |=
		/// </summary>
		OrAssign = 0x0131,
		/// <summary>
		/// ^=
		/// </summary>
		XorAssign = 0x0132,
		/// <summary>
		/// &=
		/// </summary>
		AndAssign = 0x0133,
		/// <summary>
		/// <<=
		/// </summary>
		LshAssign = 0x0134,
		/// <summary>
		/// >>=
		/// </summary>
		RshAssign = 0x0135,
		/// <summary>
		/// +=
		/// </summary>
		AddAssign = 0x0136,
		/// <summary>
		/// -=
		/// </summary>
		SubAssign = 0x0137,
		/// <summary>
		/// *=
		/// </summary>
		MulAssign = 0x0138,
		/// <summary>
		/// /=
		/// </summary>
		DivAssign = 0x0139,
		/// <summary>
		/// %=
		/// </summary>
		ModAssign = 0x013A,
		/// <summary>
		/// |		!! changed priority !!
		/// </summary>
		BitOr = 0x0941,
		/// <summary>
		/// ^		!! changed priority !!
		/// </summary>
		BitXor = 0x0D42,
		/// <summary>
		/// &		!! changed priority !!
		/// </summary>
		BitAnd = 0x0D43,
		/// <summary>
		/// <<
		/// </summary>
		ShiftLeft = 0x0A44,
		/// <summary>
		/// >>
		/// </summary>
		ShiftRight = 0x0A45,
		/// <summary>
		/// +
		/// </summary>
		Add = 0x0B46,
		/// <summary>
		/// -
		/// </summary>
		Sub = 0x0B47,
		/// <summary>
		/// *
		/// </summary>
		Mul = 0x0C48,
		/// <summary>
		/// /
		/// </summary>
		Div = 0x0C49,
		/// <summary>
		/// %
		/// </summary>
		Mod = 0x0C4A,
		/// <summary>
		/// **		(reserved)
		/// </summary>
		Power = 0x8D4B,
		/// <summary>
		/// ||
		/// </summary>
		LogicOr = 0x0250,
		/// <summary>
		/// &&
		/// </summary>
		LogicAnd = 0x0351,
		/// <summary>
		/// ==
		/// </summary>
		Equals = 0x0752,
		/// <summary>
		/// !=
		/// </summary>
		Differ = 0x0753,
		/// <summary>
		/// <
		/// </summary>
		Less = 0x0854,
		/// <summary>
		/// >
		/// </summary>
		More = 0x0855,
		/// <summary>
		/// <=
		/// </summary>
		Lesseq = 0x0856,
		/// <summary>
		/// >=
		/// </summary>
		Moreeq = 0x0857,
		/// <summary>
		/// ===
		/// </summary>
		Identity = 0x0858,
		/// <summary>
		/// !==
		/// </summary>
		NotIdentity = 0x0859,
		/// <summary>
		/// as
		/// </summary>
		As = 0x085A,
		/// <summary>
		/// as!
		/// </summary>
		Ascast = 0x085B,
		/// <summary>
		/// !		(type! value)
		/// </summary>
		Cast = 0x085C,
		/// <summary>
		/// is
		/// </summary>
		Is = 0x085D,
		/// <summary>
		/// is!
		/// </summary>
		Isnot = 0x085E,
		/// <summary>
		/// +x
		/// </summary>
		Plus = 0x4E60,
		/// <summary>
		/// -x
		/// </summary>
		Neg = 0x4E61,
		/// <summary>
		/// ~x
		/// </summary>
		Flip = 0x4E62,
		/// <summary>
		/// !x
		/// </summary>
		Not = 0x4E63,
		/// <summary>
		/// &x		(reserved)
		/// </summary>
		Addrof = 0xCE64,
		/// <summary>
		/// *x		(reserved)
		/// </summary>
		Deref = 0xCE65,
		/// <summary>
		/// typeof x
		/// </summary>
		Typeof = 0xCE68,
		/// <summary>
		/// nameof x
		/// </summary>
		Nameof = 0xCE69,
		/// <summary>
		/// await task
		/// </summary>
		Await = 0x406A,
		/// <summary>
		/// delete
		/// </summary>
		Delete = 0x406D,
		/// <summary>
		/// ref
		/// </summary>
		Ref = 0x406E,
		/// <summary>
		/// out
		/// </summary>
		Out = 0x406F,
		/// <summary>
		/// x++
		/// </summary>
		PostInc = 0x4F70,
		/// <summary>
		/// x--
		/// </summary>
		PostDec = 0x4F71,
		/// <summary>
		/// ++x
		/// </summary>
		Inc = 0x5078,
		/// <summary>
		/// --x
		/// </summary>
		Dec = 0x5079,
		/// <summary>
		/// block with its own scope
		/// </summary>
		Block = 0x0080,
		/// <summary>
		/// return
		/// </summary>
		Return = 0x0082,
		/// <summary>
		/// throw/raise (Python uses raise)
		/// </summary>
		Raise = 0x0083,
		/// <summary>
		/// throw (alternative to throw from Python)
		/// </summary>
		Throw = 0x0183,
		/// <summary>
		/// break
		/// </summary>
		Break = 0x0084,
		/// <summary>
		/// continue
		/// </summary>
		Continue = 0x0085,
		/// <summary>
		/// for init; test; iter: stts
		/// </summary>
		For = 0x0086,
		/// <summary>
		/// for var in list: stts
		/// </summary>
		Foreach = 0x0087,
		/// <summary>
		/// (for parsing only)
		/// </summary>
		In = 0x0187,
		/// <summary>
		/// while cond do stts
		/// </summary>
		While = 0x0088,
		/// <summary>
		/// do stts; while cond
		/// </summary>
		Do = 0x0089,
		/// <summary>
		/// until cond do stts  
		/// </summary>
		Until = 0x008A,
		/// <summary>
		/// do stts; until cond
		/// </summary>
		Dountil = 0x008B,
		/// <summary>
		/// if cond then truestts
		/// </summary>
		If = 0x008C,
		/// <summary>
		/// (for parsing only)
		/// </summary>
		Then = 0x018C,
		/// <summary>
		/// unless cond do falsestts
		/// </summary>
		Unless = 0x008D,
		/// <summary>
		/// if cond then truestts else falsestts
		/// </summary>
		Else = 0x008E,
		/// <summary>
		/// goto label
		/// </summary>
		Goto = 0x0090,
		/// <summary>
		/// label:   (for 1st phase of parsing/compilation)
		/// </summary>
		Label = 0x0190,
		/// <summary>
		/// goto case
		/// </summary>
		Cgoto = 0x0091,
		/// <summary>
		/// case 0:  (for parsing only)
		/// </summary>
		Case = 0x0191,
		/// <summary>
		/// switch x: case 1: break; case 2: break; default: continue
		/// </summary>
		Switch = 0x0092,
		/// <summary>
		/// switch x: case < 0: ...; case 0: ...; case > 0: ...
		/// </summary>
		Cswitch = 0x0093,
		/// <summary>
		/// using macro (try..finally dispose)
		/// </summary>
		Using = 0x0094,
		/// <summary>
		/// 'using' for script
		/// </summary>
		Let = 0x0194,
		/// <summary>
		/// try..catch..finally
		/// </summary>
		Catch = 0x0095,
		/// <summary>
		/// (for parsing only)
		/// </summary>
		Try = 0x0195,
		/// <summary>
		/// (for parsing only)
		/// </summary>
		Finally = 0x0295,
		/// <summary>
		/// catch from Python
		/// </summary>
		Except = 0x0395,
		/// <summary>
		/// with var do stts
		/// </summary>
		With = 0x0096,
		/// <summary>
		/// LINQ
		/// </summary>
		From = 0x0097,
		/// <summary>
		/// (for parsing only)
		/// </summary>
		Select = 0x0197,
		/// <summary>
		/// (for parsing only)
		/// </summary>
		Orderby = 0x0297,
		Public = 0x00A1,
		Private = 0x00A2,
		Protected = 0x00A3,
		Internal = 0x00A4,
		Iprivate = 0x00A6,
		Iprotected = 0x00A7,
		/// <summary>
		/// final (Java), sealed (C#), NotInheritable/NotOverridable (Basic)
		/// </summary>
		Final = 0x00B1,
		/// <summary>
		/// final - alternative (C#)
		/// </summary>
		Sealed = 0x01B1,
		Virtual = 0x00B2,
		/// <summary>
		/// abstract (C#/Java), MustInherit/MustOverride (Basic)
		/// </summary>
		Abstract = 0x00B3,
		Override = 0x00B4,
		Readonly = 0x00B5,
		Const = 0x00B6,
		Static = 0x00B7,
		/// <summary>
		/// static readonly
		/// </summary>
		Rostatic = 0x00B8,
		/// <summary>
		/// new
		/// </summary>
		Hide = 0x00C0,
		Partial = 0x00C1,
		Unsafe = 0x00C2,
		Async = 0x00C3,
		/// <summary>
		/// import (Java/Python), use (B#), using (C#), include, ...
		/// </summary>
		Import = 0x00D0,
		/// <summary>
		/// import - alternative (C++ Preprocessor)
		/// </summary>
		Include = 0x01D0,
		/// <summary>
		/// import - alternative (B#)
		/// </summary>
		Use = 0x02D0,
		/// <summary>
		/// using static (C#; Java: import SomeClass.*)
		/// </summary>
		UseAll = 0x00D1,
		/// <summary>
		/// using name = ns.whatever
		/// </summary>
		UseAs = 0x00D2,
		/// <summary>
		/// namespace (C#), package (Java), Module in Visual Basic is a bit different
		/// </summary>
		[Alias("namespace")]
		Space = 0x00D3,
		/// <summary>
		/// namespace - alias (Java)
		/// </summary>
		Package = 0x01D3,
		/// <summary>
		/// namespace - alias (short)
		/// </summary>
		Pkg = 0x02D3,
		/// <summary>
		/// ref-class
		/// </summary>
		Class = 0x00D4,
		/// <summary>
		/// define class or method/lambda - short
		/// </summary>
		Def = 0x01D4,
		/// <summary>
		/// define class or method/lambda - long
		/// </summary>
		Define = 0x02D4,
		/// <summary>
		/// value-type
		/// </summary>
		Struct = 0x00D5,
		/// <summary>
		/// integer with named values
		/// </summary>
		Enum = 0x00D6,
		/// <summary>
		/// interface
		/// </summary>
		[Alias("interface")]
		Face = 0x00D7,
		/// <summary>
		/// mixin class/interface (to be implemented as composed-in-struct with redirections)
		/// </summary>
		Mixin = 0x00D8,
		/// <summary>
		/// delegate (function prototype)
		/// </summary>
		Delegate = 0x00D9,
		/// <summary>
		/// attribute
		/// </summary>
		Attr = 0x00DA,
		/// <summary>
		/// method (function, procedure)
		/// </summary>
		Func = 0x00DB,
		/// <summary>
		/// operator (type conversion or +-*/% ...)
		/// </summary>
		Operator = 0x00DC,
		/// <summary>
		/// var in class
		/// </summary>
		Field = 0x00DD,
		/// <summary>
		/// event field
		/// </summary>
		Event = 0x00DE,
		/// <summary>
		/// property (get-set or add-remove method pair)
		/// </summary>
		Prop = 0x00DF,
		/// <summary>
		/// get (property)
		/// </summary>
		Get = 0x01DF,
		/// <summary>
		/// set (property)
		/// </summary>
		Set = 0x02DF,
		/// <summary>
		/// add/combine (event/delegate)
		/// </summary>
		Combine = 0x03DF,
		/// <summary>
		/// remove (event/delegate)
		/// </summary>
		Remove = 0x04DF,
		/// <summary>
		/// where rules
		/// </summary>
		Where = 0x00FB,
		/// <summary>
		/// line comment
		/// </summary>
		Comment = 0x00FC,
		/// <summary>
		/// documenting comment
		/// </summary>
		Doc = 0x00FD,
		/// <summary>
		/// multi-line comment
		/// </summary>
		Mlcomm = 0x00FE,
		/// <summary>
		/// white space
		/// </summary>
		White = 0x00FF,
		Unknown = 0xFFFF,
		Comma = 0xFEFF,
	}
	
	public static class OpcodeExtensions
	{
		public static bool Unused( this Opcode self )
		{
			return (self & Opcode.FUnused) != 0;
		}
		
		public static bool Unary( this Opcode self )
		{
			return (self & Opcode.FMulti) == Opcode.FUnary;
		}
		
		public static bool Binary( this Opcode self )
		{
			return (self & Opcode.FMulti) == 0;
		}
		
		public static bool Ternary( this Opcode self )
		{
			return (self & Opcode.FMulti) == Opcode.FTernary;
		}
		
		public static bool Multi( this Opcode self )
		{
			return (self & Opcode.FMulti) == Opcode.FMulti;
		}
		
		public static bool Postfix( this Opcode self )
		{
			return (self & Opcode.MPriority) == (Opcode.PostInc & Opcode.MPriority);
		}
		
		/// <summary>
		/// operator priority in B# ('or', 'xor' and 'and' have higher priority than in C#)
		/// </summary>
		public static Opcode Prior( this Opcode self )
		{
			return self & Opcode.MPriority;
		}
		
		/// <summary>
		/// operator priority in C# (C/C++/Java and derivates - binary or/xor/and with same priority as logic)
		/// </summary>
		public static Opcode Cprior( this Opcode self )
		{
			return (self.Code() >= Opcode.BitOr.Code()) && (self.Code() <= Opcode.BitAnd.Code()) ? ((Opcode)(((self.Code() & 7) + 3) << 8)) : self.Code() == Opcode.Cast.Code() ? 0 : self.Prior();
		}
		
		public static byte Code( this Opcode self )
		{
			return unchecked((byte)self);
		}
		
		public static Opkind Kind( this Opcode self )
		{
			return ((Opkind)(self & Opcode.MKind));
		}
		
		public static byte Numsz( this Opcode self )
		{
			return unchecked((byte)(((ushort)(self & Opcode.MSz)) >> 8));
		}
		
		public static string Text( this Opcode self )
		{
			return _text[unchecked((byte)self)];
		}
		
		public static Opcode Extend( this Opcode self )
		{
			return ((Opcode)((_info[unchecked((byte)self)] << 8) | unchecked((byte)self)));
		}
		
		public static Tflag Tflag( this Opcode self )
		{
			switch (self.Kind())
			{
			case Opkind.Access:
				return ((Tflag)(self.Code() & 15));
			case Opkind.Scope:
				return ((Tflag)((self.Code() & 15) << 4));
			case Opkind.Mods:
				return ((Tflag)(0x100 << (self.Code() & 15)));
			default:
				return 0;
			}
		}
		
		private static byte[] _info = new byte[] {
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x01,
			0x02,
			0x04,
			0x00,
			0x01,
			0x02,
			0x04,
			0x08,
			0x41,
			0x42,
			0x44,
			0x48,
			0xC4,
			0xC8,
			0xCA,
			0x01,
			0x10,
			0x90,
			0x10,
			0x50,
			0x40,
			0x40,
			0x00,
			0x20,
			0x60,
			0x00,
			0x60,
			0x00,
			0x20,
			0x60,
			0x60,
			0x00,
			0x00,
			0x00,
			0x20,
			0x80,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x81,
			0x81,
			0x81,
			0x81,
			0x81,
			0x81,
			0x09,
			0x0D,
			0x0D,
			0x0A,
			0x0A,
			0x0B,
			0x0B,
			0x0C,
			0x0C,
			0x0C,
			0x8D,
			0x8D,
			0x8D,
			0x08,
			0x08,
			0x02,
			0x03,
			0x07,
			0x07,
			0x08,
			0x08,
			0x08,
			0x08,
			0x08,
			0x08,
			0x08,
			0x08,
			0x08,
			0x08,
			0x08,
			0x88,
			0x4E,
			0x4E,
			0x4E,
			0x4E,
			0xCE,
			0xCE,
			0x4F,
			0xCE,
			0xCE,
			0xCE,
			0x40,
			0xC0,
			0xC0,
			0x40,
			0x40,
			0x40,
			0x4F,
			0x4F,
			0xCF,
			0xCF,
			0xCF,
			0xCF,
			0xCF,
			0xCF,
			0x50,
			0x50,
			0x90,
			0x90,
			0x90,
			0x90,
			0x90,
			0x90,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00
		};
		private static string[] _text = new string[] {
			"undefined",
			"null",
			"false",
			"true",
			"this",
			"base",
			"value",
			null,
			"exception",
			"default",
			null,
			"string",
			"char",
			"wchar",
			"lchar",
			"number",
			"byte",
			"ushort",
			"uint",
			"ulong",
			"sbyte",
			"short",
			"int",
			"long",
			"float",
			"double",
			"long double",
			"bool",
			"complex",
			"decimal",
			"quad",
			"hyper",
			"new",
			"()",
			"()",
			"()",
			"()",
			"[]",
			"[]",
			".",
			"var",
			".[]",
			"[]",
			"??",
			"?.",
			"?.()",
			"?:",
			"=>",
			"=",
			"|=",
			"^=",
			"&=",
			"<<=",
			">>=",
			"+=",
			"-=",
			"*=",
			"/=",
			"%=",
			null,
			null,
			null,
			null,
			"var",
			null,
			"|",
			"^",
			"&",
			"<<",
			">>",
			"+",
			"-",
			"*",
			"/",
			"%",
			"**",
			null,
			null,
			"as",
			"as!",
			"||",
			"&&",
			"==",
			"!=",
			"<",
			">",
			"<=",
			">=",
			"===",
			"!==",
			"as",
			"as!",
			"!",
			"is",
			"is!",
			null,
			"+",
			"-",
			"~",
			"!",
			"&",
			"*",
			"[]",
			null,
			"typeof",
			"nameof",
			"await",
			null,
			"new",
			"delete",
			"ref",
			"out",
			"++",
			"--",
			"++",
			"--",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"{}",
			null,
			"return",
			"throw",
			"break",
			"continue",
			"for",
			"foreach",
			"while",
			"do",
			"until",
			"do-until",
			"if",
			"unless",
			"else",
			"with",
			"switch",
			"case",
			"default",
			"label",
			"goto",
			"using",
			"catch",
			"from",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"public",
			"private",
			"protected",
			"internal",
			null,
			"private internal",
			"protected internal",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"final",
			"virtual",
			"abstract",
			"override",
			"readonly",
			"const",
			"static",
			"static readonly",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"new",
			"partial",
			"get",
			"set",
			"async",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			"import",
			"use static",
			"using",
			"namespace",
			"class",
			"struct",
			"enum",
			"interface",
			"mixin",
			"delegate",
			"@",
			"function",
			"operator",
			"field",
			"event",
			"property",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
	}
}
