using System;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.REPL;
using MoonSharp.Interpreter.Interop;
using System.Collections.Generic;
using kLua.src;
using UnityEngine.UI;
using KSP.UI.Screens;
using UnityEngine.EventSystems;

namespace kLua
{


    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class kLuaMain : MonoBehaviour
    {
        public delegate void ToggleGUI();
        static public ToggleGUI toggleGUI;

        SimpleScript script;
        static Texture2D toolbarTexture;
        Boolean guiOn = false;

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

        public void Awake()
        {
            if (toggleGUI == null)
            {
                ApplicationLauncher.Instance.AddModApplication(
                    () => { toggleGUI(); },
                    () => { toggleGUI(); },
                null, null, null, null,
                ApplicationLauncher.AppScenes.ALWAYS,
                toolbarTexture
                );
            }
            toggleGUI = OnClick;

        }

        private void OnClick()
        {
            if (guiOn)
            {
                InputLockManager.ClearControlLocks();
            }
            else
            {
                InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kLua");
            }

            guiOn = !guiOn;
        }

        

        public void Start(){
            script = new SimpleScript();
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            script.Globals["this"] = this;
            
            script.Globals["flight"] = new FlightGlobals();
            script.Options.DebugPrint = s => { Debug.Log(s); };
            //List<DialogGUIBase> dialog = new List<DialogGUIBase>();



             
        }

        public void Update()
        {
            //popo.gameObject.SetActive(true);
            //Debug.Log(popo);
        }

        string completionText = "";
        string input="";
        //string toEval = "";
        string output = "";
        Boolean cycle = false;
        GUIContent guiContent = new GUIContent("");


        private Rect _rect = new Rect(10, 100, 300, 300);
        private int cursorpos = 0;



        public void OnGUI()
        {
            if(!guiOn){
                return;
            }
            Event e = Event.current;
            

            if (e.type == EventType.Repaint)
            {
                // I have no idea how to use IMGUI library correctly beyond simple examples. 
                // Docs are hideously insufficient
                // This obscure solution for text selection and cursor control found at:
                // https://answers.unity.com/questions/145698/guistyledrawwithtextselection-how-to-use-.html
                int id = GUIUtility.GetControlID(guiContent, FocusType.Keyboard, _rect);
                //GUIUtility.keyboardControl = id; // added
                GUI.skin.textArea.DrawCursor(_rect, guiContent, id, cursorpos);

                GUI.skin.textArea.DrawWithTextSelection(_rect, guiContent, id, 0, 0);
            }
            else if (e.type == EventType.KeyDown)
            {
                switch(e.keyCode){
                    case KeyCode.Backspace:
                        int curlen = guiContent.text.Length;
                        if (curlen > 0)
                        {
                            guiContent.text = guiContent.text.Substring(0, curlen - 1);
                            cursorpos -= 1;
                        }
                        break;
                    case KeyCode.Return:
                        guiContent.text += e.character;
                        cursorpos += 1;
                        break;
                    default:
                        char ch = e.character;
                        if (!Char.IsControl(ch))
                        {
                            guiContent.text += e.character;
                            cursorpos += 1;
                        }
                        break;
                }
                e.Use();

            }
            else if(e.type==EventType.MouseDown){

            }

            output = GUILayout.TextArea(output);
            completionText = GUI.TextArea(new Rect(310, 100, 300, 300), completionText);
            if (guiContent.text.EndsWith("\n\n"))
            {
                DynValue result = script.DoString(guiContent.text);
                guiContent.text = "";
                cursorpos = 0;
                if (result.IsNil())
                {
                    output = "";
                }
                else
                {
                    output = result + "\n";
                    if (result.UserData == null)
                    {
                        output += result.Type;
                    }
                    else
                    {
                        output += result.UserData.Object.GetType();
                    }
                }

            }
            else if (guiContent.text.Contains(".") && !guiContent.text.Contains("\n"))
            {
                var splitAt = guiContent.text.LastIndexOf('.');
                var baseStr = guiContent.text.Substring(0, splitAt);
                var toEval = baseStr;
                if (toEval.Contains("="))
                {
                    toEval = toEval.Split('=')[1];
                }
                var completion = guiContent.text.Substring(splitAt + 1);
                var result = script.DoString(toEval);
                if (result.IsNotNil() && result.UserData != null)
                {
                    Boolean completing = false;
                    if (completion.Contains(" "))
                    {
                        completion = completion.Trim();
                        completing = true;
                    }

                    List<string> allMethods = ListAllMethods(result.UserData.Object);
                    List<string> compatibleMethods = new Trie(allMethods).Find(completion);
                    if (completing)
                    {

                        if (compatibleMethods.Count > 0)
                        {
                            var newCompletion = compatibleMethods[0];

                            guiContent.text = baseStr + "." + newCompletion;
                            cursorpos = guiContent.text.Length;
                        }
                    }
                    completionText = "";
                    foreach (var methodname in compatibleMethods)
                    {
                        completionText += methodname + "\n";
                    }
                }

            }
        }
    }
}
