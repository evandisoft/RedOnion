// Generated from /home/developer/Sync/BigFiles/BigProjects/Kerbalua/Kerbalua/Kerbalua/Kerbalua/Completion/Grammar/BackwardsLua.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class BackwardsLuaParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, BACKWARDS_NAME=7, WS=8, 
		NORMALSTRING=9, CHARSTRING=10, LONGSTRING=11, INT=12, HEX=13, FLOAT=14, 
		HEX_FLOAT=15, LINE_COMMENT=16, SHEBANG=17;
	public static final int
		RULE_backwardsCompletionExpr = 0, RULE_terminal = 1, RULE_backwardsPartialCompletion = 2, 
		RULE_backwardsStartSymbol = 3, RULE_completionChain = 4, RULE_segment = 5, 
		RULE_anonymousPart = 6, RULE_backwardsField = 7, RULE_backwardsAnonCall = 8, 
		RULE_backwardsAnonArray = 9, RULE_backwardsCall = 10, RULE_backwardsArgs = 11, 
		RULE_arg = 12;
	public static final String[] ruleNames = {
		"backwardsCompletionExpr", "terminal", "backwardsPartialCompletion", "backwardsStartSymbol", 
		"completionChain", "segment", "anonymousPart", "backwardsField", "backwardsAnonCall", 
		"backwardsAnonArray", "backwardsCall", "backwardsArgs", "arg"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "'.'", "')'", "'('", "']'", "'['", "','"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, null, null, null, null, null, null, "BACKWARDS_NAME", "WS", "NORMALSTRING", 
		"CHARSTRING", "LONGSTRING", "INT", "HEX", "FLOAT", "HEX_FLOAT", "LINE_COMMENT", 
		"SHEBANG"
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
	public String getGrammarFileName() { return "BackwardsLua.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public BackwardsLuaParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class BackwardsCompletionExprContext extends ParserRuleContext {
		public BackwardsStartSymbolContext backwardsStartSymbol() {
			return getRuleContext(BackwardsStartSymbolContext.class,0);
		}
		public TerminalContext terminal() {
			return getRuleContext(TerminalContext.class,0);
		}
		public BackwardsPartialCompletionContext backwardsPartialCompletion() {
			return getRuleContext(BackwardsPartialCompletionContext.class,0);
		}
		public CompletionChainContext completionChain() {
			return getRuleContext(CompletionChainContext.class,0);
		}
		public BackwardsCompletionExprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsCompletionExpr; }
	}

	public final BackwardsCompletionExprContext backwardsCompletionExpr() throws RecognitionException {
		BackwardsCompletionExprContext _localctx = new BackwardsCompletionExprContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_backwardsCompletionExpr);
		int _la;
		try {
			setState(40);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,2,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(27);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==BACKWARDS_NAME) {
					{
					setState(26);
					backwardsPartialCompletion();
					}
				}

				setState(31);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,1,_ctx) ) {
				case 1:
					{
					setState(29);
					match(T__0);
					setState(30);
					completionChain();
					}
					break;
				}
				setState(33);
				match(T__0);
				setState(34);
				backwardsStartSymbol();
				setState(35);
				terminal();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(37);
				backwardsPartialCompletion();
				setState(38);
				terminal();
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

	public static class TerminalContext extends ParserRuleContext {
		public TerminalNode EOF() { return getToken(BackwardsLuaParser.EOF, 0); }
		public TerminalContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_terminal; }
	}

	public final TerminalContext terminal() throws RecognitionException {
		TerminalContext _localctx = new TerminalContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_terminal);
		int _la;
		try {
			setState(44);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__1:
			case T__2:
			case T__3:
			case T__4:
			case T__5:
			case BACKWARDS_NAME:
			case WS:
			case NORMALSTRING:
			case CHARSTRING:
			case LONGSTRING:
			case INT:
			case HEX:
			case FLOAT:
			case HEX_FLOAT:
			case LINE_COMMENT:
			case SHEBANG:
				enterOuterAlt(_localctx, 1);
				{
				setState(42);
				_la = _input.LA(1);
				if ( _la <= 0 || (_la==T__0) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
				break;
			case EOF:
				enterOuterAlt(_localctx, 2);
				{
				setState(43);
				match(EOF);
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

	public static class BackwardsPartialCompletionContext extends ParserRuleContext {
		public TerminalNode BACKWARDS_NAME() { return getToken(BackwardsLuaParser.BACKWARDS_NAME, 0); }
		public BackwardsPartialCompletionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsPartialCompletion; }
	}

	public final BackwardsPartialCompletionContext backwardsPartialCompletion() throws RecognitionException {
		BackwardsPartialCompletionContext _localctx = new BackwardsPartialCompletionContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_backwardsPartialCompletion);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(46);
			match(BACKWARDS_NAME);
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

	public static class BackwardsStartSymbolContext extends ParserRuleContext {
		public TerminalNode BACKWARDS_NAME() { return getToken(BackwardsLuaParser.BACKWARDS_NAME, 0); }
		public BackwardsStartSymbolContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsStartSymbol; }
	}

	public final BackwardsStartSymbolContext backwardsStartSymbol() throws RecognitionException {
		BackwardsStartSymbolContext _localctx = new BackwardsStartSymbolContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_backwardsStartSymbol);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(48);
			match(BACKWARDS_NAME);
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

	public static class CompletionChainContext extends ParserRuleContext {
		public SegmentContext segment() {
			return getRuleContext(SegmentContext.class,0);
		}
		public List<CompletionChainContext> completionChain() {
			return getRuleContexts(CompletionChainContext.class);
		}
		public CompletionChainContext completionChain(int i) {
			return getRuleContext(CompletionChainContext.class,i);
		}
		public CompletionChainContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_completionChain; }
	}

	public final CompletionChainContext completionChain() throws RecognitionException {
		CompletionChainContext _localctx = new CompletionChainContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_completionChain);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(50);
			segment();
			setState(55);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,4,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(51);
					match(T__0);
					setState(52);
					completionChain();
					}
					} 
				}
				setState(57);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,4,_ctx);
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

	public static class SegmentContext extends ParserRuleContext {
		public BackwardsCallContext backwardsCall() {
			return getRuleContext(BackwardsCallContext.class,0);
		}
		public BackwardsFieldContext backwardsField() {
			return getRuleContext(BackwardsFieldContext.class,0);
		}
		public List<AnonymousPartContext> anonymousPart() {
			return getRuleContexts(AnonymousPartContext.class);
		}
		public AnonymousPartContext anonymousPart(int i) {
			return getRuleContext(AnonymousPartContext.class,i);
		}
		public SegmentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_segment; }
	}

	public final SegmentContext segment() throws RecognitionException {
		SegmentContext _localctx = new SegmentContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_segment);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(61);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,5,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(58);
					anonymousPart();
					}
					} 
				}
				setState(63);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,5,_ctx);
			}
			setState(66);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__1:
				{
				setState(64);
				backwardsCall();
				}
				break;
			case BACKWARDS_NAME:
				{
				setState(65);
				backwardsField();
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

	public static class AnonymousPartContext extends ParserRuleContext {
		public BackwardsAnonCallContext backwardsAnonCall() {
			return getRuleContext(BackwardsAnonCallContext.class,0);
		}
		public BackwardsAnonArrayContext backwardsAnonArray() {
			return getRuleContext(BackwardsAnonArrayContext.class,0);
		}
		public AnonymousPartContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_anonymousPart; }
	}

	public final AnonymousPartContext anonymousPart() throws RecognitionException {
		AnonymousPartContext _localctx = new AnonymousPartContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_anonymousPart);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(70);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__1:
				{
				setState(68);
				backwardsAnonCall();
				}
				break;
			case T__3:
				{
				setState(69);
				backwardsAnonArray();
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

	public static class BackwardsFieldContext extends ParserRuleContext {
		public TerminalNode BACKWARDS_NAME() { return getToken(BackwardsLuaParser.BACKWARDS_NAME, 0); }
		public BackwardsFieldContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsField; }
	}

	public final BackwardsFieldContext backwardsField() throws RecognitionException {
		BackwardsFieldContext _localctx = new BackwardsFieldContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_backwardsField);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(72);
			match(BACKWARDS_NAME);
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

	public static class BackwardsAnonCallContext extends ParserRuleContext {
		public BackwardsArgsContext backwardsArgs() {
			return getRuleContext(BackwardsArgsContext.class,0);
		}
		public BackwardsAnonCallContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsAnonCall; }
	}

	public final BackwardsAnonCallContext backwardsAnonCall() throws RecognitionException {
		BackwardsAnonCallContext _localctx = new BackwardsAnonCallContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_backwardsAnonCall);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(74);
			match(T__1);
			setState(75);
			backwardsArgs();
			setState(76);
			match(T__2);
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

	public static class BackwardsAnonArrayContext extends ParserRuleContext {
		public BackwardsArgsContext backwardsArgs() {
			return getRuleContext(BackwardsArgsContext.class,0);
		}
		public BackwardsAnonArrayContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsAnonArray; }
	}

	public final BackwardsAnonArrayContext backwardsAnonArray() throws RecognitionException {
		BackwardsAnonArrayContext _localctx = new BackwardsAnonArrayContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_backwardsAnonArray);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(78);
			match(T__3);
			setState(79);
			backwardsArgs();
			setState(80);
			match(T__4);
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

	public static class BackwardsCallContext extends ParserRuleContext {
		public BackwardsArgsContext backwardsArgs() {
			return getRuleContext(BackwardsArgsContext.class,0);
		}
		public TerminalNode BACKWARDS_NAME() { return getToken(BackwardsLuaParser.BACKWARDS_NAME, 0); }
		public BackwardsCallContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsCall; }
	}

	public final BackwardsCallContext backwardsCall() throws RecognitionException {
		BackwardsCallContext _localctx = new BackwardsCallContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_backwardsCall);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(82);
			match(T__1);
			setState(83);
			backwardsArgs();
			setState(84);
			match(T__2);
			setState(85);
			match(BACKWARDS_NAME);
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

	public static class BackwardsArgsContext extends ParserRuleContext {
		public List<ArgContext> arg() {
			return getRuleContexts(ArgContext.class);
		}
		public ArgContext arg(int i) {
			return getRuleContext(ArgContext.class,i);
		}
		public BackwardsArgsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_backwardsArgs; }
	}

	public final BackwardsArgsContext backwardsArgs() throws RecognitionException {
		BackwardsArgsContext _localctx = new BackwardsArgsContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_backwardsArgs);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(87);
			arg();
			setState(92);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__5) {
				{
				{
				setState(88);
				match(T__5);
				setState(89);
				arg();
				}
				}
				setState(94);
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

	public static class ArgContext extends ParserRuleContext {
		public ArgContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arg; }
	}

	public final ArgContext arg() throws RecognitionException {
		ArgContext _localctx = new ArgContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_arg);
		try {
			enterOuterAlt(_localctx, 1);
			{
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

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3\23d\4\2\t\2\4\3\t"+
		"\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t\13\4"+
		"\f\t\f\4\r\t\r\4\16\t\16\3\2\5\2\36\n\2\3\2\3\2\5\2\"\n\2\3\2\3\2\3\2"+
		"\3\2\3\2\3\2\3\2\5\2+\n\2\3\3\3\3\5\3/\n\3\3\4\3\4\3\5\3\5\3\6\3\6\3\6"+
		"\7\68\n\6\f\6\16\6;\13\6\3\7\7\7>\n\7\f\7\16\7A\13\7\3\7\3\7\5\7E\n\7"+
		"\3\b\3\b\5\bI\n\b\3\t\3\t\3\n\3\n\3\n\3\n\3\13\3\13\3\13\3\13\3\f\3\f"+
		"\3\f\3\f\3\f\3\r\3\r\3\r\7\r]\n\r\f\r\16\r`\13\r\3\16\3\16\3\16\3?\2\17"+
		"\2\4\6\b\n\f\16\20\22\24\26\30\32\2\3\3\2\3\3\2_\2*\3\2\2\2\4.\3\2\2\2"+
		"\6\60\3\2\2\2\b\62\3\2\2\2\n\64\3\2\2\2\f?\3\2\2\2\16H\3\2\2\2\20J\3\2"+
		"\2\2\22L\3\2\2\2\24P\3\2\2\2\26T\3\2\2\2\30Y\3\2\2\2\32a\3\2\2\2\34\36"+
		"\5\6\4\2\35\34\3\2\2\2\35\36\3\2\2\2\36!\3\2\2\2\37 \7\3\2\2 \"\5\n\6"+
		"\2!\37\3\2\2\2!\"\3\2\2\2\"#\3\2\2\2#$\7\3\2\2$%\5\b\5\2%&\5\4\3\2&+\3"+
		"\2\2\2\'(\5\6\4\2()\5\4\3\2)+\3\2\2\2*\35\3\2\2\2*\'\3\2\2\2+\3\3\2\2"+
		"\2,/\n\2\2\2-/\7\2\2\3.,\3\2\2\2.-\3\2\2\2/\5\3\2\2\2\60\61\7\t\2\2\61"+
		"\7\3\2\2\2\62\63\7\t\2\2\63\t\3\2\2\2\649\5\f\7\2\65\66\7\3\2\2\668\5"+
		"\n\6\2\67\65\3\2\2\28;\3\2\2\29\67\3\2\2\29:\3\2\2\2:\13\3\2\2\2;9\3\2"+
		"\2\2<>\5\16\b\2=<\3\2\2\2>A\3\2\2\2?@\3\2\2\2?=\3\2\2\2@D\3\2\2\2A?\3"+
		"\2\2\2BE\5\26\f\2CE\5\20\t\2DB\3\2\2\2DC\3\2\2\2E\r\3\2\2\2FI\5\22\n\2"+
		"GI\5\24\13\2HF\3\2\2\2HG\3\2\2\2I\17\3\2\2\2JK\7\t\2\2K\21\3\2\2\2LM\7"+
		"\4\2\2MN\5\30\r\2NO\7\5\2\2O\23\3\2\2\2PQ\7\6\2\2QR\5\30\r\2RS\7\7\2\2"+
		"S\25\3\2\2\2TU\7\4\2\2UV\5\30\r\2VW\7\5\2\2WX\7\t\2\2X\27\3\2\2\2Y^\5"+
		"\32\16\2Z[\7\b\2\2[]\5\32\16\2\\Z\3\2\2\2]`\3\2\2\2^\\\3\2\2\2^_\3\2\2"+
		"\2_\31\3\2\2\2`^\3\2\2\2ab\3\2\2\2b\33\3\2\2\2\13\35!*.9?DH^";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}