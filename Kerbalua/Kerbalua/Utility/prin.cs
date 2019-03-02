using System;
using System.Collections.Generic;
namespace Kerbalua.Utility {
    public class prin {
        public prin()
        {

        }
        public static void t(string message){
            Console.WriteLine(message);
        }
        public static void tall<T>(string message, IEnumerable<T> os)
        {
            foreach (var o in os) {
                Console.WriteLine(message + " " + o);
            }
        }
        public static void tall<T>(IEnumerable<T> os)
        {
            foreach (var o in os) {
                Console.WriteLine(o);
            }
        }
        public static void tlist<T>(string message,List<T> os)
        {
            Console.Write(message + " ("+os.Count+"): ");
            foreach (var o in os) {
                Console.Write(o+", ");
            }
            Console.WriteLine("\n");
        }
        public static void tlist<T>(IEnumerable<T> os)
        {
            foreach (var o in os) {
                Console.Write(o);
            }
            Console.WriteLine("\n");
        }
        //public static void tall<T>(string message, List<T> os)
        //{
        //    foreach (var o in os) {
        //        Console.WriteLine(message + " " + o);
        //    }
        //}
    }
}
