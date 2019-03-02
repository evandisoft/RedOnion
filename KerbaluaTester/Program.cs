using System;
using System.Collections.Generic;
using Kerbalua.Other;
using System.Reflection;


namespace kerbaluaTester
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            object instance = Construct("");
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
