// Generated from /home/developer/Sync/BigFiles/BigProjects/Kerbalua/Kerbalua/Kerbalua/Kerbalua/Completion/Grammar/IncompleteLua.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class IncompleteLuaParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, T__19=20, T__20=21, T__21=22, T__22=23, T__23=24, 
		T__24=25, T__25=26, T__26=27, T__27=28, T__28=29, T__29=30, T__30=31, 
		T__31=32, T__32=33, T__33=34, T__34=35, T__35=36, T__36=37, T__37=38, 
		T__38=39, T__39=40, T__40=41, T__41=42, T__42=43, T__43=44, T__44=45, 
		T__45=46, T__46=47, T__47=48, T__48=49, T__49=50, T__50=51, T__51=52, 
		T__52=53, T__53=54, T__54=55, NAME=56, NORMALSTRING=57, CHARSTRING=58, 
		LONGSTRING=59, INT=60, HEX=61, FLOAT=62, HEX_FLOAT=63, COMMENT=64, LINE_COMMENT=65, 
		WS=66, SHEBANG=67;
	public static final int
		RULE_chunk = 0, RULE_incompleteChunk = 1, RULE_block = 2, RULE_incompleteBlock = 3, 
		RULE_stat = 4, RULE_incompleteStat = 5, RULE_incompleteElse = 6, RULE_retstat = 7, 
		RULE_incompleteRetstat = 8, RULE_label = 9, RULE_funcname = 10, RULE_incompleteFuncname = 11, 
		RULE_varlist = 12, RULE_incompleteVarlist = 13, RULE_namelist = 14, RULE_incompleteNamelist = 15, 
		RULE_explist = 16, RULE_incompleteExplist = 17, RULE_exp = 18, RULE_incompleteExp = 19, 
		RULE_prefixexp = 20, RULE_incompletePrefixexp = 21, RULE_functioncall = 22, 
		RULE_incompleteFunctionCall = 23, RULE_varOrExp = 24, RULE_incompleteVarOrExp = 25, 
		RULE_var = 26, RULE_incompleteVar = 27, RULE_varName = 28, RULE_varSuffix = 29, 
		RULE_memberName = 30, RULE_arrayArg = 31, RULE_incompleteVarSuffix = 32, 
		RULE_nameAndArgs = 33, RULE_incompleteNameAndArgs = 34, RULE_args = 35, 
		RULE_incompleteArgs = 36, RULE_functiondef = 37, RULE_incompleteFunctiondef = 38, 
		RULE_funcbody = 39, RULE_incompleteFuncbody = 40, RULE_parlist = 41, RULE_incompleteParlist = 42, 
		RULE_tableconstructor = 43, RULE_incompleteTableconstructor = 44, RULE_fieldlist = 45, 
		RULE_incompleteFieldlist = 46, RULE_field = 47, RULE_incompleteField = 48, 
		RULE_fieldsep = 49, RULE_operatorOr = 50, RULE_operatorAnd = 51, RULE_operatorComparison = 52, 
		RULE_operatorStrcat = 53, RULE_operatorAddSub = 54, RULE_operatorMulDivMod = 55, 
		RULE_operatorBitwise = 56, RULE_operatorUnary = 57, RULE_operatorPower = 58, 
		RULE_number = 59, RULE_string = 60, RULE_incompleteString = 61, RULE_incompleteName = 62;
	public static final String[] ruleNames = {
		"chunk", "incompleteChunk", "block", "incompleteBlock", "stat", "incompleteStat", 
		"incompleteElse", "retstat", "incompleteRetstat", "label", "funcname", 
		"incompleteFuncname", "varlist", "incompleteVarlist", "namelist", "incompleteNamelist", 
		"explist", "incompleteExplist", "exp", "incompleteExp", "prefixexp", "incompletePrefixexp", 
		"functioncall", "incompleteFunctionCall", "varOrExp", "incompleteVarOrExp", 
		"var", "incompleteVar", "varName", "varSuffix", "memberName", "arrayArg", 
		"incompleteVarSuffix", "nameAndArgs", "incompleteNameAndArgs", "args", 
		"incompleteArgs", "functiondef", "incompleteFunctiondef", "funcbody", 
		"incompleteFuncbody", "parlist", "incompleteParlist", "tableconstructor", 
		"incompleteTableconstructor", "fieldlist", "incompleteFieldlist", "field", 
		"incompleteField", "fieldsep", "operatorOr", "operatorAnd", "operatorComparison", 
		"operatorStrcat", "operatorAddSub", "operatorMulDivMod", "operatorBitwise", 
		"operatorUnary", "operatorPower", "number", "string", "incompleteString", 
		"incompleteName"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "';'", "'='", "'break'", "'goto'", "'do'", "'end'", "'while'", "'repeat'", 
		"'until'", "'if'", "'then'", "'elseif'", "'else'", "'for'", "','", "'in'", 
		"'function'", "'local'", "'return'", "'::'", "'.'", "':'", "'nil'", "'false'", 
		"'true'", "'...'", "'('", "')'", "'['", "']'", "'{'", "'}'", "'or'", "'and'", 
		"'<'", "'>'", "'<='", "'>='", "'~='", "'=='", "'..'", "'+'", "'-'", "'*'", 
		"'/'", "'%'", "'//'", "'&'", "'|'", "'~'", "'<<'", "'>>'", "'not'", "'#'", 
		"'^'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, "NAME", "NORMALSTRING", 
		"CHARSTRING", "LONGSTRING", "INT", "HEX", "FLOAT", "HEX_FLOAT", "COMMENT", 
		"LINE_COMMENT", "WS", "SHEBANG"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "IncompleteLua.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public IncompleteLuaParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class ChunkContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public TerminalNode EOF() { return getToken(IncompleteLuaParser.EOF, 0); }
		public ChunkContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_chunk; }
	}

	public final ChunkContext chunk() throws RecognitionException {
		ChunkContext _localctx = new ChunkContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_chunk);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(126);
			block();
			setState(127);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteChunkContext extends ParserRuleContext {
		public IncompleteBlockContext incompleteBlock() {
			return getRuleContext(IncompleteBlockContext.class,0);
		}
		public TerminalNode EOF() { return getToken(IncompleteLuaParser.EOF, 0); }
		public IncompleteChunkContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteChunk; }
	}

	public final IncompleteChunkContext incompleteChunk() throws RecognitionException {
		IncompleteChunkContext _localctx = new IncompleteChunkContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_incompleteChunk);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(129);
			incompleteBlock();
			setState(130);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BlockContext extends ParserRuleContext {
		public List<StatContext> stat() {
			return getRuleContexts(StatContext.class);
		}
		public StatContext stat(int i) {
			return getRuleContext(StatContext.class,i);
		}
		public RetstatContext retstat() {
			return getRuleContext(RetstatContext.class,0);
		}
		public BlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_block; }
	}

	public final BlockContext block() throws RecognitionException {
		BlockContext _localctx = new BlockContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_block);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(135);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__2) | (1L << T__3) | (1L << T__4) | (1L << T__6) | (1L << T__7) | (1L << T__9) | (1L << T__13) | (1L << T__16) | (1L << T__17) | (1L << T__19) | (1L << T__26) | (1L << NAME))) != 0)) {
				{
				{
				setState(132);
				stat();
				}
				}
				setState(137);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(139);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__18) {
				{
				setState(138);
				retstat();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteBlockContext extends ParserRuleContext {
		public IncompleteStatContext incompleteStat() {
			return getRuleContext(IncompleteStatContext.class,0);
		}
		public List<StatContext> stat() {
			return getRuleContexts(StatContext.class);
		}
		public StatContext stat(int i) {
			return getRuleContext(StatContext.class,i);
		}
		public IncompleteRetstatContext incompleteRetstat() {
			return getRuleContext(IncompleteRetstatContext.class,0);
		}
		public IncompleteBlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteBlock; }
	}

	public final IncompleteBlockContext incompleteBlock() throws RecognitionException {
		IncompleteBlockContext _localctx = new IncompleteBlockContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_incompleteBlock);
		int _la;
		try {
			int _alt;
			setState(155);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,4,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(144);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,2,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(141);
						stat();
						}
						} 
					}
					setState(146);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,2,_ctx);
				}
				setState(147);
				incompleteStat();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(151);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << T__2) | (1L << T__3) | (1L << T__4) | (1L << T__6) | (1L << T__7) | (1L << T__9) | (1L << T__13) | (1L << T__16) | (1L << T__17) | (1L << T__19) | (1L << T__26) | (1L << NAME))) != 0)) {
					{
					{
					setState(148);
					stat();
					}
					}
					setState(153);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(154);
				incompleteRetstat();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StatContext extends ParserRuleContext {
		public VarlistContext varlist() {
			return getRuleContext(VarlistContext.class,0);
		}
		public ExplistContext explist() {
			return getRuleContext(ExplistContext.class,0);
		}
		public FunctioncallContext functioncall() {
			return getRuleContext(FunctioncallContext.class,0);
		}
		public LabelContext label() {
			return getRuleContext(LabelContext.class,0);
		}
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public List<BlockContext> block() {
			return getRuleContexts(BlockContext.class);
		}
		public BlockContext block(int i) {
			return getRuleContext(BlockContext.class,i);
		}
		public List<ExpContext> exp() {
			return getRuleContexts(ExpContext.class);
		}
		public ExpContext exp(int i) {
			return getRuleContext(ExpContext.class,i);
		}
		public NamelistContext namelist() {
			return getRuleContext(NamelistContext.class,0);
		}
		public FuncnameContext funcname() {
			return getRuleContext(FuncnameContext.class,0);
		}
		public FuncbodyContext funcbody() {
			return getRuleContext(FuncbodyContext.class,0);
		}
		public StatContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_stat; }
	}

	public final StatContext stat() throws RecognitionException {
		StatContext _localctx = new StatContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_stat);
		int _la;
		try {
			setState(238);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,9,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(157);
				match(T__0);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(158);
				varlist();
				setState(159);
				match(T__1);
				setState(160);
				explist();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(162);
				functioncall();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(163);
				label();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(164);
				match(T__2);
				}
				break;
			case 6:
				enterOuterAlt(_localctx, 6);
				{
				setState(165);
				match(T__3);
				setState(166);
				match(NAME);
				}
				break;
			case 7:
				enterOuterAlt(_localctx, 7);
				{
				setState(167);
				match(T__4);
				setState(168);
				block();
				setState(169);
				match(T__5);
				}
				break;
			case 8:
				enterOuterAlt(_localctx, 8);
				{
				setState(171);
				match(T__6);
				setState(172);
				exp(0);
				setState(173);
				match(T__4);
				setState(174);
				block();
				setState(175);
				match(T__5);
				}
				break;
			case 9:
				enterOuterAlt(_localctx, 9);
				{
				setState(177);
				match(T__7);
				setState(178);
				block();
				setState(179);
				match(T__8);
				setState(180);
				exp(0);
				}
				break;
			case 10:
				enterOuterAlt(_localctx, 10);
				{
				setState(182);
				match(T__9);
				setState(183);
				exp(0);
				setState(184);
				match(T__10);
				setState(185);
				block();
				setState(193);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==T__11) {
					{
					{
					setState(186);
					match(T__11);
					setState(187);
					exp(0);
					setState(188);
					match(T__10);
					setState(189);
					block();
					}
					}
					setState(195);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(198);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__12) {
					{
					setState(196);
					match(T__12);
					setState(197);
					block();
					}
				}

				setState(200);
				match(T__5);
				}
				break;
			case 11:
				enterOuterAlt(_localctx, 11);
				{
				setState(202);
				match(T__13);
				setState(203);
				match(NAME);
				setState(204);
				match(T__1);
				setState(205);
				exp(0);
				setState(206);
				match(T__14);
				setState(207);
				exp(0);
				setState(210);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__14) {
					{
					setState(208);
					match(T__14);
					setState(209);
					exp(0);
					}
				}

				setState(212);
				match(T__4);
				setState(213);
				block();
				setState(214);
				match(T__5);
				}
				break;
			case 12:
				enterOuterAlt(_localctx, 12);
				{
				setState(216);
				match(T__13);
				setState(217);
				namelist();
				setState(218);
				match(T__15);
				setState(219);
				explist();
				setState(220);
				match(T__4);
				setState(221);
				block();
				setState(222);
				match(T__5);
				}
				break;
			case 13:
				enterOuterAlt(_localctx, 13);
				{
				setState(224);
				match(T__16);
				setState(225);
				funcname();
				setState(226);
				funcbody();
				}
				break;
			case 14:
				enterOuterAlt(_localctx, 14);
				{
				setState(228);
				match(T__17);
				setState(229);
				match(T__16);
				setState(230);
				match(NAME);
				setState(231);
				funcbody();
				}
				break;
			case 15:
				enterOuterAlt(_localctx, 15);
				{
				setState(232);
				match(T__17);
				setState(233);
				namelist();
				setState(236);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__1) {
					{
					setState(234);
					match(T__1);
					setState(235);
					explist();
					}
				}

				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteStatContext extends ParserRuleContext {
		public IncompleteVarlistContext incompleteVarlist() {
			return getRuleContext(IncompleteVarlistContext.class,0);
		}
		public VarlistContext varlist() {
			return getRuleContext(VarlistContext.class,0);
		}
		public IncompleteExplistContext incompleteExplist() {
			return getRuleContext(IncompleteExplistContext.class,0);
		}
		public IncompleteFunctionCallContext incompleteFunctionCall() {
			return getRuleContext(IncompleteFunctionCallContext.class,0);
		}
		public IncompleteNameContext incompleteName() {
			return getRuleContext(IncompleteNameContext.class,0);
		}
		public IncompleteBlockContext incompleteBlock() {
			return getRuleContext(IncompleteBlockContext.class,0);
		}
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public List<ExpContext> exp() {
			return getRuleContexts(ExpContext.class);
		}
		public ExpContext exp(int i) {
			return getRuleContext(ExpContext.class,i);
		}
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public IncompleteElseContext incompleteElse() {
			return getRuleContext(IncompleteElseContext.class,0);
		}
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public IncompleteFuncnameContext incompleteFuncname() {
			return getRuleContext(IncompleteFuncnameContext.class,0);
		}
		public FuncnameContext funcname() {
			return getRuleContext(FuncnameContext.class,0);
		}
		public IncompleteFuncbodyContext incompleteFuncbody() {
			return getRuleContext(IncompleteFuncbodyContext.class,0);
		}
		public IncompleteNamelistContext incompleteNamelist() {
			return getRuleContext(IncompleteNamelistContext.class,0);
		}
		public NamelistContext namelist() {
			return getRuleContext(NamelistContext.class,0);
		}
		public IncompleteStatContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteStat; }
	}

	public final IncompleteStatContext incompleteStat() throws RecognitionException {
		IncompleteStatContext _localctx = new IncompleteStatContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_incompleteStat);
		int _la;
		try {
			setState(333);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,11,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(240);
				incompleteVarlist();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(241);
				varlist();
				setState(242);
				match(T__1);
				setState(243);
				incompleteExplist();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(245);
				incompleteFunctionCall();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(246);
				match(T__3);
				setState(247);
				incompleteName();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(248);
				match(T__4);
				setState(249);
				incompleteBlock();
				}
				break;
			case 6:
				enterOuterAlt(_localctx, 6);
				{
				setState(250);
				match(T__6);
				setState(251);
				incompleteExp();
				}
				break;
			case 7:
				enterOuterAlt(_localctx, 7);
				{
				setState(252);
				match(T__6);
				setState(253);
				exp(0);
				setState(254);
				match(T__4);
				setState(255);
				incompleteBlock();
				}
				break;
			case 8:
				enterOuterAlt(_localctx, 8);
				{
				setState(257);
				match(T__7);
				setState(258);
				incompleteBlock();
				}
				break;
			case 9:
				enterOuterAlt(_localctx, 9);
				{
				setState(259);
				match(T__7);
				setState(260);
				block();
				setState(261);
				match(T__8);
				setState(262);
				incompleteExp();
				}
				break;
			case 10:
				enterOuterAlt(_localctx, 10);
				{
				setState(264);
				match(T__9);
				setState(265);
				incompleteExp();
				}
				break;
			case 11:
				enterOuterAlt(_localctx, 11);
				{
				setState(266);
				match(T__9);
				setState(267);
				exp(0);
				setState(268);
				match(T__10);
				setState(269);
				incompleteBlock();
				}
				break;
			case 12:
				enterOuterAlt(_localctx, 12);
				{
				setState(271);
				match(T__9);
				setState(272);
				exp(0);
				setState(273);
				match(T__10);
				setState(274);
				block();
				setState(275);
				incompleteElse();
				}
				break;
			case 13:
				enterOuterAlt(_localctx, 13);
				{
				setState(277);
				match(T__13);
				setState(278);
				incompleteName();
				}
				break;
			case 14:
				enterOuterAlt(_localctx, 14);
				{
				setState(279);
				match(T__13);
				setState(280);
				match(NAME);
				setState(281);
				match(T__1);
				setState(282);
				incompleteExp();
				}
				break;
			case 15:
				enterOuterAlt(_localctx, 15);
				{
				setState(283);
				match(T__13);
				setState(284);
				match(NAME);
				setState(285);
				match(T__1);
				setState(286);
				exp(0);
				setState(287);
				match(T__14);
				setState(288);
				incompleteExp();
				}
				break;
			case 16:
				enterOuterAlt(_localctx, 16);
				{
				setState(290);
				match(T__13);
				setState(291);
				match(NAME);
				setState(292);
				match(T__1);
				setState(293);
				exp(0);
				setState(294);
				match(T__14);
				setState(295);
				exp(0);
				setState(296);
				match(T__14);
				setState(297);
				incompleteExp();
				}
				break;
			case 17:
				enterOuterAlt(_localctx, 17);
				{
				}
				break;
			case 18:
				enterOuterAlt(_localctx, 18);
				{
				setState(300);
				match(T__13);
				setState(301);
				match(NAME);
				setState(302);
				match(T__1);
				setState(303);
				exp(0);
				setState(304);
				match(T__14);
				setState(305);
				exp(0);
				setState(308);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__14) {
					{
					setState(306);
					match(T__14);
					setState(307);
					exp(0);
					}
				}

				setState(310);
				match(T__4);
				setState(311);
				incompleteBlock();
				}
				break;
			case 19:
				enterOuterAlt(_localctx, 19);
				{
				setState(313);
				match(T__16);
				setState(314);
				incompleteFuncname();
				}
				break;
			case 20:
				enterOuterAlt(_localctx, 20);
				{
				setState(315);
				match(T__16);
				setState(316);
				funcname();
				setState(317);
				incompleteFuncbody();
				}
				break;
			case 21:
				enterOuterAlt(_localctx, 21);
				{
				setState(319);
				match(T__17);
				setState(320);
				match(T__16);
				setState(321);
				incompleteName();
				}
				break;
			case 22:
				enterOuterAlt(_localctx, 22);
				{
				setState(322);
				match(T__17);
				setState(323);
				match(T__16);
				setState(324);
				match(NAME);
				setState(325);
				incompleteFuncbody();
				}
				break;
			case 23:
				enterOuterAlt(_localctx, 23);
				{
				setState(326);
				match(T__17);
				setState(327);
				incompleteNamelist();
				}
				break;
			case 24:
				enterOuterAlt(_localctx, 24);
				{
				setState(328);
				match(T__17);
				setState(329);
				namelist();
				setState(330);
				match(T__1);
				setState(331);
				incompleteExplist();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteElseContext extends ParserRuleContext {
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public List<ExpContext> exp() {
			return getRuleContexts(ExpContext.class);
		}
		public ExpContext exp(int i) {
			return getRuleContext(ExpContext.class,i);
		}
		public List<BlockContext> block() {
			return getRuleContexts(BlockContext.class);
		}
		public BlockContext block(int i) {
			return getRuleContext(BlockContext.class,i);
		}
		public IncompleteBlockContext incompleteBlock() {
			return getRuleContext(IncompleteBlockContext.class,0);
		}
		public IncompleteElseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteElse; }
	}

	public final IncompleteElseContext incompleteElse() throws RecognitionException {
		IncompleteElseContext _localctx = new IncompleteElseContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_incompleteElse);
		int _la;
		try {
			int _alt;
			setState(364);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,14,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(342);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,12,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(335);
						match(T__11);
						setState(336);
						exp(0);
						setState(337);
						match(T__10);
						setState(338);
						block();
						}
						} 
					}
					setState(344);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,12,_ctx);
				}
				setState(345);
				match(T__11);
				setState(346);
				incompleteExp();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(347);
				match(T__11);
				setState(348);
				exp(0);
				setState(349);
				match(T__10);
				setState(350);
				incompleteBlock();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(359);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==T__11) {
					{
					{
					setState(352);
					match(T__11);
					setState(353);
					exp(0);
					setState(354);
					match(T__10);
					setState(355);
					block();
					}
					}
					setState(361);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(362);
				match(T__12);
				setState(363);
				incompleteBlock();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class RetstatContext extends ParserRuleContext {
		public ExplistContext explist() {
			return getRuleContext(ExplistContext.class,0);
		}
		public RetstatContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_retstat; }
	}

	public final RetstatContext retstat() throws RecognitionException {
		RetstatContext _localctx = new RetstatContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_retstat);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(366);
			match(T__18);
			setState(368);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__16) | (1L << T__22) | (1L << T__23) | (1L << T__24) | (1L << T__25) | (1L << T__26) | (1L << T__30) | (1L << T__42) | (1L << T__49) | (1L << T__52) | (1L << T__53) | (1L << NAME) | (1L << NORMALSTRING) | (1L << CHARSTRING) | (1L << LONGSTRING) | (1L << INT) | (1L << HEX) | (1L << FLOAT) | (1L << HEX_FLOAT))) != 0)) {
				{
				setState(367);
				explist();
				}
			}

			setState(371);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__0) {
				{
				setState(370);
				match(T__0);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteRetstatContext extends ParserRuleContext {
		public IncompleteExplistContext incompleteExplist() {
			return getRuleContext(IncompleteExplistContext.class,0);
		}
		public IncompleteRetstatContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteRetstat; }
	}

	public final IncompleteRetstatContext incompleteRetstat() throws RecognitionException {
		IncompleteRetstatContext _localctx = new IncompleteRetstatContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_incompleteRetstat);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(373);
			match(T__18);
			setState(374);
			incompleteExplist();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LabelContext extends ParserRuleContext {
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public LabelContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_label; }
	}

	public final LabelContext label() throws RecognitionException {
		LabelContext _localctx = new LabelContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_label);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(376);
			match(T__19);
			setState(377);
			match(NAME);
			setState(378);
			match(T__19);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FuncnameContext extends ParserRuleContext {
		public List<TerminalNode> NAME() { return getTokens(IncompleteLuaParser.NAME); }
		public TerminalNode NAME(int i) {
			return getToken(IncompleteLuaParser.NAME, i);
		}
		public FuncnameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcname; }
	}

	public final FuncnameContext funcname() throws RecognitionException {
		FuncnameContext _localctx = new FuncnameContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_funcname);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(380);
			match(NAME);
			setState(385);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__20) {
				{
				{
				setState(381);
				match(T__20);
				setState(382);
				match(NAME);
				}
				}
				setState(387);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(390);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__21) {
				{
				setState(388);
				match(T__21);
				setState(389);
				match(NAME);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteFuncnameContext extends ParserRuleContext {
		public IncompleteNameContext incompleteName() {
			return getRuleContext(IncompleteNameContext.class,0);
		}
		public List<TerminalNode> NAME() { return getTokens(IncompleteLuaParser.NAME); }
		public TerminalNode NAME(int i) {
			return getToken(IncompleteLuaParser.NAME, i);
		}
		public IncompleteFuncnameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteFuncname; }
	}

	public final IncompleteFuncnameContext incompleteFuncname() throws RecognitionException {
		IncompleteFuncnameContext _localctx = new IncompleteFuncnameContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_incompleteFuncname);
		int _la;
		try {
			int _alt;
			setState(410);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,21,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(396);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,19,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(392);
						match(NAME);
						setState(393);
						match(T__20);
						}
						} 
					}
					setState(398);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,19,_ctx);
				}
				setState(399);
				incompleteName();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(400);
				match(NAME);
				setState(405);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while (_la==T__20) {
					{
					{
					setState(401);
					match(T__20);
					setState(402);
					match(NAME);
					}
					}
					setState(407);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(408);
				match(T__21);
				setState(409);
				incompleteName();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarlistContext extends ParserRuleContext {
		public List<VarContext> var() {
			return getRuleContexts(VarContext.class);
		}
		public VarContext var(int i) {
			return getRuleContext(VarContext.class,i);
		}
		public VarlistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_varlist; }
	}

	public final VarlistContext varlist() throws RecognitionException {
		VarlistContext _localctx = new VarlistContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_varlist);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(412);
			var();
			setState(417);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__14) {
				{
				{
				setState(413);
				match(T__14);
				setState(414);
				var();
				}
				}
				setState(419);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteVarlistContext extends ParserRuleContext {
		public IncompleteVarContext incompleteVar() {
			return getRuleContext(IncompleteVarContext.class,0);
		}
		public List<VarContext> var() {
			return getRuleContexts(VarContext.class);
		}
		public VarContext var(int i) {
			return getRuleContext(VarContext.class,i);
		}
		public IncompleteVarlistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteVarlist; }
	}

	public final IncompleteVarlistContext incompleteVarlist() throws RecognitionException {
		IncompleteVarlistContext _localctx = new IncompleteVarlistContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_incompleteVarlist);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(425);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,23,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(420);
					var();
					setState(421);
					match(T__14);
					}
					} 
				}
				setState(427);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,23,_ctx);
			}
			setState(428);
			incompleteVar();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NamelistContext extends ParserRuleContext {
		public List<TerminalNode> NAME() { return getTokens(IncompleteLuaParser.NAME); }
		public TerminalNode NAME(int i) {
			return getToken(IncompleteLuaParser.NAME, i);
		}
		public NamelistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_namelist; }
	}

	public final NamelistContext namelist() throws RecognitionException {
		NamelistContext _localctx = new NamelistContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_namelist);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(430);
			match(NAME);
			setState(435);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,24,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(431);
					match(T__14);
					setState(432);
					match(NAME);
					}
					} 
				}
				setState(437);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,24,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteNamelistContext extends ParserRuleContext {
		public IncompleteNameContext incompleteName() {
			return getRuleContext(IncompleteNameContext.class,0);
		}
		public List<TerminalNode> NAME() { return getTokens(IncompleteLuaParser.NAME); }
		public TerminalNode NAME(int i) {
			return getToken(IncompleteLuaParser.NAME, i);
		}
		public IncompleteNamelistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteNamelist; }
	}

	public final IncompleteNamelistContext incompleteNamelist() throws RecognitionException {
		IncompleteNamelistContext _localctx = new IncompleteNamelistContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_incompleteNamelist);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(442);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,25,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(438);
					match(NAME);
					setState(439);
					match(T__14);
					}
					} 
				}
				setState(444);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,25,_ctx);
			}
			setState(445);
			incompleteName();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExplistContext extends ParserRuleContext {
		public List<ExpContext> exp() {
			return getRuleContexts(ExpContext.class);
		}
		public ExpContext exp(int i) {
			return getRuleContext(ExpContext.class,i);
		}
		public ExplistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_explist; }
	}

	public final ExplistContext explist() throws RecognitionException {
		ExplistContext _localctx = new ExplistContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_explist);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(447);
			exp(0);
			setState(452);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__14) {
				{
				{
				setState(448);
				match(T__14);
				setState(449);
				exp(0);
				}
				}
				setState(454);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteExplistContext extends ParserRuleContext {
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public List<ExpContext> exp() {
			return getRuleContexts(ExpContext.class);
		}
		public ExpContext exp(int i) {
			return getRuleContext(ExpContext.class,i);
		}
		public IncompleteExplistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteExplist; }
	}

	public final IncompleteExplistContext incompleteExplist() throws RecognitionException {
		IncompleteExplistContext _localctx = new IncompleteExplistContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_incompleteExplist);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(460);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,27,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(455);
					exp(0);
					setState(456);
					match(T__14);
					}
					} 
				}
				setState(462);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,27,_ctx);
			}
			setState(463);
			incompleteExp();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExpContext extends ParserRuleContext {
		public NumberContext number() {
			return getRuleContext(NumberContext.class,0);
		}
		public StringContext string() {
			return getRuleContext(StringContext.class,0);
		}
		public FunctiondefContext functiondef() {
			return getRuleContext(FunctiondefContext.class,0);
		}
		public PrefixexpContext prefixexp() {
			return getRuleContext(PrefixexpContext.class,0);
		}
		public TableconstructorContext tableconstructor() {
			return getRuleContext(TableconstructorContext.class,0);
		}
		public OperatorUnaryContext operatorUnary() {
			return getRuleContext(OperatorUnaryContext.class,0);
		}
		public List<ExpContext> exp() {
			return getRuleContexts(ExpContext.class);
		}
		public ExpContext exp(int i) {
			return getRuleContext(ExpContext.class,i);
		}
		public OperatorPowerContext operatorPower() {
			return getRuleContext(OperatorPowerContext.class,0);
		}
		public OperatorMulDivModContext operatorMulDivMod() {
			return getRuleContext(OperatorMulDivModContext.class,0);
		}
		public OperatorAddSubContext operatorAddSub() {
			return getRuleContext(OperatorAddSubContext.class,0);
		}
		public OperatorStrcatContext operatorStrcat() {
			return getRuleContext(OperatorStrcatContext.class,0);
		}
		public OperatorComparisonContext operatorComparison() {
			return getRuleContext(OperatorComparisonContext.class,0);
		}
		public OperatorAndContext operatorAnd() {
			return getRuleContext(OperatorAndContext.class,0);
		}
		public OperatorOrContext operatorOr() {
			return getRuleContext(OperatorOrContext.class,0);
		}
		public OperatorBitwiseContext operatorBitwise() {
			return getRuleContext(OperatorBitwiseContext.class,0);
		}
		public ExpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_exp; }
	}

	public final ExpContext exp() throws RecognitionException {
		return exp(0);
	}

	private ExpContext exp(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		ExpContext _localctx = new ExpContext(_ctx, _parentState);
		ExpContext _prevctx = _localctx;
		int _startState = 36;
		enterRecursionRule(_localctx, 36, RULE_exp, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(478);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__22:
				{
				setState(466);
				match(T__22);
				}
				break;
			case T__23:
				{
				setState(467);
				match(T__23);
				}
				break;
			case T__24:
				{
				setState(468);
				match(T__24);
				}
				break;
			case INT:
			case HEX:
			case FLOAT:
			case HEX_FLOAT:
				{
				setState(469);
				number();
				}
				break;
			case NORMALSTRING:
			case CHARSTRING:
			case LONGSTRING:
				{
				setState(470);
				string();
				}
				break;
			case T__25:
				{
				setState(471);
				match(T__25);
				}
				break;
			case T__16:
				{
				setState(472);
				functiondef();
				}
				break;
			case T__26:
			case NAME:
				{
				setState(473);
				prefixexp();
				}
				break;
			case T__30:
				{
				setState(474);
				tableconstructor();
				}
				break;
			case T__42:
			case T__49:
			case T__52:
			case T__53:
				{
				setState(475);
				operatorUnary();
				setState(476);
				exp(8);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.LT(-1);
			setState(514);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,30,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(512);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,29,_ctx) ) {
					case 1:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(480);
						if (!(precpred(_ctx, 9))) throw new FailedPredicateException(this, "precpred(_ctx, 9)");
						setState(481);
						operatorPower();
						setState(482);
						exp(9);
						}
						break;
					case 2:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(484);
						if (!(precpred(_ctx, 7))) throw new FailedPredicateException(this, "precpred(_ctx, 7)");
						setState(485);
						operatorMulDivMod();
						setState(486);
						exp(8);
						}
						break;
					case 3:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(488);
						if (!(precpred(_ctx, 6))) throw new FailedPredicateException(this, "precpred(_ctx, 6)");
						setState(489);
						operatorAddSub();
						setState(490);
						exp(7);
						}
						break;
					case 4:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(492);
						if (!(precpred(_ctx, 5))) throw new FailedPredicateException(this, "precpred(_ctx, 5)");
						setState(493);
						operatorStrcat();
						setState(494);
						exp(5);
						}
						break;
					case 5:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(496);
						if (!(precpred(_ctx, 4))) throw new FailedPredicateException(this, "precpred(_ctx, 4)");
						setState(497);
						operatorComparison();
						setState(498);
						exp(5);
						}
						break;
					case 6:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(500);
						if (!(precpred(_ctx, 3))) throw new FailedPredicateException(this, "precpred(_ctx, 3)");
						setState(501);
						operatorAnd();
						setState(502);
						exp(4);
						}
						break;
					case 7:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(504);
						if (!(precpred(_ctx, 2))) throw new FailedPredicateException(this, "precpred(_ctx, 2)");
						setState(505);
						operatorOr();
						setState(506);
						exp(3);
						}
						break;
					case 8:
						{
						_localctx = new ExpContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_exp);
						setState(508);
						if (!(precpred(_ctx, 1))) throw new FailedPredicateException(this, "precpred(_ctx, 1)");
						setState(509);
						operatorBitwise();
						setState(510);
						exp(2);
						}
						break;
					}
					} 
				}
				setState(516);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,30,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class IncompleteExpContext extends ParserRuleContext {
		public IncompleteFunctiondefContext incompleteFunctiondef() {
			return getRuleContext(IncompleteFunctiondefContext.class,0);
		}
		public IncompletePrefixexpContext incompletePrefixexp() {
			return getRuleContext(IncompletePrefixexpContext.class,0);
		}
		public IncompleteTableconstructorContext incompleteTableconstructor() {
			return getRuleContext(IncompleteTableconstructorContext.class,0);
		}
		public ExpContext exp() {
			return getRuleContext(ExpContext.class,0);
		}
		public OperatorPowerContext operatorPower() {
			return getRuleContext(OperatorPowerContext.class,0);
		}
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public OperatorUnaryContext operatorUnary() {
			return getRuleContext(OperatorUnaryContext.class,0);
		}
		public OperatorMulDivModContext operatorMulDivMod() {
			return getRuleContext(OperatorMulDivModContext.class,0);
		}
		public OperatorAddSubContext operatorAddSub() {
			return getRuleContext(OperatorAddSubContext.class,0);
		}
		public OperatorStrcatContext operatorStrcat() {
			return getRuleContext(OperatorStrcatContext.class,0);
		}
		public OperatorComparisonContext operatorComparison() {
			return getRuleContext(OperatorComparisonContext.class,0);
		}
		public OperatorAndContext operatorAnd() {
			return getRuleContext(OperatorAndContext.class,0);
		}
		public OperatorOrContext operatorOr() {
			return getRuleContext(OperatorOrContext.class,0);
		}
		public OperatorBitwiseContext operatorBitwise() {
			return getRuleContext(OperatorBitwiseContext.class,0);
		}
		public IncompleteExpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteExp; }
	}

	public final IncompleteExpContext incompleteExp() throws RecognitionException {
		IncompleteExpContext _localctx = new IncompleteExpContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_incompleteExp);
		try {
			setState(555);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,31,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(517);
				incompleteFunctiondef();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(518);
				incompletePrefixexp();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(519);
				incompleteTableconstructor();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(520);
				exp(0);
				setState(521);
				operatorPower();
				setState(522);
				incompleteExp();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(524);
				operatorUnary();
				setState(525);
				incompleteExp();
				}
				break;
			case 6:
				enterOuterAlt(_localctx, 6);
				{
				setState(527);
				exp(0);
				setState(528);
				operatorMulDivMod();
				setState(529);
				incompleteExp();
				}
				break;
			case 7:
				enterOuterAlt(_localctx, 7);
				{
				setState(531);
				exp(0);
				setState(532);
				operatorAddSub();
				setState(533);
				incompleteExp();
				}
				break;
			case 8:
				enterOuterAlt(_localctx, 8);
				{
				setState(535);
				exp(0);
				setState(536);
				operatorStrcat();
				setState(537);
				incompleteExp();
				}
				break;
			case 9:
				enterOuterAlt(_localctx, 9);
				{
				setState(539);
				exp(0);
				setState(540);
				operatorComparison();
				setState(541);
				incompleteExp();
				}
				break;
			case 10:
				enterOuterAlt(_localctx, 10);
				{
				setState(543);
				exp(0);
				setState(544);
				operatorAnd();
				setState(545);
				incompleteExp();
				}
				break;
			case 11:
				enterOuterAlt(_localctx, 11);
				{
				setState(547);
				exp(0);
				setState(548);
				operatorOr();
				setState(549);
				incompleteExp();
				}
				break;
			case 12:
				enterOuterAlt(_localctx, 12);
				{
				setState(551);
				exp(0);
				setState(552);
				operatorBitwise();
				setState(553);
				incompleteExp();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PrefixexpContext extends ParserRuleContext {
		public VarOrExpContext varOrExp() {
			return getRuleContext(VarOrExpContext.class,0);
		}
		public List<NameAndArgsContext> nameAndArgs() {
			return getRuleContexts(NameAndArgsContext.class);
		}
		public NameAndArgsContext nameAndArgs(int i) {
			return getRuleContext(NameAndArgsContext.class,i);
		}
		public PrefixexpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_prefixexp; }
	}

	public final PrefixexpContext prefixexp() throws RecognitionException {
		PrefixexpContext _localctx = new PrefixexpContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_prefixexp);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(557);
			varOrExp();
			setState(561);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,32,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(558);
					nameAndArgs();
					}
					} 
				}
				setState(563);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,32,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompletePrefixexpContext extends ParserRuleContext {
		public IncompleteVarOrExpContext incompleteVarOrExp() {
			return getRuleContext(IncompleteVarOrExpContext.class,0);
		}
		public VarOrExpContext varOrExp() {
			return getRuleContext(VarOrExpContext.class,0);
		}
		public IncompleteNameAndArgsContext incompleteNameAndArgs() {
			return getRuleContext(IncompleteNameAndArgsContext.class,0);
		}
		public List<NameAndArgsContext> nameAndArgs() {
			return getRuleContexts(NameAndArgsContext.class);
		}
		public NameAndArgsContext nameAndArgs(int i) {
			return getRuleContext(NameAndArgsContext.class,i);
		}
		public IncompletePrefixexpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompletePrefixexp; }
	}

	public final IncompletePrefixexpContext incompletePrefixexp() throws RecognitionException {
		IncompletePrefixexpContext _localctx = new IncompletePrefixexpContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_incompletePrefixexp);
		try {
			int _alt;
			setState(574);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,34,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(564);
				incompleteVarOrExp();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(565);
				varOrExp();
				setState(569);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,33,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(566);
						nameAndArgs();
						}
						} 
					}
					setState(571);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,33,_ctx);
				}
				setState(572);
				incompleteNameAndArgs();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FunctioncallContext extends ParserRuleContext {
		public VarOrExpContext varOrExp() {
			return getRuleContext(VarOrExpContext.class,0);
		}
		public List<NameAndArgsContext> nameAndArgs() {
			return getRuleContexts(NameAndArgsContext.class);
		}
		public NameAndArgsContext nameAndArgs(int i) {
			return getRuleContext(NameAndArgsContext.class,i);
		}
		public FunctioncallContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_functioncall; }
	}

	public final FunctioncallContext functioncall() throws RecognitionException {
		FunctioncallContext _localctx = new FunctioncallContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_functioncall);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(576);
			varOrExp();
			setState(578); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(577);
					nameAndArgs();
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(580); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,35,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteFunctionCallContext extends ParserRuleContext {
		public IncompleteVarOrExpContext incompleteVarOrExp() {
			return getRuleContext(IncompleteVarOrExpContext.class,0);
		}
		public VarOrExpContext varOrExp() {
			return getRuleContext(VarOrExpContext.class,0);
		}
		public IncompleteNameAndArgsContext incompleteNameAndArgs() {
			return getRuleContext(IncompleteNameAndArgsContext.class,0);
		}
		public List<NameAndArgsContext> nameAndArgs() {
			return getRuleContexts(NameAndArgsContext.class);
		}
		public NameAndArgsContext nameAndArgs(int i) {
			return getRuleContext(NameAndArgsContext.class,i);
		}
		public IncompleteFunctionCallContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteFunctionCall; }
	}

	public final IncompleteFunctionCallContext incompleteFunctionCall() throws RecognitionException {
		IncompleteFunctionCallContext _localctx = new IncompleteFunctionCallContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_incompleteFunctionCall);
		try {
			int _alt;
			setState(592);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,37,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(582);
				incompleteVarOrExp();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(583);
				varOrExp();
				setState(587);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,36,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(584);
						nameAndArgs();
						}
						} 
					}
					setState(589);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,36,_ctx);
				}
				setState(590);
				incompleteNameAndArgs();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarOrExpContext extends ParserRuleContext {
		public VarContext var() {
			return getRuleContext(VarContext.class,0);
		}
		public ExpContext exp() {
			return getRuleContext(ExpContext.class,0);
		}
		public VarOrExpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_varOrExp; }
	}

	public final VarOrExpContext varOrExp() throws RecognitionException {
		VarOrExpContext _localctx = new VarOrExpContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_varOrExp);
		try {
			setState(599);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,38,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(594);
				var();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(595);
				match(T__26);
				setState(596);
				exp(0);
				setState(597);
				match(T__27);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteVarOrExpContext extends ParserRuleContext {
		public IncompleteVarContext incompleteVar() {
			return getRuleContext(IncompleteVarContext.class,0);
		}
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public IncompleteVarOrExpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteVarOrExp; }
	}

	public final IncompleteVarOrExpContext incompleteVarOrExp() throws RecognitionException {
		IncompleteVarOrExpContext _localctx = new IncompleteVarOrExpContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_incompleteVarOrExp);
		try {
			setState(604);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,39,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(601);
				incompleteVar();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(602);
				match(T__26);
				setState(603);
				incompleteExp();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarContext extends ParserRuleContext {
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public ExpContext exp() {
			return getRuleContext(ExpContext.class,0);
		}
		public List<VarSuffixContext> varSuffix() {
			return getRuleContexts(VarSuffixContext.class);
		}
		public VarSuffixContext varSuffix(int i) {
			return getRuleContext(VarSuffixContext.class,i);
		}
		public VarContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_var; }
	}

	public final VarContext var() throws RecognitionException {
		VarContext _localctx = new VarContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_var);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(612);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case NAME:
				{
				setState(606);
				match(NAME);
				}
				break;
			case T__26:
				{
				setState(607);
				match(T__26);
				setState(608);
				exp(0);
				setState(609);
				match(T__27);
				setState(610);
				varSuffix();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			setState(617);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,41,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(614);
					varSuffix();
					}
					} 
				}
				setState(619);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,41,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteVarContext extends ParserRuleContext {
		public IncompleteNameContext incompleteName() {
			return getRuleContext(IncompleteNameContext.class,0);
		}
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public ExpContext exp() {
			return getRuleContext(ExpContext.class,0);
		}
		public IncompleteVarSuffixContext incompleteVarSuffix() {
			return getRuleContext(IncompleteVarSuffixContext.class,0);
		}
		public VarNameContext varName() {
			return getRuleContext(VarNameContext.class,0);
		}
		public List<VarSuffixContext> varSuffix() {
			return getRuleContexts(VarSuffixContext.class);
		}
		public VarSuffixContext varSuffix(int i) {
			return getRuleContext(VarSuffixContext.class,i);
		}
		public IncompleteVarContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteVar; }
	}

	public final IncompleteVarContext incompleteVar() throws RecognitionException {
		IncompleteVarContext _localctx = new IncompleteVarContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_incompleteVar);
		try {
			int _alt;
			setState(644);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,44,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(620);
				incompleteName();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(621);
				match(T__26);
				setState(622);
				incompleteExp();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(623);
				match(T__26);
				setState(624);
				exp(0);
				setState(625);
				match(T__27);
				setState(626);
				incompleteVarSuffix();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(634);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case NAME:
					{
					setState(628);
					varName();
					}
					break;
				case T__26:
					{
					setState(629);
					match(T__26);
					setState(630);
					exp(0);
					setState(631);
					match(T__27);
					setState(632);
					varSuffix();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(639);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,43,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(636);
						varSuffix();
						}
						} 
					}
					setState(641);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,43,_ctx);
				}
				setState(642);
				incompleteVarSuffix();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarNameContext extends ParserRuleContext {
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public VarNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_varName; }
	}

	public final VarNameContext varName() throws RecognitionException {
		VarNameContext _localctx = new VarNameContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_varName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(646);
			match(NAME);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarSuffixContext extends ParserRuleContext {
		public ArrayArgContext arrayArg() {
			return getRuleContext(ArrayArgContext.class,0);
		}
		public MemberNameContext memberName() {
			return getRuleContext(MemberNameContext.class,0);
		}
		public List<NameAndArgsContext> nameAndArgs() {
			return getRuleContexts(NameAndArgsContext.class);
		}
		public NameAndArgsContext nameAndArgs(int i) {
			return getRuleContext(NameAndArgsContext.class,i);
		}
		public VarSuffixContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_varSuffix; }
	}

	public final VarSuffixContext varSuffix() throws RecognitionException {
		VarSuffixContext _localctx = new VarSuffixContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_varSuffix);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(651);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__21) | (1L << T__26) | (1L << T__30) | (1L << NORMALSTRING) | (1L << CHARSTRING) | (1L << LONGSTRING))) != 0)) {
				{
				{
				setState(648);
				nameAndArgs();
				}
				}
				setState(653);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(660);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__28:
				{
				setState(654);
				match(T__28);
				setState(655);
				arrayArg();
				setState(656);
				match(T__29);
				}
				break;
			case T__20:
				{
				setState(658);
				match(T__20);
				setState(659);
				memberName();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class MemberNameContext extends ParserRuleContext {
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public MemberNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_memberName; }
	}

	public final MemberNameContext memberName() throws RecognitionException {
		MemberNameContext _localctx = new MemberNameContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_memberName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(662);
			match(NAME);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArrayArgContext extends ParserRuleContext {
		public ExpContext exp() {
			return getRuleContext(ExpContext.class,0);
		}
		public ArrayArgContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arrayArg; }
	}

	public final ArrayArgContext arrayArg() throws RecognitionException {
		ArrayArgContext _localctx = new ArrayArgContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_arrayArg);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(664);
			exp(0);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteVarSuffixContext extends ParserRuleContext {
		public IncompleteNameAndArgsContext incompleteNameAndArgs() {
			return getRuleContext(IncompleteNameAndArgsContext.class,0);
		}
		public List<NameAndArgsContext> nameAndArgs() {
			return getRuleContexts(NameAndArgsContext.class);
		}
		public NameAndArgsContext nameAndArgs(int i) {
			return getRuleContext(NameAndArgsContext.class,i);
		}
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public IncompleteNameContext incompleteName() {
			return getRuleContext(IncompleteNameContext.class,0);
		}
		public IncompleteVarSuffixContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteVarSuffix; }
	}

	public final IncompleteVarSuffixContext incompleteVarSuffix() throws RecognitionException {
		IncompleteVarSuffixContext _localctx = new IncompleteVarSuffixContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_incompleteVarSuffix);
		int _la;
		try {
			int _alt;
			setState(685);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,50,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(669);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,47,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(666);
						nameAndArgs();
						}
						} 
					}
					setState(671);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,47,_ctx);
				}
				setState(672);
				incompleteNameAndArgs();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(676);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__21) | (1L << T__26) | (1L << T__30) | (1L << NORMALSTRING) | (1L << CHARSTRING) | (1L << LONGSTRING))) != 0)) {
					{
					{
					setState(673);
					nameAndArgs();
					}
					}
					setState(678);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(683);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case T__28:
					{
					setState(679);
					match(T__28);
					setState(680);
					incompleteExp();
					}
					break;
				case T__20:
					{
					setState(681);
					match(T__20);
					setState(682);
					incompleteName();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NameAndArgsContext extends ParserRuleContext {
		public ArgsContext args() {
			return getRuleContext(ArgsContext.class,0);
		}
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public NameAndArgsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_nameAndArgs; }
	}

	public final NameAndArgsContext nameAndArgs() throws RecognitionException {
		NameAndArgsContext _localctx = new NameAndArgsContext(_ctx, getState());
		enterRule(_localctx, 66, RULE_nameAndArgs);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(689);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__21) {
				{
				setState(687);
				match(T__21);
				setState(688);
				match(NAME);
				}
			}

			setState(691);
			args();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteNameAndArgsContext extends ParserRuleContext {
		public IncompleteNameContext incompleteName() {
			return getRuleContext(IncompleteNameContext.class,0);
		}
		public IncompleteArgsContext incompleteArgs() {
			return getRuleContext(IncompleteArgsContext.class,0);
		}
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public IncompleteNameAndArgsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteNameAndArgs; }
	}

	public final IncompleteNameAndArgsContext incompleteNameAndArgs() throws RecognitionException {
		IncompleteNameAndArgsContext _localctx = new IncompleteNameAndArgsContext(_ctx, getState());
		enterRule(_localctx, 68, RULE_incompleteNameAndArgs);
		int _la;
		try {
			setState(700);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,53,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(693);
				match(T__21);
				setState(694);
				incompleteName();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(697);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__21) {
					{
					setState(695);
					match(T__21);
					setState(696);
					match(NAME);
					}
				}

				setState(699);
				incompleteArgs();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArgsContext extends ParserRuleContext {
		public ExplistContext explist() {
			return getRuleContext(ExplistContext.class,0);
		}
		public TableconstructorContext tableconstructor() {
			return getRuleContext(TableconstructorContext.class,0);
		}
		public StringContext string() {
			return getRuleContext(StringContext.class,0);
		}
		public ArgsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_args; }
	}

	public final ArgsContext args() throws RecognitionException {
		ArgsContext _localctx = new ArgsContext(_ctx, getState());
		enterRule(_localctx, 70, RULE_args);
		int _la;
		try {
			setState(709);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__26:
				enterOuterAlt(_localctx, 1);
				{
				setState(702);
				match(T__26);
				setState(704);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__16) | (1L << T__22) | (1L << T__23) | (1L << T__24) | (1L << T__25) | (1L << T__26) | (1L << T__30) | (1L << T__42) | (1L << T__49) | (1L << T__52) | (1L << T__53) | (1L << NAME) | (1L << NORMALSTRING) | (1L << CHARSTRING) | (1L << LONGSTRING) | (1L << INT) | (1L << HEX) | (1L << FLOAT) | (1L << HEX_FLOAT))) != 0)) {
					{
					setState(703);
					explist();
					}
				}

				setState(706);
				match(T__27);
				}
				break;
			case T__30:
				enterOuterAlt(_localctx, 2);
				{
				setState(707);
				tableconstructor();
				}
				break;
			case NORMALSTRING:
			case CHARSTRING:
			case LONGSTRING:
				enterOuterAlt(_localctx, 3);
				{
				setState(708);
				string();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteArgsContext extends ParserRuleContext {
		public IncompleteExplistContext incompleteExplist() {
			return getRuleContext(IncompleteExplistContext.class,0);
		}
		public IncompleteTableconstructorContext incompleteTableconstructor() {
			return getRuleContext(IncompleteTableconstructorContext.class,0);
		}
		public IncompleteStringContext incompleteString() {
			return getRuleContext(IncompleteStringContext.class,0);
		}
		public IncompleteArgsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteArgs; }
	}

	public final IncompleteArgsContext incompleteArgs() throws RecognitionException {
		IncompleteArgsContext _localctx = new IncompleteArgsContext(_ctx, getState());
		enterRule(_localctx, 72, RULE_incompleteArgs);
		try {
			setState(715);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__26:
				enterOuterAlt(_localctx, 1);
				{
				setState(711);
				match(T__26);
				setState(712);
				incompleteExplist();
				}
				break;
			case T__30:
				enterOuterAlt(_localctx, 2);
				{
				setState(713);
				incompleteTableconstructor();
				}
				break;
			case NORMALSTRING:
			case CHARSTRING:
			case LONGSTRING:
				enterOuterAlt(_localctx, 3);
				{
				setState(714);
				incompleteString();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FunctiondefContext extends ParserRuleContext {
		public FuncbodyContext funcbody() {
			return getRuleContext(FuncbodyContext.class,0);
		}
		public FunctiondefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_functiondef; }
	}

	public final FunctiondefContext functiondef() throws RecognitionException {
		FunctiondefContext _localctx = new FunctiondefContext(_ctx, getState());
		enterRule(_localctx, 74, RULE_functiondef);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(717);
			match(T__16);
			setState(718);
			funcbody();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteFunctiondefContext extends ParserRuleContext {
		public IncompleteFuncbodyContext incompleteFuncbody() {
			return getRuleContext(IncompleteFuncbodyContext.class,0);
		}
		public IncompleteFunctiondefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteFunctiondef; }
	}

	public final IncompleteFunctiondefContext incompleteFunctiondef() throws RecognitionException {
		IncompleteFunctiondefContext _localctx = new IncompleteFunctiondefContext(_ctx, getState());
		enterRule(_localctx, 76, RULE_incompleteFunctiondef);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(720);
			match(T__16);
			setState(721);
			incompleteFuncbody();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FuncbodyContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public ParlistContext parlist() {
			return getRuleContext(ParlistContext.class,0);
		}
		public FuncbodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcbody; }
	}

	public final FuncbodyContext funcbody() throws RecognitionException {
		FuncbodyContext _localctx = new FuncbodyContext(_ctx, getState());
		enterRule(_localctx, 78, RULE_funcbody);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(723);
			match(T__26);
			setState(725);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__25 || _la==NAME) {
				{
				setState(724);
				parlist();
				}
			}

			setState(727);
			match(T__27);
			setState(728);
			block();
			setState(729);
			match(T__5);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteFuncbodyContext extends ParserRuleContext {
		public IncompleteParlistContext incompleteParlist() {
			return getRuleContext(IncompleteParlistContext.class,0);
		}
		public IncompleteBlockContext incompleteBlock() {
			return getRuleContext(IncompleteBlockContext.class,0);
		}
		public ParlistContext parlist() {
			return getRuleContext(ParlistContext.class,0);
		}
		public IncompleteFuncbodyContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteFuncbody; }
	}

	public final IncompleteFuncbodyContext incompleteFuncbody() throws RecognitionException {
		IncompleteFuncbodyContext _localctx = new IncompleteFuncbodyContext(_ctx, getState());
		enterRule(_localctx, 80, RULE_incompleteFuncbody);
		int _la;
		try {
			setState(739);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,59,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(731);
				match(T__26);
				setState(732);
				incompleteParlist();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(733);
				match(T__26);
				setState(735);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__25 || _la==NAME) {
					{
					setState(734);
					parlist();
					}
				}

				setState(737);
				match(T__27);
				setState(738);
				incompleteBlock();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParlistContext extends ParserRuleContext {
		public NamelistContext namelist() {
			return getRuleContext(NamelistContext.class,0);
		}
		public ParlistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parlist; }
	}

	public final ParlistContext parlist() throws RecognitionException {
		ParlistContext _localctx = new ParlistContext(_ctx, getState());
		enterRule(_localctx, 82, RULE_parlist);
		int _la;
		try {
			setState(747);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case NAME:
				enterOuterAlt(_localctx, 1);
				{
				setState(741);
				namelist();
				setState(744);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==T__14) {
					{
					setState(742);
					match(T__14);
					setState(743);
					match(T__25);
					}
				}

				}
				break;
			case T__25:
				enterOuterAlt(_localctx, 2);
				{
				setState(746);
				match(T__25);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteParlistContext extends ParserRuleContext {
		public IncompleteNamelistContext incompleteNamelist() {
			return getRuleContext(IncompleteNamelistContext.class,0);
		}
		public IncompleteParlistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteParlist; }
	}

	public final IncompleteParlistContext incompleteParlist() throws RecognitionException {
		IncompleteParlistContext _localctx = new IncompleteParlistContext(_ctx, getState());
		enterRule(_localctx, 84, RULE_incompleteParlist);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(749);
			incompleteNamelist();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TableconstructorContext extends ParserRuleContext {
		public FieldlistContext fieldlist() {
			return getRuleContext(FieldlistContext.class,0);
		}
		public TableconstructorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_tableconstructor; }
	}

	public final TableconstructorContext tableconstructor() throws RecognitionException {
		TableconstructorContext _localctx = new TableconstructorContext(_ctx, getState());
		enterRule(_localctx, 86, RULE_tableconstructor);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(751);
			match(T__30);
			setState(753);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__16) | (1L << T__22) | (1L << T__23) | (1L << T__24) | (1L << T__25) | (1L << T__26) | (1L << T__28) | (1L << T__30) | (1L << T__42) | (1L << T__49) | (1L << T__52) | (1L << T__53) | (1L << NAME) | (1L << NORMALSTRING) | (1L << CHARSTRING) | (1L << LONGSTRING) | (1L << INT) | (1L << HEX) | (1L << FLOAT) | (1L << HEX_FLOAT))) != 0)) {
				{
				setState(752);
				fieldlist();
				}
			}

			setState(755);
			match(T__31);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteTableconstructorContext extends ParserRuleContext {
		public IncompleteFieldlistContext incompleteFieldlist() {
			return getRuleContext(IncompleteFieldlistContext.class,0);
		}
		public IncompleteTableconstructorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteTableconstructor; }
	}

	public final IncompleteTableconstructorContext incompleteTableconstructor() throws RecognitionException {
		IncompleteTableconstructorContext _localctx = new IncompleteTableconstructorContext(_ctx, getState());
		enterRule(_localctx, 88, RULE_incompleteTableconstructor);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(757);
			match(T__30);
			setState(758);
			incompleteFieldlist();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FieldlistContext extends ParserRuleContext {
		public List<FieldContext> field() {
			return getRuleContexts(FieldContext.class);
		}
		public FieldContext field(int i) {
			return getRuleContext(FieldContext.class,i);
		}
		public List<FieldsepContext> fieldsep() {
			return getRuleContexts(FieldsepContext.class);
		}
		public FieldsepContext fieldsep(int i) {
			return getRuleContext(FieldsepContext.class,i);
		}
		public FieldlistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fieldlist; }
	}

	public final FieldlistContext fieldlist() throws RecognitionException {
		FieldlistContext _localctx = new FieldlistContext(_ctx, getState());
		enterRule(_localctx, 90, RULE_fieldlist);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(760);
			field();
			setState(766);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,63,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(761);
					fieldsep();
					setState(762);
					field();
					}
					} 
				}
				setState(768);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,63,_ctx);
			}
			setState(770);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__0 || _la==T__14) {
				{
				setState(769);
				fieldsep();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteFieldlistContext extends ParserRuleContext {
		public IncompleteFieldContext incompleteField() {
			return getRuleContext(IncompleteFieldContext.class,0);
		}
		public List<FieldContext> field() {
			return getRuleContexts(FieldContext.class);
		}
		public FieldContext field(int i) {
			return getRuleContext(FieldContext.class,i);
		}
		public List<FieldsepContext> fieldsep() {
			return getRuleContexts(FieldsepContext.class);
		}
		public FieldsepContext fieldsep(int i) {
			return getRuleContext(FieldsepContext.class,i);
		}
		public IncompleteFieldlistContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteFieldlist; }
	}

	public final IncompleteFieldlistContext incompleteFieldlist() throws RecognitionException {
		IncompleteFieldlistContext _localctx = new IncompleteFieldlistContext(_ctx, getState());
		enterRule(_localctx, 92, RULE_incompleteFieldlist);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(777);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,65,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(772);
					field();
					setState(773);
					fieldsep();
					}
					} 
				}
				setState(779);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,65,_ctx);
			}
			setState(780);
			incompleteField();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FieldContext extends ParserRuleContext {
		public List<ExpContext> exp() {
			return getRuleContexts(ExpContext.class);
		}
		public ExpContext exp(int i) {
			return getRuleContext(ExpContext.class,i);
		}
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public FieldContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_field; }
	}

	public final FieldContext field() throws RecognitionException {
		FieldContext _localctx = new FieldContext(_ctx, getState());
		enterRule(_localctx, 94, RULE_field);
		try {
			setState(792);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,66,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(782);
				match(T__28);
				setState(783);
				exp(0);
				setState(784);
				match(T__29);
				setState(785);
				match(T__1);
				setState(786);
				exp(0);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(788);
				match(NAME);
				setState(789);
				match(T__1);
				setState(790);
				exp(0);
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(791);
				exp(0);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteFieldContext extends ParserRuleContext {
		public IncompleteExpContext incompleteExp() {
			return getRuleContext(IncompleteExpContext.class,0);
		}
		public ExpContext exp() {
			return getRuleContext(ExpContext.class,0);
		}
		public IncompleteNameContext incompleteName() {
			return getRuleContext(IncompleteNameContext.class,0);
		}
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public IncompleteFieldContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteField; }
	}

	public final IncompleteFieldContext incompleteField() throws RecognitionException {
		IncompleteFieldContext _localctx = new IncompleteFieldContext(_ctx, getState());
		enterRule(_localctx, 96, RULE_incompleteField);
		try {
			setState(807);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,67,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(794);
				match(T__28);
				setState(795);
				incompleteExp();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(796);
				match(T__28);
				setState(797);
				exp(0);
				setState(798);
				match(T__29);
				setState(799);
				match(T__1);
				setState(800);
				incompleteExp();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(802);
				incompleteName();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(803);
				match(NAME);
				setState(804);
				match(T__1);
				setState(805);
				incompleteExp();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(806);
				incompleteExp();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FieldsepContext extends ParserRuleContext {
		public FieldsepContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_fieldsep; }
	}

	public final FieldsepContext fieldsep() throws RecognitionException {
		FieldsepContext _localctx = new FieldsepContext(_ctx, getState());
		enterRule(_localctx, 98, RULE_fieldsep);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(809);
			_la = _input.LA(1);
			if ( !(_la==T__0 || _la==T__14) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorOrContext extends ParserRuleContext {
		public OperatorOrContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorOr; }
	}

	public final OperatorOrContext operatorOr() throws RecognitionException {
		OperatorOrContext _localctx = new OperatorOrContext(_ctx, getState());
		enterRule(_localctx, 100, RULE_operatorOr);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(811);
			match(T__32);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorAndContext extends ParserRuleContext {
		public OperatorAndContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorAnd; }
	}

	public final OperatorAndContext operatorAnd() throws RecognitionException {
		OperatorAndContext _localctx = new OperatorAndContext(_ctx, getState());
		enterRule(_localctx, 102, RULE_operatorAnd);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(813);
			match(T__33);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorComparisonContext extends ParserRuleContext {
		public OperatorComparisonContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorComparison; }
	}

	public final OperatorComparisonContext operatorComparison() throws RecognitionException {
		OperatorComparisonContext _localctx = new OperatorComparisonContext(_ctx, getState());
		enterRule(_localctx, 104, RULE_operatorComparison);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(815);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__34) | (1L << T__35) | (1L << T__36) | (1L << T__37) | (1L << T__38) | (1L << T__39))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorStrcatContext extends ParserRuleContext {
		public OperatorStrcatContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorStrcat; }
	}

	public final OperatorStrcatContext operatorStrcat() throws RecognitionException {
		OperatorStrcatContext _localctx = new OperatorStrcatContext(_ctx, getState());
		enterRule(_localctx, 106, RULE_operatorStrcat);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(817);
			match(T__40);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorAddSubContext extends ParserRuleContext {
		public OperatorAddSubContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorAddSub; }
	}

	public final OperatorAddSubContext operatorAddSub() throws RecognitionException {
		OperatorAddSubContext _localctx = new OperatorAddSubContext(_ctx, getState());
		enterRule(_localctx, 108, RULE_operatorAddSub);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(819);
			_la = _input.LA(1);
			if ( !(_la==T__41 || _la==T__42) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorMulDivModContext extends ParserRuleContext {
		public OperatorMulDivModContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorMulDivMod; }
	}

	public final OperatorMulDivModContext operatorMulDivMod() throws RecognitionException {
		OperatorMulDivModContext _localctx = new OperatorMulDivModContext(_ctx, getState());
		enterRule(_localctx, 110, RULE_operatorMulDivMod);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(821);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__43) | (1L << T__44) | (1L << T__45) | (1L << T__46))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorBitwiseContext extends ParserRuleContext {
		public OperatorBitwiseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorBitwise; }
	}

	public final OperatorBitwiseContext operatorBitwise() throws RecognitionException {
		OperatorBitwiseContext _localctx = new OperatorBitwiseContext(_ctx, getState());
		enterRule(_localctx, 112, RULE_operatorBitwise);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(823);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__47) | (1L << T__48) | (1L << T__49) | (1L << T__50) | (1L << T__51))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorUnaryContext extends ParserRuleContext {
		public OperatorUnaryContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorUnary; }
	}

	public final OperatorUnaryContext operatorUnary() throws RecognitionException {
		OperatorUnaryContext _localctx = new OperatorUnaryContext(_ctx, getState());
		enterRule(_localctx, 114, RULE_operatorUnary);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(825);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__42) | (1L << T__49) | (1L << T__52) | (1L << T__53))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OperatorPowerContext extends ParserRuleContext {
		public OperatorPowerContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operatorPower; }
	}

	public final OperatorPowerContext operatorPower() throws RecognitionException {
		OperatorPowerContext _localctx = new OperatorPowerContext(_ctx, getState());
		enterRule(_localctx, 116, RULE_operatorPower);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(827);
			match(T__54);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NumberContext extends ParserRuleContext {
		public TerminalNode INT() { return getToken(IncompleteLuaParser.INT, 0); }
		public TerminalNode HEX() { return getToken(IncompleteLuaParser.HEX, 0); }
		public TerminalNode FLOAT() { return getToken(IncompleteLuaParser.FLOAT, 0); }
		public TerminalNode HEX_FLOAT() { return getToken(IncompleteLuaParser.HEX_FLOAT, 0); }
		public NumberContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_number; }
	}

	public final NumberContext number() throws RecognitionException {
		NumberContext _localctx = new NumberContext(_ctx, getState());
		enterRule(_localctx, 118, RULE_number);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(829);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << INT) | (1L << HEX) | (1L << FLOAT) | (1L << HEX_FLOAT))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StringContext extends ParserRuleContext {
		public TerminalNode NORMALSTRING() { return getToken(IncompleteLuaParser.NORMALSTRING, 0); }
		public TerminalNode CHARSTRING() { return getToken(IncompleteLuaParser.CHARSTRING, 0); }
		public TerminalNode LONGSTRING() { return getToken(IncompleteLuaParser.LONGSTRING, 0); }
		public StringContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_string; }
	}

	public final StringContext string() throws RecognitionException {
		StringContext _localctx = new StringContext(_ctx, getState());
		enterRule(_localctx, 120, RULE_string);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(831);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << NORMALSTRING) | (1L << CHARSTRING) | (1L << LONGSTRING))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteStringContext extends ParserRuleContext {
		public TerminalNode NORMALSTRING() { return getToken(IncompleteLuaParser.NORMALSTRING, 0); }
		public TerminalNode CHARSTRING() { return getToken(IncompleteLuaParser.CHARSTRING, 0); }
		public TerminalNode LONGSTRING() { return getToken(IncompleteLuaParser.LONGSTRING, 0); }
		public IncompleteStringContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteString; }
	}

	public final IncompleteStringContext incompleteString() throws RecognitionException {
		IncompleteStringContext _localctx = new IncompleteStringContext(_ctx, getState());
		enterRule(_localctx, 122, RULE_incompleteString);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(833);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << NORMALSTRING) | (1L << CHARSTRING) | (1L << LONGSTRING))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IncompleteNameContext extends ParserRuleContext {
		public TerminalNode NAME() { return getToken(IncompleteLuaParser.NAME, 0); }
		public IncompleteNameContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_incompleteName; }
	}

	public final IncompleteNameContext incompleteName() throws RecognitionException {
		IncompleteNameContext _localctx = new IncompleteNameContext(_ctx, getState());
		enterRule(_localctx, 124, RULE_incompleteName);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(835);
			match(NAME);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 18:
			return exp_sempred((ExpContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean exp_sempred(ExpContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 9);
		case 1:
			return precpred(_ctx, 7);
		case 2:
			return precpred(_ctx, 6);
		case 3:
			return precpred(_ctx, 5);
		case 4:
			return precpred(_ctx, 4);
		case 5:
			return precpred(_ctx, 3);
		case 6:
			return precpred(_ctx, 2);
		case 7:
			return precpred(_ctx, 1);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3E\u0348\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\4&\t&\4\'\t\'\4(\t(\4)\t)\4*\t*\4+\t+\4"+
		",\t,\4-\t-\4.\t.\4/\t/\4\60\t\60\4\61\t\61\4\62\t\62\4\63\t\63\4\64\t"+
		"\64\4\65\t\65\4\66\t\66\4\67\t\67\48\t8\49\t9\4:\t:\4;\t;\4<\t<\4=\t="+
		"\4>\t>\4?\t?\4@\t@\3\2\3\2\3\2\3\3\3\3\3\3\3\4\7\4\u0088\n\4\f\4\16\4"+
		"\u008b\13\4\3\4\5\4\u008e\n\4\3\5\7\5\u0091\n\5\f\5\16\5\u0094\13\5\3"+
		"\5\3\5\7\5\u0098\n\5\f\5\16\5\u009b\13\5\3\5\5\5\u009e\n\5\3\6\3\6\3\6"+
		"\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3"+
		"\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\7\6\u00c2\n\6\f"+
		"\6\16\6\u00c5\13\6\3\6\3\6\5\6\u00c9\n\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3"+
		"\6\3\6\3\6\5\6\u00d5\n\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3"+
		"\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\6\5\6\u00ef\n\6\5\6\u00f1"+
		"\n\6\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3"+
		"\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7"+
		"\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3"+
		"\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\5\7"+
		"\u0137\n\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7"+
		"\3\7\3\7\3\7\3\7\3\7\3\7\3\7\3\7\5\7\u0150\n\7\3\b\3\b\3\b\3\b\3\b\7\b"+
		"\u0157\n\b\f\b\16\b\u015a\13\b\3\b\3\b\3\b\3\b\3\b\3\b\3\b\3\b\3\b\3\b"+
		"\3\b\3\b\7\b\u0168\n\b\f\b\16\b\u016b\13\b\3\b\3\b\5\b\u016f\n\b\3\t\3"+
		"\t\5\t\u0173\n\t\3\t\5\t\u0176\n\t\3\n\3\n\3\n\3\13\3\13\3\13\3\13\3\f"+
		"\3\f\3\f\7\f\u0182\n\f\f\f\16\f\u0185\13\f\3\f\3\f\5\f\u0189\n\f\3\r\3"+
		"\r\7\r\u018d\n\r\f\r\16\r\u0190\13\r\3\r\3\r\3\r\3\r\7\r\u0196\n\r\f\r"+
		"\16\r\u0199\13\r\3\r\3\r\5\r\u019d\n\r\3\16\3\16\3\16\7\16\u01a2\n\16"+
		"\f\16\16\16\u01a5\13\16\3\17\3\17\3\17\7\17\u01aa\n\17\f\17\16\17\u01ad"+
		"\13\17\3\17\3\17\3\20\3\20\3\20\7\20\u01b4\n\20\f\20\16\20\u01b7\13\20"+
		"\3\21\3\21\7\21\u01bb\n\21\f\21\16\21\u01be\13\21\3\21\3\21\3\22\3\22"+
		"\3\22\7\22\u01c5\n\22\f\22\16\22\u01c8\13\22\3\23\3\23\3\23\7\23\u01cd"+
		"\n\23\f\23\16\23\u01d0\13\23\3\23\3\23\3\24\3\24\3\24\3\24\3\24\3\24\3"+
		"\24\3\24\3\24\3\24\3\24\3\24\3\24\5\24\u01e1\n\24\3\24\3\24\3\24\3\24"+
		"\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24"+
		"\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24\3\24"+
		"\7\24\u0203\n\24\f\24\16\24\u0206\13\24\3\25\3\25\3\25\3\25\3\25\3\25"+
		"\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25"+
		"\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25\3\25"+
		"\3\25\3\25\3\25\3\25\5\25\u022e\n\25\3\26\3\26\7\26\u0232\n\26\f\26\16"+
		"\26\u0235\13\26\3\27\3\27\3\27\7\27\u023a\n\27\f\27\16\27\u023d\13\27"+
		"\3\27\3\27\5\27\u0241\n\27\3\30\3\30\6\30\u0245\n\30\r\30\16\30\u0246"+
		"\3\31\3\31\3\31\7\31\u024c\n\31\f\31\16\31\u024f\13\31\3\31\3\31\5\31"+
		"\u0253\n\31\3\32\3\32\3\32\3\32\3\32\5\32\u025a\n\32\3\33\3\33\3\33\5"+
		"\33\u025f\n\33\3\34\3\34\3\34\3\34\3\34\3\34\5\34\u0267\n\34\3\34\7\34"+
		"\u026a\n\34\f\34\16\34\u026d\13\34\3\35\3\35\3\35\3\35\3\35\3\35\3\35"+
		"\3\35\3\35\3\35\3\35\3\35\3\35\3\35\5\35\u027d\n\35\3\35\7\35\u0280\n"+
		"\35\f\35\16\35\u0283\13\35\3\35\3\35\5\35\u0287\n\35\3\36\3\36\3\37\7"+
		"\37\u028c\n\37\f\37\16\37\u028f\13\37\3\37\3\37\3\37\3\37\3\37\3\37\5"+
		"\37\u0297\n\37\3 \3 \3!\3!\3\"\7\"\u029e\n\"\f\"\16\"\u02a1\13\"\3\"\3"+
		"\"\7\"\u02a5\n\"\f\"\16\"\u02a8\13\"\3\"\3\"\3\"\3\"\5\"\u02ae\n\"\5\""+
		"\u02b0\n\"\3#\3#\5#\u02b4\n#\3#\3#\3$\3$\3$\3$\5$\u02bc\n$\3$\5$\u02bf"+
		"\n$\3%\3%\5%\u02c3\n%\3%\3%\3%\5%\u02c8\n%\3&\3&\3&\3&\5&\u02ce\n&\3\'"+
		"\3\'\3\'\3(\3(\3(\3)\3)\5)\u02d8\n)\3)\3)\3)\3)\3*\3*\3*\3*\5*\u02e2\n"+
		"*\3*\3*\5*\u02e6\n*\3+\3+\3+\5+\u02eb\n+\3+\5+\u02ee\n+\3,\3,\3-\3-\5"+
		"-\u02f4\n-\3-\3-\3.\3.\3.\3/\3/\3/\3/\7/\u02ff\n/\f/\16/\u0302\13/\3/"+
		"\5/\u0305\n/\3\60\3\60\3\60\7\60\u030a\n\60\f\60\16\60\u030d\13\60\3\60"+
		"\3\60\3\61\3\61\3\61\3\61\3\61\3\61\3\61\3\61\3\61\3\61\5\61\u031b\n\61"+
		"\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\3\62\5\62"+
		"\u032a\n\62\3\63\3\63\3\64\3\64\3\65\3\65\3\66\3\66\3\67\3\67\38\38\3"+
		"9\39\3:\3:\3;\3;\3<\3<\3=\3=\3>\3>\3?\3?\3@\3@\3@\2\3&A\2\4\6\b\n\f\16"+
		"\20\22\24\26\30\32\34\36 \"$&(*,.\60\62\64\668:<>@BDFHJLNPRTVXZ\\^`bd"+
		"fhjlnprtvxz|~\2\n\4\2\3\3\21\21\3\2%*\3\2,-\3\2.\61\3\2\62\66\5\2--\64"+
		"\64\678\3\2>A\3\2;=\2\u0390\2\u0080\3\2\2\2\4\u0083\3\2\2\2\6\u0089\3"+
		"\2\2\2\b\u009d\3\2\2\2\n\u00f0\3\2\2\2\f\u014f\3\2\2\2\16\u016e\3\2\2"+
		"\2\20\u0170\3\2\2\2\22\u0177\3\2\2\2\24\u017a\3\2\2\2\26\u017e\3\2\2\2"+
		"\30\u019c\3\2\2\2\32\u019e\3\2\2\2\34\u01ab\3\2\2\2\36\u01b0\3\2\2\2 "+
		"\u01bc\3\2\2\2\"\u01c1\3\2\2\2$\u01ce\3\2\2\2&\u01e0\3\2\2\2(\u022d\3"+
		"\2\2\2*\u022f\3\2\2\2,\u0240\3\2\2\2.\u0242\3\2\2\2\60\u0252\3\2\2\2\62"+
		"\u0259\3\2\2\2\64\u025e\3\2\2\2\66\u0266\3\2\2\28\u0286\3\2\2\2:\u0288"+
		"\3\2\2\2<\u028d\3\2\2\2>\u0298\3\2\2\2@\u029a\3\2\2\2B\u02af\3\2\2\2D"+
		"\u02b3\3\2\2\2F\u02be\3\2\2\2H\u02c7\3\2\2\2J\u02cd\3\2\2\2L\u02cf\3\2"+
		"\2\2N\u02d2\3\2\2\2P\u02d5\3\2\2\2R\u02e5\3\2\2\2T\u02ed\3\2\2\2V\u02ef"+
		"\3\2\2\2X\u02f1\3\2\2\2Z\u02f7\3\2\2\2\\\u02fa\3\2\2\2^\u030b\3\2\2\2"+
		"`\u031a\3\2\2\2b\u0329\3\2\2\2d\u032b\3\2\2\2f\u032d\3\2\2\2h\u032f\3"+
		"\2\2\2j\u0331\3\2\2\2l\u0333\3\2\2\2n\u0335\3\2\2\2p\u0337\3\2\2\2r\u0339"+
		"\3\2\2\2t\u033b\3\2\2\2v\u033d\3\2\2\2x\u033f\3\2\2\2z\u0341\3\2\2\2|"+
		"\u0343\3\2\2\2~\u0345\3\2\2\2\u0080\u0081\5\6\4\2\u0081\u0082\7\2\2\3"+
		"\u0082\3\3\2\2\2\u0083\u0084\5\b\5\2\u0084\u0085\7\2\2\3\u0085\5\3\2\2"+
		"\2\u0086\u0088\5\n\6\2\u0087\u0086\3\2\2\2\u0088\u008b\3\2\2\2\u0089\u0087"+
		"\3\2\2\2\u0089\u008a\3\2\2\2\u008a\u008d\3\2\2\2\u008b\u0089\3\2\2\2\u008c"+
		"\u008e\5\20\t\2\u008d\u008c\3\2\2\2\u008d\u008e\3\2\2\2\u008e\7\3\2\2"+
		"\2\u008f\u0091\5\n\6\2\u0090\u008f\3\2\2\2\u0091\u0094\3\2\2\2\u0092\u0090"+
		"\3\2\2\2\u0092\u0093\3\2\2\2\u0093\u0095\3\2\2\2\u0094\u0092\3\2\2\2\u0095"+
		"\u009e\5\f\7\2\u0096\u0098\5\n\6\2\u0097\u0096\3\2\2\2\u0098\u009b\3\2"+
		"\2\2\u0099\u0097\3\2\2\2\u0099\u009a\3\2\2\2\u009a\u009c\3\2\2\2\u009b"+
		"\u0099\3\2\2\2\u009c\u009e\5\22\n\2\u009d\u0092\3\2\2\2\u009d\u0099\3"+
		"\2\2\2\u009e\t\3\2\2\2\u009f\u00f1\7\3\2\2\u00a0\u00a1\5\32\16\2\u00a1"+
		"\u00a2\7\4\2\2\u00a2\u00a3\5\"\22\2\u00a3\u00f1\3\2\2\2\u00a4\u00f1\5"+
		".\30\2\u00a5\u00f1\5\24\13\2\u00a6\u00f1\7\5\2\2\u00a7\u00a8\7\6\2\2\u00a8"+
		"\u00f1\7:\2\2\u00a9\u00aa\7\7\2\2\u00aa\u00ab\5\6\4\2\u00ab\u00ac\7\b"+
		"\2\2\u00ac\u00f1\3\2\2\2\u00ad\u00ae\7\t\2\2\u00ae\u00af\5&\24\2\u00af"+
		"\u00b0\7\7\2\2\u00b0\u00b1\5\6\4\2\u00b1\u00b2\7\b\2\2\u00b2\u00f1\3\2"+
		"\2\2\u00b3\u00b4\7\n\2\2\u00b4\u00b5\5\6\4\2\u00b5\u00b6\7\13\2\2\u00b6"+
		"\u00b7\5&\24\2\u00b7\u00f1\3\2\2\2\u00b8\u00b9\7\f\2\2\u00b9\u00ba\5&"+
		"\24\2\u00ba\u00bb\7\r\2\2\u00bb\u00c3\5\6\4\2\u00bc\u00bd\7\16\2\2\u00bd"+
		"\u00be\5&\24\2\u00be\u00bf\7\r\2\2\u00bf\u00c0\5\6\4\2\u00c0\u00c2\3\2"+
		"\2\2\u00c1\u00bc\3\2\2\2\u00c2\u00c5\3\2\2\2\u00c3\u00c1\3\2\2\2\u00c3"+
		"\u00c4\3\2\2\2\u00c4\u00c8\3\2\2\2\u00c5\u00c3\3\2\2\2\u00c6\u00c7\7\17"+
		"\2\2\u00c7\u00c9\5\6\4\2\u00c8\u00c6\3\2\2\2\u00c8\u00c9\3\2\2\2\u00c9"+
		"\u00ca\3\2\2\2\u00ca\u00cb\7\b\2\2\u00cb\u00f1\3\2\2\2\u00cc\u00cd\7\20"+
		"\2\2\u00cd\u00ce\7:\2\2\u00ce\u00cf\7\4\2\2\u00cf\u00d0\5&\24\2\u00d0"+
		"\u00d1\7\21\2\2\u00d1\u00d4\5&\24\2\u00d2\u00d3\7\21\2\2\u00d3\u00d5\5"+
		"&\24\2\u00d4\u00d2\3\2\2\2\u00d4\u00d5\3\2\2\2\u00d5\u00d6\3\2\2\2\u00d6"+
		"\u00d7\7\7\2\2\u00d7\u00d8\5\6\4\2\u00d8\u00d9\7\b\2\2\u00d9\u00f1\3\2"+
		"\2\2\u00da\u00db\7\20\2\2\u00db\u00dc\5\36\20\2\u00dc\u00dd\7\22\2\2\u00dd"+
		"\u00de\5\"\22\2\u00de\u00df\7\7\2\2\u00df\u00e0\5\6\4\2\u00e0\u00e1\7"+
		"\b\2\2\u00e1\u00f1\3\2\2\2\u00e2\u00e3\7\23\2\2\u00e3\u00e4\5\26\f\2\u00e4"+
		"\u00e5\5P)\2\u00e5\u00f1\3\2\2\2\u00e6\u00e7\7\24\2\2\u00e7\u00e8\7\23"+
		"\2\2\u00e8\u00e9\7:\2\2\u00e9\u00f1\5P)\2\u00ea\u00eb\7\24\2\2\u00eb\u00ee"+
		"\5\36\20\2\u00ec\u00ed\7\4\2\2\u00ed\u00ef\5\"\22\2\u00ee\u00ec\3\2\2"+
		"\2\u00ee\u00ef\3\2\2\2\u00ef\u00f1\3\2\2\2\u00f0\u009f\3\2\2\2\u00f0\u00a0"+
		"\3\2\2\2\u00f0\u00a4\3\2\2\2\u00f0\u00a5\3\2\2\2\u00f0\u00a6\3\2\2\2\u00f0"+
		"\u00a7\3\2\2\2\u00f0\u00a9\3\2\2\2\u00f0\u00ad\3\2\2\2\u00f0\u00b3\3\2"+
		"\2\2\u00f0\u00b8\3\2\2\2\u00f0\u00cc\3\2\2\2\u00f0\u00da\3\2\2\2\u00f0"+
		"\u00e2\3\2\2\2\u00f0\u00e6\3\2\2\2\u00f0\u00ea\3\2\2\2\u00f1\13\3\2\2"+
		"\2\u00f2\u0150\5\34\17\2\u00f3\u00f4\5\32\16\2\u00f4\u00f5\7\4\2\2\u00f5"+
		"\u00f6\5$\23\2\u00f6\u0150\3\2\2\2\u00f7\u0150\5\60\31\2\u00f8\u00f9\7"+
		"\6\2\2\u00f9\u0150\5~@\2\u00fa\u00fb\7\7\2\2\u00fb\u0150\5\b\5\2\u00fc"+
		"\u00fd\7\t\2\2\u00fd\u0150\5(\25\2\u00fe\u00ff\7\t\2\2\u00ff\u0100\5&"+
		"\24\2\u0100\u0101\7\7\2\2\u0101\u0102\5\b\5\2\u0102\u0150\3\2\2\2\u0103"+
		"\u0104\7\n\2\2\u0104\u0150\5\b\5\2\u0105\u0106\7\n\2\2\u0106\u0107\5\6"+
		"\4\2\u0107\u0108\7\13\2\2\u0108\u0109\5(\25\2\u0109\u0150\3\2\2\2\u010a"+
		"\u010b\7\f\2\2\u010b\u0150\5(\25\2\u010c\u010d\7\f\2\2\u010d\u010e\5&"+
		"\24\2\u010e\u010f\7\r\2\2\u010f\u0110\5\b\5\2\u0110\u0150\3\2\2\2\u0111"+
		"\u0112\7\f\2\2\u0112\u0113\5&\24\2\u0113\u0114\7\r\2\2\u0114\u0115\5\6"+
		"\4\2\u0115\u0116\5\16\b\2\u0116\u0150\3\2\2\2\u0117\u0118\7\20\2\2\u0118"+
		"\u0150\5~@\2\u0119\u011a\7\20\2\2\u011a\u011b\7:\2\2\u011b\u011c\7\4\2"+
		"\2\u011c\u0150\5(\25\2\u011d\u011e\7\20\2\2\u011e\u011f\7:\2\2\u011f\u0120"+
		"\7\4\2\2\u0120\u0121\5&\24\2\u0121\u0122\7\21\2\2\u0122\u0123\5(\25\2"+
		"\u0123\u0150\3\2\2\2\u0124\u0125\7\20\2\2\u0125\u0126\7:\2\2\u0126\u0127"+
		"\7\4\2\2\u0127\u0128\5&\24\2\u0128\u0129\7\21\2\2\u0129\u012a\5&\24\2"+
		"\u012a\u012b\7\21\2\2\u012b\u012c\5(\25\2\u012c\u0150\3\2\2\2\u012d\u0150"+
		"\3\2\2\2\u012e\u012f\7\20\2\2\u012f\u0130\7:\2\2\u0130\u0131\7\4\2\2\u0131"+
		"\u0132\5&\24\2\u0132\u0133\7\21\2\2\u0133\u0136\5&\24\2\u0134\u0135\7"+
		"\21\2\2\u0135\u0137\5&\24\2\u0136\u0134\3\2\2\2\u0136\u0137\3\2\2\2\u0137"+
		"\u0138\3\2\2\2\u0138\u0139\7\7\2\2\u0139\u013a\5\b\5\2\u013a\u0150\3\2"+
		"\2\2\u013b\u013c\7\23\2\2\u013c\u0150\5\30\r\2\u013d\u013e\7\23\2\2\u013e"+
		"\u013f\5\26\f\2\u013f\u0140\5R*\2\u0140\u0150\3\2\2\2\u0141\u0142\7\24"+
		"\2\2\u0142\u0143\7\23\2\2\u0143\u0150\5~@\2\u0144\u0145\7\24\2\2\u0145"+
		"\u0146\7\23\2\2\u0146\u0147\7:\2\2\u0147\u0150\5R*\2\u0148\u0149\7\24"+
		"\2\2\u0149\u0150\5 \21\2\u014a\u014b\7\24\2\2\u014b\u014c\5\36\20\2\u014c"+
		"\u014d\7\4\2\2\u014d\u014e\5$\23\2\u014e\u0150\3\2\2\2\u014f\u00f2\3\2"+
		"\2\2\u014f\u00f3\3\2\2\2\u014f\u00f7\3\2\2\2\u014f\u00f8\3\2\2\2\u014f"+
		"\u00fa\3\2\2\2\u014f\u00fc\3\2\2\2\u014f\u00fe\3\2\2\2\u014f\u0103\3\2"+
		"\2\2\u014f\u0105\3\2\2\2\u014f\u010a\3\2\2\2\u014f\u010c\3\2\2\2\u014f"+
		"\u0111\3\2\2\2\u014f\u0117\3\2\2\2\u014f\u0119\3\2\2\2\u014f\u011d\3\2"+
		"\2\2\u014f\u0124\3\2\2\2\u014f\u012d\3\2\2\2\u014f\u012e\3\2\2\2\u014f"+
		"\u013b\3\2\2\2\u014f\u013d\3\2\2\2\u014f\u0141\3\2\2\2\u014f\u0144\3\2"+
		"\2\2\u014f\u0148\3\2\2\2\u014f\u014a\3\2\2\2\u0150\r\3\2\2\2\u0151\u0152"+
		"\7\16\2\2\u0152\u0153\5&\24\2\u0153\u0154\7\r\2\2\u0154\u0155\5\6\4\2"+
		"\u0155\u0157\3\2\2\2\u0156\u0151\3\2\2\2\u0157\u015a\3\2\2\2\u0158\u0156"+
		"\3\2\2\2\u0158\u0159\3\2\2\2\u0159\u015b\3\2\2\2\u015a\u0158\3\2\2\2\u015b"+
		"\u015c\7\16\2\2\u015c\u016f\5(\25\2\u015d\u015e\7\16\2\2\u015e\u015f\5"+
		"&\24\2\u015f\u0160\7\r\2\2\u0160\u0161\5\b\5\2\u0161\u016f\3\2\2\2\u0162"+
		"\u0163\7\16\2\2\u0163\u0164\5&\24\2\u0164\u0165\7\r\2\2\u0165\u0166\5"+
		"\6\4\2\u0166\u0168\3\2\2\2\u0167\u0162\3\2\2\2\u0168\u016b\3\2\2\2\u0169"+
		"\u0167\3\2\2\2\u0169\u016a\3\2\2\2\u016a\u016c\3\2\2\2\u016b\u0169\3\2"+
		"\2\2\u016c\u016d\7\17\2\2\u016d\u016f\5\b\5\2\u016e\u0158\3\2\2\2\u016e"+
		"\u015d\3\2\2\2\u016e\u0169\3\2\2\2\u016f\17\3\2\2\2\u0170\u0172\7\25\2"+
		"\2\u0171\u0173\5\"\22\2\u0172\u0171\3\2\2\2\u0172\u0173\3\2\2\2\u0173"+
		"\u0175\3\2\2\2\u0174\u0176\7\3\2\2\u0175\u0174\3\2\2\2\u0175\u0176\3\2"+
		"\2\2\u0176\21\3\2\2\2\u0177\u0178\7\25\2\2\u0178\u0179\5$\23\2\u0179\23"+
		"\3\2\2\2\u017a\u017b\7\26\2\2\u017b\u017c\7:\2\2\u017c\u017d\7\26\2\2"+
		"\u017d\25\3\2\2\2\u017e\u0183\7:\2\2\u017f\u0180\7\27\2\2\u0180\u0182"+
		"\7:\2\2\u0181\u017f\3\2\2\2\u0182\u0185\3\2\2\2\u0183\u0181\3\2\2\2\u0183"+
		"\u0184\3\2\2\2\u0184\u0188\3\2\2\2\u0185\u0183\3\2\2\2\u0186\u0187\7\30"+
		"\2\2\u0187\u0189\7:\2\2\u0188\u0186\3\2\2\2\u0188\u0189\3\2\2\2\u0189"+
		"\27\3\2\2\2\u018a\u018b\7:\2\2\u018b\u018d\7\27\2\2\u018c\u018a\3\2\2"+
		"\2\u018d\u0190\3\2\2\2\u018e\u018c\3\2\2\2\u018e\u018f\3\2\2\2\u018f\u0191"+
		"\3\2\2\2\u0190\u018e\3\2\2\2\u0191\u019d\5~@\2\u0192\u0197\7:\2\2\u0193"+
		"\u0194\7\27\2\2\u0194\u0196\7:\2\2\u0195\u0193\3\2\2\2\u0196\u0199\3\2"+
		"\2\2\u0197\u0195\3\2\2\2\u0197\u0198\3\2\2\2\u0198\u019a\3\2\2\2\u0199"+
		"\u0197\3\2\2\2\u019a\u019b\7\30\2\2\u019b\u019d\5~@\2\u019c\u018e\3\2"+
		"\2\2\u019c\u0192\3\2\2\2\u019d\31\3\2\2\2\u019e\u01a3\5\66\34\2\u019f"+
		"\u01a0\7\21\2\2\u01a0\u01a2\5\66\34\2\u01a1\u019f\3\2\2\2\u01a2\u01a5"+
		"\3\2\2\2\u01a3\u01a1\3\2\2\2\u01a3\u01a4\3\2\2\2\u01a4\33\3\2\2\2\u01a5"+
		"\u01a3\3\2\2\2\u01a6\u01a7\5\66\34\2\u01a7\u01a8\7\21\2\2\u01a8\u01aa"+
		"\3\2\2\2\u01a9\u01a6\3\2\2\2\u01aa\u01ad\3\2\2\2\u01ab\u01a9\3\2\2\2\u01ab"+
		"\u01ac\3\2\2\2\u01ac\u01ae\3\2\2\2\u01ad\u01ab\3\2\2\2\u01ae\u01af\58"+
		"\35\2\u01af\35\3\2\2\2\u01b0\u01b5\7:\2\2\u01b1\u01b2\7\21\2\2\u01b2\u01b4"+
		"\7:\2\2\u01b3\u01b1\3\2\2\2\u01b4\u01b7\3\2\2\2\u01b5\u01b3\3\2\2\2\u01b5"+
		"\u01b6\3\2\2\2\u01b6\37\3\2\2\2\u01b7\u01b5\3\2\2\2\u01b8\u01b9\7:\2\2"+
		"\u01b9\u01bb\7\21\2\2\u01ba\u01b8\3\2\2\2\u01bb\u01be\3\2\2\2\u01bc\u01ba"+
		"\3\2\2\2\u01bc\u01bd\3\2\2\2\u01bd\u01bf\3\2\2\2\u01be\u01bc\3\2\2\2\u01bf"+
		"\u01c0\5~@\2\u01c0!\3\2\2\2\u01c1\u01c6\5&\24\2\u01c2\u01c3\7\21\2\2\u01c3"+
		"\u01c5\5&\24\2\u01c4\u01c2\3\2\2\2\u01c5\u01c8\3\2\2\2\u01c6\u01c4\3\2"+
		"\2\2\u01c6\u01c7\3\2\2\2\u01c7#\3\2\2\2\u01c8\u01c6\3\2\2\2\u01c9\u01ca"+
		"\5&\24\2\u01ca\u01cb\7\21\2\2\u01cb\u01cd\3\2\2\2\u01cc\u01c9\3\2\2\2"+
		"\u01cd\u01d0\3\2\2\2\u01ce\u01cc\3\2\2\2\u01ce\u01cf\3\2\2\2\u01cf\u01d1"+
		"\3\2\2\2\u01d0\u01ce\3\2\2\2\u01d1\u01d2\5(\25\2\u01d2%\3\2\2\2\u01d3"+
		"\u01d4\b\24\1\2\u01d4\u01e1\7\31\2\2\u01d5\u01e1\7\32\2\2\u01d6\u01e1"+
		"\7\33\2\2\u01d7\u01e1\5x=\2\u01d8\u01e1\5z>\2\u01d9\u01e1\7\34\2\2\u01da"+
		"\u01e1\5L\'\2\u01db\u01e1\5*\26\2\u01dc\u01e1\5X-\2\u01dd\u01de\5t;\2"+
		"\u01de\u01df\5&\24\n\u01df\u01e1\3\2\2\2\u01e0\u01d3\3\2\2\2\u01e0\u01d5"+
		"\3\2\2\2\u01e0\u01d6\3\2\2\2\u01e0\u01d7\3\2\2\2\u01e0\u01d8\3\2\2\2\u01e0"+
		"\u01d9\3\2\2\2\u01e0\u01da\3\2\2\2\u01e0\u01db\3\2\2\2\u01e0\u01dc\3\2"+
		"\2\2\u01e0\u01dd\3\2\2\2\u01e1\u0204\3\2\2\2\u01e2\u01e3\f\13\2\2\u01e3"+
		"\u01e4\5v<\2\u01e4\u01e5\5&\24\13\u01e5\u0203\3\2\2\2\u01e6\u01e7\f\t"+
		"\2\2\u01e7\u01e8\5p9\2\u01e8\u01e9\5&\24\n\u01e9\u0203\3\2\2\2\u01ea\u01eb"+
		"\f\b\2\2\u01eb\u01ec\5n8\2\u01ec\u01ed\5&\24\t\u01ed\u0203\3\2\2\2\u01ee"+
		"\u01ef\f\7\2\2\u01ef\u01f0\5l\67\2\u01f0\u01f1\5&\24\7\u01f1\u0203\3\2"+
		"\2\2\u01f2\u01f3\f\6\2\2\u01f3\u01f4\5j\66\2\u01f4\u01f5\5&\24\7\u01f5"+
		"\u0203\3\2\2\2\u01f6\u01f7\f\5\2\2\u01f7\u01f8\5h\65\2\u01f8\u01f9\5&"+
		"\24\6\u01f9\u0203\3\2\2\2\u01fa\u01fb\f\4\2\2\u01fb\u01fc\5f\64\2\u01fc"+
		"\u01fd\5&\24\5\u01fd\u0203\3\2\2\2\u01fe\u01ff\f\3\2\2\u01ff\u0200\5r"+
		":\2\u0200\u0201\5&\24\4\u0201\u0203\3\2\2\2\u0202\u01e2\3\2\2\2\u0202"+
		"\u01e6\3\2\2\2\u0202\u01ea\3\2\2\2\u0202\u01ee\3\2\2\2\u0202\u01f2\3\2"+
		"\2\2\u0202\u01f6\3\2\2\2\u0202\u01fa\3\2\2\2\u0202\u01fe\3\2\2\2\u0203"+
		"\u0206\3\2\2\2\u0204\u0202\3\2\2\2\u0204\u0205\3\2\2\2\u0205\'\3\2\2\2"+
		"\u0206\u0204\3\2\2\2\u0207\u022e\5N(\2\u0208\u022e\5,\27\2\u0209\u022e"+
		"\5Z.\2\u020a\u020b\5&\24\2\u020b\u020c\5v<\2\u020c\u020d\5(\25\2\u020d"+
		"\u022e\3\2\2\2\u020e\u020f\5t;\2\u020f\u0210\5(\25\2\u0210\u022e\3\2\2"+
		"\2\u0211\u0212\5&\24\2\u0212\u0213\5p9\2\u0213\u0214\5(\25\2\u0214\u022e"+
		"\3\2\2\2\u0215\u0216\5&\24\2\u0216\u0217\5n8\2\u0217\u0218\5(\25\2\u0218"+
		"\u022e\3\2\2\2\u0219\u021a\5&\24\2\u021a\u021b\5l\67\2\u021b\u021c\5("+
		"\25\2\u021c\u022e\3\2\2\2\u021d\u021e\5&\24\2\u021e\u021f\5j\66\2\u021f"+
		"\u0220\5(\25\2\u0220\u022e\3\2\2\2\u0221\u0222\5&\24\2\u0222\u0223\5h"+
		"\65\2\u0223\u0224\5(\25\2\u0224\u022e\3\2\2\2\u0225\u0226\5&\24\2\u0226"+
		"\u0227\5f\64\2\u0227\u0228\5(\25\2\u0228\u022e\3\2\2\2\u0229\u022a\5&"+
		"\24\2\u022a\u022b\5r:\2\u022b\u022c\5(\25\2\u022c\u022e\3\2\2\2\u022d"+
		"\u0207\3\2\2\2\u022d\u0208\3\2\2\2\u022d\u0209\3\2\2\2\u022d\u020a\3\2"+
		"\2\2\u022d\u020e\3\2\2\2\u022d\u0211\3\2\2\2\u022d\u0215\3\2\2\2\u022d"+
		"\u0219\3\2\2\2\u022d\u021d\3\2\2\2\u022d\u0221\3\2\2\2\u022d\u0225\3\2"+
		"\2\2\u022d\u0229\3\2\2\2\u022e)\3\2\2\2\u022f\u0233\5\62\32\2\u0230\u0232"+
		"\5D#\2\u0231\u0230\3\2\2\2\u0232\u0235\3\2\2\2\u0233\u0231\3\2\2\2\u0233"+
		"\u0234\3\2\2\2\u0234+\3\2\2\2\u0235\u0233\3\2\2\2\u0236\u0241\5\64\33"+
		"\2\u0237\u023b\5\62\32\2\u0238\u023a\5D#\2\u0239\u0238\3\2\2\2\u023a\u023d"+
		"\3\2\2\2\u023b\u0239\3\2\2\2\u023b\u023c\3\2\2\2\u023c\u023e\3\2\2\2\u023d"+
		"\u023b\3\2\2\2\u023e\u023f\5F$\2\u023f\u0241\3\2\2\2\u0240\u0236\3\2\2"+
		"\2\u0240\u0237\3\2\2\2\u0241-\3\2\2\2\u0242\u0244\5\62\32\2\u0243\u0245"+
		"\5D#\2\u0244\u0243\3\2\2\2\u0245\u0246\3\2\2\2\u0246\u0244\3\2\2\2\u0246"+
		"\u0247\3\2\2\2\u0247/\3\2\2\2\u0248\u0253\5\64\33\2\u0249\u024d\5\62\32"+
		"\2\u024a\u024c\5D#\2\u024b\u024a\3\2\2\2\u024c\u024f\3\2\2\2\u024d\u024b"+
		"\3\2\2\2\u024d\u024e\3\2\2\2\u024e\u0250\3\2\2\2\u024f\u024d\3\2\2\2\u0250"+
		"\u0251\5F$\2\u0251\u0253\3\2\2\2\u0252\u0248\3\2\2\2\u0252\u0249\3\2\2"+
		"\2\u0253\61\3\2\2\2\u0254\u025a\5\66\34\2\u0255\u0256\7\35\2\2\u0256\u0257"+
		"\5&\24\2\u0257\u0258\7\36\2\2\u0258\u025a\3\2\2\2\u0259\u0254\3\2\2\2"+
		"\u0259\u0255\3\2\2\2\u025a\63\3\2\2\2\u025b\u025f\58\35\2\u025c\u025d"+
		"\7\35\2\2\u025d\u025f\5(\25\2\u025e\u025b\3\2\2\2\u025e\u025c\3\2\2\2"+
		"\u025f\65\3\2\2\2\u0260\u0267\7:\2\2\u0261\u0262\7\35\2\2\u0262\u0263"+
		"\5&\24\2\u0263\u0264\7\36\2\2\u0264\u0265\5<\37\2\u0265\u0267\3\2\2\2"+
		"\u0266\u0260\3\2\2\2\u0266\u0261\3\2\2\2\u0267\u026b\3\2\2\2\u0268\u026a"+
		"\5<\37\2\u0269\u0268\3\2\2\2\u026a\u026d\3\2\2\2\u026b\u0269\3\2\2\2\u026b"+
		"\u026c\3\2\2\2\u026c\67\3\2\2\2\u026d\u026b\3\2\2\2\u026e\u0287\5~@\2"+
		"\u026f\u0270\7\35\2\2\u0270\u0287\5(\25\2\u0271\u0272\7\35\2\2\u0272\u0273"+
		"\5&\24\2\u0273\u0274\7\36\2\2\u0274\u0275\5B\"\2\u0275\u0287\3\2\2\2\u0276"+
		"\u027d\5:\36\2\u0277\u0278\7\35\2\2\u0278\u0279\5&\24\2\u0279\u027a\7"+
		"\36\2\2\u027a\u027b\5<\37\2\u027b\u027d\3\2\2\2\u027c\u0276\3\2\2\2\u027c"+
		"\u0277\3\2\2\2\u027d\u0281\3\2\2\2\u027e\u0280\5<\37\2\u027f\u027e\3\2"+
		"\2\2\u0280\u0283\3\2\2\2\u0281\u027f\3\2\2\2\u0281\u0282\3\2\2\2\u0282"+
		"\u0284\3\2\2\2\u0283\u0281\3\2\2\2\u0284\u0285\5B\"\2\u0285\u0287\3\2"+
		"\2\2\u0286\u026e\3\2\2\2\u0286\u026f\3\2\2\2\u0286\u0271\3\2\2\2\u0286"+
		"\u027c\3\2\2\2\u02879\3\2\2\2\u0288\u0289\7:\2\2\u0289;\3\2\2\2\u028a"+
		"\u028c\5D#\2\u028b\u028a\3\2\2\2\u028c\u028f\3\2\2\2\u028d\u028b\3\2\2"+
		"\2\u028d\u028e\3\2\2\2\u028e\u0296\3\2\2\2\u028f\u028d\3\2\2\2\u0290\u0291"+
		"\7\37\2\2\u0291\u0292\5@!\2\u0292\u0293\7 \2\2\u0293\u0297\3\2\2\2\u0294"+
		"\u0295\7\27\2\2\u0295\u0297\5> \2\u0296\u0290\3\2\2\2\u0296\u0294\3\2"+
		"\2\2\u0297=\3\2\2\2\u0298\u0299\7:\2\2\u0299?\3\2\2\2\u029a\u029b\5&\24"+
		"\2\u029bA\3\2\2\2\u029c\u029e\5D#\2\u029d\u029c\3\2\2\2\u029e\u02a1\3"+
		"\2\2\2\u029f\u029d\3\2\2\2\u029f\u02a0\3\2\2\2\u02a0\u02a2\3\2\2\2\u02a1"+
		"\u029f\3\2\2\2\u02a2\u02b0\5F$\2\u02a3\u02a5\5D#\2\u02a4\u02a3\3\2\2\2"+
		"\u02a5\u02a8\3\2\2\2\u02a6\u02a4\3\2\2\2\u02a6\u02a7\3\2\2\2\u02a7\u02ad"+
		"\3\2\2\2\u02a8\u02a6\3\2\2\2\u02a9\u02aa\7\37\2\2\u02aa\u02ae\5(\25\2"+
		"\u02ab\u02ac\7\27\2\2\u02ac\u02ae\5~@\2\u02ad\u02a9\3\2\2\2\u02ad\u02ab"+
		"\3\2\2\2\u02ae\u02b0\3\2\2\2\u02af\u029f\3\2\2\2\u02af\u02a6\3\2\2\2\u02b0"+
		"C\3\2\2\2\u02b1\u02b2\7\30\2\2\u02b2\u02b4\7:\2\2\u02b3\u02b1\3\2\2\2"+
		"\u02b3\u02b4\3\2\2\2\u02b4\u02b5\3\2\2\2\u02b5\u02b6\5H%\2\u02b6E\3\2"+
		"\2\2\u02b7\u02b8\7\30\2\2\u02b8\u02bf\5~@\2\u02b9\u02ba\7\30\2\2\u02ba"+
		"\u02bc\7:\2\2\u02bb\u02b9\3\2\2\2\u02bb\u02bc\3\2\2\2\u02bc\u02bd\3\2"+
		"\2\2\u02bd\u02bf\5J&\2\u02be\u02b7\3\2\2\2\u02be\u02bb\3\2\2\2\u02bfG"+
		"\3\2\2\2\u02c0\u02c2\7\35\2\2\u02c1\u02c3\5\"\22\2\u02c2\u02c1\3\2\2\2"+
		"\u02c2\u02c3\3\2\2\2\u02c3\u02c4\3\2\2\2\u02c4\u02c8\7\36\2\2\u02c5\u02c8"+
		"\5X-\2\u02c6\u02c8\5z>\2\u02c7\u02c0\3\2\2\2\u02c7\u02c5\3\2\2\2\u02c7"+
		"\u02c6\3\2\2\2\u02c8I\3\2\2\2\u02c9\u02ca\7\35\2\2\u02ca\u02ce\5$\23\2"+
		"\u02cb\u02ce\5Z.\2\u02cc\u02ce\5|?\2\u02cd\u02c9\3\2\2\2\u02cd\u02cb\3"+
		"\2\2\2\u02cd\u02cc\3\2\2\2\u02ceK\3\2\2\2\u02cf\u02d0\7\23\2\2\u02d0\u02d1"+
		"\5P)\2\u02d1M\3\2\2\2\u02d2\u02d3\7\23\2\2\u02d3\u02d4\5R*\2\u02d4O\3"+
		"\2\2\2\u02d5\u02d7\7\35\2\2\u02d6\u02d8\5T+\2\u02d7\u02d6\3\2\2\2\u02d7"+
		"\u02d8\3\2\2\2\u02d8\u02d9\3\2\2\2\u02d9\u02da\7\36\2\2\u02da\u02db\5"+
		"\6\4\2\u02db\u02dc\7\b\2\2\u02dcQ\3\2\2\2\u02dd\u02de\7\35\2\2\u02de\u02e6"+
		"\5V,\2\u02df\u02e1\7\35\2\2\u02e0\u02e2\5T+\2\u02e1\u02e0\3\2\2\2\u02e1"+
		"\u02e2\3\2\2\2\u02e2\u02e3\3\2\2\2\u02e3\u02e4\7\36\2\2\u02e4\u02e6\5"+
		"\b\5\2\u02e5\u02dd\3\2\2\2\u02e5\u02df\3\2\2\2\u02e6S\3\2\2\2\u02e7\u02ea"+
		"\5\36\20\2\u02e8\u02e9\7\21\2\2\u02e9\u02eb\7\34\2\2\u02ea\u02e8\3\2\2"+
		"\2\u02ea\u02eb\3\2\2\2\u02eb\u02ee\3\2\2\2\u02ec\u02ee\7\34\2\2\u02ed"+
		"\u02e7\3\2\2\2\u02ed\u02ec\3\2\2\2\u02eeU\3\2\2\2\u02ef\u02f0\5 \21\2"+
		"\u02f0W\3\2\2\2\u02f1\u02f3\7!\2\2\u02f2\u02f4\5\\/\2\u02f3\u02f2\3\2"+
		"\2\2\u02f3\u02f4\3\2\2\2\u02f4\u02f5\3\2\2\2\u02f5\u02f6\7\"\2\2\u02f6"+
		"Y\3\2\2\2\u02f7\u02f8\7!\2\2\u02f8\u02f9\5^\60\2\u02f9[\3\2\2\2\u02fa"+
		"\u0300\5`\61\2\u02fb\u02fc\5d\63\2\u02fc\u02fd\5`\61\2\u02fd\u02ff\3\2"+
		"\2\2\u02fe\u02fb\3\2\2\2\u02ff\u0302\3\2\2\2\u0300\u02fe\3\2\2\2\u0300"+
		"\u0301\3\2\2\2\u0301\u0304\3\2\2\2\u0302\u0300\3\2\2\2\u0303\u0305\5d"+
		"\63\2\u0304\u0303\3\2\2\2\u0304\u0305\3\2\2\2\u0305]\3\2\2\2\u0306\u0307"+
		"\5`\61\2\u0307\u0308\5d\63\2\u0308\u030a\3\2\2\2\u0309\u0306\3\2\2\2\u030a"+
		"\u030d\3\2\2\2\u030b\u0309\3\2\2\2\u030b\u030c\3\2\2\2\u030c\u030e\3\2"+
		"\2\2\u030d\u030b\3\2\2\2\u030e\u030f\5b\62\2\u030f_\3\2\2\2\u0310\u0311"+
		"\7\37\2\2\u0311\u0312\5&\24\2\u0312\u0313\7 \2\2\u0313\u0314\7\4\2\2\u0314"+
		"\u0315\5&\24\2\u0315\u031b\3\2\2\2\u0316\u0317\7:\2\2\u0317\u0318\7\4"+
		"\2\2\u0318\u031b\5&\24\2\u0319\u031b\5&\24\2\u031a\u0310\3\2\2\2\u031a"+
		"\u0316\3\2\2\2\u031a\u0319\3\2\2\2\u031ba\3\2\2\2\u031c\u031d\7\37\2\2"+
		"\u031d\u032a\5(\25\2\u031e\u031f\7\37\2\2\u031f\u0320\5&\24\2\u0320\u0321"+
		"\7 \2\2\u0321\u0322\7\4\2\2\u0322\u0323\5(\25\2\u0323\u032a\3\2\2\2\u0324"+
		"\u032a\5~@\2\u0325\u0326\7:\2\2\u0326\u0327\7\4\2\2\u0327\u032a\5(\25"+
		"\2\u0328\u032a\5(\25\2\u0329\u031c\3\2\2\2\u0329\u031e\3\2\2\2\u0329\u0324"+
		"\3\2\2\2\u0329\u0325\3\2\2\2\u0329\u0328\3\2\2\2\u032ac\3\2\2\2\u032b"+
		"\u032c\t\2\2\2\u032ce\3\2\2\2\u032d\u032e\7#\2\2\u032eg\3\2\2\2\u032f"+
		"\u0330\7$\2\2\u0330i\3\2\2\2\u0331\u0332\t\3\2\2\u0332k\3\2\2\2\u0333"+
		"\u0334\7+\2\2\u0334m\3\2\2\2\u0335\u0336\t\4\2\2\u0336o\3\2\2\2\u0337"+
		"\u0338\t\5\2\2\u0338q\3\2\2\2\u0339\u033a\t\6\2\2\u033as\3\2\2\2\u033b"+
		"\u033c\t\7\2\2\u033cu\3\2\2\2\u033d\u033e\79\2\2\u033ew\3\2\2\2\u033f"+
		"\u0340\t\b\2\2\u0340y\3\2\2\2\u0341\u0342\t\t\2\2\u0342{\3\2\2\2\u0343"+
		"\u0344\t\t\2\2\u0344}\3\2\2\2\u0345\u0346\7:\2\2\u0346\177\3\2\2\2F\u0089"+
		"\u008d\u0092\u0099\u009d\u00c3\u00c8\u00d4\u00ee\u00f0\u0136\u014f\u0158"+
		"\u0169\u016e\u0172\u0175\u0183\u0188\u018e\u0197\u019c\u01a3\u01ab\u01b5"+
		"\u01bc\u01c6\u01ce\u01e0\u0202\u0204\u022d\u0233\u023b\u0240\u0246\u024d"+
		"\u0252\u0259\u025e\u0266\u026b\u027c\u0281\u0286\u028d\u0296\u029f\u02a6"+
		"\u02ad\u02af\u02b3\u02bb\u02be\u02c2\u02c7\u02cd\u02d7\u02e1\u02e5\u02ea"+
		"\u02ed\u02f3\u0300\u0304\u030b\u031a\u0329";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}