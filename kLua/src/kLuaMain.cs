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
        // ToggleGui has to be reset to a new relevant value each time we 
        // enter a new scene, otherwise its referencing stuff that
        // no longer has any relevance.
        static public ToggleGUI ToggleGui;

        SimpleScript script;
        static Texture2D toolbarTexture;
        Boolean guiOn = false;
        int maxOutputBytes = 80000;

        static public HashSet<string> ListAllMembers(object o){
            var strs = new HashSet<string>();
            foreach(var member in o.GetType().GetMembers()){
                if(member.Name.Contains("_")){
                    strs.Add(member.Name.Split('_')[1]);
                }
                else{
                    strs.Add(member.Name);
                }
            }
            return strs;
        }



        public void Awake()
        {
            if (ToggleGui == null)
            {
                ApplicationLauncher.Instance.AddModApplication(
                    () => { ToggleGui(); },
                    () => { ToggleGui(); },
                null, null, null, null,
                ApplicationLauncher.AppScenes.ALWAYS,
                toolbarTexture
                );
            }
            // We have to connect it with a function that has relevance in this
            // particular scene. If ToggleGui was already set, it was with
            // a delegate from another scene.
            ToggleGui = LocalToggleGui;

        }

        void LocalToggleGui()
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
            script.Options.DebugPrint = s => { outputContent.text += Environment.NewLine + s; };


            //outputStyle.normal.textColor = Color.yellow;
        }

        public void Update()
        {
            //popo.gameObject.SetActive(true);
            //Debug.Log(popo);
        }

        GUISkin guiSkin = new GUISkin();
        GUIContent completionContent= new GUIContent("");
        GUIContent outputContent = new GUIContent("");
        GUIContent guiContent = new GUIContent("");
        GUIStyle outputStyle; 


        Rect inputRect = new Rect(0, 400, 700, 100);
        Rect completionRect = new Rect(310, 100, 300, 300);
        Rect outputRect= new Rect(0, 0, 700, 400);
        Rect windowRect = new Rect(100, 100, 700, 500);
        int cursorpos = 0;
        bool completing;
        int windowID = 0;

        public void OnGUI()
        {
            if(!guiOn){
                return;
            }

            try{
                windowRect=GUI.Window(windowID, windowRect, WindowFunc, "kLua REPL");
                CompletionBox();
            }
            catch(Exception e){
                Debug.Log(e);
            }
        }


        void WindowFunc(int id){
            GUI.DragWindow(new Rect(0, 0, windowRect.width, windowRect.height));
            InputBox();
            OutputBox();

        }

        void InputBox(){
            Event e = Event.current;


            if (e.type == EventType.Repaint) {
                // I have no idea how to use IMGUI library correctly beyond simple examples. 
                // Docs are hideously insufficient
                // This obscure solution for text selection and cursor control found at:
                // https://answers.unity.com/questions/145698/guistyledrawwithtextselection-how-to-use-.html
                int id = GUIUtility.GetControlID(guiContent, FocusType.Keyboard, inputRect);
                //GUIUtility.keyboardControl = id; // added
                GUI.skin.textArea.DrawCursor(inputRect, guiContent, id, cursorpos);

                GUI.skin.textArea.DrawWithTextSelection(inputRect, guiContent, id, 0, 0);
            } else if (e.type == EventType.KeyDown) {
                switch (e.keyCode) {
                case KeyCode.Backspace:
                    int curlen = guiContent.text.Length;
                    if (curlen > 0) {
                        guiContent.text = guiContent.text.Substring(0, curlen - 1);
                        cursorpos -= 1;
                    }
                    break;
                case KeyCode.Return:
                    guiContent.text += e.character;
                    cursorpos += 1;
                    break;
                case KeyCode.Tab:
                    completing = true;
                    break;
                default:
                    char ch = e.character;
                    if (!Char.IsControl(ch)) {
                        guiContent.text += e.character;
                        cursorpos += 1;
                    }
                    break;
                }
                int diff = outputContent.text.Length- maxOutputBytes;
                if(diff>0){
                    outputContent.text = outputContent.text.Substring(diff);
                }

            } else if (e.type == EventType.MouseDown) {

            }

        }
        Vector2 scrollPos = new Vector2(0, 0);
        void OutputBox(){
            outputStyle = new GUIStyle(GUI.skin.textArea) {
                alignment = TextAnchor.LowerLeft
            };

            scrollPos =GUI.BeginScrollView(outputRect, scrollPos, outputRect);
            outputContent.text = GUI.TextArea(outputRect, outputContent.text, outputStyle);
            // if the user pressed enter twice in a row, submit the value
            if (guiContent.text.EndsWith(Environment.NewLine + Environment.NewLine)) {
                DynValue result = new DynValue();
                try{
                    result=script.DoString(guiContent.text);
                    outputContent.text += Environment.NewLine;
                    if (result.UserData == null) {
                        outputContent.text += result;
                    } else {
                        outputContent.text += result.UserData.Object;
                        if (result.UserData.Object == null) {
                            outputContent.text += " (" + result.UserData.Object.GetType() + ")";
                        }
                    }
                } catch(Exception e){
                    Debug.Log(e);
                }
                guiContent.text = "";
                cursorpos = 0;
                completionContent.text = "";

            } 
            GUI.EndScrollView();
        }

        void CompletionBox(){
            completionRect.height= outputStyle.CalcSize(completionContent).y;
            completionRect.y = windowRect.y;
            completionRect.x = windowRect.x + windowRect.width;
            if (guiContent.text.Contains(".") && !guiContent.text.Contains(Environment.NewLine)) {
                var splitAt = guiContent.text.LastIndexOf('.');
                var baseStr = guiContent.text.Substring(0, splitAt);
                var toEval = baseStr;
                if (toEval.Contains("=")) {
                    toEval = toEval.Split('=')[1];
                }
                var completion = guiContent.text.Substring(splitAt + 1);
                var result = script.DoString(toEval);
                if (result.IsNotNil() && result.UserData != null) {
                    HashSet<string> allMembers = ListAllMembers(result.UserData.Object);
                    List<string> compatibleMethods = new List<string>();
                    foreach(var memberName in allMembers){
                        if (memberName.StartsWith(completion)) { compatibleMethods.Add(memberName); }
                    }
                    compatibleMethods.Sort();
                    if (completing) {
                        if (compatibleMethods.Count > 0) {
                            var newCompletion = compatibleMethods[0];

                            guiContent.text = baseStr + "." + newCompletion;
                            cursorpos = guiContent.text.Length;
                        }

                        completing = false;
                    }
                    completionContent.text = "";
                    foreach (var methodname in compatibleMethods) {
                        completionContent.text += methodname + Environment.NewLine;
                    }
                    completionContent.text = GUI.TextArea(completionRect, completionContent.text);
                }

            } else{
                completionContent.text = "";
            }
        }
    }
}
