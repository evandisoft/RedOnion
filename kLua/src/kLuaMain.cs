using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace kLua
{
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class kLuaMain : MonoBehaviour
    {
        public void Start(){

        }

        public void Update()
        {
            Debug.Log(Script.VERSION);
        }
    }
}
