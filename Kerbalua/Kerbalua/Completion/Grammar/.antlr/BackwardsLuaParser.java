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
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, BACKWARDS_NAME=9, 
		WS=10;
	public static final int
		RULE_backwardsCompletionExpr = 0, RULE_terminal = 1, RULE_backwardsPartialCompletion = 2, 
		RULE_backwardsStartSymbol = 3, RULE_completionChain = 4, RULE_segment = 5, 
		RULE_anonymousPart = 6, RULE_backwardsField = 7, RULE_backwardsAnonCall = 8, 
		RULE_backwardsAnonArray = 9, RULE_backwardsCall = 10, RULE_backwardsArgs = 11, 
		RULE_arg = 12, RULE_ignoredExpr = 13;
	public static final String[] ruleNames = {
		"backwardsCompletionExpr", "terminal", "backwardsPartialCompletion", "backwardsStartSymbol", 
		"completionChain", "segment", "anonymousPart", "backwardsField", "backwardsAnonCall", 
		"backwardsAnonArray", "backwardsCall", "backwardsArgs", "arg", "ignoredExpr"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "'.'", "')'", "'('", "']'", "'['", "','", "'}'", "'{'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, null, null, null, null, null, null, null, null, "BACKWARDS_NAME", 
		"WS"
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
			setState(42);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,2,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(29);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==BACKWARDS_NAME) {
					{
					setState(28);
					backwardsPartialCompletion();
					}
				}

				setState(33);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,1,_ctx) ) {
				case 1:
					{
					setState(31);
					match(T__0);
					setState(32);
					completionChain();
					}
					break;
				}
				setState(35);
				match(T__0);
				setState(36);
				backwardsStartSymbol();
				setState(37);
				terminal();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(39);
				backwardsPartialCompletion();
				setState(40);
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
			setState(46);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__1:
			case T__2:
			case T__3:
			case T__4:
			case T__5:
			case T__6:
			case T__7:
			case BACKWARDS_NAME:
			case WS:
				enterOuterAlt(_localctx, 1);
				{
				setState(44);
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
				setState(45);
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
			setState(50);
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
			setState(52);
			segment();
			setState(57);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,4,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(53);
					match(T__0);
					setState(54);
					completionChain();
					}
					} 
				}
				setState(59);
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
			setState(63);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,5,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(60);
					anonymousPart();
					}
					} 
				}
				setState(65);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,5,_ctx);
			}
			setState(68);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__1:
				{
				setState(66);
				backwardsCall();
				}
				break;
			case BACKWARDS_NAME:
				{
				setState(67);
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
			setState(72);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__1:
				{
				setState(70);
				backwardsAnonCall();
				}
				break;
			case T__3:
				{
				setState(71);
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
			setState(74);
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
			setState(76);
			match(T__1);
			setState(77);
			backwardsArgs();
			setState(78);
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
			setState(80);
			match(T__3);
			setState(81);
			backwardsArgs();
			setState(82);
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
			setState(84);
			match(T__1);
			setState(85);
			backwardsArgs();
			setState(86);
			match(T__2);
			setState(87);
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
			setState(89);
			arg();
			setState(94);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__5) {
				{
				{
				setState(90);
				match(T__5);
				setState(91);
				arg();
				}
				}
				setState(96);
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
		public IgnoredExprContext ignoredExpr() {
			return getRuleContext(IgnoredExprContext.class,0);
		}
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
			setState(97);
			ignoredExpr();
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

	public static class IgnoredExprContext extends ParserRuleContext {
		public List<IgnoredExprContext> ignoredExpr() {
			return getRuleContexts(IgnoredExprContext.class);
		}
		public IgnoredExprContext ignoredExpr(int i) {
			return getRuleContext(IgnoredExprContext.class,i);
		}
		public IgnoredExprContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ignoredExpr; }
	}

	public final IgnoredExprContext ignoredExpr() throws RecognitionException {
		IgnoredExprContext _localctx = new IgnoredExprContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_ignoredExpr);
		int _la;
		try {
			int _alt;
			setState(124);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__1:
				enterOuterAlt(_localctx, 1);
				{
				setState(99);
				match(T__1);
				setState(100);
				ignoredExpr();
				setState(101);
				match(T__2);
				}
				break;
			case T__3:
				enterOuterAlt(_localctx, 2);
				{
				setState(103);
				match(T__3);
				setState(104);
				ignoredExpr();
				setState(105);
				match(T__4);
				}
				break;
			case T__6:
				enterOuterAlt(_localctx, 3);
				{
				setState(107);
				match(T__6);
				setState(108);
				ignoredExpr();
				setState(109);
				match(T__7);
				}
				break;
			case T__0:
			case T__2:
			case T__4:
			case T__5:
			case T__7:
			case BACKWARDS_NAME:
			case WS:
				enterOuterAlt(_localctx, 4);
				{
				setState(114);
				_errHandler.sync(this);
				_la = _input.LA(1);
				while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__0) | (1L << BACKWARDS_NAME) | (1L << WS))) != 0)) {
					{
					{
					setState(111);
					_la = _input.LA(1);
					if ( _la <= 0 || ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__1) | (1L << T__2) | (1L << T__3) | (1L << T__4) | (1L << T__5) | (1L << T__6) | (1L << T__7))) != 0)) ) {
					_errHandler.recoverInline(this);
					}
					else {
						if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
						_errHandler.reportMatch(this);
						consume();
					}
					}
					}
					setState(116);
					_errHandler.sync(this);
					_la = _input.LA(1);
				}
				setState(121);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
				while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						setState(117);
						match(T__5);
						setState(118);
						ignoredExpr();
						}
						} 
					}
					setState(123);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
				}
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

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3\f\u0081\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\3\2\5\2 \n\2\3\2\3\2\5\2$\n\2"+
		"\3\2\3\2\3\2\3\2\3\2\3\2\3\2\5\2-\n\2\3\3\3\3\5\3\61\n\3\3\4\3\4\3\5\3"+
		"\5\3\6\3\6\3\6\7\6:\n\6\f\6\16\6=\13\6\3\7\7\7@\n\7\f\7\16\7C\13\7\3\7"+
		"\3\7\5\7G\n\7\3\b\3\b\5\bK\n\b\3\t\3\t\3\n\3\n\3\n\3\n\3\13\3\13\3\13"+
		"\3\13\3\f\3\f\3\f\3\f\3\f\3\r\3\r\3\r\7\r_\n\r\f\r\16\rb\13\r\3\16\3\16"+
		"\3\17\3\17\3\17\3\17\3\17\3\17\3\17\3\17\3\17\3\17\3\17\3\17\3\17\7\17"+
		"s\n\17\f\17\16\17v\13\17\3\17\3\17\7\17z\n\17\f\17\16\17}\13\17\5\17\177"+
		"\n\17\3\17\3A\2\20\2\4\6\b\n\f\16\20\22\24\26\30\32\34\2\4\3\2\3\3\3\2"+
		"\4\n\2\u0080\2,\3\2\2\2\4\60\3\2\2\2\6\62\3\2\2\2\b\64\3\2\2\2\n\66\3"+
		"\2\2\2\fA\3\2\2\2\16J\3\2\2\2\20L\3\2\2\2\22N\3\2\2\2\24R\3\2\2\2\26V"+
		"\3\2\2\2\30[\3\2\2\2\32c\3\2\2\2\34~\3\2\2\2\36 \5\6\4\2\37\36\3\2\2\2"+
		"\37 \3\2\2\2 #\3\2\2\2!\"\7\3\2\2\"$\5\n\6\2#!\3\2\2\2#$\3\2\2\2$%\3\2"+
		"\2\2%&\7\3\2\2&\'\5\b\5\2\'(\5\4\3\2(-\3\2\2\2)*\5\6\4\2*+\5\4\3\2+-\3"+
		"\2\2\2,\37\3\2\2\2,)\3\2\2\2-\3\3\2\2\2.\61\n\2\2\2/\61\7\2\2\3\60.\3"+
		"\2\2\2\60/\3\2\2\2\61\5\3\2\2\2\62\63\7\13\2\2\63\7\3\2\2\2\64\65\7\13"+
		"\2\2\65\t\3\2\2\2\66;\5\f\7\2\678\7\3\2\28:\5\n\6\29\67\3\2\2\2:=\3\2"+
		"\2\2;9\3\2\2\2;<\3\2\2\2<\13\3\2\2\2=;\3\2\2\2>@\5\16\b\2?>\3\2\2\2@C"+
		"\3\2\2\2AB\3\2\2\2A?\3\2\2\2BF\3\2\2\2CA\3\2\2\2DG\5\26\f\2EG\5\20\t\2"+
		"FD\3\2\2\2FE\3\2\2\2G\r\3\2\2\2HK\5\22\n\2IK\5\24\13\2JH\3\2\2\2JI\3\2"+
		"\2\2K\17\3\2\2\2LM\7\13\2\2M\21\3\2\2\2NO\7\4\2\2OP\5\30\r\2PQ\7\5\2\2"+
		"Q\23\3\2\2\2RS\7\6\2\2ST\5\30\r\2TU\7\7\2\2U\25\3\2\2\2VW\7\4\2\2WX\5"+
		"\30\r\2XY\7\5\2\2YZ\7\13\2\2Z\27\3\2\2\2[`\5\32\16\2\\]\7\b\2\2]_\5\32"+
		"\16\2^\\\3\2\2\2_b\3\2\2\2`^\3\2\2\2`a\3\2\2\2a\31\3\2\2\2b`\3\2\2\2c"+
		"d\5\34\17\2d\33\3\2\2\2ef\7\4\2\2fg\5\34\17\2gh\7\5\2\2h\177\3\2\2\2i"+
		"j\7\6\2\2jk\5\34\17\2kl\7\7\2\2l\177\3\2\2\2mn\7\t\2\2no\5\34\17\2op\7"+
		"\n\2\2p\177\3\2\2\2qs\n\3\2\2rq\3\2\2\2sv\3\2\2\2tr\3\2\2\2tu\3\2\2\2"+
		"u{\3\2\2\2vt\3\2\2\2wx\7\b\2\2xz\5\34\17\2yw\3\2\2\2z}\3\2\2\2{y\3\2\2"+
		"\2{|\3\2\2\2|\177\3\2\2\2}{\3\2\2\2~e\3\2\2\2~i\3\2\2\2~m\3\2\2\2~t\3"+
		"\2\2\2\177\35\3\2\2\2\16\37#,\60;AFJ`t{~";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}