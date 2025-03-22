using System;

namespace TaskList.Interfaces.Console
{
    /// <summary>
    /// Implementation of IConsole that uses the system console for input and output operations.
    /// Provides direct interaction with the standard input and output streams.
    /// </summary>
    public class RealConsole : IConsole
    {
        /// <summary>
        /// Reads a line of characters from the system console.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        /// <summary>
        /// Writes the specified formatted string to the system console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using the format.</param>
        public void Write(string format, params object[] args)
        {
            System.Console.Write(format, args);
        }

        /// <summary>
        /// Writes the specified formatted string followed by the current line terminator to the system console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using the format.</param>
        public void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        /// <summary>
        /// Writes the current line terminator to the system console.
        /// </summary>
        public void WriteLine()
        {
            System.Console.WriteLine();
        }
    }
}
