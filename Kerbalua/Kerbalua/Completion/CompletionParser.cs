﻿using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Kerbalua.Completion {
    public class CompletionParser {
        public static void Main(string[] args)
        {
            AntlrTest();
        }

        public static void AntlrTest()
        {
            string input = "local a=a[a1a].b[b1a](b2.b2a)().c(b3)[b4[a.b";
            string input1 = "function() return b";

            var terminalVar = ParseCompletion(input);
            Console.WriteLine(terminalVar);
        }

        public static TerminalVarNode ParseCompletion(string str){
            ICharStream stream = CharStreams.fromstring(str);
            ITokenSource lexer = new LuaCompletionLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            LuaCompletionParser parser = new LuaCompletionParser(tokens);
            parser.BuildParseTree = true;

            IParseTree tree = parser.chunk();
            var lastVarListener = new LastVarListener();
            ParseTreeWalker.Default.Walk(lastVarListener, tree);

            return lastVarListener.terminalVar;
        }

        public class TerminalVarNode {
            public ITerminalNode Name;
            public List<VarSuffixNode> suffixes = new List<VarSuffixNode>();

            public override string ToString()
            {
                string str = "";
                str += Name.ToString();
                foreach (var suffix in suffixes) {
                    str += suffix.ToString();
                }
                return str;
            }

            public VarSuffixNode GetCurrentSuffixNode()
            {
                if (suffixes.Count == 0) {
                    return null;
                }
                return suffixes[suffixes.Count - 1];
            }
            public Funcall GetCurrentFuncall()
            {
                var funcalls = GetCurrentSuffixNode()?.funcalls;
                if (funcalls != null && funcalls.Count != 0) {
                    return funcalls[funcalls.Count - 1];
                }
                return null;
            }
        }

        public class Funcall {
            public int NArgs = 0;
            public override string ToString()
            {
                return $"({NArgs})";
            }
        }

        public class VarSuffixNode {
            public ITerminalNode Name;
            public int NArgs = 0;
            public List<Funcall> funcalls = new List<Funcall>();
            public override string ToString()
            {
                string str = "";
                foreach (var funcall in funcalls) {
                    str += funcall.ToString();
                }
                if (Name == null) {
                    str += $"[{NArgs}]";
                } else {
                    str += "." + Name;
                }
                return str;
            }
        }


        class LastVarListener : LuaCompletionBaseListener {
            int varCount = 0;
            public TerminalVarNode terminalVar;

            public bool NearestVarAncestorNotTerminal()
            {
                return varCount > 0 || TerminalVarNotSet();
            }

            public bool TerminalVarNotSet()
            {
                return terminalVar == null;
            }

            public override void EnterTerminalVar(LuaCompletionParser.TerminalVarContext context)
            {
                terminalVar = new TerminalVarNode();
                terminalVar.Name = context.NAME();
            }
            public override void ExitTerminalVar(LuaCompletionParser.TerminalVarContext context)
            {

            }

            public override void EnterVarSuffix(LuaCompletionParser.VarSuffixContext context)
            {
                if (NearestVarAncestorNotTerminal()) return;

                var node = new VarSuffixNode();
                node.Name = context.NAME();
                terminalVar.suffixes.Add(node);
            }

            public override void ExitVarSuffix(LuaCompletionParser.VarSuffixContext context)
            {
                //Console.WriteLine("Exiting varsuffix " + context.NAME());
            }
            public override void EnterTerminalVarSuffix(LuaCompletionParser.TerminalVarSuffixContext context)
            {
                if (NearestVarAncestorNotTerminal()) return;

                var node = new VarSuffixNode();
                node.Name = context.NAME();
                terminalVar.suffixes.Add(node);
            }

            public override void ExitTerminalVarSuffix(LuaCompletionParser.TerminalVarSuffixContext context)
            {
                //Console.WriteLine("Exiting varsuffix " + context.NAME());
            }
            public override void EnterNameAndArgs(LuaCompletionParser.NameAndArgsContext context)
            {
                if (NearestVarAncestorNotTerminal()) return;

                var funcall = new Funcall();
                var suffix = terminalVar.GetCurrentSuffixNode();
                suffix.funcalls.Add(funcall);
                //Console.WriteLine(nameof(EnterNameAndArgs) + context.NAME());
            }

            public override void ExitNameAndArgs(LuaCompletionParser.NameAndArgsContext context)
            {
                //Console.WriteLine(nameof(ExitNameAndArgs) + context.NAME());
            }
            public override void EnterVar(LuaCompletionParser.VarContext context)
            {
                if (TerminalVarNotSet()) return;

                varCount++;
            }
            public override void ExitVar(LuaCompletionParser.VarContext context)
            {
                if (TerminalVarNotSet()) return;

                varCount--;
            }
        }
    }
}