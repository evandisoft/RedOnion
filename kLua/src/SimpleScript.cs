using System;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using UnityEngine;

namespace kLua.src
{
    public class SimpleScript:Script
    {
        public SimpleScript()
        {

        }

        public DynValue DoString(string str){
            try{
                DynValue result = base.DoString("return "+str);
                return result;
            }
            catch (SyntaxErrorException e)
            {
                DynValue result = base.DoString(str);
                return result;
            }
            
        }
    }
}
