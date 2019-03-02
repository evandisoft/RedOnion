using Bee.Run;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bee
{
	public partial class Parser
	{
		private IGenerator _cgen;
		protected IGenerator Cgen
		{
			get
			{
				return _cgen;
			}
		}//Cgen
		
		public Parser( IGenerator cgen )
		{
			_cgen = cgen ?? new DummyGenerator();
		}//.ctor
		
		/// <summary>
		/// Callback from parser to generator
		/// </summary>
		public interface IGenerator
		{
			/// <summary>
			/// push simple value (this, null, false, ...)
			/// </summary>
			void Push( Opcode opcode );
			
			/// <summary>
			/// push string value (string, identifier)
			/// </summary>
			void Push( Opcode opcode, string @string );
			
			/// <summary>
			/// prepare top operator (prepare postfix record or expression tree node)
			/// </summary>
			void Prepare( Opcode opcode );
			
			/// <summary>
			/// start block of statements (make room fore size integer)
			/// </summary>
			int BlockStart(  );
			
			/// <summary>
			/// end block of statements (write size at proper position)
			/// @count number of statements in the block
			/// </summary>
			void BlockEnd( int @int, int count );
			
			/// <summary>
			/// start of full expression
			/// </summary>
			int ExprStart(  );
			
			/// <summary>
			/// end of full expression (rewrite and remove)
			/// </summary>
			void ExprEnd( int @int );
			
			/// <summary>
			/// start of type reference
			/// </summary>
			int TypeStart(  );
			
			/// <summary>
			/// end of type reference (rewrite and remove)
			/// </summary>
			void TypeEnd( int @int );
			
			/// <summary>
			/// start statement (write the code)
			/// </summary>
			int Write( Opcode opcode );
			
			/// <summary>
			/// change statement (patch do-until, for-in)
			/// </summary>
			void Write( Opcode opcode, int @int );
			
			/// <summary>
			/// write identifier (variable/type/namesapce) name
			/// </summary>
			void Ident( string @string );
			
			/// <summary>
			/// write statement with identifier (variable/type/namesapce) name
			/// (should most often be same as `write opcode; ident string`)
			/// </summary>
			void Ident( Opcode opcode, string @string );
			
			/// <summary>
			/// start of type definition (class, struct, enum, interface)
			/// </summary>
			int ClassStart( string name );
			
			/// <summary>
			/// end of type header, start of body (methods, properties, variables)
			/// </summary>
			int ClassBody( int @int, string name, Opcode opcode, int gtnum, int bcnum, Tflag tflag );
			
			/// <summary>
			/// end of type definition (class, struct, enum, interface)
			/// </summary>
			void ClassEnd( int @int, string name );
			
			/// <summary>
			/// start of field
			/// </summary>
			int FieldStart( string name );
			
			/// <summary>
			/// end of field
			/// </summary>
			void FieldEnd( int @int, string name, Opcode opcode, Tflag tflag );
			
			/// <summary>
			/// start of function or method
			/// </summary>
			int FuncStart( string fname );
			
			/// <summary>
			/// end of function type, start of argument/parameter list
			/// </summary>
			int FuncTypeEnd( int @int, string fname );
			
			/// <summary>
			/// add argument/parameter to function/method
			/// </summary>
			int FuncArg( int @int, string fname, int index, string aname );
			
			/// <summary>
			/// finish type, start default value
			/// </summary>
			int FuncArgDef( int fn, int arg, string fname, int index, string aname );
			
			/// <summary>
			/// finish argument/parameter
			/// </summary>
			void FuncArgEnd( int fn, int arg, string fname, int index, string aname );
			
			/// <summary>
			/// end of function header (arguments), start of body (statements)
			/// </summary>
			int FuncBody( int @int, string fname, int argc, Tflag tflag );
			
			/// <summary>
			/// end of function or method
			/// </summary>
			void FuncEnd( int @int, string fname );
			
			/// <summary>
			/// transition from function to property (after calling funcBody)		
			/// </summary>
			int Func2prop( int @int, string pname );
			
			/// <summary>
			/// start of field
			/// </summary>
			int PropFieldStart( int @int, string pname );
			
			/// <summary>
			/// end of field
			/// </summary>
			void PropFieldEnd( int pm, int @int, string pname );
			
			/// <summary>
			/// start of function or method
			/// </summary>
			int PropFuncStart( int @int, string pname, Opcode kind );
			
			/// <summary>
			/// end of function header (arguments), start of body (statements)
			/// </summary>
			int PropFuncBody( int pm, int @int, string pname, Opcode kind, Tflag tflag );
			
			/// <summary>
			/// end of function or method
			/// </summary>
			void PropFuncEnd( int pm, int @int, string pname, Opcode kind );
			
			/// <summary>
			/// end of property
			/// </summary>
			void PropEnd( int @int, string pname );
		}//IGenerator
		
		/// <summary>
		/// Dummy generator (if parsing only is needed)
		/// </summary>
		public class DummyGenerator: IGenerator
		{
			public void Push( Opcode opcode )
			{
			}//Push
			
			public void Push( Opcode opcode, string @string )
			{
			}//Push
			
			public void Prepare( Opcode opcode )
			{
			}//Prepare
			
			public int BlockStart(  )
			{
				return 0;
			}//BlockStart
			
			public void BlockEnd( int @int, int count )
			{
			}//BlockEnd
			
			public int ExprStart(  )
			{
				return 0;
			}//ExprStart
			
			public void ExprEnd( int @int )
			{
			}//ExprEnd
			
			public int TypeStart(  )
			{
				return 0;
			}//TypeStart
			
			public void TypeEnd( int @int )
			{
			}//TypeEnd
			
			public int Write( Opcode opcode )
			{
				return 0;
			}//Write
			
			public void Write( Opcode opcode, int @int )
			{
			}//Write
			
			public void Ident( string @string )
			{
			}//Ident
			
			public void Ident( Opcode opcode, string @string )
			{
			}//Ident
			
			public int ClassStart( string name )
			{
				return 0;
			}//ClassStart
			
			public int ClassBody( int @int, string name, Opcode opcode, int gtnum, int bcnum, Tflag tflag )
			{
				return 0;
			}//ClassBody
			
			public void ClassEnd( int @int, string name )
			{
			}//ClassEnd
			
			public int FieldStart( string name )
			{
				return 0;
			}//FieldStart
			
			public void FieldEnd( int @int, string name, Opcode opcode, Tflag tflag )
			{
			}//FieldEnd
			
			public int FuncStart( string fname )
			{
				return 0;
			}//FuncStart
			
			public int FuncTypeEnd( int @int, string fname )
			{
				return 0;
			}//FuncTypeEnd
			
			public int FuncArg( int @int, string fname, int index, string aname )
			{
				return 0;
			}//FuncArg
			
			public int FuncArgDef( int fn, int arg, string fname, int index, string aname )
			{
				return 0;
			}//FuncArgDef
			
			public void FuncArgEnd( int fn, int arg, string fname, int index, string aname )
			{
			}//FuncArgEnd
			
			public int FuncBody( int @int, string fname, int argc, Tflag tflag )
			{
				return 0;
			}//FuncBody
			
			public void FuncEnd( int @int, string fname )
			{
			}//FuncEnd
			
			public int Func2prop( int @int, string pname )
			{
				return 0;
			}//Func2prop
			
			public int PropFieldStart( int @int, string pname )
			{
				return 0;
			}//PropFieldStart
			
			public void PropFieldEnd( int pm, int @int, string pname )
			{
			}//PropFieldEnd
			
			public int PropFuncStart( int @int, string pname, Opcode kind )
			{
				return 0;
			}//PropFuncStart
			
			public int PropFuncBody( int pm, int @int, string pname, Opcode kind, Tflag tflag )
			{
				return 0;
			}//PropFuncBody
			
			public void PropFuncEnd( int pm, int @int, string pname, Opcode kind )
			{
			}//PropFuncEnd
			
			public void PropEnd( int @int, string pname )
			{
			}//PropEnd
		}//DummyGenerator
	}//Parser
}//Bee
