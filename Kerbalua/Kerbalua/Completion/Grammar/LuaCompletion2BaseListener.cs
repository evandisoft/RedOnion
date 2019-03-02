//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from LuaCompletion2.g4 by ANTLR 4.7.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ILuaCompletion2Listener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.1")]
[System.CLSCompliant(false)]
public partial class LuaCompletion2BaseListener : ILuaCompletion2Listener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.chunk"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterChunk([NotNull] LuaCompletion2Parser.ChunkContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.chunk"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitChunk([NotNull] LuaCompletion2Parser.ChunkContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.block"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBlock([NotNull] LuaCompletion2Parser.BlockContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.block"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBlock([NotNull] LuaCompletion2Parser.BlockContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.stat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStat([NotNull] LuaCompletion2Parser.StatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.stat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStat([NotNull] LuaCompletion2Parser.StatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.retstat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRetstat([NotNull] LuaCompletion2Parser.RetstatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.retstat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRetstat([NotNull] LuaCompletion2Parser.RetstatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.label"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLabel([NotNull] LuaCompletion2Parser.LabelContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.label"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLabel([NotNull] LuaCompletion2Parser.LabelContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.funcname"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFuncname([NotNull] LuaCompletion2Parser.FuncnameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.funcname"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFuncname([NotNull] LuaCompletion2Parser.FuncnameContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.varlist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVarlist([NotNull] LuaCompletion2Parser.VarlistContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.varlist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVarlist([NotNull] LuaCompletion2Parser.VarlistContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.namelist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNamelist([NotNull] LuaCompletion2Parser.NamelistContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.namelist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNamelist([NotNull] LuaCompletion2Parser.NamelistContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.explist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExplist([NotNull] LuaCompletion2Parser.ExplistContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.explist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExplist([NotNull] LuaCompletion2Parser.ExplistContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.exp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExp([NotNull] LuaCompletion2Parser.ExpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.exp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExp([NotNull] LuaCompletion2Parser.ExpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.prefixexp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrefixexp([NotNull] LuaCompletion2Parser.PrefixexpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.prefixexp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrefixexp([NotNull] LuaCompletion2Parser.PrefixexpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.functioncall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFunctioncall([NotNull] LuaCompletion2Parser.FunctioncallContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.functioncall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFunctioncall([NotNull] LuaCompletion2Parser.FunctioncallContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.varOrExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVarOrExp([NotNull] LuaCompletion2Parser.VarOrExpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.varOrExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVarOrExp([NotNull] LuaCompletion2Parser.VarOrExpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.var"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVar([NotNull] LuaCompletion2Parser.VarContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.var"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVar([NotNull] LuaCompletion2Parser.VarContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.varSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVarSuffix([NotNull] LuaCompletion2Parser.VarSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.varSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVarSuffix([NotNull] LuaCompletion2Parser.VarSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.terminalVar"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerminalVar([NotNull] LuaCompletion2Parser.TerminalVarContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.terminalVar"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerminalVar([NotNull] LuaCompletion2Parser.TerminalVarContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.terminalVarSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerminalVarSuffix([NotNull] LuaCompletion2Parser.TerminalVarSuffixContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.terminalVarSuffix"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerminalVarSuffix([NotNull] LuaCompletion2Parser.TerminalVarSuffixContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.nameAndArgs"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNameAndArgs([NotNull] LuaCompletion2Parser.NameAndArgsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.nameAndArgs"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNameAndArgs([NotNull] LuaCompletion2Parser.NameAndArgsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.newExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNewExp([NotNull] LuaCompletion2Parser.NewExpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.newExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNewExp([NotNull] LuaCompletion2Parser.NewExpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.arrayAccessExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArrayAccessExp([NotNull] LuaCompletion2Parser.ArrayAccessExpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.arrayAccessExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArrayAccessExp([NotNull] LuaCompletion2Parser.ArrayAccessExpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.args"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArgs([NotNull] LuaCompletion2Parser.ArgsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.args"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArgs([NotNull] LuaCompletion2Parser.ArgsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.functiondef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFunctiondef([NotNull] LuaCompletion2Parser.FunctiondefContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.functiondef"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFunctiondef([NotNull] LuaCompletion2Parser.FunctiondefContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.funcbody"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFuncbody([NotNull] LuaCompletion2Parser.FuncbodyContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.funcbody"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFuncbody([NotNull] LuaCompletion2Parser.FuncbodyContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.parlist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParlist([NotNull] LuaCompletion2Parser.ParlistContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.parlist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParlist([NotNull] LuaCompletion2Parser.ParlistContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.tableconstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTableconstructor([NotNull] LuaCompletion2Parser.TableconstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.tableconstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTableconstructor([NotNull] LuaCompletion2Parser.TableconstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.fieldlist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldlist([NotNull] LuaCompletion2Parser.FieldlistContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.fieldlist"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldlist([NotNull] LuaCompletion2Parser.FieldlistContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.field"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterField([NotNull] LuaCompletion2Parser.FieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.field"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitField([NotNull] LuaCompletion2Parser.FieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.terminalField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTerminalField([NotNull] LuaCompletion2Parser.TerminalFieldContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.terminalField"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTerminalField([NotNull] LuaCompletion2Parser.TerminalFieldContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.fieldsep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFieldsep([NotNull] LuaCompletion2Parser.FieldsepContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.fieldsep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFieldsep([NotNull] LuaCompletion2Parser.FieldsepContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorOr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorOr([NotNull] LuaCompletion2Parser.OperatorOrContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorOr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorOr([NotNull] LuaCompletion2Parser.OperatorOrContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorAnd"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorAnd([NotNull] LuaCompletion2Parser.OperatorAndContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorAnd"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorAnd([NotNull] LuaCompletion2Parser.OperatorAndContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorComparison"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorComparison([NotNull] LuaCompletion2Parser.OperatorComparisonContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorComparison"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorComparison([NotNull] LuaCompletion2Parser.OperatorComparisonContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorStrcat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorStrcat([NotNull] LuaCompletion2Parser.OperatorStrcatContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorStrcat"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorStrcat([NotNull] LuaCompletion2Parser.OperatorStrcatContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorAddSub"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorAddSub([NotNull] LuaCompletion2Parser.OperatorAddSubContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorAddSub"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorAddSub([NotNull] LuaCompletion2Parser.OperatorAddSubContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorMulDivMod"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorMulDivMod([NotNull] LuaCompletion2Parser.OperatorMulDivModContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorMulDivMod"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorMulDivMod([NotNull] LuaCompletion2Parser.OperatorMulDivModContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorBitwise"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorBitwise([NotNull] LuaCompletion2Parser.OperatorBitwiseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorBitwise"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorBitwise([NotNull] LuaCompletion2Parser.OperatorBitwiseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorUnary"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorUnary([NotNull] LuaCompletion2Parser.OperatorUnaryContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorUnary"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorUnary([NotNull] LuaCompletion2Parser.OperatorUnaryContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.operatorPower"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOperatorPower([NotNull] LuaCompletion2Parser.OperatorPowerContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.operatorPower"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOperatorPower([NotNull] LuaCompletion2Parser.OperatorPowerContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.number"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNumber([NotNull] LuaCompletion2Parser.NumberContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.number"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNumber([NotNull] LuaCompletion2Parser.NumberContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="LuaCompletion2Parser.string"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterString([NotNull] LuaCompletion2Parser.StringContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="LuaCompletion2Parser.string"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitString([NotNull] LuaCompletion2Parser.StringContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}