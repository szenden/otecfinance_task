using System;

namespace TaskList.Interfaces.Console
{
    /// <summary>
    /// Defines the contract for console input and output operations.
    /// Provides an abstraction layer for console interactions to support testing and different console implementations.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Reads a line of characters from the console.
        /// </summary>
        /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
        string ReadLine();

        /// <summary>
        /// Writes the specified formatted string to the console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using the format.</param>
        void Write(string format, params object[] args);

        /// <summary>
        /// Writes the specified formatted string followed by the current line terminator to the console.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to write using the format.</param>
        void WriteLine(string format, params object[] args);

        /// <summary>
        /// Writes the current line terminator to the console.
        /// </summary>
        void WriteLine();
    }
}
