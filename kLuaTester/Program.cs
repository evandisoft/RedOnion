using System;
using System.Collections.Generic;
using kLua.src;

namespace kLuaTester
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            List<string> examples = new List<string> { "blah", "b", "hernodale" };
            var st = new Trie(examples);
            List<string> hits = st.Find("b");
            foreach (var hit in hits)
            {
                Console.WriteLine(hit);
            }
        }
    }
}
