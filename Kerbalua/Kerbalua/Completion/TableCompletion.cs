using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System.Collections.Generic;
using UnityEngine;
using Kerbalua.Utility;

namespace Kerbalua.Completion {
    public class TableCompletion {
        static public List<string> Complete(Table table, List<LastVarExtracter.Token> correctTokens)
        {
            List<string> completions = new List<string>();

            var tokenEnumerator = correctTokens.GetEnumerator();

            if (table.MetaTable != null) {
                QueueLogger.Completion.Enqueue("Table has a metatable");
                return new List<string>();
            }

            LastVarExtracter.Token currentToken = tokenEnumerator.Current;
            while (tokenEnumerator.MoveNext()) {
                currentToken = tokenEnumerator.Current;
                QueueLogger.Completion.Enqueue("Current token is "+ currentToken.content);
                QueueLogger.Completion.Enqueue("Type is " + currentToken.type);
                if (currentToken.type != LastVarExtracter.TokenType.IDENTIFIER) {
                    return new List<string>();
                }
                if (!tokenEnumerator.MoveNext()) {
                    QueueLogger.Completion.Enqueue("No more tokens.");
                    break;
                }

                var nextObject = table[currentToken.content];

                currentToken = tokenEnumerator.Current;
                if (nextObject == null) {
                    QueueLogger.Completion.Enqueue("Table doesn't contain next object.");
                    return new List<string>();
                }
                if(nextObject is Table){
                    QueueLogger.Completion.Enqueue("Next Object is a Table.");
                    table = nextObject as Table;

                    if (table.MetaTable != null) {
                        QueueLogger.Completion.Enqueue("Table has a metatable");
                        return new List<string>();
                    }
                } else {
                    QueueLogger.Completion.Enqueue("Next Object is not a Table."+ nextObject.GetType());
                    return new List<string>();
                }
            }

            var match = "";
            if (currentToken.type != LastVarExtracter.TokenType.DOT) {
                match = currentToken.content;
                QueueLogger.Completion.Enqueue("Match is "+ match);
            }

            foreach(var entry in table.Keys) {
                QueueLogger.Completion.Enqueue("Looking through keys,"+ entry);
                if (entry.Type == DataType.String) {
                    QueueLogger.Completion.Enqueue("entry is a string "+ entry.String);
                    if (entry.String.StartsWith(match)) {
                        QueueLogger.Completion.Enqueue("It's a match");
                        completions.Add(entry.String);
                    }
                }
            }

            return completions;
        }
    }
}
