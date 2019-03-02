using System;
using System.IO;
using System.Text;
using Bee.Run;

namespace Bee
{
	public class AliasAttribute: Attribute
	{
		private string _names;
		public string Names
		{
			get
			{
				return _names;
			}
			set
			{
				_names = value;
			}
		}//Names
		
		public AliasAttribute( string names )
		{
			this.Names = names;
		}//.ctor
	}//AliasAttribute
	
	public static class Program
	{
		public static void Main( string[] args )
		{
			if( args.Length > 1 )
			{
				Console.WriteLine( "Sorry, not supported now" );
				return;
			}
			if( args.Length == 1 )
			{
				string script;
				using( var rd = new StreamReader( new FileStream( args[0], FileMode.Open, FileAccess.Read ) ) )
				{
					script = rd.ReadToEnd();
				}
				new Engine().Eval( script );
				return;
			}
			Console.WriteLine( "Bee Interactive Console" );
			Console.WriteLine( "Type 'return' or 'break' to exit" );
			var cgen = new CodeGenerator( Parser.Opt.Script, Parser.Opt.None );
			var eng = new Engine();
			var sb = new StringBuilder();
			for( ; ;  )
			{
				var more = sb.Length > 0;
				Console.Write( more ? "... " : ">>> " );
				var line = Console.ReadLine();
				sb.AppendLine( line );
				try
				{
					cgen.Unit( sb.ToString() );
				}
				catch( Exception exception )
				{
					if( cgen.Eof && (line != "") )
					{
						continue;
					}
					Console.WriteLine( "Exception: " + exception.Message );
				}
				var statement = (cgen.CodeAt > 0) && (cgen.Code[0] >= unchecked((byte)Opkind.Statement));
				if( ((statement && (line != "")) && (line != "return")) && (line != "break") )
				{
					continue;
				}
				sb.Length = 0;
				var code = cgen.ToArray();
				cgen.Reset();
				try
				{
					eng.Eval( code );
				}
				catch( Exception exception )
				{
					Console.WriteLine( "Exception: " + exception.Message );
					#if DEBUG
					Console.WriteLine( exception.ToString() );
					#endif
					continue;
				}
				if( eng.Exit != Opcode.Undef )
				{
					Console.WriteLine( "Exit code: {0}; Result: {1}", eng.Exit, eng.Result );
					#if DEBUG
					Console.Read();
					#endif
					return;
				}
				Console.WriteLine( eng.Result.String );
			}
		}//Main
	}//Program
}//Bee
