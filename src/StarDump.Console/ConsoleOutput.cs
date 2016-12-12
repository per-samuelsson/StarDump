using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarDump.Console
{
    public class ConsoleOutput : StarDump.Output
    {
        public override void WriteErrorLine(string message = null)
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

        public override void WriteErrorLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteErrorLine(message);
            }
        }

        public override void WriteWarningLine(string message = null)
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

        public override void WriteWarningLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteWarningLine(message);
            }
        }

        public override void WriteLine(string message = null)
        {
            if (message == null)
            {
                System.Console.WriteLine();
                return;
            }

            System.Console.WriteLine(message);
        }

        public override void WriteLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                WriteLine(message);
            }
        }
    }
}
