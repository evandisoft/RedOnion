using System;
using System.Collections.Generic;
using Kerbalua.Other;
using System.Reflection;
using System.Threading;


namespace KerbaluaTester
{
    class MainClass
    {
		static Object lockObj = new Object();
		private static Mutex mut = new Mutex();

		static bool mainTurn;
		static Random random = new Random();
		public static void Main(string[] args)
        {
			Thread t = new Thread(new ThreadStart(OtherThread));

			t.Start();

			while (t.IsAlive) {
				AllowOtherThreadAndWaitForItToPause();
				Console.WriteLine("Main Thread");
				//Thread.Sleep((int)(random.NextDouble() * 1000));
			}
		}

		static void AllowOtherThreadAndWaitForItToPause()
		{
			lock (lockObj) {
				mainTurn = false;
			}
			while (true) {
				lock (lockObj) {
					if (mainTurn) {
						break;
					}
				}
				Thread.Sleep(10);
			}
		}

		static void AllowMainThreadAndWaitForItToPause()
		{
			lock (lockObj) {
				mainTurn = true;
			}
			while (true) {
				lock (lockObj) {
					if (!mainTurn) {
						break;
					}
				}
				Thread.Sleep(10);
			}
		}

		static public void OtherThread()
		{

			SimpleScript script = new SimpleScript(MoonSharp.Interpreter.CoreModules.Preset_Complete);
			Debugger debugger = new Debugger(AllowMainThreadAndWaitForItToPause);
			script.AttachDebugger(debugger);
			string source =
@"
-- defines a factorial function
function fact (n)
	if (n == 0) then
		return 1
	else
		return n*fact(n - 1)
	end
end

return fact(5)";

			//for(int i=0;i<100;i++) {
			//	Console.WriteLine("Script Thread "+i);

			//	//Thread.Sleep(1000);

			//	WaitForMainThread();
			//}

			//";
			try {
				//Console.WriteLine(script.EvaluateAsCoroutine(source));
			}
			catch(Exception e) {
				Console.WriteLine(e);
			}
			//Thread.Sleep(1000);

		}

		public static void TrieTest(){
            List<string> examples = new List<string> { "blah", "b", "hernodale" };
            var st = new Trie(examples);
            List<string> hits = st.Find("b");
            foreach (var hit in hits) {
                Console.WriteLine(hit);
            }
        }


        public static object Construct(string typename)
        {
            Type t = Type.GetType("FlightDriver,Assembly-CSharp");
            Console.WriteLine("T is " + t);

            //object instance = Activator.CreateInstance(t);
            //Console.WriteLine(instance);
            //ConstructorInfo constructorInfo = t.GetConstructor(new Type[] { });
            
            FieldInfo method = t.GetField("fetch");
            Console.WriteLine("Constructorinfo is " + method);

            object instance = method.GetValue(null);
            Console.WriteLine("instance is "+instance);
            //script.Globals[typename] = instance;
            //return DynValue.FromObject(script, instance);
            return instance;
        }
    }


}
