using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarDump.Common
{
    public class Output
    {
        public void WriteErrorLine(string message = null)
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

        public void WriteErrorLine(Exception e)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(e);
            System.Console.ResetColor();
        }

        public void WriteErrorLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteErrorLine(message);
            }
        }

        public void WriteWarningLine(string message = null)
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

        public void WriteWarningLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteWarningLine(message);
            }
        }

        public void WriteLine(string message = null)
        {
            if (message == null)
            {
                System.Console.WriteLine();
                return;
            }

            System.Console.WriteLine(message);
        }

        public void WriteLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteLine(message);
            }
        }
    }
}
