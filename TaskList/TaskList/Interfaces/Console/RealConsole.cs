using System;

namespace TaskList.Interfaces.Console
{
    public class RealConsole : IConsole
    {
        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public void Write(string format, params object[] args)
        {
            System.Console.Write(format, args);
        }

        public void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        public void WriteLine()
        {
            System.Console.WriteLine();
        }
    }
}
