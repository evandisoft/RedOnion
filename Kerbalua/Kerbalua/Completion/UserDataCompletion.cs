using System;
using System.Collections.Generic;
using System.Reflection;
using Kerbalua.Utility;
using MoonSharp.Interpreter;

namespace Kerbalua.Completion {
    public class UserDataCompletion {

        public static List<string> Complete(Table globals, List<LastVarExtracter.Token> tokens)
        {
            var completions = new List<string>();
            // The very last part of "suffixes" is assumed
            // to be an incomplete name so we want the type immediately 
            // prior to it
            var tokenEnumerator = tokens.GetEnumerator();
            if (!tokenEnumerator.MoveNext()) {
                return completions;
            }

            var firstIdentifier = tokenEnumerator.Current;

            var firstObject = globals[firstIdentifier.content];
            if (firstObject == null) {
                return completions;
            }
            if (firstObject is Table) {
                return completions;
            }
            var firstType = firstObject.GetType();
            var nodes = new List<Node>();
            if (firstIdentifier is LastVarExtracter.NamedFuncallToken) {
                nodes.Add(new NamelessCall(0));
            }
            nodes.AddRange(CreateNodes(tokenEnumerator));
            var lastToken = tokens[tokens.Count - 1];
            if (lastToken.type == LastVarExtracter.TokenType.IDENTIFIER) {
                nodes.RemoveAt(nodes.Count - 1);
            }
            prin.tlist(nodes);
            var lastType = GetLastType(firstType, nodes);
            var allMembers = ListAllMembers(lastType);
            var lastIdentifier = lastToken.content == "." ? "" : lastToken.content;
            foreach (var member in allMembers) {
                if (member.StartsWith(lastIdentifier)) {
                    completions.Add(member);
                }
            }

            completions.Sort();


            return completions;
        }

        static List<Node> CreateNodes(List<LastVarExtracter.Token>.Enumerator tokenEnumerator)
        {
            var nodes = new List<Node>();

            LastVarExtracter.Token currentToken = null;
            while (tokenEnumerator.MoveNext()) {
                currentToken = tokenEnumerator.Current;
                switch (currentToken.type) {
                case LastVarExtracter.TokenType.DOT:
                    break;
                case LastVarExtracter.TokenType.ARRAY_ACCESS:
                    nodes.Add(new ArrayAccess(0));
                    break;
                case LastVarExtracter.TokenType.FUNCALL:
                    nodes.Add(new NamelessCall(0));
                    break;
                case LastVarExtracter.TokenType.IDENTIFIER:
                    nodes.Add(new FieldOrProperty(currentToken.content, 0));
                    break;
                case LastVarExtracter.TokenType.NAMED_FUNCALL:
                    nodes.Add(new MethodCall(currentToken.content, 0));
                    break;
                }
            }

            return nodes;
        }

        static public HashSet<string> ListAllMembers(Type t)
        {
            var strs = new HashSet<string>();
            foreach (var member in t.GetMembers()) {
                if (member.Name.Contains("_")) {
                    strs.Add(member.Name.Split('_')[1]);
                } else {
                    strs.Add(member.Name);
                }
            }
            return strs;
        }

  
        static Type GetLastType(Type t, List<Node> nodes)
        {
            for (int i = 0; i < nodes.Count; i++) {
                t = nodes[i].GetReturnType(t);
            }
            return t;
        }

        static List<Node> CreateNodes(IEnumerator<LastVarExtracter.Token> tokens){
            var nodes = new List<Node>();

            return nodes;
        }




        class MethodCall : Node {
            string name;
            public MethodCall(string name,int argNum) : base(argNum)
            {
                this.name = name;
            }

            public override Type GetReturnType(Type t)
            {
                MethodInfo mi=t.GetMethod(name);
                if(mi!=null){
                    return mi.ReturnType;
                }

                var fieldOrPropertyNode = new FieldOrProperty(name, 0);
                var delegateT = fieldOrPropertyNode.GetReturnType(t);
                return delegateT.GetMethod("Invoke").ReturnType;
            }

            public override string ToString()
            {
                return "." + name + "("+argNum+")";
            }
        }
        class FieldOrProperty : Node {
            public string name;
            public FieldOrProperty(string name,int argNum) : base(argNum)
            {
                this.name = name;
            }

            public override Type GetReturnType(Type t)
            {
                var p = t.GetProperty(name);
                if(p!=null){
                    return p.PropertyType;
                }
                var f = t.GetField(name);
                return f.FieldType;
            }

            public override string ToString()
            {
                return "." + name;
            }
        }
        class ArrayAccess : Node {
            public ArrayAccess(int argNum) : base(argNum)
            {
            }

            public override Type GetReturnType(Type t)
            {
                return t.GetProperty("Item").PropertyType;
            }

            public override string ToString()
            {
                return "[" + argNum + "]";
            }
        }
        class NamelessCall : Node {
            public NamelessCall(int argNum) : base(argNum)
            {
            }

            public override Type GetReturnType(Type t)
            {
                return t.GetMethod("Invoke").ReturnType;
            }

            public override string ToString()
            {
                return "(" + argNum + ")";
            }
        }
        abstract class Node{
            protected int argNum;
            protected Node(int argNum){
                this.argNum = argNum;
            }

            abstract public Type GetReturnType(Type t);

            abstract public override string ToString();
        }
    }
}
