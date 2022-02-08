using System;

namespace Parikhmaher
{
    public class ConsoleHelper
    {
        public static object LockObject = new object();
        public static void WriteToConsole(string info, string write)
        {
            lock(LockObject)
            {
                Console.WriteLine(info + " : " + write);
            }
        }
    }
}