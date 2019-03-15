// Generated from /home/developer/Sync/BigFiles/BigProjects/Kerbalua/Kerbalua/Kerbalua/Kerbalua/Completion/Grammar/BackwardsLua.g4 by ANTLR 4.7.1
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class BackwardsLuaLexer extends Lexer {
	static { RuntimeMetaData.checkVersion("4.7.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		NAME=1, NORMALSTRING=2, CHARSTRING=3, LONGSTRING=4, INT=5, HEX=6, FLOAT=7, 
		HEX_FLOAT=8, COMMENT=9, LINE_COMMENT=10, WS=11, SHEBANG=12;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	public static final String[] ruleNames = {
		"NAME", "NORMALSTRING", "CHARSTRING", "LONGSTRING", "NESTED_STR", "INT", 
		"HEX", "FLOAT", "HEX_FLOAT", "ExponentPart", "HexExponentPart", "EscapeSequence", 
		"DecimalEscape", "HexEscape", "UtfEscape", "Digit", "HexDigit", "COMMENT", 
		"LINE_COMMENT", "WS", "SHEBANG"
	};

	private static final String[] _LITERAL_NAMES = {
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, "NAME", "NORMALSTRING", "CHARSTRING", "LONGSTRING", "INT", "HEX", 
		"FLOAT", "HEX_FLOAT", "COMMENT", "LINE_COMMENT", "WS", "SHEBANG"
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


	public BackwardsLuaLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "BackwardsLua.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2\16\u0130\b\1\4\2"+
		"\t\2\4\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4"+
		"\13\t\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22"+
		"\t\22\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\3\2\3\2\7\2\60\n\2\f\2\16"+
		"\2\63\13\2\3\3\3\3\3\3\7\38\n\3\f\3\16\3;\13\3\3\3\3\3\3\4\3\4\3\4\7\4"+
		"B\n\4\f\4\16\4E\13\4\3\4\3\4\3\5\3\5\3\5\3\5\3\6\3\6\3\6\3\6\3\6\3\6\7"+
		"\6S\n\6\f\6\16\6V\13\6\3\6\5\6Y\n\6\3\7\6\7\\\n\7\r\7\16\7]\3\b\3\b\3"+
		"\b\6\bc\n\b\r\b\16\bd\3\t\6\th\n\t\r\t\16\ti\3\t\3\t\7\tn\n\t\f\t\16\t"+
		"q\13\t\3\t\5\tt\n\t\3\t\3\t\6\tx\n\t\r\t\16\ty\3\t\5\t}\n\t\3\t\6\t\u0080"+
		"\n\t\r\t\16\t\u0081\3\t\3\t\5\t\u0086\n\t\3\n\3\n\3\n\6\n\u008b\n\n\r"+
		"\n\16\n\u008c\3\n\3\n\7\n\u0091\n\n\f\n\16\n\u0094\13\n\3\n\5\n\u0097"+
		"\n\n\3\n\3\n\3\n\3\n\6\n\u009d\n\n\r\n\16\n\u009e\3\n\5\n\u00a2\n\n\3"+
		"\n\3\n\3\n\6\n\u00a7\n\n\r\n\16\n\u00a8\3\n\3\n\5\n\u00ad\n\n\3\13\3\13"+
		"\5\13\u00b1\n\13\3\13\6\13\u00b4\n\13\r\13\16\13\u00b5\3\f\3\f\5\f\u00ba"+
		"\n\f\3\f\6\f\u00bd\n\f\r\f\16\f\u00be\3\r\3\r\3\r\3\r\5\r\u00c5\n\r\3"+
		"\r\3\r\3\r\3\r\5\r\u00cb\n\r\3\16\3\16\3\16\3\16\3\16\3\16\3\16\3\16\3"+
		"\16\3\16\3\16\5\16\u00d8\n\16\3\17\3\17\3\17\3\17\3\17\3\20\3\20\3\20"+
		"\3\20\3\20\6\20\u00e4\n\20\r\20\16\20\u00e5\3\20\3\20\3\21\3\21\3\22\3"+
		"\22\3\23\3\23\3\23\3\23\3\23\3\23\3\23\3\23\3\23\3\24\3\24\3\24\3\24\3"+
		"\24\3\24\7\24\u00fd\n\24\f\24\16\24\u0100\13\24\3\24\3\24\7\24\u0104\n"+
		"\24\f\24\16\24\u0107\13\24\3\24\3\24\7\24\u010b\n\24\f\24\16\24\u010e"+
		"\13\24\3\24\3\24\7\24\u0112\n\24\f\24\16\24\u0115\13\24\5\24\u0117\n\24"+
		"\3\24\3\24\3\24\5\24\u011c\n\24\3\24\3\24\3\25\6\25\u0121\n\25\r\25\16"+
		"\25\u0122\3\25\3\25\3\26\3\26\3\26\7\26\u012a\n\26\f\26\16\26\u012d\13"+
		"\26\3\26\3\26\3T\2\27\3\3\5\4\7\5\t\6\13\2\r\7\17\b\21\t\23\n\25\2\27"+
		"\2\31\2\33\2\35\2\37\2!\2#\2%\13\'\f)\r+\16\3\2\23\5\2C\\aac|\6\2\62;"+
		"C\\aac|\4\2$$^^\4\2))^^\4\2ZZzz\4\2GGgg\4\2--//\4\2RRrr\f\2$$))^^cdhh"+
		"ppttvvxx||\3\2\62\64\3\2\62;\5\2\62;CHch\6\2\f\f\17\17??]]\4\2\f\f\17"+
		"\17\5\2\f\f\17\17]]\4\3\f\f\17\17\5\2\13\f\16\17\"\"\2\u0155\2\3\3\2\2"+
		"\2\2\5\3\2\2\2\2\7\3\2\2\2\2\t\3\2\2\2\2\r\3\2\2\2\2\17\3\2\2\2\2\21\3"+
		"\2\2\2\2\23\3\2\2\2\2%\3\2\2\2\2\'\3\2\2\2\2)\3\2\2\2\2+\3\2\2\2\3-\3"+
		"\2\2\2\5\64\3\2\2\2\7>\3\2\2\2\tH\3\2\2\2\13X\3\2\2\2\r[\3\2\2\2\17_\3"+
		"\2\2\2\21\u0085\3\2\2\2\23\u00ac\3\2\2\2\25\u00ae\3\2\2\2\27\u00b7\3\2"+
		"\2\2\31\u00ca\3\2\2\2\33\u00d7\3\2\2\2\35\u00d9\3\2\2\2\37\u00de\3\2\2"+
		"\2!\u00e9\3\2\2\2#\u00eb\3\2\2\2%\u00ed\3\2\2\2\'\u00f6\3\2\2\2)\u0120"+
		"\3\2\2\2+\u0126\3\2\2\2-\61\t\2\2\2.\60\t\3\2\2/.\3\2\2\2\60\63\3\2\2"+
		"\2\61/\3\2\2\2\61\62\3\2\2\2\62\4\3\2\2\2\63\61\3\2\2\2\649\7$\2\2\65"+
		"8\5\31\r\2\668\n\4\2\2\67\65\3\2\2\2\67\66\3\2\2\28;\3\2\2\29\67\3\2\2"+
		"\29:\3\2\2\2:<\3\2\2\2;9\3\2\2\2<=\7$\2\2=\6\3\2\2\2>C\7)\2\2?B\5\31\r"+
		"\2@B\n\5\2\2A?\3\2\2\2A@\3\2\2\2BE\3\2\2\2CA\3\2\2\2CD\3\2\2\2DF\3\2\2"+
		"\2EC\3\2\2\2FG\7)\2\2G\b\3\2\2\2HI\7]\2\2IJ\5\13\6\2JK\7_\2\2K\n\3\2\2"+
		"\2LM\7?\2\2MN\5\13\6\2NO\7?\2\2OY\3\2\2\2PT\7]\2\2QS\13\2\2\2RQ\3\2\2"+
		"\2SV\3\2\2\2TU\3\2\2\2TR\3\2\2\2UW\3\2\2\2VT\3\2\2\2WY\7_\2\2XL\3\2\2"+
		"\2XP\3\2\2\2Y\f\3\2\2\2Z\\\5!\21\2[Z\3\2\2\2\\]\3\2\2\2][\3\2\2\2]^\3"+
		"\2\2\2^\16\3\2\2\2_`\7\62\2\2`b\t\6\2\2ac\5#\22\2ba\3\2\2\2cd\3\2\2\2"+
		"db\3\2\2\2de\3\2\2\2e\20\3\2\2\2fh\5!\21\2gf\3\2\2\2hi\3\2\2\2ig\3\2\2"+
		"\2ij\3\2\2\2jk\3\2\2\2ko\7\60\2\2ln\5!\21\2ml\3\2\2\2nq\3\2\2\2om\3\2"+
		"\2\2op\3\2\2\2ps\3\2\2\2qo\3\2\2\2rt\5\25\13\2sr\3\2\2\2st\3\2\2\2t\u0086"+
		"\3\2\2\2uw\7\60\2\2vx\5!\21\2wv\3\2\2\2xy\3\2\2\2yw\3\2\2\2yz\3\2\2\2"+
		"z|\3\2\2\2{}\5\25\13\2|{\3\2\2\2|}\3\2\2\2}\u0086\3\2\2\2~\u0080\5!\21"+
		"\2\177~\3\2\2\2\u0080\u0081\3\2\2\2\u0081\177\3\2\2\2\u0081\u0082\3\2"+
		"\2\2\u0082\u0083\3\2\2\2\u0083\u0084\5\25\13\2\u0084\u0086\3\2\2\2\u0085"+
		"g\3\2\2\2\u0085u\3\2\2\2\u0085\177\3\2\2\2\u0086\22\3\2\2\2\u0087\u0088"+
		"\7\62\2\2\u0088\u008a\t\6\2\2\u0089\u008b\5#\22\2\u008a\u0089\3\2\2\2"+
		"\u008b\u008c\3\2\2\2\u008c\u008a\3\2\2\2\u008c\u008d\3\2\2\2\u008d\u008e"+
		"\3\2\2\2\u008e\u0092\7\60\2\2\u008f\u0091\5#\22\2\u0090\u008f\3\2\2\2"+
		"\u0091\u0094\3\2\2\2\u0092\u0090\3\2\2\2\u0092\u0093\3\2\2\2\u0093\u0096"+
		"\3\2\2\2\u0094\u0092\3\2\2\2\u0095\u0097\5\27\f\2\u0096\u0095\3\2\2\2"+
		"\u0096\u0097\3\2\2\2\u0097\u00ad\3\2\2\2\u0098\u0099\7\62\2\2\u0099\u009a"+
		"\t\6\2\2\u009a\u009c\7\60\2\2\u009b\u009d\5#\22\2\u009c\u009b\3\2\2\2"+
		"\u009d\u009e\3\2\2\2\u009e\u009c\3\2\2\2\u009e\u009f\3\2\2\2\u009f\u00a1"+
		"\3\2\2\2\u00a0\u00a2\5\27\f\2\u00a1\u00a0\3\2\2\2\u00a1\u00a2\3\2\2\2"+
		"\u00a2\u00ad\3\2\2\2\u00a3\u00a4\7\62\2\2\u00a4\u00a6\t\6\2\2\u00a5\u00a7"+
		"\5#\22\2\u00a6\u00a5\3\2\2\2\u00a7\u00a8\3\2\2\2\u00a8\u00a6\3\2\2\2\u00a8"+
		"\u00a9\3\2\2\2\u00a9\u00aa\3\2\2\2\u00aa\u00ab\5\27\f\2\u00ab\u00ad\3"+
		"\2\2\2\u00ac\u0087\3\2\2\2\u00ac\u0098\3\2\2\2\u00ac\u00a3\3\2\2\2\u00ad"+
		"\24\3\2\2\2\u00ae\u00b0\t\7\2\2\u00af\u00b1\t\b\2\2\u00b0\u00af\3\2\2"+
		"\2\u00b0\u00b1\3\2\2\2\u00b1\u00b3\3\2\2\2\u00b2\u00b4\5!\21\2\u00b3\u00b2"+
		"\3\2\2\2\u00b4\u00b5\3\2\2\2\u00b5\u00b3\3\2\2\2\u00b5\u00b6\3\2\2\2\u00b6"+
		"\26\3\2\2\2\u00b7\u00b9\t\t\2\2\u00b8\u00ba\t\b\2\2\u00b9\u00b8\3\2\2"+
		"\2\u00b9\u00ba\3\2\2\2\u00ba\u00bc\3\2\2\2\u00bb\u00bd\5!\21\2\u00bc\u00bb"+
		"\3\2\2\2\u00bd\u00be\3\2\2\2\u00be\u00bc\3\2\2\2\u00be\u00bf\3\2\2\2\u00bf"+
		"\30\3\2\2\2\u00c0\u00c1\7^\2\2\u00c1\u00cb\t\n\2\2\u00c2\u00c4\7^\2\2"+
		"\u00c3\u00c5\7\17\2\2\u00c4\u00c3\3\2\2\2\u00c4\u00c5\3\2\2\2\u00c5\u00c6"+
		"\3\2\2\2\u00c6\u00cb\7\f\2\2\u00c7\u00cb\5\33\16\2\u00c8\u00cb\5\35\17"+
		"\2\u00c9\u00cb\5\37\20\2\u00ca\u00c0\3\2\2\2\u00ca\u00c2\3\2\2\2\u00ca"+
		"\u00c7\3\2\2\2\u00ca\u00c8\3\2\2\2\u00ca\u00c9\3\2\2\2\u00cb\32\3\2\2"+
		"\2\u00cc\u00cd\7^\2\2\u00cd\u00d8\5!\21\2\u00ce\u00cf\7^\2\2\u00cf\u00d0"+
		"\5!\21\2\u00d0\u00d1\5!\21\2\u00d1\u00d8\3\2\2\2\u00d2\u00d3\7^\2\2\u00d3"+
		"\u00d4\t\13\2\2\u00d4\u00d5\5!\21\2\u00d5\u00d6\5!\21\2\u00d6\u00d8\3"+
		"\2\2\2\u00d7\u00cc\3\2\2\2\u00d7\u00ce\3\2\2\2\u00d7\u00d2\3\2\2\2\u00d8"+
		"\34\3\2\2\2\u00d9\u00da\7^\2\2\u00da\u00db\7z\2\2\u00db\u00dc\5#\22\2"+
		"\u00dc\u00dd\5#\22\2\u00dd\36\3\2\2\2\u00de\u00df\7^\2\2\u00df\u00e0\7"+
		"w\2\2\u00e0\u00e1\7}\2\2\u00e1\u00e3\3\2\2\2\u00e2\u00e4\5#\22\2\u00e3"+
		"\u00e2\3\2\2\2\u00e4\u00e5\3\2\2\2\u00e5\u00e3\3\2\2\2\u00e5\u00e6\3\2"+
		"\2\2\u00e6\u00e7\3\2\2\2\u00e7\u00e8\7\177\2\2\u00e8 \3\2\2\2\u00e9\u00ea"+
		"\t\f\2\2\u00ea\"\3\2\2\2\u00eb\u00ec\t\r\2\2\u00ec$\3\2\2\2\u00ed\u00ee"+
		"\7/\2\2\u00ee\u00ef\7/\2\2\u00ef\u00f0\7]\2\2\u00f0\u00f1\3\2\2\2\u00f1"+
		"\u00f2\5\13\6\2\u00f2\u00f3\7_\2\2\u00f3\u00f4\3\2\2\2\u00f4\u00f5\b\23"+
		"\2\2\u00f5&\3\2\2\2\u00f6\u00f7\7/\2\2\u00f7\u00f8\7/\2\2\u00f8\u0116"+
		"\3\2\2\2\u00f9\u0117\3\2\2\2\u00fa\u00fe\7]\2\2\u00fb\u00fd\7?\2\2\u00fc"+
		"\u00fb\3\2\2\2\u00fd\u0100\3\2\2\2\u00fe\u00fc\3\2\2\2\u00fe\u00ff\3\2"+
		"\2\2\u00ff\u0117\3\2\2\2\u0100\u00fe\3\2\2\2\u0101\u0105\7]\2\2\u0102"+
		"\u0104\7?\2\2\u0103\u0102\3\2\2\2\u0104\u0107\3\2\2\2\u0105\u0103\3\2"+
		"\2\2\u0105\u0106\3\2\2\2\u0106\u0108\3\2\2\2\u0107\u0105\3\2\2\2\u0108"+
		"\u010c\n\16\2\2\u0109\u010b\n\17\2\2\u010a\u0109\3\2\2\2\u010b\u010e\3"+
		"\2\2\2\u010c\u010a\3\2\2\2\u010c\u010d\3\2\2\2\u010d\u0117\3\2\2\2\u010e"+
		"\u010c\3\2\2\2\u010f\u0113\n\20\2\2\u0110\u0112\n\17\2\2\u0111\u0110\3"+
		"\2\2\2\u0112\u0115\3\2\2\2\u0113\u0111\3\2\2\2\u0113\u0114\3\2\2\2\u0114"+
		"\u0117\3\2\2\2\u0115\u0113\3\2\2\2\u0116\u00f9\3\2\2\2\u0116\u00fa\3\2"+
		"\2\2\u0116\u0101\3\2\2\2\u0116\u010f\3\2\2\2\u0117\u011b\3\2\2\2\u0118"+
		"\u0119\7\17\2\2\u0119\u011c\7\f\2\2\u011a\u011c\t\21\2\2\u011b\u0118\3"+
		"\2\2\2\u011b\u011a\3\2\2\2\u011c\u011d\3\2\2\2\u011d\u011e\b\24\2\2\u011e"+
		"(\3\2\2\2\u011f\u0121\t\22\2\2\u0120\u011f\3\2\2\2\u0121\u0122\3\2\2\2"+
		"\u0122\u0120\3\2\2\2\u0122\u0123\3\2\2\2\u0123\u0124\3\2\2\2\u0124\u0125"+
		"\b\25\3\2\u0125*\3\2\2\2\u0126\u0127\7%\2\2\u0127\u012b\7#\2\2\u0128\u012a"+
		"\n\17\2\2\u0129\u0128\3\2\2\2\u012a\u012d\3\2\2\2\u012b\u0129\3\2\2\2"+
		"\u012b\u012c\3\2\2\2\u012c\u012e\3\2\2\2\u012d\u012b\3\2\2\2\u012e\u012f"+
		"\b\26\2\2\u012f,\3\2\2\2*\2\61\679ACTX]diosy|\u0081\u0085\u008c\u0092"+
		"\u0096\u009e\u00a1\u00a8\u00ac\u00b0\u00b5\u00b9\u00be\u00c4\u00ca\u00d7"+
		"\u00e5\u00fe\u0105\u010c\u0113\u0116\u011b\u0122\u012b\4\2\3\2\b\2\2";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}