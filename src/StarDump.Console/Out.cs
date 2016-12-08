using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarDump.Console
{
    public class Out
    {
        internal static void WriteErrorLine(string message = null)
        {
            if (message == null)
            {
                System.Console.WriteLine();
                return;
            }

            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
        }

        internal static void WriteErrorLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteErrorLine(message);
            }
        }

        internal static void WriteWarningLine(string message = null)
        {
            if (message == null)
            {
                System.Console.WriteLine();
                return;
            }

            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(message);
            System.Console.ResetColor();
        }

        internal static void WriteWarningLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteErrorLine(message);
            }
        }

        internal static void WriteLine(string message = null)
        {
            if (message == null)
            {
                System.Console.WriteLine();
                return;
            }

            System.Console.WriteLine(message);
        }

        internal static void WriteLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteLine(message);
            }
        }
    }
}
