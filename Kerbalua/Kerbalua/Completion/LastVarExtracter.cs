using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using System.Text;
using Kerbalua.Utility;

namespace Kerbalua.Completion {
    public class LastVarExtracter {

        public static void Test(){
            string tokenstring2 = "local a=a[a1a].b[ b1a]( b2.b2a)().c(b3).asdf[ a() . asdf().asdf";
            string tokenstring = "(b3)";
            var rawtokens = CreateTokensBackwardsFromEnd(tokenstring2);
            prin.tlist("Raw tokens",rawtokens);
            var correctTokens = ParseBackwardsTokens(rawtokens);
            prin.tlist("Correct Tokens",correctTokens);
        }

        public static List<Token> Parse(string stringToParse)
        {
            var rawtokens = CreateTokensBackwardsFromEnd(stringToParse);
            var correctTokens = ParseBackwardsTokens(rawtokens);
            return correctTokens;
        }

        /// <summary>
        /// Ensure that the token list we have is in the right format.\
        /// and replace identifier,funcall with namedfuncall
        /// </summary>
        /// <returns>The backwards tokens.</returns>
        /// <param name="tokens">Tokens.</param>
        static List<Token> ParseBackwardsTokens(List<Token> tokens){
            var correctTokens = new List<Token>();
            var rawTokens = tokens.GetEnumerator();
            if(!rawTokens.MoveNext()){
                return correctTokens;
            }
            Token previousToken = rawTokens.Current;
            correctTokens.Add(previousToken);
            Token currentToken = null;
            while(rawTokens.MoveNext()){
                currentToken = rawTokens.Current;
                correctTokens.Add(currentToken);
                switch(previousToken.type){
                case TokenType.IDENTIFIER:
                    if(currentToken.type!=TokenType.DOT){
                        return new List<Token>();
                    }
                    break;
                case TokenType.DOT:
                    if(currentToken.type==TokenType.DOT){
                        return new List<Token>();
                    }
                    break;
                case TokenType.ARRAY_ACCESS:
                    if (currentToken.type == TokenType.DOT) {
                        return new List<Token>();
                    }
                    break;
                case TokenType.FUNCALL:
                    if(currentToken.type==TokenType.DOT){
                        return new List<Token>();
                    }
                    // Replace patterns of IDENTIFIER FUNCALL with 
                    // NAMEDFUNCALL
                    if(currentToken.type==TokenType.IDENTIFIER){
                        int lastIndex = correctTokens.Count - 1;
                        correctTokens.RemoveAt(lastIndex--);
                        Token lastToken = correctTokens[lastIndex];

                        correctTokens[correctTokens.Count - 1] = new NamedFuncallToken(currentToken.content);
                    }
                    break;
                }
                previousToken = currentToken;
            }
            switch(correctTokens[0].type){
            case TokenType.ARRAY_ACCESS:
            case TokenType.FUNCALL:
                return new List<Token>();
            }
            correctTokens.Reverse();
            switch(correctTokens[0].type){
            case TokenType.IDENTIFIER:
            case TokenType.NAMED_FUNCALL:
                return correctTokens;
            }

            return new List<Token>();
        }




        static string ReverseString(string stringToReverse){
            var toBuilder = new StringBuilder();
            for (int i = stringToReverse.Length - 1; i >= 0;i--){
                toBuilder.Append(stringToReverse[i]);
            }
            return toBuilder.ToString();
        }

        /// <summary>
        /// Creates the tokens backwards from the end.
        /// </summary>
        /// <returns>The tokens backwards from the end.</returns>
        /// <param name="luaString">Lua string.</param>
        static public List<Token> CreateTokensBackwardsFromEnd(string luaString)
        {
            var tokens = new List<Token>();
            bool endOfInput;
            var charStack = StringToCharStack(luaString);
            while (charStack.Count>0) {
                char currentChar = charStack.Peek();

                if (IsIdentifierChar(currentChar)) {
                    tokens.Add(
                        new IdentifierToken(
                            ExtractIdentifierBackwards(charStack)));
                } else if (currentChar == ']') {
                    var token=new ArrayAccessToken(
                        ExtractToMatchingBackwards(charStack, ']', '[')
                    );
                    if(token.content==""){
                        return tokens;
                    }
                    tokens.Add(token);
                } else if (currentChar == ')') {
                    var token = new FuncallToken(
                       ExtractToMatchingBackwards(charStack, ')', '(')
                    );
                    if (token.content == "") {
                        return tokens;
                    }
                    tokens.Add(token);
                } else if (char.IsWhiteSpace(currentChar)) {
                    charStack.Pop();
                } else if (currentChar == '.') {
                    tokens.Add(new DotToken());
                    charStack.Pop();
                } else {
                    break;
                }
            }

            return tokens;
        }



        static string ExtractIdentifierBackwards(Stack<char> charStack){
            var identifier = new StringBuilder();
            while (charStack.Count>0 && IsIdentifierChar(charStack.Peek())){
                identifier.Append(charStack.Pop());
            }

            return ReverseString(identifier.ToString());
        }

        static string ExtractToMatchingBackwards(
            Stack<char> charStack,char startChar,char endChar){
            var content = new StringBuilder();
            int depth = 0;
            char currentChar;
            do {
                currentChar = charStack.Pop();
                if (currentChar == startChar) {
                    depth++;
                } else if (currentChar == endChar) {
                    depth--;
                }
                content.Append(currentChar);
                if (charStack.Count==0) {
                    if (depth != 0) return "";
                    break;
                }
            } while (depth != 0);
            return ReverseString(content.ToString());
        }


        static Stack<char> StringToCharStack(string str){
            var charStack = new Stack<char>();
            foreach(var ch in str) {
                charStack.Push(ch);
            }
            return charStack;
        }

        static public bool IsIdentifierChar(char ch){
            return Char.IsLetterOrDigit(ch) || ch == '_' || ch == '@';
        }

        public enum TokenType {
            IDENTIFIER,
            NAMED_FUNCALL,
            ARRAY_ACCESS,
            FUNCALL,
            DOT,
        }

        public class NamedFuncallToken : Token {
            public NamedFuncallToken(string content) : base(content)
            {
                type = TokenType.NAMED_FUNCALL;
            }
        }
        public class FuncallToken : Token {
            public FuncallToken(string content) : base(content)
            {
                type = TokenType.FUNCALL;
            }
        }
        public class ArrayAccessToken : Token {
            public ArrayAccessToken(string content) : base(content)
            {
                type = TokenType.ARRAY_ACCESS;
            }
        }
        public class IdentifierToken : Token {
            public IdentifierToken(string content) : base(content)
            {
                type = TokenType.IDENTIFIER;
            }
        }
        public class DotToken : Token {
            public DotToken() : base(".")
            {
                type = TokenType.DOT;
            }
        }
        public abstract class Token {
            protected Token(string content)
            {
                this.content = content;
            }
            public TokenType type;
            public string content;

            public override string ToString(){
                return "("+ type+":" + content+")";
            }
        }
    }
}
