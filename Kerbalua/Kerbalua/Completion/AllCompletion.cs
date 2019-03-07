using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;
using System.Collections.Generic;
using Kerbalua.Other;
using Kerbalua.Utility;
using Kerbalua.Gui;

namespace Kerbalua.Completion {
    public class AllCompletion {
        public static void Main(string[] args){
            var script = new MoonSharpReplEvaluator(CoreModules.Preset_Default);
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            //script.Globals["Adf"] = new Adf();
            var data = "return adf.asdf().blah[Adf.asdf().blah[Adf.adfs[3]().";
            var data2 = "a";
            var tokens = LastVarExtracter.Parse(data);
            prin.tlist(tokens);
            //var completions = UserDataCompletion.Complete(script.Globals, tokens);
            //prin.tall("Completions are ",completions);
        }
        Action<string> messageTarget;

        static public Adf aaa(int i)
        {
            return new Adf();
        }
        public delegate Adf blah3(int i);

        public class Adf {
            public Adf asdf(){
                return this;
            }
            public string asd="badsf";
            public delegate Adf blah2(int i);
            public blah2 blah;
            public List<blah2> adfs;
        }

        public static void SplitInput(
            string input, 
            int cursorPos, 
            out string basePart,
            out string completionPart,
            out string endPart
        )
        {
            int completionStart, completionEnd;

            for (
                completionStart = cursorPos - 1;
                completionStart >= 0 && LastVarExtracter.IsIdentifierChar(input[completionStart]);
                completionStart--
            ) { }
            completionStart++;

            for (
                completionEnd = cursorPos + 1;
                completionEnd < input.Length && LastVarExtracter.IsIdentifierChar(input[completionEnd]);
                completionEnd++
            ) { }
            completionEnd--;

            basePart = input.Substring(0, completionStart);
            completionPart = input.Substring(completionStart, completionEnd - completionStart);
            endPart = input.Substring(completionEnd);
        }


		public static void Complete(
			Table globals,
			EditingArea inputArea,
			GUIContent completionBoxContent,
			bool completing
        )
        {
            string basePart, completionPart, endPart;
            SplitInput(inputArea.content.text, inputArea.cursorIndex, out basePart, out completionPart, out endPart);
			//Debug.Log(inputArea.content.text+","+basePart + "," + completionPart + "," + endPart);

            var correctTokens = LastVarExtracter.Parse(basePart+completionPart);
	

            List<string> completions = new List<string>();
            if (correctTokens.Count > 0) {
                object rootObject = globals[correctTokens[0].content];
                if (rootObject == null) {
                    foreach (var dynValue in globals.Keys) {
                        if (dynValue.Type == DataType.String) {
                            var strval = dynValue.String;
                            if (strval.StartsWith(correctTokens[0].content)) {
                                completions.Add(strval);
                            }
                        }
                    }
                } else {
                    try {
                        if (rootObject is Table table) {
                            QueueLogger.Completion.Enqueue("Checking table completion");
                            completions = TableCompletion.Complete(globals, correctTokens);
                        } else if (!(rootObject is DynValue)) {
                            completions = UserDataCompletion.Complete(globals, correctTokens);
                        }
                    }
                    catch(Exception e) {
                        QueueLogger.Completion.Enqueue(e);
                    }
                }
            } else {
                foreach (var dynValue in globals.Keys) {
                    if (dynValue.Type == DataType.String) {
                        completions.Add(dynValue.String);
                    }
                }
            }
            completions.Sort();

            if (completions.Count > 0) {
                if (completing) {
                    inputArea.content.text = basePart + completions[0] + endPart;
                    inputArea.cursorIndex= basePart.Length + completions[0].Length;
					inputArea.selectIndex = inputArea.cursorIndex;
					//Debug.Log("completing " + completions.Count);
                } else {
					completionBoxContent.text = "";
					foreach (var completion in completions) {
                        completionBoxContent.text += completion + Environment.NewLine;
                    }
					//Debug.Log("not completing "+completions.Count);
                }

            } else {
                completionBoxContent.text = "";
            }
        }
    }
}
