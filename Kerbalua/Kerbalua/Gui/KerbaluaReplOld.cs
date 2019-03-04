using System;
using UnityEngine;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Kerbalua.Other;
using Kerbalua.Completion;

namespace Kerbalua.Gui {
    public class KerbaluaReplOld {
        int maxOutputBytes = 80000;
        SimpleScript script;

		KerbaluaMain.KSPRaw kspApi;

        public class FlightControl {
            Vessel vessel;
            DynValue callback;
            SimpleScript script;

            void FlightCallback(FlightCtrlState st)
            {
                script.Call(callback, st);
            }

            public FlightControl(Vessel vessel,SimpleScript script)
            {
                this.vessel = vessel;
                this.script = script;
            }

            public void SetCallback(DynValue callback) {
                this.callback = callback;
                vessel.OnFlyByWire += FlightCallback;
            }
        }

        public KerbaluaReplOld(KerbaluaMain.KSPRaw kspApi)
        {
            this.kspApi = kspApi;
            script = new SimpleScript(CoreModules.Preset_Complete);
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;

            script.Globals["ksp"] = kspApi;
            script.Globals["flight"] = new FlightControl(FlightGlobals.ActiveVessel, script);

            script.Options.DebugPrint = Print;
            InputLockManager.ClearControlLocks();
            outputStyle = new GUIStyle();
            
        }

        public void Print(string str){
            outputContent.text += Environment.NewLine + str;
        }

        public void Render(bool guiActive){
            if (!guiActive) return;

            try {
                //replRect = GUI.Window(windowID, replRect, ScriptWindow, "kerbalua REPL");
                CompletionBox();
                inputChanged = false;
            } catch (Exception e) {
                Debug.Log(e);
            }
        }

        GUISkin guiSkin = new GUISkin();
        GUIContent completionContent = new GUIContent("");
        GUIContent outputContent = new GUIContent("");
        GUIContent inputContent = new GUIContent("");
        GUIStyle outputStyle;
        Rect completionRect = new Rect(310, 100, 300, 300);


        int cursorPos = 0;
        bool completing;
        int windowID = 0;
        bool inputChanged=true;
        Vector2 scrollPos = new Vector2(0, 0);



        void ScriptWindow(int id)
        {
			Rect windowRect= new Rect(100, 100, 700, 500);
			Rect editorRect = new Rect(0, 0, windowRect.width / 2, windowRect.height);
			Rect replRect = new Rect(windowRect.width / 2, 0, windowRect.width / 2, windowRect.height);
			GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
            HandleInput();
			EditorBox(editorRect);
            ReplBox(replRect);
        }

		void EditorBox(Rect editorRect)
		{
			outputContent.text = GUI.TextArea(editorRect, outputContent.text, outputStyle);

		}

		void ReplBox(Rect replRect)
        {
            float inputHeight = outputStyle.CalcHeight(inputContent, replRect.width);
            float inputStart = replRect.height - inputHeight;

            Rect outputRect = new Rect(0, 0, replRect.width, inputStart);
            Rect inputRect = new Rect(0, inputStart, replRect.width, inputHeight);

            InputBox(inputRect);
            OutputBox(outputRect);
        }

        void HandleInput(){
            Event e = Event.current;
            if (e.type == EventType.KeyDown) {
                switch (e.keyCode) {
                case KeyCode.Backspace:
                    int curlen = inputContent.text.Length;
                    if (curlen > 0) {
                        inputContent.text = inputContent.text.Substring(0, curlen - 1);
                        cursorPos -= 1;
                    }
                    break;
                case KeyCode.Return:
                    inputContent.text += e.character;
                    cursorPos += 1;
                    break;
                case KeyCode.Tab:
                    completing = true;
                    break;
                default:
                    char ch = e.character;
                    if (!char.IsControl(ch)) {
                        inputContent.text += e.character;
                        cursorPos += 1;
                    }
                    break;
                }
                int diff = outputContent.text.Length - maxOutputBytes;
                if (diff > 0) {
                    outputContent.text = outputContent.text.Substring(diff);
                }
                inputChanged = true;
                e.Use();
            }

            //if (Mouse.Left.GetClick() || Mouse.Right.GetClick()) {
            //    if (replRect.Contains(new Vector2(e.mousePosition.x + 100, e.mousePosition.y + 100))) {
            //        InputLockManager.SetControlLock(ControlTypes.KEYBOARDINPUT, "kerbalua");
            //    } else {
            //        InputLockManager.ClearControlLocks();
            //    }
            //}
        }

        void InputBox(Rect rect)
        {
            Event e = Event.current;
            if (e.type == EventType.Repaint) {
                // I have no idea how to use IMGUI library correctly beyond simple examples. 
                // Docs are hideously insufficient
                // This obscure solution for text selection and cursor control found at:
                // https://answers.unity.com/questions/145698/guistyledrawwithtextselection-how-to-use-.html
                int id = GUIUtility.GetControlID(inputContent, FocusType.Keyboard, rect);
                //GUIUtility.keyboardControl = id; // added
                GUI.skin.textArea.DrawCursor(rect, inputContent, id, cursorPos);

                //inputContent.text=GUI.TextArea(inputRect, inputContent.text);
                GUI.skin.textArea.DrawWithTextSelection(rect, inputContent, id, 0, 0);
            }

        }

        void OutputBox(Rect rect)
        {
            outputStyle = new GUIStyle(GUI.skin.textArea) {
                alignment = TextAnchor.LowerLeft
            };

            float outputHeight = outputStyle.CalcHeight(outputContent, rect.width);
            Rect outputContentRect = new Rect(rect);
            outputContentRect.height = Math.Max(outputHeight,rect.height);
            scrollPos = GUI.BeginScrollView(rect, scrollPos, outputContentRect);
            outputContent.text = GUI.TextArea(outputContentRect, outputContent.text, outputStyle);


            if (inputChanged) {
                scrollPos.y = rect.height;
            }

            // if the user pressed enter twice in a row, submit the value
            if (inputContent.text.EndsWith(Environment.NewLine + Environment.NewLine)) {
                DynValue result = new DynValue();
                try {
                    result = script.DoString(inputContent.text);
                    outputContent.text += Environment.NewLine;
                    if (result.UserData == null) {
                        outputContent.text += result;
                    } else {
                        outputContent.text += result.UserData.Object;
                        if (result.UserData.Object == null) {
                            outputContent.text += " (" + result.UserData.Object.GetType() + ")";
                        }
                    }
                } catch (Exception e) {
                    Debug.Log(e);
                }
                inputContent.text = "";
                cursorPos = 0;
                completionContent.text = "";

            }
            GUI.EndScrollView();
        }



        void CompletionBox()
        {
            if (inputChanged || completing) {
                AllCompletion.Complete(script.Globals, inputContent, completionContent, cursorPos, completing, out cursorPos);
                completing = false;
            }

            completionRect.height = outputStyle.CalcSize(completionContent).y;
            ////completionRect.y = replRect.y;
            ////completionRect.x = replRect.x + replRect.width;
            ////if (completionContent.text.Length > 0) {
            //    completionContent.text = GUI.TextArea(completionRect, completionContent.text);
            //}
        }
    }


}
