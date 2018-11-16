using System;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using System.Collections.Generic;

namespace kLua
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class kLuaMain : MonoBehaviour
    {
        Script script;



        public void Start(){
            script = new Script();
            repl = new ReplHistoryInterpreter(script, 1000);
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            script.Globals["this"] = this;
            
            script.Globals["flight"] = new FlightGlobals();
            script.Options.DebugPrint = s => { Debug.Log(s); };
        }

        public void Update()
        {
            
            //Debug.Log(Script.VERSION);
        }

        ReplHistoryInterpreter repl;
        string input="";
        //string toEval = "";
        string output = "";
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 100, 300, 300));
            // This line feeds "This is the tooltip" into GUI.tooltip
            input = GUILayout.TextArea(input);
            output=GUILayout.TextArea(output);
            if(input.EndsWith("\n\n")){
                try{
                    if(input.StartsWith(",")){
                        input = "return " + input.Substring(1);
                    }
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
                catch(Exception e){
                    Debug.Log(e);
                    input = "";
                    output = e + "\n";
                }
            }
            if(GUILayout.Button("Reset")){
                script = new Script();
                
            }
            GUILayout.EndArea();
        }
    }
}
