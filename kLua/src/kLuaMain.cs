using System;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using System.Collections.Generic;
using kLua.src;

namespace kLua
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class kLuaMain : MonoBehaviour
    {
        SimpleScript script;

        static public List<string> ListAllMethods(object o){
            var strs = new List<string>();
            foreach(var method in o.GetType().GetMethods()){
                if(method.Name.Contains("_")){
                    strs.Add(method.Name.Split('_')[1]);
                }
                else{
                    strs.Add(method.Name);
                }
            }
            return strs;
        }

        public void Start(){
            script = new SimpleScript();
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            script.Globals["this"] = this;
            
            script.Globals["flight"] = new FlightGlobals();
            script.Options.DebugPrint = s => { Debug.Log(s); };
        }

        public void Update()
        {
        }

        string completionText = "";
        string input="";
        //string toEval = "";
        string output = "";
        void OnGUI()
        {
            completionText=GUI.TextArea(new Rect(310, 100, 300, 300), completionText);
            GUILayout.BeginArea(new Rect(10, 100, 300, 300));
            // This line feeds "This is the tooltip" into GUI.tooltip
            input = GUILayout.TextArea(input);
            output=GUILayout.TextArea(output);
            if(input.EndsWith("\n\n")){
                DynValue result = script.DoString(input);
                input = "";
                if(result.IsNil()){
                    output = "";
                }
                else{
                    output = result + "\n";
                    if (result.UserData == null){
                        output += result.Type;
                    }else{
                        output += result.UserData.Object.GetType();
                    }
                }

            }
            else if(input.Contains(".") && !input.Contains("\n")){
                var splitAt = input.LastIndexOf('.');
                var toEval = input.Substring(0, splitAt);
                if(toEval.Contains("=")){
                    toEval = toEval.Split('=')[1];
                }
                var completion = input.Substring(splitAt + 1);
                var result = script.DoString(toEval);
                if(result.IsNotNil()&&result.UserData!=null){
                    List<string> allMethods = ListAllMethods(result.UserData.Object);
                    List<string> compatibleMethods = new Trie(allMethods).Find(completion);
                    if(completion.Contains(" ")){

                        if(compatibleMethods.Count>0){
                            var newCompletion = compatibleMethods[0];
                            input = input + newCompletion.Substring(completion.Length - 1);
                        };

                    }
                    completionText = "";
                    foreach(var methodname in compatibleMethods){
                        completionText += methodname + "\n";
                    }
                }
            }
            else{
                completionText = "";
            }
           
            GUILayout.EndArea();
        }
    }
}
